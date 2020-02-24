// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Adapter behavior that executes tasks on the same thread
    /// </summary>
    public class StubBehavior : Behavior
    {
        public StubBehavior()
        { }

        /// <summary>
        /// Initializes this Behavior
        /// </summary>
        public override void Initialize()
        {
            State = AdapterState.Ready;
        }
        /// <summary>
        /// Stores a task in this Behavior to be processed
        /// </summary>
        /// <param name="context">The execution context to store</param>
        /// <returns>True if the task was successfully received, false otherwise</returns>
        public override bool Enqueue(AdapterContext context)
        {
            context.Execute();
            return true;
        }
    }
}
