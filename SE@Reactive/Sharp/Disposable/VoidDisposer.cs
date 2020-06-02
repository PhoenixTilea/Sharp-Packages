// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Empty disposer calling to void
    /// </summary>
    public class VoidDisposer : IDisposable
    {
        /// <summary>
        /// A generic void disposer instance
        /// </summary>
        public static readonly VoidDisposer Instance = new VoidDisposer();

        private VoidDisposer()
        { }
        public void Dispose()
        { }
    }
}
