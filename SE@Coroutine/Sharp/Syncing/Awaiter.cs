// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Coroutines
{
    /// <summary>
    /// An awaiter to be used as coroutine parent
    /// </summary>
    public class Awaiter : IReceiver
    {
        ReadWriteLock stateLock = new ReadWriteLock();
        ExecutionState state;
        atomic_int returnCounter;

        object result;
        /// <summary>
        /// The returned result if any
        /// </summary>
        public object Result
        {
            get { return result; }
        }

        Exception lastError;
        /// <summary>
        /// Determines the last error occurred if any
        /// </summary>
        public Exception LastError
        {
            get { return lastError; }
        }

        public Awaiter()
        { }

        public void SetResult(object host, object result)
        {
            using (ThreadContext.ReadLock(stateLock))
            {
                this.result = result;

                if (state == null) returnCounter.Increment();
                else state.Signal();
            }
        }
        public void SetError(object host, Exception error)
        {
            using (ThreadContext.ReadLock(stateLock))
            {
                this.lastError = error;

                if (state == null) returnCounter.Increment();
                else state.Signal();
            }
        }

        /// <summary>
        /// Creates a state used to control the scheduler for current coroutine
        /// </summary>
        /// <param name="desiredCount">The amount of coroutines to await</param>
        public ExecutionState Await(UInt32 desiredCount = 0)
        {
            using (ThreadContext.WriteLock(stateLock))
            {
                if (state != null || returnCounter > desiredCount)
                {
                    throw new ArgumentOutOfRangeException();
                }
                else if (returnCounter == desiredCount)
                {
                    state = ExecutionState.Create(ExecutionFlags.Active);
                }
                else
                {
                    state = ExecutionState.Create(ExecutionFlags.Pending, desiredCount - (UInt32)returnCounter.Value);
                }
                return state;
            }
        }
    }
}