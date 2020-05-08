// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    public static partial class EnumerableExtension
    {
        struct ParallelContext<T>
        {
            EnumerablePromise<T> promise;
            Action<T> action;

            public ParallelContext(EnumerablePromise<T> promise, Action<T> action)
            {
                this.promise = promise;
                this.action = action;
            }
            public void Process(object value)
            {
                try
                {
                    action((T)value);
                    promise.OnResolve((T)value);
                }
                catch (Exception er)
                {
                    promise.OnReject(er);
                }
            }
        }

        /// <summary>
        /// Executes an action parallel for each item in the data vector
        /// </summary>
        /// <param name="action">An action to execute</param>
        /// <returns>An awaitable to the asynchronous vectoring operation</returns>
        public static EnumerablePromise<T> ParallelFor<T>(this ICollection<T> items, Action<T> action, bool highPriority = false)
        {
            return ParallelFor<T>(items as IEnumerable<T>, action, highPriority);
        }
        /// <summary>
        /// Executes an action parallel for each item in the data vector
        /// </summary>
        /// <param name="action">An action to execute</param>
        /// <returns>An awaitable to the asynchronous vectoring operation</returns>
        public static EnumerablePromise<T> ParallelFor<T>(this T[] items, Action<T> action, bool highPriority = false)
        {
            return ParallelFor<T>(items as IEnumerable<T>, action, highPriority);
        }
        /// <summary>
        /// Executes an action parallel for each item in the data vector
        /// </summary>
        /// <param name="action">An action to execute</param>
        /// <returns>An awaitable to the asynchronous vectoring operation</returns>
        public static EnumerablePromise<T> ParallelFor<T>(this IEnumerable<T> items, Action<T> action, bool highPriority = false)
        {
            EnumerablePromise<T> promise = new EnumerablePromise<T>();
            try
            {
                ParallelContext<T> context = new ParallelContext<T>(promise, action);
                foreach (T item in items)
                {
                    promise.Increment();

                    while (!ThreadScheduler.Start(context.Process, item, highPriority))
                        ThreadScheduler.Join();
                }
                promise.OnResolve();
            }
            catch (Exception er)
            {
                promise.OnReject(er);
            }
            return promise;
        }
    }
}
