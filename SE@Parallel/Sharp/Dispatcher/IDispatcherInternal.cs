// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// For internal use only
    /// </summary>
    internal interface IDispatcherInternal
    {
        /// <summary>
        /// Called by ChannelBase constructor
        /// </summary>
        ChannelBase Owner { set; }
    }
}
