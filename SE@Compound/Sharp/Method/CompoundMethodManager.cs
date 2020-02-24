// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// Manages dynamic added extension methods on compound objects
    /// </summary>
    public static class CompoundMethodManager
    {
        private readonly static Dictionary<UInt64, Delegate> extensions;
        private readonly static ReadWriteLock extensionLock;

        static CompoundMethodManager()
        {
            extensions = new Dictionary<UInt64, Delegate>();
            extensionLock = new ReadWriteLock();
        }

        /// <summary>
        /// Adds an extension method to the given compound object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <param name="name">An extension name to add</param>
        /// <param name="extension">The extension delegate to add to the object</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool AddMethod(InstanceId objectId, string name, Delegate extension)
        {
            objectId |= name.Fnv32();
            using (ThreadContext.WriteLock(extensionLock))
            {
                if (!extensions.ContainsKey(objectId))
                {
                    extensions.Add(objectId, extension);
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// Determines if the given compound object has any methods attached
        /// </summary>
        /// <param name="objectId">The compount id to look for</param>
        /// <returns>True if the given compound object has methods attached, false otherwise</returns>
        public static bool HasMethods(InstanceId objectId)
        {
            using (ThreadContext.ReadLock(extensionLock))
                foreach (InstanceId key in extensions.Keys)
                    if (key.ObjectId == objectId.ObjectId)
                        return true;

            return false;
        }

        /// <summary>
        /// Tries to return an extension method for a specific object
        /// </summary>
        /// <param name="objectId">The property ID used to identify the object's extension</param>
        /// <returns>True if the extension method exists on the object, false otherwise</returns>
        public static bool TryGetMethod(InstanceId propertyId, out Delegate result)
        {
            using (ThreadContext.ReadLock(extensionLock))
                return extensions.TryGetValue(propertyId, out result);
        }
        /// <summary>
        /// Tries to return an extension method for a specific object
        /// </summary>
        /// <param name="objectId">The property ID used to identify the object's extension</param>
        /// <param name="name">An extension name to get a delegate for</param>
        /// <returns>True if the extension method exists on the object, false otherwise</returns>
        public static bool TryGetMethod(InstanceId objectId, string name, out Delegate result)
        {
            return TryGetMethod(objectId | name.Fnv32(), out result);
        }

        /// <summary>
        /// Removes an existing extension method from the given compound object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <param name="name">An extension name to remove if existing</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool RemoveMethod(InstanceId objectId, string name)
        {
            objectId |= name.Fnv32();
            using (ThreadContext.WriteLock(extensionLock))
                return extensions.Remove(objectId);
        }
    }
}
