// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Coroutines
{
    /// <summary>
    /// Control flags for the scheduler to handle certain context
    /// </summary>
    public enum ExecutionFlags : byte
    {
        Active = 1,
        Reset,

        Pending,

        Cancel,
        Completed,
        Failed
    }
}