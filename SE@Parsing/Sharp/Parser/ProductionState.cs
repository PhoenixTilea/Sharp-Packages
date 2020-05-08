// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// A collection of production rules used by the TreeBuilder
    /// </summary>
    public enum ProductionState : byte
    {
        Next = 0,

        Preserve = 0x1,
        Shift = 0x2,
        Reduce = 0x4,
        Revert = 0x8,

        Failure = 0x40,
        Success = 0x80
    }
}