// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// A binary search tree with up to 3 children per node
    /// </summary>
    public class TernaryTree<T> : ICollection<IEnumerable<T>> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
    {
        readonly Comparer<T> comparer;

        TernaryTreeNode<T> root;
        /// <summary>
        /// This trees current root node
        /// </summary>
        public TernaryTreeNode<T> Root
        {
            get { return root; }
        }

        int count;
        public int Count
        {
            get { return count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Creates a new instance of the tree
        /// </summary>
        public TernaryTree(Comparer<T> comparer)
        {
            this.comparer = comparer;

            Clear();
        }
        /// <summary>
        /// Creates a new instance of the tree
        /// </summary>
        public TernaryTree()
         : this(Comparer<T>.Default)
        { }

        public virtual bool Add(IEnumerable<T> item)
        {
            bool result = false;

            TernaryTreeNode<T> current = root;
            IEnumerator<T> iterator = item.GetEnumerator();
            bool hasValue = iterator.MoveNext();
            while(hasValue)
            {
                if (root == null)
                {
                    root = new TernaryTreeNode<T>(iterator.Current);
                    current = root;
                    result = true;
                    count++;
                }

            Repeat:
                switch (comparer.Compare(iterator.Current, current.Shard).Clamp(-1, 1))
                {
                    case -1:
                        {
                            if (current.LowChild == null)
                            {
                                current.LowChild = new TernaryTreeNode<T>(iterator.Current);
                                result = true;
                                count++;
                            }
                            current = current.LowChild;
                        }
                        goto Repeat;
                    case 1:
                        {
                            if (current.HighChild == null)
                            {
                                current.HighChild = new TernaryTreeNode<T>(iterator.Current);
                                result = true;
                                count++;
                            }
                            current = current.HighChild;
                        }
                        goto Repeat;
                    case 0:
                        {
                            hasValue = iterator.MoveNext();
                            if (hasValue)
                            {
                                if (current.EqualChild == null)
                                {
                                    current.EqualChild = new TernaryTreeNode<T>(iterator.Current);
                                    result = true;
                                    count++;
                                }
                                current = current.EqualChild;
                            }
                        }
                        break;
                }
            }
            if (!current.IsLeaf)
            {
                current.IsLeaf = true;
                result = true;
            }
            else result = false;
            return result;
        }
        void ICollection<IEnumerable<T>>.Add(IEnumerable<T> item)
        {
            Add(item);
        }

        /// <summary>
        /// Adds a bunch of items to the tree at once
        /// </summary>
        public void AddRange(IEnumerable<IEnumerable<T>> items)
        {
            foreach (IEnumerable<T> item in items)
                Add(item);
        }

        public virtual void Clear()
        {
            root = null;
        }

        public bool Contains(IEnumerable<T> item)
        {
            DataPath<TernaryTreeNode<T>> result;
            return Find(item, out result);
        }
        
        /// <summary>
        /// Finds the matching or most matching node in the tree
        /// </summary>
        /// <param name="item">A list of values treated as item to match</param>
        /// <returns>True if an exact math was found, false otherwise</returns>
        public virtual bool Find(IEnumerable<T> item, out DataPath<TernaryTreeNode<T>> result)
        {
            result = new DataPath<TernaryTreeNode<T>>(root);
            TernaryTreeNode<T> current = root;
            IEnumerator<T> iterator = item.GetEnumerator();
            bool hasValue = iterator.MoveNext();
            while (hasValue)
            {
                switch (comparer.Compare(iterator.Current, result.Item.Shard).Clamp(-1, 1))
                {
                    case -1:
                        {
                            result = result.Append(result.Item.LowChild);
                        }
                        break;
                    case 1:
                        {
                            result = result.Append(result.Item.HighChild);
                        }
                        break;
                    case 0:
                        {
                            hasValue = iterator.MoveNext();
                            if (hasValue)
                            {
                                result = result.Append(result.Item.EqualChild);
                            }
                        }
                        break;
                }
                if (result.Item == null)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Finds the matching or most matching node in the tree
        /// </summary>
        /// <param name="item">A list of values treated as item to match</param>
        /// <returns>The most matching tree node if available</returns>
        public TernaryTreeNode<T> Find(IEnumerable<T> item)
        {
            DataPath<TernaryTreeNode<T>> result;
            Find(item, out result);

            return result.Item;
        }

        public void CopyTo(IEnumerable<T>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(IEnumerable<T> item)
        {
            DataPath<TernaryTreeNode<T>> result;
            if (Find(item, out result) && result.Item.IsLeaf)
            {
                if (!result.Item.HasChildren)
                {
                    RemoveFromPath(result);
                }
                else result.Item.IsLeaf = false;
                return true;
            }
            else return false;
        }
        void RemoveFromPath(DataPath<TernaryTreeNode<T>> path)
        {
            if (path.IsRoot)
            {
                root = null;
                count--;
            }
            else
            {
                DataPath<TernaryTreeNode<T>> parent = path.Parent;
                if (parent.Item.LowChild == path.Item)
                {
                    parent.Item.LowChild = null;
                    count--;
                }
                else if (parent.Item.EqualChild == path.Item)
                {
                    parent.Item.EqualChild = null;
                    count--;
                }
                else if (parent.Item.HighChild == path.Item)
                {
                    parent.Item.HighChild = null;
                    count--;
                }
                else throw new IndexOutOfRangeException();
                if (!parent.Item.IsLeaf && !parent.Item.HasChildren)
                {
                    RemoveFromPath(parent);
                }
            }
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            List<IEnumerable<T>> items = CollectionPool<List<IEnumerable<T>>, IEnumerable<T>>.Get();
            if (root != null)
            {
                root.Get(items);
            }
            foreach (IEnumerable<T> item in items)
            {
                yield return item;
            }
            CollectionPool<List<IEnumerable<T>>, IEnumerable<T>>.Return(items);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}