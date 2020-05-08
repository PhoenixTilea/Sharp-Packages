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
        /// Finds the matching or most matching path to this tree node as root node
        /// </summary>
        /// <param name="item">A collection of shards to address a path in the tree</param>
        /// <param name="result">A path to the matching or most matching node</param>
        /// <returns>True if a full match was found, false otherwise</returns>
        public static bool TryGet<T, TNode>(this TNode root, IEnumerable<T> item, out DataPath<TNode> result, Comparer<T> comparer) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                                                                                    where TNode : TernaryTreeNode<T>
        {
            result = new DataPath<TNode>(root);

            TernaryTreeNode<T> current = root;
            IEnumerator<T> iterator = item.GetEnumerator();
            bool hasValue = iterator.MoveNext();
            while (hasValue)
            {
                switch (comparer.Compare(iterator.Current, result.Item.Shard).Clamp(-1, 1))
                {
                    case -1:
                        {
                            result = result.Append(result.Item.LowChild as TNode);
                        }
                        break;
                    case 1:
                        {
                            result = result.Append(result.Item.HighChild as TNode);
                        }
                        break;
                    case 0:
                        {
                            hasValue = iterator.MoveNext();
                            if (hasValue)
                            {
                                result = result.Append(result.Item.EqualChild as TNode);
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
        /// Finds the matching or most matching path to this tree node as root node
        /// </summary>
        /// <param name="item">A collection of shards to address a path in the tree</param>
        /// <param name="result">A path to the matching or most matching node</param>
        /// <returns>True if a full match was found, false otherwise</returns>
        public static bool TryGet<T, TNode>(this TNode root, IEnumerable<T> item, out DataPath<TNode> result) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
                                                                                                              where TNode : TernaryTreeNode<T>
        {
            return TryGet<T, TNode>(root, item, out result, Comparer<T>.Default);
        }
    }
}