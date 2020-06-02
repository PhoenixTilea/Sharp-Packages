// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Disposer to handle a collection of disposables
    /// </summary>
    public struct BatchDisposer : IDisposable
    {
        IEnumerable<IDisposable> targets;

        /// <summary>
        /// Creates a new disposable container from the provided instances
        /// </summary>
        public BatchDisposer(IEnumerable<IDisposable> targets)
        {
            this.targets = targets;
        }
        public void Dispose()
        {
            foreach (IDisposable target in targets)
                target.Dispose();
        }
    }
}
