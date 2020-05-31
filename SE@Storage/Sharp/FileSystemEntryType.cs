// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Storage
{
    /// <summary>
    /// A flag to indicate the type of a file system entry
    /// </summary>
    public enum FileSystemEntryType : byte
    {
        File = 0,
        Directory = 1,
        Remote = 2
    }
}
