// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Special Queue that implements ICollection<T>
    /// </summary>
    public class DispatcherQueue<T> : Queue<T>, ICollection<T>
    {
        protected EqualityComparer<T> comparer;
        /// <summary>
        /// The comparer attached to this Queue
        /// </summary>
        public EqualityComparer<T> Comparer
        {
            get { return comparer; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Initializes the Queue with the default comparer
        /// </summary>
        public DispatcherQueue()
        {
            this.comparer = EqualityComparer<T>.Default;
        }
        /// <summary>
        /// Initializes the Queue with a special comparer
        /// </summary>
        /// <param name="comparer">A comparer capable of testing T for equality</param>
        public DispatcherQueue(EqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public void Add(T item)
        {
            Enqueue(item);
        }

        public bool Remove(T item)
        {
            T first = default(T);
            for (; !comparer.Equals(Peek(), first);)
            {
                T tmp = Dequeue();
                if (comparer.Equals(tmp, item))
                    return true;

                if (first == null)
                    first = tmp;

                Enqueue(tmp);
            }
            return false;
        }
    }
}
