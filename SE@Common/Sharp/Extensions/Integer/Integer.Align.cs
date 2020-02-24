// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class IntegerExtension
    {
        /// <summary>
        /// Aligns this integer to be at least of or a multiple of the provided alignment
        /// </summary>
        /// <param name="alignment">The alignment this integer should fit into</param>
        public static Int16 Align(this Int16 i, Int16 alignment)
        {
            int mod = i % alignment;
            if (mod > 0) return (Int16)(i + (alignment - mod));
            else return i;
        }
        /// <summary>
        /// Aligns this integer to be at least of or a multiple of the provided alignment
        /// </summary>
        /// <param name="alignment">The alignment this integer should fit into</param>
        public static UInt16 Align(this UInt16 i, UInt16 alignment)
        {
            int mod = i % alignment;
            if (mod > 0) return (UInt16)(i + (alignment - mod));
            else return i;
        }

        /// <summary>
        /// Aligns this integer to be at least of or a multiple of the provided alignment
        /// </summary>
        /// <param name="alignment">The alignment this integer should fit into</param>
        public static Int32 Align(this Int32 i, Int32 alignment)
        {
            Int32 mod = i % alignment;
            if (mod > 0) return (Int32)(i + (alignment - mod));
            else return i;
        }
        /// <summary>
        /// Aligns this integer to be at least of or a multiple of the provided alignment
        /// </summary>
        /// <param name="alignment">The alignment this integer should fit into</param>
        public static UInt32 Align(this UInt32 i, UInt32 alignment)
        {
            UInt32 mod = i % alignment;
            if (mod > 0) return (UInt32)(i + (alignment - mod));
            else return i;
        }

        /// <summary>
        /// Aligns this integer to be at least of or a multiple of the provided alignment
        /// </summary>
        /// <param name="alignment">The alignment this integer should fit into</param>
        public static Int64 Align(this Int64 i, Int64 alignment)
        {
            Int64 mod = i % alignment;
            if (mod > 0) return (Int64)(i + (alignment - mod));
            else return i;
        }
        /// <summary>
        /// Aligns this integer to be at least of or a multiple of the provided alignment
        /// </summary>
        /// <param name="alignment">The alignment this integer should fit into</param>
        public static UInt64 Align(this UInt64 i, UInt64 alignment)
        {
            UInt64 mod = i % alignment;
            if (mod > 0) return (UInt64)(i + (alignment - mod));
            else return i;
        }
    }
}
