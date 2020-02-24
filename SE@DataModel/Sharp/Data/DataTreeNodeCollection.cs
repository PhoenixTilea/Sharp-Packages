// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using SE;

namespace SE.DataModel
{
    /// <summary>
    /// A collection of nodes managed by the data tree
    /// </summary>
    public abstract class DataTreeNodeCollection<DataNodeType, T> : ICollection<T> where DataNodeType : struct, IConvertible, IComparable, IFormattable
                                                                                   where T : DataTreeNode<DataNodeType, T>
    {
        protected T instance;

        public abstract int Count
        {
            get;
        }
        public bool IsReadOnly
        {
            get { return false; }
        }

        public abstract T this[int index]
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new instance of the collection
        /// </summary>
        public DataTreeNodeCollection(T instance)
        {
            this.instance = instance;
        }

        public abstract void Add(T item);
        public abstract void Clear();
        public abstract bool Contains(T item);
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract bool Remove(T item);

        public IEnumerator<T> GetEnumerator()
        {
            return Iterate();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract IEnumerator<T> Iterate();
    }
}
