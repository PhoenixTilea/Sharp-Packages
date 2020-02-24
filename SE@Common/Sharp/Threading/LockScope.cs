// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// A region to ensure certain circumstances like locking an object. Should be
    /// used in a using-block statement
    /// </summary>
    public struct LockScope : IDisposable
    {
        IScopeable scopeObject;

        /// <summary>
        /// Calls Acquire on the passed scope host object automatically
        /// </summary>
        /// <param name="scopeObject">An object to be used as scope host</param>
        public LockScope(IScopeable scopeObject)
        {
            scopeObject.Acquire();
            this.scopeObject = scopeObject;
        }

        /// <summary>
        /// Releases this scope on leaving the using-block statement and calls
        /// Release on the passed scope host
        /// </summary>
        public void Dispose()
        {
            scopeObject.Release();
        }
    }
}
