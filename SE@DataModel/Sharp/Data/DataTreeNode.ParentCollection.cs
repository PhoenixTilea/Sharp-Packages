// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.DataModel
{
    /// <summary>
    /// A collection of parent nodes of a data node
    /// </summary>
    public abstract partial class DataTreeNode<DataNodeType, T> where DataNodeType : struct, IConvertible, IComparable, IFormattable
                                                                where T : DataTreeNode<DataNodeType, T>
    {
        /// <summary>
        /// The collection of parents for a specific node
        /// </summary>
        class ParentCollection : DataTreeNodeCollection<DataNodeType, T>
        {
            public override int Count
            {
                get { return instance.parent.Length; }
            }
            public override T this[int index]
            {
                get { return instance.parent.GetValue(index); }
                set { instance.parent.SetValue(index, value); }
            }

            /// <summary>
            /// Creates a new container instance
            /// </summary>
            public ParentCollection(T instance)
                : base(instance)
            { }

            public override void Add(T item)
            {
                if (instance.parent.Length == 0 && instance.parent[0] != default(T))
                    instance.parent.Resize(1);

                AddParent(instance, item);
                if (item.Child != default(T))
                {
                    if (!item.Nodes.Contains(instance))
                    {
                        if (!item.HasNodes)
                            AddChild(item, item.Child);

                        AddChild(item, instance);
                    }
                }
                else item.Child = instance;
            }
            public override void Clear()
            {
                int length = instance.parent.Length;
                T item;

                for (; length > 0; length--)
                {
                    item = instance.parent[0];
                    if ((!item.HasNodes || !item.Nodes.Remove(instance)) && item.Child == instance)
                        item.Child = default(T);
                }
                instance.parent.Resize(0);

                item = instance.parent[0];
                if (item != default(T))
                {
                    if ((!item.HasNodes || !item.Nodes.Remove(instance)) && item.Child == instance)
                        item.Child = default(T);

                    instance.parent[0] = default(T);
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
                    if (instance.parent.Length == 0) instance.parent[0] = null;
                    if ((!item.HasNodes || !item.Nodes.Remove(instance)) && item.Child == instance)
                        item.Child = default(T);

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
