// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Adapter behavior to determine the kind of task processing
    /// </summary>
    public abstract class Behavior : IDisposable
    {
        atomic_int state = (int)AdapterState.Undefined;
        /// <summary>
        /// The working state of this Behavior
        /// </summary>
        public AdapterState State
        {
            get { return (AdapterState)state.Value; }
            protected set { state = (int)value; }
        }

        /// <summary>
        /// Indicates if this Behavior is enabled and ready for processing
        /// </summary>
        public bool Enabled
        {
            get
            {
                AdapterState tmp = State;
                return (tmp == AdapterState.Ready ||
                        tmp == AdapterState.Processing ||
                        tmp == AdapterState.Suspended ||
                        tmp == AdapterState.Error);
            }
        }

        public Behavior()
        { }

        protected void ThrowOnExecution()
        {
            if (Enabled)
                throw new InvalidOperationException();
        }

        protected bool TrySetState(AdapterState current, AdapterState @new)
        {
            return (state.CompareExchange((int)@new, (int)current) == (int)current);
        }
        protected void SetErrorState()
        {
            AdapterState tmp = State;
            if (tmp == AdapterState.Ready ||
                tmp == AdapterState.Processing ||
                tmp == AdapterState.Suspended)
            State = AdapterState.Error;
        }

        /// <summary>
        /// Initializes this Behavior
        /// </summary>
        public abstract void Initialize();
        /// <summary>
        /// Stores a task in this Behavior to be processed
        /// </summary>
        /// <param name="context">The execution context to store</param>
        /// <returns>True if the task was successfully received, false otherwise</returns>
        public abstract bool Enqueue(AdapterContext context);

        public virtual void Dispose()
        {
            State = AdapterState.Discarded;
        }
    }
}
