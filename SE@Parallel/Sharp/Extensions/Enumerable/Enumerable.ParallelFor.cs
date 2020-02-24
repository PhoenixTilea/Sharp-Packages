// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Parallel.Processing;

namespace SE.Parallel
{
    public static partial class EnumerableExtension
    {
        /// <summary>
        /// Executes an action parallel for each item in the data vector
        /// </summary>
        /// <param name="action">An action to execute</param>
        /// <returns>The amount of items processed</returns>
        public static int ParallelFor<T>(this ICollection<T> items, Action<T> action)
        {
            try
            {
                ThreadScheduler.Acquire();
                using (Channel<T> channel = new Channel<T>(new BatchDispatcher()))
                {
                    channel.Register(ParallelExtension.ParallelAdapter, action);
                    if (items.Count > 0)
                    {
                        using (ThreadPoolAwaiter awaiter = new ThreadPoolAwaiter())
                        {
                            foreach (T item in items)
                            {
                                while (!channel.Dispatch(awaiter, item))
                                    ThreadScheduler.Join();

                                awaiter.Increment();
                            }

                            if (!awaiter.Await())
                                throw awaiter.Error;
                            else if (!awaiter.Finished)
                                throw new System.Threading.ThreadInterruptedException();
                        }
                    }
                }
                return items.Count;
            }
            finally
            {
                ThreadScheduler.Release();
            }
        }
        /// <summary>
        /// Executes an action parallel for each item in the data vector
        /// </summary>
        /// <param name="action">An action to execute</param>
        /// <returns>The amount of items processed</returns>
        public static int ParallelFor<T>(this T[] items, Action<T> action)
        {
            try
            {
                ThreadScheduler.Acquire();
                using (Channel<T> channel = new Channel<T>(new BatchDispatcher()))
                {
                    channel.Register(ParallelExtension.ParallelAdapter, action);
                    if (items.Length > 0)
                    {
                        using (ThreadPoolAwaiter awaiter = new ThreadPoolAwaiter())
                        {
                            foreach (T item in items)
                            {
                                while (!channel.Dispatch(awaiter, item))
                                    ThreadScheduler.Join();

                                awaiter.Increment();
                            }

                            if (!awaiter.Await())
                                throw awaiter.Error;
                            else if (!awaiter.Finished)
                                throw new System.Threading.ThreadInterruptedException();
                        }
                    }
                }
                return items.Length;
            }
            finally
            {
                ThreadScheduler.Release();
            }
        }
        /// <summary>
        /// Executes an action parallel for each item in the data vector
        /// </summary>
        /// <param name="action">An action to execute</param>
        /// <returns>The amount of items processed</returns>
        public static int ParallelFor<T>(this IEnumerable<T> items, Action<T> action)
        {
            int count = 0;

            try
            {
                ThreadScheduler.Acquire();
                using (Channel<T> channel = new Channel<T>(new BatchDispatcher()))
                {
                    channel.Register(ParallelExtension.ParallelAdapter, action);
                    using (ThreadPoolAwaiter awaiter = new ThreadPoolAwaiter())
                    {
                        foreach (T item in items)
                        {
                            while (!channel.Dispatch(awaiter, item))
                                ThreadScheduler.Join();

                            awaiter.Increment();
                            count++;
                        }

                        if (!awaiter.Await())
                            throw awaiter.Error;
                        else if (!awaiter.Finished)
                            throw new System.Threading.ThreadInterruptedException();
                    }
                }

                return count;
            }
            finally
            {
                ThreadScheduler.Release();
            }
        }
    }
}
