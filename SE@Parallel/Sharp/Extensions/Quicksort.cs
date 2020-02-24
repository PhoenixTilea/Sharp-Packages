// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Parallel.Processing;

namespace SE.Parallel
{
    public static class Quicksort
    {
        private const int SequentialThreshold = 2048;

        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static T[] ParallelSort<T>(this T[] items)
        {
            ParallelSort(items, 0, items.Length - 1, Comparer<T>.Default);
            return items;
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static List<T> ParallelSort<T>(this List<T> items)
        {
            ParallelSort(items, 0, items.Count - 1, Comparer<T>.Default);
            return items;
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static T[] ParallelSort<T>(this T[] items, IComparer<T> comparer)
        {
            ParallelSort(items, 0, items.Length - 1, comparer);
            return items;
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static List<T> ParallelSort<T>(this List<T> items, IComparer<T> comparer)
        {
            ParallelSort(items, 0, items.Count - 1, comparer);
            return items;
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        /// <param name="left">The start index of the range to csort</param>
        /// <param name="right">The end index of the range to sort</param>
        public static void ParallelSort<T>(T[] items, int left, int right, IComparer<T> comparer)
        {
            try
            {
                ThreadScheduler.Acquire();
                if (right > left)
                {
                    if (right - left < SequentialThreshold) SE.Quicksort.Sort(items, left, right, comparer);
                    else
                    {
                        int pivot = SE.Quicksort.Partition(items, left, right, comparer);
                        using (Channel<int, int> channel = new Channel<int, int>(new BatchDispatcher()))
                        using (ThreadPoolAwaiter awaiter = new ThreadPoolAwaiter())
                        {
                            channel.Register(ParallelExtension.ParallelAdapter, (l, r) =>
                            {
                                ParallelSortInternal(items, l, r, comparer, channel, awaiter);
                            });
                            while (!channel.Dispatch(awaiter, left, pivot - 1))
                                ThreadScheduler.Join();

                            awaiter.Increment();
                            while (!channel.Dispatch(awaiter, pivot + 1, right))
                                ThreadScheduler.Join();

                            awaiter.Increment();

                            if (!awaiter.Await())
                                throw awaiter.Error;
                            else if (!awaiter.Finished)
                                throw new System.Threading.ThreadInterruptedException();
                        }
                    }
                }
            }
            finally
            {
                ThreadScheduler.Release();
            }
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        /// <param name="left">The start index of the range to csort</param>
        /// <param name="right">The end index of the range to sort</param>
        public static void ParallelSort<T>(List<T> items, int left, int right, IComparer<T> comparer)
        {
            try
            {
                ThreadScheduler.Acquire();
                if (right > left)
                {
                    if (right - left < SequentialThreshold) SE.Quicksort.Sort(items, left, right, comparer);
                    else
                    {
                        int pivot = SE.Quicksort.Partition(items, left, right, comparer);
                        using (Channel<int, int> channel = new Channel<int, int>(new BatchDispatcher()))
                        using (ThreadPoolAwaiter awaiter = new ThreadPoolAwaiter())
                        {
                            channel.Register(ParallelExtension.ParallelAdapter, (l, r) =>
                            {
                                ParallelSortInternal(items, l, r, comparer, channel, awaiter);
                            });
                            while (!channel.Dispatch(awaiter, left, pivot - 1))
                                ThreadScheduler.Join();

                            awaiter.Increment();
                            while (!channel.Dispatch(awaiter, pivot + 1, right))
                                ThreadScheduler.Join();

                            awaiter.Increment();

                            if (!awaiter.Await())
                                throw awaiter.Error;
                            else if (!awaiter.Finished)
                                throw new System.Threading.ThreadInterruptedException();
                        }
                    }
                }
            }
            finally
            {
                ThreadScheduler.Release();
            }
        }

        private static void ParallelSortInternal<T>(T[] items, int left, int right, IComparer<T> comparer, Channel<int, int> channel, ThreadPoolAwaiter awaiter)
        {
            if (right > left)
            {
                if (right - left < SequentialThreshold) SE.Quicksort.Sort(items, left, right, comparer);
                else
                {
                    int pivot = SE.Quicksort.Partition(items, left, right, comparer);

                    awaiter.Increment();
                    while (!channel.Dispatch(awaiter, left, pivot - 1))
                        ThreadScheduler.Join();

                    awaiter.Increment();
                    while (!channel.Dispatch(awaiter, pivot + 1, right))
                        ThreadScheduler.Join();
                }
            }
        }
        private static void ParallelSortInternal<T>(List<T> items, int left, int right, IComparer<T> comparer, Channel<int, int> channel, ThreadPoolAwaiter awaiter)
        {
            if (right > left)
            {
                if (right - left < SequentialThreshold) SE.Quicksort.Sort(items, left, right, comparer);
                else
                {
                    int pivot = SE.Quicksort.Partition(items, left, right, comparer);

                    awaiter.Increment();
                    while (!channel.Dispatch(awaiter, left, pivot - 1))
                        ThreadScheduler.Join();

                    awaiter.Increment();
                    while (!channel.Dispatch(awaiter, pivot + 1, right))
                        ThreadScheduler.Join();
                }
            }
        }
    }
}