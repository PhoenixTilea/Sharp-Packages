// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// A region to ensure exclusive write access. Should be
    /// used in a using-block statement
    /// </summary>
    public struct WriteScope : IDisposable
    {
        ReadWriteLock scopeObject;

        /// <summary>
        /// Locks the region automatically to exclusive write access
        /// </summary>
        public WriteScope(ReadWriteLock scopeObject)
        {
            scopeObject.WriteLock();
            this.scopeObject = scopeObject;
        }

        /// <summary>
        /// Releases this scope on leaving the using-block statement and calls
        /// Release on the passed scope host
        /// </summary>
        public void Dispose()
        {
            scopeObject.WriteRelease();
        }
    }
}