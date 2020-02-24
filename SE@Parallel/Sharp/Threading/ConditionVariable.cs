// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;
using SE;

namespace SE.Parallel
{
    /// <summary>
    /// Causes owning thread to wait until condition is set
    /// </summary>
    public class ConditionVariable
    {
        readonly SemaphoreSlim @lock;
        atomic_int signal;

        /// <summary>
        /// Creates a new condition variable instance
        /// </summary>
        public ConditionVariable()
        {
            @lock = new SemaphoreSlim(0, 1);
        }

        /// <summary>
        /// Returns current signal state of the condition
        /// </summary>
        public bool Signaled
        {
            get { return (signal.Value <= 0); }
        }

        /// <summary>
        /// Sets owning thread into wait mode until condition is set
        /// </summary>
        public void Await()
        {
            if (signal.Increment() > 0)
                @lock.Wait();

            signal.Exchange(0);
        }
        /// <summary>
        /// Sets condition and resumes owning thread from wait mode
        /// </summary>
        public void Set()
        {
            if (signal.Decrement() == 0)
                @lock.Release();
        }
    }
}
