// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class Fnv
    {
        /// <summary>
        /// Returns a 16 bit hash value from this string
        /// </summary>
        /// <param name="offsetBasis">An optional 32 bit hash value to concatenate</param>
        /// <returns>The resulting 16 bit hash value</returns>
        public static UInt16 Fnv16(this string str, UInt32 offsetBasis = FnvOffsetBias)
        {
            offsetBasis = Fnv32(str, offsetBasis);
            offsetBasis ^= (offsetBasis >> 16);

            return (UInt16)(offsetBasis & UInt16.MaxValue);
        }
        /// <summary>
        /// Returns a 16 bit hash value from this string
        /// </summary>
        /// <param name="offsetBasis">An optional 32 bit hash value to concatenate</param>
        /// <returns>The resulting 16 bit hash value</returns>
        public static UInt16 Fnv16(this byte[] array, UInt32 offsetBasis = FnvOffsetBias)
        {
            offsetBasis = Fnv32(array, offsetBasis);
            offsetBasis ^= (offsetBasis >> 16);

            return (UInt16)(offsetBasis & UInt16.MaxValue);
        }
        /// <summary>
        /// Returns a 16 bit hash value from this stream
        /// </summary>
        /// <param name="size">The size of bytes to process</param>
        /// <param name="offsetBasis">An optional 32 bit hash value to concatenate</param>
        /// <returns>The resulting 16 bit hash value</returns>
        public static UInt16 Fnv16(this System.IO.Stream stream, int size, UInt32 offsetBasis = FnvOffsetBias)
        {
            offsetBasis = Fnv32(stream, size, offsetBasis);
            offsetBasis ^= (offsetBasis >> 16);

            return (UInt16)(offsetBasis & UInt16.MaxValue);
        }
    }
}
