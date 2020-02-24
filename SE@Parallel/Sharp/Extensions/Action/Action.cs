// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    public static partial class ActionExtension
    {
        private static HashSet<UInt32> onceSet;
        private static Spinlock onceLock;

        static ActionExtension()
        {
            onceSet = new HashSet<UInt32>();
            onceLock = new Spinlock();
        }
    }
}
