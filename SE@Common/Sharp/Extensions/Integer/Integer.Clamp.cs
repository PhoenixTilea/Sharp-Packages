// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class IntegerExtension
    {
        /// <summary>
        /// Clamps this value to fit into min/max range
        /// </summary>
        /// <param name="min">The lower end of the range</param>
        /// <param name="max">The upper end of the range</param>
        /// <returns>The value that is in range</returns>
        public static Int16 Clamp(this Int16 value, Int16 min, Int16 max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }
        /// <summary>
        /// Clamps this value to fit into min/max range
        /// </summary>
        /// <param name="min">The lower end of the range</param>
        /// <param name="max">The upper end of the range</param>
        /// <returns>The value that is in range</returns>
        public static UInt16 Clamp(this UInt16 value, UInt16 min, UInt16 max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }
        /// <summary>
        /// Clamps this value to fit into min/max range
        /// </summary>
        /// <param name="min">The lower end of the range</param>
        /// <param name="max">The upper end of the range</param>
        /// <returns>The value that is in range</returns>
        public static Int32 Clamp(this Int32 value, Int32 min, Int32 max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }
        /// <summary>
        /// Clamps this value to fit into min/max range
        /// </summary>
        /// <param name="min">The lower end of the range</param>
        /// <param name="max">The upper end of the range</param>
        /// <returns>The value that is in range</returns>
        public static UInt32 Clamp(this UInt32 value, UInt32 min, UInt32 max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }
        /// <summary>
        /// Clamps this value to fit into min/max range
        /// </summary>
        /// <param name="min">The lower end of the range</param>
        /// <param name="max">The upper end of the range</param>
        /// <returns>The value that is in range</returns>
        public static Int64 Clamp(this Int64 value, Int64 min, Int64 max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }
        /// <summary>
        /// Clamps this value to fit into min/max range
        /// </summary>
        /// <param name="min">The lower end of the range</param>
        /// <param name="max">The upper end of the range</param>
        /// <returns>The value that is in range</returns>
        public static UInt64 Clamp(this UInt64 value, UInt64 min, UInt64 max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }
    }
}