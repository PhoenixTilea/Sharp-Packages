// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// Provides caching behavior for frequently reused collections
    /// </summary>
    public static class CollectionPool<TInstance, TValue> where TInstance : class, ICollection<TValue>, new()
    {
        private readonly static ObjectPool<TInstance> pool;

        static CollectionPool()
        {
            pool = new ObjectPool<TInstance>(CollectionPolicy<TInstance, TValue>.Default);
        }

        /// <summary>
        /// Gets the next available object instance from the cache
        /// </summary>
        /// <returns>A cached object instance if available, a newly created one otherwise</returns>
        public static TInstance Get()
        {
            return pool.Get();
        }

        /// <summary>
        /// Returns the provided object instance into the cache. The instance will be reset to
        /// default and discarded if capacity is exceeded
        /// </summary>
        /// <param name="instance">An object instance to cache for reuse</param>
        /// <returns>True if the instance is cached properly, false otherwise</returns>
        public static bool Return(TInstance instance)
        {
            return pool.Return(instance);
        }
    }
    /// <summary>
    /// Provides caching behavior for frequently reused collections
    /// </summary>
    public static class CollectionPool<TInstance, TKey, TValue> where TInstance : class, ICollection<KeyValuePair<TKey, TValue>>, new()
    {
        private readonly static ObjectPool<TInstance> pool;

        static CollectionPool()
        {
            pool = new ObjectPool<TInstance>(CollectionPolicy<TInstance, TKey, TValue>.Default);
        }

        /// <summary>
        /// Gets the next available object instance from the cache
        /// </summary>
        /// <returns>A cached object instance if available, a newly created one otherwise</returns>
        public static TInstance Get()
        {
            return pool.Get();
        }

        /// <summary>
        /// Returns the provided object instance into the cache. The instance will be reset to
        /// default and discarded if capacity is exceeded
        /// </summary>
        /// <param name="instance">An object instance to cache for reuse</param>
        /// <returns>True if the instance is cached properly, false otherwise</returns>
        public static bool Return(TInstance instance)
        {
            return pool.Return(instance);
        }
    }
}