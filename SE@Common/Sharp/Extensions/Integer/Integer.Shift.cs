// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class IntegerExtension
    {
        /// <summary>
        /// Shifts the bits of this integer left n positions and appends the
        /// carry to the front
        /// </summary>
        /// <param name="shift">The amount of positions to shift the bits</param>
        /// <returns>The resulting bits integer</returns>
        public static UInt16 LeftRotateShift(this UInt16 i, UInt16 shift)
        {
            int bits = sizeof(UInt16) * 8;
            int offset = (bits - shift);

            UInt16 mask = (UInt16)(i.MaxValue() >> offset);
            UInt16 carry = (UInt16)((i & (mask << offset)) >> offset);

            return (UInt16)((i << shift) | carry);
        }
        /// <summary>
        /// Shifts the bits of this integer left n positions and appends the
        /// carry to the front
        /// </summary>
        /// <param name="shift">The amount of positions to shift the bits</param>
        /// <returns>The resulting bits integer</returns>
        public static UInt16 RightRotateShift(this UInt16 i, UInt16 shift)
        {
            int bits = sizeof(UInt16) * 8;
            int offset = (bits - shift);

            UInt16 mask = (UInt16)(i.MaxValue() >> offset);
            UInt16 carry = (UInt16)((i & mask) << offset);

            return (UInt16)((i >> shift) | carry);
        }

        /// <summary>
        /// Shifts the bits of this integer left n positions and appends the
        /// carry to the front
        /// </summary>
        /// <param name="shift">The amount of positions to shift the bits</param>
        /// <returns>The resulting bits integer</returns>
        public static UInt32 LeftRotateShift(this UInt32 i, UInt16 shift)
        {
            int bits = sizeof(UInt32) * 8;
            int offset = (bits - shift);

            UInt32 mask = (i.MaxValue() >> offset);
            UInt32 carry = ((i & (mask << offset)) >> offset);

            return ((i << shift) | carry);
        }
        /// <summary>
        /// Shifts the bits of this integer left n positions and appends the
        /// carry to the front
        /// </summary>
        /// <param name="shift">The amount of positions to shift the bits</param>
        /// <returns>The resulting bits integer</returns>
        public static UInt32 RightRotateShift(this UInt32 i, UInt16 shift)
        {
            int bits = sizeof(UInt32) * 8;
            int offset = (bits - shift);

            UInt32 mask = (i.MaxValue() >> offset);
            UInt32 carry = ((i & mask) << offset);

            return ((i >> shift) | carry);
        }

        /// <summary>
        /// Shifts the bits of this integer left n positions and appends the
        /// carry to the front
        /// </summary>
        /// <param name="shift">The amount of positions to shift the bits</param>
        /// <returns>The resulting bits integer</returns>
        public static UInt64 LeftRotateShift(this UInt64 i, UInt16 shift)
        {
            int bits = sizeof(UInt64) * 8;
            int offset = (bits - shift);

            UInt64 mask = (i.MaxValue() >> offset);
            UInt64 carry = ((i & (mask << offset)) >> offset);

            return ((i << shift) | carry);
        }
        /// <summary>
        /// Shifts the bits of this integer left n positions and appends the
        /// carry to the front
        /// </summary>
        /// <param name="shift">The amount of positions to shift the bits</param>
        /// <returns>The resulting bits integer</returns>
        public static UInt64 RightRotateShift(this UInt64 i, UInt16 shift)
        {
            int bits = sizeof(UInt64) * 8;
            int offset = (bits - shift);

            UInt64 mask = (i.MaxValue() >> offset);
            UInt64 carry = ((i & mask) << offset);

            return ((i >> shift) | carry);
        }
    }
}
