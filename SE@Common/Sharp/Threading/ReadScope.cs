// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// A region to ensure parallel read access. Should be
    /// used in a using-block statement
    /// </summary>
    public struct ReadScope : IDisposable
    {
        ReadWriteLock scopeObject;

        /// <summary>
        /// Locks the region automatically to parallel read access
        /// </summary>
        public ReadScope(ReadWriteLock scopeObject)
        {
            scopeObject.ReadLock();
            this.scopeObject = scopeObject;
        }

        /// <summary>
        /// Releases this scope on leaving the using-block statement and calls
        /// Release on the passed scope host
        /// </summary>
        public void Dispose()
        {
            scopeObject.ReadRelease();
        }
    }
}