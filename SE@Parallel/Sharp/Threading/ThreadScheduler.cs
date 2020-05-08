// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;
using SE;

namespace SE.Parallel
{
    /// <summary>
    /// A pooling class to execute tasks in parallel
    /// </summary>
    public static class ThreadScheduler
    {
        /// <summary>
        /// A worker class to continuously execute tasks
        /// </summary>
        public class PoolWorker
        {
            Thread baseThread;
            SemaphoreSlim @lock;
            atomic_bool signal;

            bool detached;
            /// <summary>
            /// Gets if this thread is detached from the scheduler
            /// </summary>
            public bool Detached
            {
                get { return detached; }
            }

            /// <summary>
            /// Creates a new worker instance given the default scheduler behavior
            /// </summary>
            public PoolWorker()
            {
                this.baseThread = new Thread(ThreadScheduler.Iterator);
                this.@lock = new SemaphoreSlim(0);
                this.signal = false;
            }

            /// <summary>
            /// Replaces the worker behavior with a single function
            /// </summary>
            public void Replace(ParameterizedThreadStart callback)
            {
                baseThread = new Thread(callback);
            }

            /// <summary>
            /// Initializes this worker
            /// </summary>
            public void Run()
            {
                baseThread.IsBackground = true;
                baseThread.Start(this);
            }

            /// <summary>
            /// Awaits the completion of this worker
            /// </summary>
            public void Await()
            {
                detached = true;
                if (baseThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId && baseThread.ThreadState != ThreadState.Unstarted)
                    do
                    {
                        Awake();
                    }
                    while (baseThread.IsAlive);
            }

            /// <summary>
            /// Yields the worker and it's calling thread into pending state
            /// </summary>
            public void Yield()
            {
                while(!signal)
                    @lock.Wait();

                signal.Value = false;
            }

            /// <summary>
            /// Resumes the worker and it's pending thread to run
            /// </summary>
            public void Awake()
            {
                if(!signal.CompareExchange(true, false))
                    @lock.Release();
            }
        }
        class Context
        {
            WaitCallback callBack;
            object state;

            public Context(WaitCallback callBack, object state)
            {
                this.callBack = callBack;
                this.state = state;
            }
            public void Execute()
            {
                callBack(state);
            }
        }

        private static int highPriorityCapacity = 512;
        /// <summary>
        /// Gets or sets the buffer capacity for high priority tasks
        /// </summary>
        public static int HighPriorityCapacity
        {
            get { return highPriorityCapacity; }
            set { highPriorityCapacity = value; }
        }

        private static int lowPriorityCapacity = 4096;
        /// <summary>
        /// Gets or sets the buffer capacity for low priority tasks
        /// </summary>
        public static int LowPriorityCapacity
        {
            get { return lowPriorityCapacity; }
            set { lowPriorityCapacity = value; }
        }

        private static PoolWorker[] workerThreads;
        private static ConcurrentQueue<PoolWorker> suspendedWorker;
        private static ConcurrentQueue<PoolWorker> availableWorker;

        private static atomic_int state;
        /// <summary>
        /// Indicates if the pool is currently in use
        /// </summary>
        public static ThreadSchedulerState State
        {
            get { return (ThreadSchedulerState)state.Value; }
        }

        private static ConcurrentQueue<Context> highPriorityJobs;
        private static ConcurrentQueue<Context> lowPriorityJobs;

        /// <summary>
        /// The amount of threads used to schedule workload
        /// </summary>
        public static int Threads
	    { 
            get
            {
                if (workerThreads == null) return 0;
                else return workerThreads.Length;
            }
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
        /// The amount of threads waiting to be initialized
        /// </summary>
        public static int Ready
        {
            get
            {
                if (availableWorker == null) return 0;
                else return availableWorker.Length;
            }
        }

        public static event ThreadExceptionEventHandler ExecutionException;

        static ThreadScheduler()
        { }

        private static void Initialize()
        {
            workerThreads = new PoolWorker[(Environment.ProcessorCount * 2) - 1];
            suspendedWorker = new ConcurrentQueue<PoolWorker>(workerThreads.Length.NextPowerOfTwo());
            availableWorker = new ConcurrentQueue<PoolWorker>(workerThreads.Length.NextPowerOfTwo());

            highPriorityJobs = new ConcurrentQueue<Context>(highPriorityCapacity.NextPowerOfTwo());
            lowPriorityJobs = new ConcurrentQueue<Context>(lowPriorityCapacity.NextPowerOfTwo());

            for (int i = 0; i < workerThreads.Length; i++)
            {
                workerThreads[i] = new PoolWorker();
                availableWorker.Enqueue(workerThreads[i]);
            }
            state.Exchange((int)ThreadSchedulerState.Running);
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
            if (availableWorker != null)
            {
                PoolWorker worker;
                while (availableWorker.Dequeue(out worker))
                    ;
            }
            if (workerThreads != null)
                for (int i = 0; i < workerThreads.Length; i++)
                {
                    workerThreads[i].Awake();
                    workerThreads[i].Await();
                }

            workerThreads = null;
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
        /// Schedules a new task into the pool
        /// </summary>
        /// <param name="callBack">The function to be executed</param>
        /// <param name="state">A parameter to be passed to the function</param>
        /// <param name="highPriority">True, if the task should run in higher priority, false otherwise</param>
        /// <returns>True, if the task was successfully enqueued, false otherwise</returns>
        public static bool Start(WaitCallback callBack, object state, bool highPriority)
        {
            Acquire();

            while (State != ThreadSchedulerState.Running)
                Thread.Sleep(1);

            try
            {
                if (highPriority && !highPriorityJobs.Enqueue(new Context(callBack, state))) return false;
                else if (!highPriority && !lowPriorityJobs.Enqueue(new Context(callBack, state))) return false;

                PoolWorker worker;
                if (suspendedWorker.Dequeue(out worker)) worker.Awake();
                else if (availableWorker.Dequeue(out worker)) worker.Run();
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Schedules a new task into the pool
        /// </summary>
        /// <param name="callBack">The function to be executed</param>
        /// <param name="highPriority">True, if the task should run in higher priority, false otherwise</param>
        /// <returns>True, if the task was successfully enqueued, false otherwise</returns>
        public static bool Start(WaitCallback callBack, bool highPriority)
        {
            return Start(callBack, null, highPriority);
        }

        /// <summary>
        /// Reserves a thread from the pool for exclusive use
        /// </summary>
        /// <param name="callBack">The function to be executed</param>
        /// <returns>True, if the task was successfully decoupled, false otherwise</returns>
        public static bool Decouple(ParameterizedThreadStart callBack)
        {
            Acquire();

            while (State != ThreadSchedulerState.Running)
                Thread.Sleep(1);

            try
            {
                PoolWorker worker; if (availableWorker.Dequeue(out worker))
                {
                    worker.Replace(callBack);
                    worker.Run();

                    return true;
                }
                else return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Joins the pool to pass runtime of the calling thread to the pool
        /// </summary>
        public static bool Join()
        {
            Context context; if (State == ThreadSchedulerState.Running)
            {
                if (!highPriorityJobs.Dequeue(out context))
                    if (!lowPriorityJobs.Dequeue(out context))
                        return false;

                try
                {
                    context.Execute();
                }
                catch(Exception e)
                {
                    if (ExecutionException != null)
                        ExecutionException.Invoke(null, new ThreadExceptionEventArgs(e));
                }
            }
            return true;
        }

        private static void Iterator(object args)
        {
            PoolWorker worker = args as PoolWorker;
			Context context;

            while (!worker.Detached)
            {
                if (!highPriorityJobs.Dequeue(out context))
                {
                    if (!lowPriorityJobs.Dequeue(out context))
                    {
                        suspendedWorker.Enqueue(worker);
                        if (!worker.Detached)
                            worker.Yield();

                        continue;
                    }
                }

                try
                {
                    context.Execute();
                }
                catch (Exception e)
                {
                    if (ExecutionException != null)
                        ExecutionException.Invoke(null, new ThreadExceptionEventArgs(e));
                }
            }
        }
    }
}
