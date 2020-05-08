// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// Controls the caching behavior of an object pool
    /// </summary>
    public interface IPoolingPolicy<T>
    {
        /// <summary>
        /// Creates a new object instance
        /// </summary>
        T Create();

        /// <summary>
        /// Handles an object instance returned to the pool
        /// </summary>
        /// <param name="instance">An object instance to cache for reuse</param>
        /// <returns>True if the instance was reset to default properly, false otherwise</returns>
        bool Return(T instance);

        /// <summary>
        /// Deletes an object instance from memory
        /// </summary>
        /// <param name="instance">An object instance to delete</param>
        /// <returns>True if the instance was prepared for deletion successfully, false otherwise</returns>
        bool Delete(T instance);
    }
}