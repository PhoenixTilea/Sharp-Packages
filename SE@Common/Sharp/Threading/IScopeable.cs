// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// Base interface for LockScope compatible objects
    /// </summary>
    public interface IScopeable
    {
        /// <summary>
        /// Is called when a scope is entered
        /// </summary>
        void Acquire();
        /// <summary>
        /// Is called when the scope was leaved
        /// </summary>
        void Release();
    }
}
