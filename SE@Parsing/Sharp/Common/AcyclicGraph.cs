// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// A strictly directed acyclic graph of a finite node hierarchy with no directed cycles
    /// </summary>
    public class AcyclicGraph<T> : ICollection<IEnumerable<T>> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
    {
        protected HashSet<AcyclicGraphEdge<T>> values;

        public int Count
        {
            get { return values.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Creates a new instance of the graph
        /// </summary>
        public AcyclicGraph()
        {
            values = new HashSet<AcyclicGraphEdge<T>>();
        }

        public bool Add(IEnumerable<T> item)
        {
            bool result = false;

            T current = default(T);
            IEnumerator<T> iterator = item.GetEnumerator();
            for (UInt32 i = 0, id = Fnv.FnvOffsetBias; iterator.MoveNext(); i++)
            {
                id = iterator.Current.Fnv32(id);

                AcyclicGraphEdge<T> edge = new AcyclicGraphEdge<T>((int)id, (int)i, current, iterator.Current);
                result |= values.Add(edge);
                current = edge.Outgoing;
            }
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

        public void Clear()
        {
            values.Clear();
        }

        public bool Contains(IEnumerable<T> item)
        {
            bool result = false;

            T current = default(T);
            IEnumerator<T> iterator = item.GetEnumerator();
            for (UInt32 i = 0, id = Fnv.FnvOffsetBias; iterator.MoveNext(); i++)
            {
                id = iterator.Current.Fnv32(id);

                AcyclicGraphEdge<T> edge = new AcyclicGraphEdge<T>((int)id, (int)i, current, iterator.Current);
                result &= values.Contains(edge);
                current = edge.Outgoing;
            }
            return result;
        }

        /// <summary>
        /// Determines if the current edge has successor nodes
        /// </summary>
        /// <param name="current">The current edge to get successor nodes from</param>
        /// <returns>True if the slice exists, false otherwise</returns>
        public bool HasSuccessor(AcyclicGraphEdge<T> current)
        {
            int index = current.Slice + 1;
            foreach (AcyclicGraphEdge<T> edge in values)
            {
                if (edge.Slice == index && edge.Id == (int)edge.Outgoing.Fnv32((UInt32)current.Id))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Obtains a copy for each item contained in this tree
        /// </summary>
        /// <param name="items">A collection to copy items to</param>
        public void Get(ICollection<IEnumerable<T>> items)
        {
            List<T> cache = CollectionPool<List<T>, T>.Get();
            List<AcyclicGraphEdge<T>> nodes = CollectionPool<List<AcyclicGraphEdge<T>>, AcyclicGraphEdge<T>>.Get();
            if (TryGet(0, nodes))
            {
                foreach (AcyclicGraphEdge<T> root in nodes)
                {
                    cache.Add(root.Outgoing);
                    Get(root, cache, 1, items);
                    cache.Clear();
                }
            }
            CollectionPool<List<AcyclicGraphEdge<T>>, AcyclicGraphEdge<T>>.Return(nodes);
            CollectionPool<List<T>, T>.Return(cache);
        }
        void Get(AcyclicGraphEdge<T> root, List<T> cache, int size, ICollection<IEnumerable<T>> items)
        {
            List<AcyclicGraphEdge<T>> nodes = CollectionPool<List<AcyclicGraphEdge<T>>, AcyclicGraphEdge<T>>.Get();
            if (TryGet(root, nodes))
            {
                foreach (AcyclicGraphEdge<T> successor in nodes)
                {
                    bool hasSuccessor = HasSuccessor(successor);
                    if (hasSuccessor)
                    {
                        cache.Add(successor.Outgoing);
                        Get(successor, cache, cache.Count, items);
                        cache.RemoveRange(size, cache.Count - size);
                    }
                    else
                    {
                        cache.Add(successor.Outgoing);
                        items.Add(cache.ToArray());
                    }
                }
            }
            CollectionPool<List<AcyclicGraphEdge<T>>, AcyclicGraphEdge<T>>.Return(nodes);
        }

        /// <summary>
        /// Tries to obtain a slice at the provided index
        /// </summary>
        /// <param name="index">The slice index to obtain</param>
        /// <param name="slice">A collection to fill with the slice data</param>
        /// <returns>True if the slice exists, false otherwise</returns>
        public bool TryGet(int index, ICollection<AcyclicGraphEdge<T>> slice)
        {
            slice.Clear();
            foreach (AcyclicGraphEdge<T> edge in values)
            {
                if (edge.Slice == index)
                    slice.Add(edge);
            }
            return (slice.Count > 0);
        }
        /// <summary>
        /// Tries to obtain the successor nodes of the provided edge
        /// </summary>
        /// <param name="current">The current edge to get successor nodes from</param>
        /// <param name="slice">A collection to fill with the slice data</param>
        /// <returns>True if the slice exists, false otherwise</returns>
        public bool TryGet(AcyclicGraphEdge<T> current, ICollection<AcyclicGraphEdge<T>> slice)
        {
            slice.Clear();

            int index = current.Slice + 1;
            foreach (AcyclicGraphEdge<T> edge in values)
            {
                if (edge.Slice == index && edge.Id == (int)edge.Outgoing.Fnv32((UInt32)current.Id))
                    slice.Add(edge);
            }
            return (slice.Count > 0);
        }

        public void CopyTo(IEnumerable<T>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(IEnumerable<T> item)
        {
            //this is not supported
            return false;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            List<IEnumerable<T>> items = CollectionPool<List<IEnumerable<T>>, IEnumerable<T>>.Get();
            Get(items);

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
