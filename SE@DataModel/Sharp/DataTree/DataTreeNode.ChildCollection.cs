// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.DataModel
{
    /// <summary>
    /// A collection of child nodes of a data node
    /// </summary>
    public abstract partial class DataTreeNode<DataNodeType, T> where DataNodeType : struct, IConvertible, IComparable, IFormattable
                                                                where T : DataTreeNode<DataNodeType, T>
    {
        /// <summary>
        /// The collection of child nodes for a specific node
        /// </summary>
        class ChildCollection : DataTreeNodeCollection<DataNodeType, T>
        {
            public override int Count
            {
                get { return instance.nodes.Length; }
            }
            public override T this[int index]
            {
                get { return instance.nodes.GetValue(index); }
                set { instance.nodes.SetValue(index, value); }
            }

            /// <summary>
            /// Creates a new container instance
            /// </summary>
            public ChildCollection(T instance)
                : base(instance)
            { }

            public override void Add(T item)
            {
                if (instance.nodes.Length == 0 && instance.nodes[0] != default(T))
                    instance.nodes.Resize(1);

                AddChild(instance, item);
                if (item.Parent != default(T))
                {
                    DataTreeNodeCollection<DataNodeType, T> nodes = item.Parents;
                    if (!item.Parents.Contains(instance))
                    {
                        if (!item.HasParents)
                            AddParent(item, item.Parent);

                        AddParent(item, instance);
                    }
                }
                else item.Parent = instance;
            }
            public override void Clear()
            {
                int length = instance.nodes.Length;
                T item;

                for (; length > 0; length--)
                {
                    item = instance.nodes[0];
                    if ((!item.HasParents || !item.Parents.Remove(instance)) && item.Parent == instance)
                        item.Parent = default(T);
                }
                instance.nodes.Resize(0);

                item = instance.nodes[0];
                if (item != default(T))
                {
                    if ((!item.HasParents || !item.Parents.Remove(instance)) && item.Parent == instance)
                        item.Parent = default(T);

                    instance.nodes[0] = default(T);
                }
            }
            public override bool Contains(T item)
            {
                int length = instance.nodes.Length;
                for (int i = 0; i < length; i++)
                {
                    if (item.Equals(instance.nodes[i]))
                        return true;
                }
                return false;
            }
            public override void CopyTo(T[] array, int arrayIndex)
            {
                int length = instance.nodes.Length;
                for (int i = 0; i < length; i++)
                    array[arrayIndex + i] = instance.nodes[i];
            }

            public override bool Remove(T item)
            {
                if (RemoveInternal(item))
                {
                    if (instance.nodes.Length == 0) instance.nodes[0] = null;
                    if ((!item.HasParents || !item.Parents.Remove(instance)) && item.Parent == instance)
                        item.Parent = default(T);

                    return true;
                }
                else return false;
            }
            bool RemoveInternal(T item)
            {
                int length = instance.nodes.Length;
                for (int i = 0; i < length; i++)
                {
                    if (item.Equals(instance.nodes[i]))
                    {
                        int index = i;
                        i++;

                        for (; i < length; i++, index++)
                            instance.nodes[index] = instance.nodes[i];

                        instance.nodes.Resize(length - 1);
                        return true;
                    }
                }
                return false;
            }

            protected override IEnumerator<T> Iterate()
            {
                int length = instance.nodes.Length;
                for (int i = 0; i < length; i++)
                    yield return instance.nodes[i];

                yield break;
            }
        }
    }
}
