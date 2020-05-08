// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    public static partial class StringExtension
    {
        /// <summary>
        /// Converts this string to 32 bit component ID
        /// </summary>
        public static UInt32 ToComponentId(this string s)
        {
            return (UInt32)s.GetHashCode();
        }
    }
}
