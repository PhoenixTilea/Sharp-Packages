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
        /// <summary>
        /// Creates a new thread to execute tasks
        /// </summary>
        public ThreadBehavior()
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
                Thread th = new Thread(context.Execute);
                return true;
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

        public override void Dispose()
        {
            if (State != AdapterState.Discarded)
                base.Dispose();
        }
    }
}
