// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using SE;

namespace SE.Parallel.Coroutines
{
    /// <summary>
    /// Manages 3 way execution locks of handling the condition of a critical section,
    /// completion of the operation and blocking until the barrier resolves
    /// </summary>
    public class ExecutionBarrier : IReceiver
    {
        private static Dictionary<UInt32, ExecutionBarrier> index;
        private static ReadWriteLock indexerLock;

        ReadWriteLock guardLock;

        IExecutionGuard guard;
        /// <summary>
        /// Gets or sets the barriers current guard instance
        /// </summary>
        public IExecutionGuard Guard
        {
            get
            {
                using (ThreadContext.ReadLock(guardLock))
                    return guard;
            }
            set
            {
                using (ThreadContext.WriteLock(guardLock))
                    if (guard == null) guard = value;
                    else throw new InvalidOperationException();
            }
        }

        static ExecutionBarrier()
        {
            index = new Dictionary<UInt32, ExecutionBarrier>();
            indexerLock = new ReadWriteLock();
        }
        protected ExecutionBarrier()
        {
            guardLock = new ReadWriteLock();
        }

        public void SetError(object host, Exception error)
        {
            SetResult(host, error);
        }
        public void SetResult(object host, object result)
        {
            IExecutionGuard context = Guard;
            if (context != null) context.Set(result);
            else TaskScheduler.Start(ResultCarrierLoop(result), true);
        }
        IEnumerator ResultCarrierLoop(object result)
        {
            while (Guard == null)
                yield return ExecutionState.Create(ExecutionFlags.Pending);

            Guard.Set(result);
            yield break;
        }

        /// <summary>
        /// Suspends current coroutine until the barrier has been signaled
        /// </summary>
        /// <returns>True if this coroutine was the first one passing the barrier, false otherwise</returns>
        public IEnumerator Await()
        {
            while (Guard == null)
                yield return ExecutionState.Create(ExecutionFlags.Pending);

            yield return Guard.Await();
            yield return !Guard.Passed;
        }

        /// <summary>
        /// Creates a barrier to a critical section with the given ID. An instance is
        /// returned if the barrier already exists
        /// </summary>
        /// <param name="id">An ID the barrier should be associated with</param>
        /// <returns>The instance of the barrier requested</returns>
        public static ExecutionBarrier Create(UInt32 id)
        {
            ExecutionBarrier result;
            using (ThreadContext.ReadLock(indexerLock))
                if (index.TryGetValue(id, out result))
                    return result;

            using (ThreadContext.WriteLock(indexerLock))
            {
                if (!index.TryGetValue(id, out result))
                {
                    result = new ExecutionBarrier();
                    index.Add(id, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Removes the association between the ID and a barrier instance if existing
        /// </summary>
        /// <param name="id">The ID a barrier should no longer be associated with</param>
        /// <returns>True if removed successfully, false otherwise</returns>
        public static bool Remove(UInt32 id)
        {
            using (ThreadContext.WriteLock(indexerLock))
                return index.Remove(id);
        }
    }
}