// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    /// <summary>
    /// Determines current state of the scheduler
    /// </summary>
    public enum ThreadSchedulerState : byte
    {
        Pending = 0,
        Initializing = 1,
        Running = 2
    }
}
