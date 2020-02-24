// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// Stores property instances for compound objects
    /// </summary>
    public class PropertyContainer : IEnumerable<KeyValuePair<UInt32, PropertyInstance>>, IEnumerable
    {
        readonly Dictionary<UInt32, PropertyInstance> properties;
        readonly ReadWriteLock propertyLock;

        public int Count
        {
            get
            {
                using (ThreadContext.ReadLock(propertyLock))
                    return properties.Count;
            }
        }

        public ICollection<UInt32> Keys
        {
            get
            {
                using (ThreadContext.ReadLock(propertyLock))
                    return properties.Keys;
            }
        }
        public ICollection<PropertyInstance> Values
        {
            get
            {
                using (ThreadContext.ReadLock(propertyLock))
                    return properties.Values;
            }
        }

        /// <summary>
        /// Creates a new property container with the given access context
        /// </summary>
        public PropertyContainer()
        {
            this.properties = new Dictionary<UInt32, PropertyInstance>();
            this.propertyLock = new ReadWriteLock();
        }

        public static implicit operator ReadWriteLock(PropertyContainer container)
        {
            return container.propertyLock;
        }

        public void Add(UInt32 key, PropertyInstance value)
        {
            using (ThreadContext.WriteLock(propertyLock))
            {
                if (properties.ContainsKey(key)) properties[key] = value;
                else properties.Add(key, value);
            }
        }
        
        public void Clear()
        {
            using (ThreadContext.WriteLock(propertyLock))
                properties.Clear();
        }

        public bool ContainsKey(UInt32 key)
        {
            using (ThreadContext.ReadLock(propertyLock))
                return properties.ContainsKey(key);
        }

        public bool TryGetValue(UInt32 key, out PropertyInstance value)
        {
            using (ThreadContext.ReadLock(propertyLock))
                return properties.TryGetValue(key, out value);
        }

        public bool Remove(UInt32 key)
        {
            using (ThreadContext.WriteLock(propertyLock))
                return properties.Remove(key);
        }

        public IEnumerator<KeyValuePair<UInt32, PropertyInstance>> GetEnumerator()
        {
            using (ThreadContext.ReadLock(propertyLock))
                return properties.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            using (ThreadContext.ReadLock(propertyLock))
                return properties.GetEnumerator();
        }
    }
}
