// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Adapter behavior processing one task at a time in an external thread
    /// </summary>
    public class ThreadBehavior : Behavior
    {
        public const int DefaultBacklogBufferSize = 32;
        const int EnqueueFailureThreshold = 10;

        protected Thread thread;
        protected ConcurrentQueue<AdapterContext> pending;
        protected ConditionVariable yieldSignal;
        protected ConditionVariable signal;

        protected int bufferSize;
        /// <summary>
        /// Size of back buffer to hold tasks to execute
        /// </summary>
        public int BufferSize
        {
            get { return bufferSize; }
            set
            {
                ThrowOnExecution();
                bufferSize = value;
            }
        }

        /// <summary>
        /// Creates a new thread to execute tasks
        /// </summary>
        /// <param name="backlog">Size of the back buffer to hold tasks to execute</param>
        public ThreadBehavior(int backlog)
        {
            this.bufferSize = backlog;
            this.thread = new Thread(Loop);
        }
        /// <summary>
        /// Creates a new thread to execute tasks
        /// </summary>
        public ThreadBehavior()
            : this(DefaultBacklogBufferSize)
        { }

        /// <summary>
        /// Initializes this Behavior
        /// </summary>
        public override void Initialize()
        {
            ThrowOnExecution();
            thread.Start();

            pending = new ConcurrentQueue<AdapterContext>(bufferSize);
            yieldSignal = new ConditionVariable();
            signal = new ConditionVariable();
            State = AdapterState.Ready;
        }

        protected void Awake()
        {
            do
            {
                yieldSignal.Set();
            }
            while (TrySetState(AdapterState.Suspended, AdapterState.Ready));
        }

        /// <summary>
        /// Stores a task in this Behavior to be processed
        /// </summary>
        /// <param name="context">The execution context to store</param>
        /// <returns>True if the task was successfully received, false otherwise</returns>
        public override bool Enqueue(AdapterContext context)
        {
            try
            {
                int count = 0;
                while (Enabled && !pending.Enqueue(context))
                {
                    if (count > EnqueueFailureThreshold && TrySetState(AdapterState.Ready, AdapterState.Processing))
                        return false;

                    Thread.Sleep(count);
                    count++;

                    if (count > EnqueueFailureThreshold)
                        signal.Await();
                }

                if (Enabled)
                {
                    if (!yieldSignal.Signaled)
                        Awake();

                    return true;
                }
            }
            catch (Exception er)
            {
                if (context != null) context.Sender.SetError(this, er);
                else if (Enabled) TrySetState(State, AdapterState.Error);
            }
            return false;
        }
        protected void Loop()
        {
            while (Enabled)
            {
                AdapterContext context;
                if (!pending.Dequeue(out context))
                {
                    if (TrySetState(AdapterState.Ready, AdapterState.Suspended))
                        yieldSignal.Await();

                    continue;
                }
                else TrySetState(AdapterState.Processing, AdapterState.Ready);
                context.Execute();
            }
        }

        public override void Dispose()
        {
            if (State != AdapterState.Discarded)
            {
                base.Dispose();
                yieldSignal.Set();

                thread.Join();
            }
        }
    }
}
