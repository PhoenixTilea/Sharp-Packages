// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// Default generic object policy
    /// </summary>
    public class CollectionPolicy<TInstance, TValue> : IPoolingPolicy<TInstance> where TInstance : ICollection<TValue>, new()
    {
        /// <summary>
        /// A default generic pooling policy instance
        /// </summary>
        public readonly static CollectionPolicy<TInstance, TValue> Default = new CollectionPolicy<TInstance, TValue>();

        /// <summary>
        /// Creates a new pooling policy instance
        /// </summary>
        public CollectionPolicy()
        { }

        public TInstance Create()
        {
            return new TInstance();
        }
        public bool Return(TInstance instance)
        {
            instance.Clear();
            return true;
        }
        public bool Delete(TInstance instance)
        {
            instance.Clear();

            IDisposable disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            return true;
        }
    }
    /// <summary>
    /// Default generic object policy
    /// </summary>
    public class CollectionPolicy<TInstance, TKey, TValue> : IPoolingPolicy<TInstance> where TInstance : ICollection<KeyValuePair<TKey, TValue>>, new()
    {
        /// <summary>
        /// A default generic pooling policy instance
        /// </summary>
        public readonly static CollectionPolicy<TInstance, TKey, TValue> Default = new CollectionPolicy<TInstance, TKey, TValue>();

        /// <summary>
        /// Creates a new pooling policy instance
        /// </summary>
        public CollectionPolicy()
        { }

        public TInstance Create()
        {
            return new TInstance();
        }
        public bool Return(TInstance instance)
        {
            instance.Clear();
            return true;
        }
        public bool Delete(TInstance instance)
        {
            instance.Clear();

            IDisposable disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            return true;
        }
    }
}