// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// Default generic object policy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPolicy<T> : IPoolingPolicy<T> where T : class, new()
    {
        /// <summary>
        /// A default generic pooling policy instance
        /// </summary>
        public readonly static ObjectPolicy<T> Default = new ObjectPolicy<T>();

        /// <summary>
        /// Creates a new pooling policy instance
        /// </summary>
        public ObjectPolicy()
        { }

        public T Create()
        {
            return new T();
        }
        public bool Return(T instance)
        {
            return true;
        }
        public bool Delete(T instance)
        {
            IDisposable disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            return true;
        }
    }
}