// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// Manages dynamic added properties on compound objects
    /// </summary>
    public static class CompoundPropertyManager
    {
        private readonly static Dictionary<UInt32, PropertyContainer> properties;
        private readonly static ReadWriteLock propertyListLock;

        static CompoundPropertyManager()
        {
            properties = new Dictionary<UInt32, PropertyContainer>();
            propertyListLock = new ReadWriteLock();
        }

        /// <summary>
        /// Adds a new property to the given compound object or changes the property type already
        /// present at the object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <param name="name">A property name to add or change</param>
        /// <param name="type">The type of the new property</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool AddProperty(InstanceId objectId, string name, Type type)
        {
            PropertyContainer container;

            objectId |= name.Fnv32();
            using (ThreadContext.WriteLock(propertyListLock))
            {
                if (!properties.TryGetValue(objectId.ComponentId, out container))
                {
                    container = new PropertyContainer();
                    properties.Add(objectId.ComponentId, container);
                }
            }
            using (ThreadContext.WriteLock(container))
            {
                PropertyInstance instance; if (container.TryGetValue(objectId.ObjectId, out instance))
                {
                    if (instance.PropertyType == type)
                        return false;
                }

                container.Add(objectId.ObjectId, new PropertyInstance(type));
                return true;
            }
        }

        /// <summary>
        /// Determines if the given compound object has any properties attached
        /// </summary>
        /// <param name="objectId">The compount id to look for</param>
        /// <returns>True if the given compound object has properties attached, false otherwise</returns>
        public static bool HasProperties(InstanceId objectId)
        {
            using (ThreadContext.ReadLock(propertyListLock))
            {
                foreach (PropertyContainer container in properties.Values)
                    using (ThreadContext.ReadLock(container))
                    {
                        foreach (InstanceId key in container.Keys)
                            if (key.ObjectId == objectId.ObjectId)
                                return true;
                    }
            }
            return false;
        }

        /// <summary>
        /// Fills the provided collection with currently contained property IDs
        /// </summary>
        /// <param name="result">A collection to be filled with the property IDs existing currently</param>
        public static void GetPropertieIds(ICollection<UInt32> result)
        {
            using (ThreadContext.ReadLock(propertyListLock))
                foreach (UInt32 key in properties.Keys)
                    result.Add(key);
        }

        /// <summary>
        /// Tries to return a property instance for a specific object
        /// </summary>
        /// <param name="objectId">The property ID used to identify the object's property</param>
        /// <returns>True if the property instance exists on the object, false otherwise</returns>
        public static bool TryGetProperty(InstanceId propertyId, out PropertyInstance result)
        {
            using (ThreadContext.ReadLock(propertyListLock))
            {
                PropertyContainer container; if (!properties.TryGetValue(propertyId.ComponentId, out container))
                {
                    result = null;
                    return false;
                }
                else return container.TryGetValue(propertyId.ObjectId, out result);
            }
        }
        /// <summary>
        /// Tries to return a property instance for a specific object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <param name="name">A property name to get the instance for</param>
        /// <returns>True if the property instance exists on the object, false otherwise</returns>
        public static bool TryGetProperty(InstanceId objectId, string name, out PropertyInstance result)
        {
            return TryGetProperty(objectId | name.Fnv32(), out result);
        }

        /// <summary>
        /// Tries to return a read-safe iterator for the given property
        /// </summary>
        /// <param name="propertyId">The property to be iterated</param>
        /// <param name="result">The read-safe iterator instance. Needs to be disposed to release the access</param>
        /// <returns>True if property instances exist, false otherwise</returns>
        public static bool TryGetProperties(UInt32 propertyId, out PropertyIterator result)
        {
            using (ThreadContext.ReadLock(propertyListLock))
            {
                PropertyContainer container; if (!properties.TryGetValue(propertyId, out container))
                {
                    result = null;
                    return false;
                }
                else
                {
                    result = new PropertyIterator(container);
                    return true;
                }
            }
        }
        /// <summary>
        /// Tries to return a read-safe iterator for the given property
        /// </summary>
        /// <param name="name">The property name to be iterated</param>
        /// <param name="result">The read-safe iterator instance. Needs to be disposed to release the access</param>
        /// <returns>True if property instances exist, false otherwise</returns>
        public static bool TryGetProperties(string name, out PropertyIterator result)
        {
            return TryGetProperties(name.Fnv32(), out result);
        }

        /// <summary>
        /// Removes an existing property from the given compound object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <param name="name">A property name to remove if existing</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool RemoveProperty(InstanceId objectId, string name)
        {
            objectId |= name.Fnv32();
            using (ThreadContext.WriteLock(propertyListLock))
            {
                PropertyContainer container; if (properties.TryGetValue(objectId.ComponentId, out container))
                {
                    if (!container.Remove(objectId.ObjectId))
                        return false;

                    if (container.Count == 0)
                        properties.Remove(objectId.ComponentId);

                    return true;
                }
                else return false;
            }
        }
    }
}
