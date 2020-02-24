// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class BinaryExtension
    {
        /// <summary>
        /// Tests two byte arrays for equality
        /// </summary>
        /// <param name="rhs">The array this array should be compared to</param>
        /// <returns>True if both arrays are equal at byte level, false otherwise</returns>
        public static bool BinaryEquals(this byte[] lhs, byte[] rhs)
        {
            if (lhs == rhs) return true;
            else if ((lhs == null && rhs != null) || (lhs != null && rhs == null) || lhs.Length != rhs.Length) return false;
            else for (int i = 0; i < lhs.Length; i++)
                    if (lhs[i] != rhs[i])
                        return false;

            return true;
        }
    }
}