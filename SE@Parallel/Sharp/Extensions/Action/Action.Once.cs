// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    public static partial class ActionExtension
    {
        /// <summary>
        /// Executes this action only once for all calling threads
        /// </summary>
        /// <param name="id">An identifier to determine if action was already running</param>
        /// <returns>True if the action was executed, false otherwise</returns>
        public static bool Once(this Action action, UInt32 id)
        {
            using (ThreadContext.Lock(onceLock))
            {
                if (!onceSet.Add(id))
                    return false;
            }

            action();
            return true;
        }
    }
}
