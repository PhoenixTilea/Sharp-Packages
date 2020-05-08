// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    public static class Quicksort
    {
        struct QuicksortArrayContext<T>
        {
            EnumerablePromise<T> promise;

            T[] items;
            IComparer<T> comparer;

            bool highPriority;
            public bool HighPriority
            {
                get { return highPriority; }
            }

            public QuicksortArrayContext(EnumerablePromise<T> promise, T[] items, IComparer<T> comparer, bool highPriority)
            {
                this.promise = promise;

                this.items = items;
                this.comparer = comparer;
                this.highPriority = highPriority;
            }
            public void Execute(object values)
            {
                try
                {
                    Tuple<int, int> lr = (Tuple<int, int>)values;
                    ParallelSortInternal<T>(items, lr.Item1, lr.Item2, comparer, this);
                    promise.OnResolve(default(T));
                }
                catch (Exception er)
                {
                    promise.OnReject(er);
                }
            }

            public void Increment()
            {
                promise.Increment();
            }
            public void SetError(Exception er)
            {
                promise.OnReject(er);
            }
        }
        struct QuicksortListContext<T>
        {
            EnumerablePromise<T> promise;

            List<T> items;
            IComparer<T> comparer;

            bool highPriority;
            public bool HighPriority
            {
                get { return highPriority; }
            }

            public QuicksortListContext(EnumerablePromise<T> promise, List<T> items, IComparer<T> comparer, bool highPriority)
            {
                this.promise = promise;

                this.items = items;
                this.comparer = comparer;
                this.highPriority = highPriority;
            }
            public void Execute(object values)
            {
                try
                {
                    Tuple<int, int> lr = (Tuple<int, int>)values;
                    ParallelSortInternal<T>(items, lr.Item1, lr.Item2, comparer, this);
                    promise.OnResolve(default(T));
                }
                catch (Exception er)
                {
                    promise.OnReject(er);
                }
            }

            public void Increment()
            {
                promise.Increment();
            }
            public void SetError(Exception er)
            {
                promise.OnReject(er);
            }
        }

        private const int SequentialThreshold = 2048;

        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static EnumerablePromise<T> ParallelSort<T>(this T[] items, bool highPriority = false)
        {
            return ParallelSort(items, 0, items.Length - 1, Comparer<T>.Default, highPriority);
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static EnumerablePromise<T> ParallelSort<T>(this List<T> items, bool highPriority = false)
        {
            return ParallelSort(items, 0, items.Count - 1, Comparer<T>.Default, highPriority);
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static EnumerablePromise<T> ParallelSort<T>(this T[] items, IComparer<T> comparer, bool highPriority = false)
        {
            return ParallelSort(items, 0, items.Length - 1, comparer, highPriority);
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static EnumerablePromise<T> ParallelSort<T>(this List<T> items, IComparer<T> comparer, bool highPriority = false)
        {
            return ParallelSort(items, 0, items.Count - 1, comparer, highPriority);
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        /// <param name="left">The start index of the range to csort</param>
        /// <param name="right">The end index of the range to sort</param>
        public static EnumerablePromise<T> ParallelSort<T>(T[] items, int left, int right, IComparer<T> comparer, bool highPriority = false)
        {
            EnumerablePromise<T> promise = new EnumerablePromise<T>();
            try
            {
                if (right > left)
                {
                    if (right - left < SequentialThreshold) SE.Quicksort.Sort(items, left, right, comparer);
                    else
                    {
                        QuicksortArrayContext<T> context = new QuicksortArrayContext<T>(promise, items, comparer, highPriority);
                        ParallelSortInternal(items, left, right, comparer, context);
                    }
                }
                promise.OnResolve();
            }
            catch (Exception er)
            {
                promise.OnReject(er);
            }
            return promise;
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        /// <param name="left">The start index of the range to csort</param>
        /// <param name="right">The end index of the range to sort</param>
        public static EnumerablePromise<T> ParallelSort<T>(List<T> items, int left, int right, IComparer<T> comparer, bool highPriority = false)
        {
            EnumerablePromise<T> promise = new EnumerablePromise<T>();
            try
            {
                if (right > left)
                {
                    if (right - left < SequentialThreshold) SE.Quicksort.Sort(items, left, right, comparer);
                    else
                    {
                        QuicksortListContext<T> context = new QuicksortListContext<T>(promise, items, comparer, highPriority);
                        ParallelSortInternal(items, left, right, comparer, context);
                    }
                }
                promise.OnResolve();
            }
            catch (Exception er)
            {
                promise.OnReject(er);
            }
            return promise;
        }

        private static void ParallelSortInternal<T>(T[] items, int left, int right, IComparer<T> comparer, QuicksortArrayContext<T> context)
        {
            if (right > left)
                try
                {
                    if (right - left < SequentialThreshold) SE.Quicksort.Sort(items, left, right, comparer);
                    else
                    {
                        int pivot = SE.Quicksort.Partition(items, left, right, comparer);

                        context.Increment();
                        while (!ThreadScheduler.Start(context.Execute, new Tuple<int, int>(left, pivot - 1), context.HighPriority))
                            ThreadScheduler.Join();

                        context.Increment();
                        while (!ThreadScheduler.Start(context.Execute, new Tuple<int, int>(pivot + 1, right), context.HighPriority))
                            ThreadScheduler.Join();
                    }
                }
                catch (Exception er)
                {
                    context.SetError(er);
                }
        }
        private static void ParallelSortInternal<T>(List<T> items, int left, int right, IComparer<T> comparer, QuicksortListContext<T> context)
        {
            if (right > left)
                try
                {
                    if (right - left < SequentialThreshold) SE.Quicksort.Sort(items, left, right, comparer);
                    else
                    {
                        int pivot = SE.Quicksort.Partition(items, left, right, comparer);

                        context.Increment();
                        while (!ThreadScheduler.Start(context.Execute, new Tuple<int, int>(left, pivot - 1), context.HighPriority))
                            ThreadScheduler.Join();

                        context.Increment();
                        while (!ThreadScheduler.Start(context.Execute, new Tuple<int, int>(pivot + 1, right), context.HighPriority))
                            ThreadScheduler.Join();
                    }
                }
                catch (Exception er)
                {
                    context.SetError(er);
                }
        }
    }
}