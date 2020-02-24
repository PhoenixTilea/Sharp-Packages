// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Default receiver in case of no-return context execution
    /// </summary>
    public class ShadowReceiver : IReceiver
    {
        public ShadowReceiver()
        { }

        public void SetError(object host, Exception error)
        {
            #if DEBUG
            throw error;
            #endif
        }
        public void SetResult(object host, object result)
        { }
    }
}
