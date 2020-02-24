// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;

namespace SE
{
    /// <summary>
    /// Spin awaits until access is passed
    /// </summary>
    public class Spinlock : IScopeable
    {
        atomic_int @lock;
        /// <summary>
        /// Gets current lock state
        /// </summary>
        public Int32 State
        {
            get { return @lock.Value; }
        }

        Int32 scopeId;

        atomic_int references;
        /// <summary>
        /// Gets the amount of nesting in this scope
        /// </summary>
        public Int32 RefCount
        {
            get { return references.Value; }
        }

        /// <summary>
        /// Creates a new lock instance
        /// </summary>
        public Spinlock()
        { }

        /// <summary>
        /// Implicite call to Lock()
        /// </summary>
        public void Acquire() { Lock(); }
        /// <summary>
        /// Try to acquire access to the critical section
        /// </summary>
        public void Lock()
        {
            if (scopeId == ThreadContext.LocalId)
            {
                references.Increment();
                return;
            }

            while (@lock.Exchange(1) != 0)
            {
                while (@lock != 0)
                { }
            }

            scopeId = ThreadContext.LocalId;
        }

        /// <summary>
        /// Passes access for this lock object back
        /// </summary>
        public void Release()
        {
            if (references > 0) references.Decrement();
            else
            {
                scopeId = ~scopeId;
                @lock.Exchange(0);
            }
        }
    }
}
