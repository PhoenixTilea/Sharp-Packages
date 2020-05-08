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
        /// Appends the provided item to this tree node as root node
        /// </summary>
        /// <param name="item">A collection of shards to address a path in the tree</param>
        /// <param name="node">The last node in the path</param>
        /// <returns>True if the path has been extended, false otherwise</returns>
        public static bool Append<T, TNode>(this TNode root, IEnumerable<T> item, out TNode node, Comparer<T> comparer) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                                                                        where TNode : TernaryTreeNode<T>, new()
        {
            bool result = false;
            node = root;

            IEnumerator<T> iterator = item.GetEnumerator();
            bool hasValue = iterator.MoveNext();
            while (hasValue)
            {

            Repeat:
                switch (comparer.Compare(iterator.Current, node.Shard).Clamp(-1, 1))
                {
                    case -1:
                        {
                            if (node.LowChild == null)
                            {
                                TNode next = new TNode();
                                next.Shard = iterator.Current;
                                node.LowChild = next;
                                result = true;
                            }
                            node = node.LowChild as TNode;
                        }
                        goto Repeat;
                    case 1:
                        {
                            if (node.HighChild == null)
                            {
                                TNode next = new TNode();
                                next.Shard = iterator.Current;
                                node.HighChild = next;
                                result = true;
                            }
                            node = node.HighChild as TNode;
                        }
                        goto Repeat;
                    case 0:
                        {
                            hasValue = iterator.MoveNext();
                            if (hasValue)
                            {
                                if (node.EqualChild == null)
                                {
                                    TNode next = new TNode();
                                    next.Shard = iterator.Current;
                                    node.EqualChild = next;
                                    result = true;
                                }
                                node = node.EqualChild as TNode;
                            }
                        }
                        break;
                }
            }
            if (!node.IsLeaf)
            {
                node.IsLeaf = true;
                result = true;
            }
            else result = false;
            return result;
        }
        /// <summary>
        /// Appends the provided item to this tree node as root node
        /// </summary>
        /// <param name="item">A collection of shards to address a path in the tree</param>
        /// <param name="node">The last node in the path</param>
        /// <returns>True if the path has been extended, false otherwise</returns>
        public static bool Append<T, TNode>(this TNode root, IEnumerable<T> item, out TNode node) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                                                  where TNode : TernaryTreeNode<T>, new()
        {
            return Append<T, TNode>(root, item, out node, Comparer<T>.Default);
        }
        /// <summary>
        /// Appends the provided item to this tree node as root node
        /// </summary>
        /// <param name="item">A collection of shards to address a path in the tree</param>
        /// <param name="node">The last node in the path</param>
        /// <returns>True if the path has been extended, false otherwise</returns>
        public static TNode Append<T, TNode>(this TNode root, IEnumerable<T> item, Comparer<T> comparer) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                                                         where TNode : TernaryTreeNode<T>, new()
        {
            TNode node;
            Append<T, TNode>(root, item, out node, comparer);

            return node;
        }
        /// <summary>
        /// Appends the provided item to this tree node as root node
        /// </summary>
        /// <param name="item">A collection of shards to address a path in the tree</param>
        /// <param name="node">The last node in the path</param>
        /// <returns>True if the path has been extended, false otherwise</returns>
        public static TNode Append<T, TNode>(this TNode root, IEnumerable<T> item) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                                   where TNode : TernaryTreeNode<T>, new()
        {
            TNode node;
            Append<T, TNode>(root, item, out node, Comparer<T>.Default);

            return node;
        }
    }
}