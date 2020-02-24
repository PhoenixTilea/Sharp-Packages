// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using SE;

namespace SE.Parallel.Coroutines
{
    /// <summary>
    /// Manages locking of a barrier due to certain context
    /// </summary>
    public interface IExecutionGuard
    {
        /// <summary>
        /// Determines if the caller is the first process that passes the barrier
        /// </summary>
        bool Passed { get; }

        /// <summary>
        /// Sets coroutines into pending state until the barrier is released
        /// </summary>
        IEnumerator Await();
        /// <summary>
        /// Signals a leave of the critical section and resumes coroutines from pending state
        /// if the condition is met
        /// </summary>
        void Set(object result);
    }
}