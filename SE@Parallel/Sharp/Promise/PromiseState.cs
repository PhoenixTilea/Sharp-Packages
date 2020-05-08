// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    /// <summary>
    /// Specifies the state of a promise.
    /// </summary>
    public enum PromiseState : byte
    {
        Resolved = 0,
        Pending = 1,
        Rejected = 2
    }
}
