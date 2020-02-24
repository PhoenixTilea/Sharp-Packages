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
        private readonly static Spinlock initializationLock;
        private static UInt32 initializationCounter;

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

        private static ConcurrentQueue<ThreadScheduler.PoolWorker> suspendedWorker;

        /// <summary>
        /// Indicates if the pool is currently in use
        /// </summary>
        public static bool Active
        {
            get { return ThreadScheduler.Active; }
        }

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
        {
            initializationLock = new Spinlock();
            initializationCounter = 0;
        }

        private static void Initialize()
        {
            ThreadScheduler.Acquire();
            suspendedWorker = new ConcurrentQueue<ThreadScheduler.PoolWorker>(((Environment.ProcessorCount * 2) - 1).NextPowerOfTwo());

            highPriorityJobs = new ConcurrentQueue<ExecutionContext>(highPriorityCapacity.NextPowerOfTwo());
            lowPriorityJobs = new ConcurrentQueue<ExecutionContext>(lowPriorityCapacity.NextPowerOfTwo());
            pendingJobs = new ConcurrentQueue<ExecutionContext>((highPriorityCapacity + lowPriorityCapacity).NextPowerOfTwo());

            threads = 0;
            for (int i = (int)(((Environment.ProcessorCount * 2) - 1) * threadCoverage); i > 0; i--)
                if (ThreadScheduler.Decouple(Iterator))
                    threads++;
        }
        /// <summary>
        /// Initializes the ThreadPool if not already in use
        /// </summary>
        public static void Acquire()
        {
            using (ThreadContext.Lock(initializationLock))
            {
                if (++initializationCounter <= 1)
                {
                    Initialize();
                    initializationCounter = 1;
                }
            }
        }

        private static void Unload()
        {
            ThreadScheduler.Release();
            lowPriorityJobs = null;
            highPriorityJobs = null;
            pendingJobs = null;
            suspendedWorker = null;
        }
        /// <summary>
        /// Closes the pool and awaits any active worker to shutdown properly if
        /// no longer in use
        /// </summary>
        public static void Release()
        {
            using (ThreadContext.Lock(initializationLock))
            {
                if (--initializationCounter <= 0)
                {
                    Unload();
                    initializationCounter = 0;
                }
            }
        }

        /// <summary>
        /// Schedules a new coroutine into the pool
        /// </summary>
        /// <param name="context">The coroutine to be executed</param>
        /// <param name="highPriority">True, if the coroutine should run in higher priority, false otherwise</param>
        /// <returns>True, if the coroutine was successfully enqueued, false otherwise</returns>
        public static bool Start(IEnumerator context, bool highPriority, IReceiver parent = null)
        {
            if (suspendedWorker == null)
                throw new ObjectDisposedException("CoroutineScheduler");

            if (!ThreadScheduler.Active)
                return false;

            if (highPriority && !highPriorityJobs.Enqueue(new ExecutionContext(context, parent))) return false;
            else if (!highPriority && !lowPriorityJobs.Enqueue(new ExecutionContext(context, parent))) return false;

            ThreadScheduler.PoolWorker worker;
            if (suspendedWorker.Dequeue(out worker)) worker.Awake();
            return true;
        }

        /// <summary>
        /// Joins the pool to pass runtime of the calling thread to the pool
        /// </summary>
        public static void Join()
        {
            if (suspendedWorker == null)
                throw new ObjectDisposedException("CoroutineScheduler");

            ExecutionContext context = null;
            if (ThreadScheduler.Active)
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

            while (ThreadScheduler.Active)
            {
                bool hasPendingJobs = GetPendingJob(ref context);
                if (context == null && !highPriorityJobs.Dequeue(out context))
                {
                    if (!lowPriorityJobs.Dequeue(out context))
                    {
                        if (hasPendingJobs) Thread.Sleep(1);
                        else
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
