// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class Fnv
    {
        /// <summary>
        /// FNVb Prime Number
        /// </summary>
        public const UInt32 FnvPrime = 16777619u;
        /// <summary>
        /// FNVb Initial Value
        /// </summary>
        public const UInt32 FnvOffsetBias = 2166136261u;
    }
}
