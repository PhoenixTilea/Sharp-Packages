// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Indicates current work state of an Adapter
    /// </summary>
    public enum AdapterState
    {
        Undefined,

        Ready,
        Processing,
        Suspended,

        Error,
        Discarded
    }
}
