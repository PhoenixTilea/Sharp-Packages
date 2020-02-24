// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// Base class for runtime dynamic generated interface proxies
    /// </summary>
    public abstract class DynamicInterfaceProxy
    {
        protected object host;

        /// <summary>
        /// Creates a new instance of this class bound to certain host object
        /// </summary>
        /// <param name="host">The object this proxy routes function calls to</param>
        public DynamicInterfaceProxy(object host)
        {
            this.host = host;
        }
    }
}
