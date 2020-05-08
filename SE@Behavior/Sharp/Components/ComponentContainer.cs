// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    public class ComponentContainer : IEnumerable<KeyValuePair<UInt32, object>>, IDisposable
    {
        readonly Dictionary<UInt32, object> components;

        public int Count
        {
            get { return components.Count; }
        }

        public bool IsEmpty
        {
            get { return (components.Count == 0); }
        }

        public ComponentContainer()
        {
            this.components = CollectionPool<Dictionary<UInt32, object>, UInt32, object>.Get();
        }
        public void Dispose()
        {
            CollectionPool<Dictionary<UInt32, object>, UInt32, object>.Return(components);
        }

        public bool Add(UInt32 id, object instance)
        {
            if (components.ContainsKey(id))
                return false;

            components.Add(id, instance);
            return true;
        }

        public bool Contains(UInt32 id)
        {
            return components.ContainsKey(id);
        }

        public bool TryGetValue(UInt32 id, out object instance)
        {
            return components.TryGetValue(id, out instance);
        }

        public bool Remove(UInt32 id)
        {
            return components.Remove(id);
        }

        public IEnumerator<KeyValuePair<uint, object>> GetEnumerator()
        {
            return components.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
