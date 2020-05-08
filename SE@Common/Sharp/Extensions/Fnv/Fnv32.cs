// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class Fnv
    {
        /// <summary>
        /// Returns a 32 bit hash value from this value
        /// </summary>
        /// <param name="offsetBasis">An optional 32 bit hash value to concatenate</param>
        /// <returns>The resulting 32 bit hash value</returns>
        public static UInt32 Fnv32<T>(this T v, UInt32 offsetBasis = FnvOffsetBias) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
        {
            offsetBasis ^= (UInt32)Convert.ChangeType(v, typeof(UInt32));
            offsetBasis *= FnvPrime;

            return offsetBasis;
        }
        /// <summary>
        /// Returns a 32 bit hash value from this string
        /// </summary>
        /// <param name="offsetBasis">An optional 32 bit hash value to concatenate</param>
        /// <returns>The resulting 32 bit hash value</returns>
        public static UInt32 Fnv32(this string str, UInt32 offsetBasis = FnvOffsetBias)
        {
            for (int i = 0; i < str.Length; i++)
            {
                offsetBasis ^= str[i];
                offsetBasis *= FnvPrime;
            }
            return offsetBasis;
        }
        /// <summary>
        /// Returns a 32 bit hash value from this string
        /// </summary>
        /// <param name="offsetBasis">An optional 32 bit hash value to concatenate</param>
        /// <returns>The resulting 32 bit hash value</returns>
        public static UInt32 Fnv32(this byte[] array, UInt32 offsetBasis = FnvOffsetBias)
        {
            for (int i = 0; i < array.Length; i++)
            {
                offsetBasis ^= array[i];
                offsetBasis *= FnvPrime;
            }
            return offsetBasis;
        }
        /// <summary>
        /// Returns a 32 bit hash value from this stream
        /// </summary>
        /// <param name="size">The size of bytes to process</param>
        /// <param name="offsetBasis">An optional 32 bit hash value to concatenate</param>
        /// <returns>The resulting 32 bit hash value</returns>
        public static UInt32 Fnv32(this System.IO.Stream stream, int size, UInt32 offsetBasis = FnvOffsetBias)
        {
            for (; size > 0; size--)
            {
                offsetBasis ^= stream.Get();
                offsetBasis *= FnvPrime;
            }
            return offsetBasis;
        }
    }
}
