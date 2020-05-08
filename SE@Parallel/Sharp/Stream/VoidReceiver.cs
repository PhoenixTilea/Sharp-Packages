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
    public class EmptyNotifier : IPromiseNotifier<object>
    {
        public EmptyNotifier()
        { }

        public void OnResolve(object value)
        { }
        public void OnReject(Exception error)
        {
            #if DEBUG
            throw error;
            #endif
        }
    }
}
