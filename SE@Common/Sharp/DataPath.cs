// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;

namespace SE
{
    /// <summary>
    /// Handles data paths with branches
    /// </summary>
    public class DataPath<T> : IEnumerable<T>
    {
        public readonly static DataPath<T> Empty = new DataPath<T>(default(T));

        /// <summary>
        /// A class specific allocation free enumerator
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            public readonly static Enumerator Empty = new Enumerator();

            DataPath<T> current;

            public T Current
            {
                get { return current.Item; }
            }
            object IEnumerator.Current
            {
                get { return Current; }
            }

            public Enumerator(DataPath<T> current)
            {
                this.current = current;
            }
            public void Dispose()
            {
                current = null;
            }

            public bool MoveNext()
            {
                if (current != null && current.parent != null)
                {
                    current = current.parent;
                    return true;
                }
                else return false;
            }
            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        readonly DataPath<T> parent;
        /// <summary>
        /// This elements parent node
        /// </summary>
        public DataPath<T> Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Determines if this element is the root of a path
        /// </summary>
        public bool IsRoot
        {
            get { return (parent == null); }
        }

        readonly T item;
        /// <summary>
        /// This elements carried value
        /// </summary>
        public T Item
        {
            get { return item; }
        }

        /// <summary>
        /// Creates a new root element for a path
        /// </summary>
        /// <param name="item">The value carried by this element</param>
        public DataPath(T item)
        {
            this.item = item;
        }
        private DataPath(DataPath<T> parent, T item)
        {
            this.parent = parent;
            this.item = item;
        }

        public static implicit operator T(DataPath<T> element)
        {
            return element.item;
        }

        /// <summary>
        /// Appends a new child element to the data path
        /// </summary>
        /// <param name="item">The value carried by the element</param>
        /// <returns>The appended element</returns>
        public DataPath<T> Append(T item)
        {
            return new DataPath<T>(this, item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
