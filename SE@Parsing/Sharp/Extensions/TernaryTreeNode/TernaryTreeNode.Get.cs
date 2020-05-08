// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    public static partial class TernaryTreeNodeExtension
    {
        /// <summary>
        /// Obtains a copy for each item contained in this tree node as root node
        /// </summary>
        /// <param name="items">A collection to copy items to</param>
        public static void Get<T, TNode>(this TNode root, ICollection<IEnumerable<T>> items) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                                      where TNode : TernaryTreeNode<T>
        {
            List<T> cache = CollectionPool<List<T>, T>.Get();
            Get<T>(root, cache, 0, items);
            CollectionPool<List<T>, T>.Return(cache);
        }
        /// <summary>
        /// Obtains a copy for each item contained in this tree node as root node
        /// </summary>
        public static List<IEnumerable<T>> Get<T, TNode>(this TNode root) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                          where TNode : TernaryTreeNode<T>
        {
            List<IEnumerable<T>> result = new List<IEnumerable<T>>();
            Get<T, TNode>(root, result);

            return result;
        }

        private static void Get<T>(this TernaryTreeNode<T> root, List<T> cache, int size, ICollection<IEnumerable<T>> items) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
        {
            if (root.HighChild != null)
            {
                Get<T>(root.HighChild, cache, cache.Count, items);
            }
            cache.Add(root.Shard);
            if (root.IsLeaf)
            {
                items.Add(cache.ToArray());
            }
            else if (root.EqualChild != null)
            {
                Get<T>(root.EqualChild, cache, cache.Count, items);
                cache.RemoveRange(size, cache.Count - size);
            }
            if (root.LowChild != null)
            {
                Get<T>(root.LowChild, cache, cache.Count, items);
                cache.RemoveRange(size, cache.Count - size);
            }
        }
    }
}
