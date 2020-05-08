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
        /// Performs an action for each item contained in this tree node as root node
        /// </summary>
        /// <param name="action">An action to perform</param>
        public static void ForEach<T, TNode>(this TNode root, Action<IEnumerable<T>> action) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                                             where TNode : TernaryTreeNode<T>
        {
            List<T> cache = CollectionPool<List<T>, T>.Get();
            ForEach<T>(root, cache, 0, action);
            CollectionPool<List<T>, T>.Return(cache);
        }

        private static void ForEach<T>(this TernaryTreeNode<T> root, List<T> cache, int size, Action<IEnumerable<T>> action) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
        {
            if (root.HighChild != null)
            {
                ForEach<T>(root.HighChild, cache, cache.Count, action);
            }
            cache.Add(root.Shard);
            if (root.IsLeaf)
            {
                action(cache);
            }
            else if (root.EqualChild != null)
            {
                ForEach<T>(root.EqualChild, cache, cache.Count, action);
                cache.RemoveRange(size, cache.Count - size);
            }
            if (root.LowChild != null)
            {
                ForEach<T>(root.LowChild, cache, cache.Count, action);
                cache.RemoveRange(size, cache.Count - size);
            }
        }
    }
}
