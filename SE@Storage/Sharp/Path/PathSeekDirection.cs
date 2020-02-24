// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Storage
{
    /// <summary>
    /// Flags pointing to the seek direction in file system traversal
    /// </summary>
    public enum PathSeekDirection : byte
    {
        Forward,
        Backward
    }
}
