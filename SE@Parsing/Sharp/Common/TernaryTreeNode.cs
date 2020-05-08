// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// A 3-component binary tree node
    /// </summary>
    public class TernaryTreeNode<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
    {
        T shard;
        /// <summary>
        /// The ID value belonging to this node
        /// </summary>
        public T Shard
        {
            get { return shard; }
            internal set { shard = value; }
        }

        TernaryTreeNode<T> lowChild;
        /// <summary>
        /// A child node where ID is smaller
        /// </summary>
        public TernaryTreeNode<T> LowChild
        {
            get { return lowChild; }
            internal set { lowChild = value; }
        }

        TernaryTreeNode<T> equalChild;
        /// <summary>
        /// A child node where ID is equal
        /// </summary>
        public TernaryTreeNode<T> EqualChild
        {
            get { return equalChild; }
            internal set { equalChild = value; }
        }

        TernaryTreeNode<T> highChild;
        /// <summary>
        /// A child node where ID is greater
        /// </summary>
        public TernaryTreeNode<T> HighChild
        {
            get { return highChild; }
            internal set { highChild = value; }
        }

        /// <summary>
        /// Determines if this node has any child node attached
        /// </summary>
        public bool HasChildren
        {
            get { return (lowChild != null || equalChild != null || highChild != null); }
        }

        bool isLeaf;
        /// <summary>
        /// Determines if this node is marked as the end of a sequence
        /// </summary>
        public bool IsLeaf
        {
            get { return isLeaf; }
            internal set { isLeaf = value; }
        }

        /// <summary>
        /// Creates a new tree node with the provided ID
        /// </summary>
        public TernaryTreeNode(T shard)
        {
            this.shard = shard;
        }
    }
}