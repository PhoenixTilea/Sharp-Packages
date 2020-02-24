// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Storage
{
    /// <summary>
    /// Manages a list of statement tokens to verify certain input
    /// </summary>
    public class Filter
    {
        protected List<FilterToken> nodes = new List<FilterToken>();
        /// <summary>
        /// The list of statement tokens this Filter manages
        /// </summary>
        public List<FilterToken> Nodes
        {
            get { return nodes; }
        }
        /// <summary>
        /// The first token in this Filter
        /// </summary>
        public FilterToken Root
        {
            get
            {
                if (nodes.Count > 0) return nodes[0];
                else return null;
            }
        }

        /// <summary>
        /// Creates a new empty Filter
        /// </summary>
        public Filter()
        { }

        /// <summary>
        /// Adds a new statement token to the root level
        /// </summary>
        /// <param name="pattern">A pattern to attach to the statement token</param>
        /// <returns>The created statement token instance</returns>
        public FilterToken Add(string pattern)
        {
            FilterToken node = new FilterToken(pattern);
            FilterToken root = Root;
            if (root != null)
            {
                if (root.Next == null) root.Next = node;
                else
                {
                    FilterToken prevNext = root.Next;
                    for (; ; )
                    {
                        if (prevNext.Next != null) prevNext = prevNext.Next;
                        else
                        {
                            prevNext.Next = node;
                            break;
                        }
                    }

                }
            }
            nodes.Add(node);
            return node;
        }
        /// <summary>
        /// Adds a new statement token below a given parent node
        /// </summary>
        /// <param name="root">A statement token to attach the newly createt token as child</param>
        /// <param name="pattern">A pattern to attach to the statement token</param>
        /// <returns>The created statement token instance</returns>
        public FilterToken Add(FilterToken root, string pattern)
        {
            if (root == null)
                return null;

            FilterToken node = new FilterToken(pattern);
            AddChild(root, node);

            nodes.Add(node);
            return node;
        }
        /// <summary>
        /// Adds an existing statement token below a given parent node
        /// </summary>
        /// <param name="root">A statement token to attach the existing token as child</param>
        /// <param name="node">A statement token to add as child node</param>
        public void AddAppend(FilterToken root, FilterToken node)
        {
            if (root == null)
                return;

            AddChild(root, node);
            nodes.Add(node);
        }

        void AddChild(FilterToken root, FilterToken child)
        {
            if (root.Child == null) root.Child = child;
            else
            {
                FilterToken prevChild = root.Child;
                for (; ; )
                {
                    if (prevChild.Next != null) prevChild = prevChild.Next;
                    else
                    {
                        prevChild.Next = child;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes certain existing statement token and all child tokens from this Filter
        /// </summary>
        /// <param name="node">The statement token to be removed</param>
        /// <returns>True if the statement token was successfully removed, false otherwise</returns>
        public bool Remove(FilterToken node)
        {
            if (node == null) return false;
            for (int i = 0; i < nodes.Count; i++)
                if (nodes[i].Next == node)
                {
                    nodes[i].Next = node.Next;
                    break;
                }

            while (node.Child != null)
            {
                Remove(node.Child);
                node.Child = null;
            }

            nodes.Remove(node);
            return true;
        }
        /// <summary>
        /// Removes any statement token attached to this Filter
        /// </summary>
        public void Clear()
        {
            nodes.Clear();
        }
    }
}
