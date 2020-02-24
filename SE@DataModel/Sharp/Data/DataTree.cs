// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using SE;

namespace SE.DataModel
{
    /// <summary>
    /// A DOM data structure
    /// </summary>
    public abstract class DataTree<DataNodeType, T> : IEnumerable<T>, IEnumerable where DataNodeType : struct, IConvertible, IComparable, IFormattable
                                                                                  where T : DataTreeNode<DataNodeType, T>
    {
        protected HashSet<T> nodes;
        /// <summary>
        /// The number of odes contained in this tree
        /// </summary>
        public int Count
        {
            get { return nodes.Count; }
        }

        /// <summary>
        /// Creates a new instance of the data tree
        /// </summary>
        public DataTree()
        {
            nodes = new HashSet<T>();
        }

        /// <summary>
        /// Adds a new node of the desired type to this trees root
        /// </summary>
        /// <returns>The created data node instance</returns>
        public T AddNode(DataNodeType type)
        {
            return AddNode(null, type);
        }
        /// <summary>
        /// Adds a new node of the desired type to provided parent node
        /// </summary>
        /// <returns>The created data node instance</returns>
        public abstract T AddNode(T root, DataNodeType type);

        /// <summary>
        /// Appends an existing node to the provided parent
        /// </summary>
        public virtual void AddAppend(T root, T node)
        {
            if (root != default(T))
            {
                if (root.Child == default(T)) root.Child = node;
                else root.Nodes.Add(node);

                if (node.Parent == default(T)) node.Parent = root;
                else node.Parents.Add(root);

                nodes.Add(root);
            }
            nodes.Add(node);
        }

        /// <summary>
        /// Tries to remove the entire node hierarchy from this tree
        /// </summary>
        /// <returns>True if the nodes was removed successfully, false otherwise</returns>
        public bool RemoveNode(T node)
        {
            if (node == null)
                return false;

            node.Parents.Clear();
            if (node.HasNodes)
            {
                List<T> childs = new List<T>(node.Nodes);
                node.Nodes.Clear();

                foreach (T tmp in childs)
                {
                    if (tmp.HasParents) tmp.Parents.Remove(node);
                    else if (tmp.Parent == node)
                        RemoveNode(tmp);
                }
            }
            else if(node.Child != default(T))
            {
                T tmp = node.Child;
                if (tmp.HasParents) tmp.Parents.Remove(node);
                else if(tmp.Parent == node)
                    RemoveNode(tmp);
            }

            nodes.Remove(node);
            return true;
        }

        /// <summary>
        /// Removes all nodes from this tree
        /// </summary>
        public void Clear()
        {
            foreach (T node in nodes)
                RemoveNode(node);

            nodes.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
