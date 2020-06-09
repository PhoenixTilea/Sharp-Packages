// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SE
{
    /// <summary>
    /// Provides caching behavior for frequently reused objects
    /// </summary>
    public class ObjectPool<T> : IDisposable where T : class
    {
        /// <summary>
        /// The policy attached to the pool
        /// </summary>
        protected readonly IPoolingPolicy<T> policy;
        /// <summary>
        /// A fixed size buffer of items stored in the pool
        /// </summary>
        protected readonly T[] items;
        /// <summary>
        /// The most recent item cached
        /// </summary>
        protected T head;

        /// <summary>
        /// Creates a new cache instance of certain policy. Objects get deleted from
        /// memory if they exceed the capacity provided
        /// </summary>
        /// <param name="policy">The policy that handles certain objects</param>
        /// <param name="capacity">The maximum capacity of the cache</param>
        public ObjectPool(IPoolingPolicy<T> policy, int capacity)
        {
            this.policy = policy;
            items = new T[Math.Max(capacity - 1, 0)];
        }
        /// <summary>
        /// Creates a new cache instance of certain policy and default capacity
        /// </summary>
        /// <param name="policy">The policy that handles certain objects</param>
        public ObjectPool(IPoolingPolicy<T> policy)
            : this(policy, (Environment.ProcessorCount * 2).NextPowerOfTwo())
        { }
        public void Dispose()
        {
            T item = Interlocked.Exchange<T>(ref head, null);
            if (item != null)
            {
                policy.Delete(item);
            }
            for (var i = 0; i < items.Length; i++)
            {
                item = Interlocked.Exchange<T>(ref items[i], null);
                if (item != null)
                {
                    policy.Delete(item);
                }
            }
        }

        /// <summary>
        /// Gets the next available object instance from the cache
        /// </summary>
        /// <returns>A cached object instance if available, a newly created one otherwise</returns>
        public T Get()
        {
            T item = head;
            if (item == null || Interlocked.CompareExchange<T>(ref head, null, item) != item)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    item = items[i]; 
                    if (item != null && Interlocked.CompareExchange<T>(ref items[i], null, item) == item)
                        return item;
                }
                return policy.Create();
            }
            else return item;
        }

        /// <summary>
        /// Returns the provided object instance into the cache. The instance will be reset to
        /// default and discarded if capacity is exceeded
        /// </summary>
        /// <param name="instance">An object instance to cache for reuse</param>
        /// <returns>True if the instance is cached properly, false otherwise</returns>
        public bool Return(T instance)
        {
            if (instance == null)
                return false;

            #if DEBUG
            Validate(instance);
            #endif

            if (policy.Return(instance))
            {
                if (Interlocked.CompareExchange<T>(ref head, instance, null) != null)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (Interlocked.CompareExchange<T>(ref items[i], instance, null) == null)
                        {
                            return true;
                        }
                    }
                    return policy.Delete(instance);
                }
                else return true;
            }
            else return policy.Delete(instance);
        }

        #if DEBUG
        void Validate(T instance)
        {
            Debug.Assert(head != instance);
            for (int i = 0; i < items.Length; i++)
            {
                T item = items[i];
                if (item == null)
                {
                    return;
                }
                Debug.Assert(item != instance);
            }
        }
        #endif
    }
}