// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using SE;

namespace SE.Parallel.Coroutines
{
    /// <summary>
    /// An advanced pooling class to execute coroutines in parallel
    /// </summary>
    public static class TaskScheduler
    {
        private static float threadCoverage = 0.5f;
        /// <summary>
        /// Gets or sets the amount of threads reserved from the thread pool
        /// </summary>
        public static float ThreadCoverage
        {
            get { return threadCoverage; }
            set { threadCoverage = value; }
        }

        private static int highPriorityCapacity = 512;
        /// <summary>
        /// Gets or sets the buffer capacity for high priority coroutines
        /// </summary>
        public static int HighPriorityCapacity
        {
            get { return highPriorityCapacity; }
            set { highPriorityCapacity = value; }
        }

        private static int lowPriorityCapacity = 4096;
        /// <summary>
        /// Gets or sets the buffer capacity for low priority coroutines
        /// </summary>
        public static int LowPriorityCapacity
        {
            get { return lowPriorityCapacity; }
            set { lowPriorityCapacity = value; }
        }

        private static atomic_bool mainProcess;
        private static atomic_int state;
        /// <summary>
        /// Indicates if the pool is currently in use
        /// </summary>
        public static ThreadSchedulerState State
        {
            get { return (ThreadSchedulerState)state.Value; }
        }

        private static ConcurrentQueue<ThreadScheduler.PoolWorker> suspendedWorker;
        private static ConcurrentQueue<ExecutionContext> pendingJobs;
        private static ConcurrentQueue<ExecutionContext> highPriorityJobs;
        private static ConcurrentQueue<ExecutionContext> lowPriorityJobs;

        private static int threads;
        /// <summary>
        /// The amount of threads used to schedule workload
        /// </summary>
        public static int Threads
        {
            get { return threads; }
        }
        /// <summary>
        /// The amount of threads that are in idle mode
        /// </summary>
        public static int Suspended
        {
            get
            {
                if (suspendedWorker == null) return 0;
                else return suspendedWorker.Length;
            }
        }

        /// <summary>
        /// The amount of coroutines the scheduler is about to handle. This includes all
        /// pending coroutines as same as waiting for execution and non-idle threads
        /// </summary>
        public static int ActiveCoroutines
        {
            get
            {
                int result = (threads - Suspended);
                if (pendingJobs != null)
                    result += pendingJobs.Length;

                if (highPriorityJobs != null)
                    result += highPriorityJobs.Length;

                if (lowPriorityJobs != null)
                    result += lowPriorityJobs.Length;

                return result;
            }
        }

        public static event ThreadExceptionEventHandler ExecutionException;

        static TaskScheduler()
        { }

        private static void Initialize()
        {
            suspendedWorker = new ConcurrentQueue<ThreadScheduler.PoolWorker>(((Environment.ProcessorCount * 2) - 1).NextPowerOfTwo());

            highPriorityJobs = new ConcurrentQueue<ExecutionContext>(highPriorityCapacity.NextPowerOfTwo());
            lowPriorityJobs = new ConcurrentQueue<ExecutionContext>(lowPriorityCapacity.NextPowerOfTwo());
            pendingJobs = new ConcurrentQueue<ExecutionContext>((highPriorityCapacity + lowPriorityCapacity).NextPowerOfTwo());

            threads = 0;
            mainProcess = true;
            for (int i = (int)(((Environment.ProcessorCount * 2) - 1) * threadCoverage); i > 0; i--)
                if (ThreadScheduler.Decouple(Iterator))
                    threads++;

            state.Exchange((int)ThreadSchedulerState.Running);

            if (threads == 0)
                throw new ArgumentOutOfRangeException("ThreadScheduler.Decouple");
        }
        /// <summary>
        /// Initializes the ThreadPool if not already in use
        /// </summary>
        public static bool Acquire()
        {
            if (state.CompareExchange((int)ThreadSchedulerState.Initializing, (int)ThreadSchedulerState.Pending) == (int)ThreadSchedulerState.Pending)
            {
                Initialize();
                return true;
            }
            else return false;
        }

        private static void Unload()
        {
            ThreadScheduler.Release();
            lowPriorityJobs = null;
            highPriorityJobs = null;
            pendingJobs = null;
            suspendedWorker = null;

            state.Exchange((int)ThreadSchedulerState.Pending);
        }
        /// <summary>
        /// Closes the pool and awaits any active worker to shutdown properly if
        /// no longer in use
        /// </summary>
        public static bool Release()
        {
            if (state.CompareExchange((int)ThreadSchedulerState.Initializing, (int)ThreadSchedulerState.Running) == (int)ThreadSchedulerState.Running)
            {
                Unload();
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Schedules a new coroutine into the pool
        /// </summary>
        /// <param name="context">The coroutine to be executed</param>
        /// <param name="highPriority">True, if the coroutine should run in higher priority, false otherwise</param>
        /// <returns>True, if the coroutine was successfully enqueued, false otherwise</returns>
        public static bool Start(IEnumerator context, bool highPriority, IPromiseNotifier<object> parent = null)
        {
            Acquire();

            while (State != ThreadSchedulerState.Running)
                Thread.Sleep(1);

            try
            {
                if (highPriority && !highPriorityJobs.Enqueue(new ExecutionContext(context, parent))) return false;
                else if (!highPriority && !lowPriorityJobs.Enqueue(new ExecutionContext(context, parent))) return false;

                ThreadScheduler.PoolWorker worker;
                if (suspendedWorker.Dequeue(out worker)) worker.Awake();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Joins the pool to pass runtime of the calling thread to the pool
        /// </summary>
        public static void Join()
        {
            if (suspendedWorker == null)
                throw new ObjectDisposedException("CoroutineScheduler");

            ExecutionContext context = null;
            if (State == ThreadSchedulerState.Running)
            {
                GetPendingJob(ref context);
                if (context == null && !highPriorityJobs.Dequeue(out context))
                {
                    if (!lowPriorityJobs.Dequeue(out context))
                        return;
                }
                ExecuteCoroutine(context);
            }
        }

        private static void AwakeWorker()
        {
            ThreadScheduler.PoolWorker tmp;
            if (suspendedWorker.Dequeue(out tmp))
                tmp.Awake();
        }
        private static bool GetPendingJob(ref ExecutionContext context)
        {
            int count = pendingJobs.Length;
            if (pendingJobs.Dequeue(out context))
            {
                if ((count - 1) > 0) AwakeWorker();
                if (!context.Signaled)
                {
                    while (!pendingJobs.Enqueue(context))
                        AwakeWorker();

                    context = null;
                }
            }
            else context = null;
            return (count > 0);
        }
        private static void ExecuteCoroutine(ExecutionContext context)
        {
            bool running = context.SwitchContext();
            switch (context.State.Flag)
            {
                case ExecutionFlags.Pending:
                    {
                        if (!running) break;
                        while (!pendingJobs.Enqueue(context))
                            AwakeWorker();
                    }
                    break;
                case ExecutionFlags.Active:
                case ExecutionFlags.Reset:
                    {
                        if (!running) break;
                        do
                        {
                            AwakeWorker();
                        }
                        while (!lowPriorityJobs.Enqueue(context));
                    }
                    break;
                case ExecutionFlags.Failed:
                    {
                        if (ExecutionException != null)
                            ExecutionException.Invoke(null, new ThreadExceptionEventArgs(context.LastError));
                    }
                    break;
            }
        }

        internal static void Iterator(object aArgs)
        {
            ThreadScheduler.PoolWorker worker = aArgs as ThreadScheduler.PoolWorker;
            ExecutionContext context = null;

            bool isMainProcess = mainProcess.Exchange(false);
            while (!worker.Detached)
            {
                bool hasPendingJobs = GetPendingJob(ref context);
                if (context == null && !highPriorityJobs.Dequeue(out context))
                {
                    if (!lowPriorityJobs.Dequeue(out context))
                    {
                        if (hasPendingJobs) Thread.Sleep(1);
                        else if(!isMainProcess || !ThreadScheduler.Join())
                        {
                            suspendedWorker.Enqueue(worker);
                            worker.Yield();
                        }
                        continue;
                    }
                }
                ExecuteCoroutine(context);
            }
        }
    }
}
