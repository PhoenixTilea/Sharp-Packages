// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Adapter behavior processing multiple task in a thread pool
    /// </summary>
    public class SchedulingBehavior : Behavior
    {
        protected int bufferSize;
        /// <summary>
        /// Size of back buffer to hold tasks to execute
        /// </summary>
        public int BufferSize
        {
            get { return 0; }
            set
            {
                ThrowOnExecution();
            }
        }

        /// <summary>
        /// Creates a  thread pool behavior to execute tasks
        /// </summary>
        public SchedulingBehavior()
        { }

        /// <summary>
        /// Initializes this Behavior
        /// </summary>
        public override void Initialize()
        {
            ThrowOnExecution();
            State = AdapterState.Ready;
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
                if (Enabled)
                    return ThreadScheduler.Start(context.Execute, false);
            }
            catch (Exception er)
            {
                if (context != null)
                {
                    //er.Data.Add(context.Sender, this);
                    context.Sender.OnReject(er);
                }
                else if (Enabled) TrySetState(State, AdapterState.Error);
            }
            return false;
        }
    }
}
