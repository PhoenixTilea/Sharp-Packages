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
        /// Tries to remove a given path from this tree node as root node
        /// </summary>
        /// <param name="item">A collection of shards to address a path in the tree</param>
        /// <param name="node">The last node in the path</param>
        /// <returns>True if the node was removed successfully, false otherwise</returns>
        public static bool Remove<T, TNode>(this TNode root, IEnumerable<T> item, out TNode node, Comparer<T> comparer) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                                                                        where TNode : TernaryTreeNode<T>
        {
            DataPath<TNode> result;
            if (root.TryGet<T, TNode>(item, out result, comparer) && result.Item.IsLeaf)
            {
                node = result.Item;
                if (!result.Item.HasChildren)
                {
                    RemoveFromPath<T, TNode>(result);
                }
                else result.Item.IsLeaf = false;
                return true;
            }
            else
            {
                node = result.Item;
                return false;
            }
        }
        /// <summary>
        /// Tries to remove a given path from this tree node as root node
        /// </summary>
        /// <param name="item">A collection of shards to address a path in the tree</param>
        /// <param name="node">The last node in the path</param>
        /// <returns>True if the node was removed successfully, false otherwise</returns>
        public static bool Remove<T, TNode>(this TNode root, IEnumerable<T> item, out TNode node) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                                                  where TNode : TernaryTreeNode<T>
        {
            return Remove<T, TNode>(root, item, out node, Comparer<T>.Default);
        }

        private static void RemoveFromPath<T, TNode>(DataPath<TNode> path) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                           where TNode : TernaryTreeNode<T>
        {
            if (!path.IsRoot)
            {
                DataPath<TNode> parent = path.Parent;
                if (parent.Item.LowChild == path.Item)
                {
                    parent.Item.LowChild = null;
                }
                else if (parent.Item.EqualChild == path.Item)
                {
                    parent.Item.EqualChild = null;
                }
                else if (parent.Item.HighChild == path.Item)
                {
                    parent.Item.HighChild = null;
                }
                else throw new IndexOutOfRangeException();
                if (!parent.Item.IsLeaf && !parent.Item.HasChildren)
                {
                    RemoveFromPath<T, TNode>(parent);
                }
            }
        }
    }
}