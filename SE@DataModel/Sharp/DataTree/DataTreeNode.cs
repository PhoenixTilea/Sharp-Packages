// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using SE;
using SE.DataModel;

namespace SE.DataModel
{
    /// <summary>
    /// A data node with variable amount of parent and child nodes
    /// </summary>
    public abstract partial class DataTreeNode<DataNodeType, T> : FlexFieldContainer, IEnumerable<T>, IEnumerable where DataNodeType : struct, IConvertible, IComparable, IFormattable
                                                                                                                  where T : DataTreeNode<DataNodeType, T>
    {
        protected FlexField<DataNodeType> type;
        /// <summary>
        /// Determines the type of this node
        /// </summary>
        public DataNodeType Type
        {
            get { return type.Value; }
            protected internal set { type.Value = value; }
        }

        protected FlexField<T> parent;
        /// <summary>
        /// Determines the primary parent of this node
        /// </summary>
        public T Parent
        {
            get { return parent.Value; }
            protected internal set { parent.Value = value; }
        }
        /// <summary>
        /// Gets a collection of parent nodes if the field is expanded
        /// </summary>
        public DataTreeNodeCollection<DataNodeType, T> Parents
        {
            get { return new ParentCollection(this as T); }
        }

        /// <summary>
        /// Gets if the node has any parent nodes available
        /// </summary>
        public bool HasParents
        {
            get { return parent.IsArray; }
        }

        protected FlexField<T> nodes;
        /// <summary>
        /// Determines the primary child of this node
        /// </summary>
        public T Child
        {
            get { return nodes.Value; }
            protected internal set { nodes.Value = value; }
        }
        /// <summary>
        /// Gets a collection of child nodes if the field is expanded
        /// </summary>
        public DataTreeNodeCollection<DataNodeType, T> Nodes
        {
            get { return new ChildCollection(this as T); }
        }

        /// <summary>
        /// Gets if the node has any child nodes available
        /// </summary>
        public bool HasNodes
        {
            get { return nodes.IsArray; }
        }

        /// <summary>
        /// The number of child nodes attached to this node
        /// </summary>
        public int Count
        {
            get
            {
                if (!HasNodes) return 0;
                else return nodes.Length;
            }
        }

        /// <summary>
        /// Creates a new instance of the data node
        /// </summary>
        public DataTreeNode()
        { }

        protected override void Dispose(bool disposing)
        {
            type.Dispose();
            parent.Dispose();
            nodes.Dispose();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!HasNodes) return Iterate();
            return Nodes.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> Iterate()
        {
            if (Child != null)
                yield return Child;

            yield break;
        }

        private static void AddParent(T root, T item)
        {
            root.parent.Resize(root.parent.Length + 1);
            root.parent[root.parent.Length - 1] = item;
        }
        private static void AddChild(T root, T item)
        {
            root.nodes.Resize(root.nodes.Length + 1);
            root.nodes[root.nodes.Length - 1] = item;
        }
    }
}
