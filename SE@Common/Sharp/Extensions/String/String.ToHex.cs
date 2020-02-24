// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class StringExtension
    {
        /// <summary>
        /// Converts this hex string into the corresponding byte array
        /// </summary>
        /// <returns>An array of bytes translated from the hex notation</returns>
        public static byte[] ToHex(this string hex)
        {
            byte[] bytes = new byte[hex.Length / 2 + hex.Length % 2];
            for (int i = 0; i < hex.Length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, Math.Min(2, hex.Length - i)), 16);

            return bytes;
        }
    }
}
