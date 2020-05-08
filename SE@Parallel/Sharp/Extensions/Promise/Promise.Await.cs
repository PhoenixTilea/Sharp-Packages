// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using SE;

namespace SE.Parallel
{
    public static partial class PromiseExtension
    {
        /// <summary>
        /// Passes runtime to the scheduler in order to await an asynchronous operation
        /// </summary>
        /// <param name="action">An action to execute while waiting</param>
        /// <returns>True if the operation was resolved successfully, false otherwise</returns>
        public static bool Await<T>(this IPromise<T> promise, Action action)
        {
            try
            {
                while (promise.State == PromiseState.Pending)
                    action();
            }
            catch (Exception er)
            {
                try
                {
                    promise.OnReject(er);
                }
                catch { }
            }
            return (promise.State == PromiseState.Resolved);
        }
        /// <summary>
        /// Passes runtime to the scheduler in order to await an asynchronous operation,
        /// canceled after time
        /// </summary>
        /// <param name="timeout">Awaits at least provided time in milliseconds</param>
        /// <returns>True if the operation was resolved successfully, false otherwise</returns>
        public static bool Await<T>(this IPromise<T> promise, int timeout)
        {
            Stopwatch timer = Stopwatch.StartNew();
            return Await<T>(promise, () =>
            {
                if (timer.ElapsedMilliseconds >= timeout)
                    throw new TimeoutException();

                ThreadScheduler.Join();
            });
        }
        /// <summary>
        /// Passes runtime to the scheduler in order to await an asynchronous operation
        /// </summary>
        /// <returns>True if the operation was resolved successfully, false otherwise</returns>
        public static bool Await<T>(this IPromise<T> promise)
        {
            return Await<T>(promise, () => { ThreadScheduler.Join(); });
        }
    }
}
