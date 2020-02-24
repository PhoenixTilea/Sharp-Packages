// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using SE;

namespace SE.DataModel
{
    /// <summary>
    /// An abstract base class to classes that hold a flex field
    /// </summary>
    public abstract class FlexFieldContainer : CriticalFinalizerObject, IDisposable
    {
        protected FlexFieldContainer()
        { }
        ~FlexFieldContainer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected abstract void Dispose(bool disposing);
    }
}
