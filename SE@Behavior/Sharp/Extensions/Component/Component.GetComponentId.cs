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
        /// Gets the 32 bit component ID of this object
        /// </summary>
        public static UInt32 GetComponentId(this object component)
        {
            return (UInt32)component.GetType().GetHashCode();
        }
        /// <summary>
        /// Gets the 32 bit component ID of this type
        /// </summary>
        public static UInt32 GetComponentId(this Type component)
        {
            return (UInt32)component.GetHashCode();
        }
    }
}
