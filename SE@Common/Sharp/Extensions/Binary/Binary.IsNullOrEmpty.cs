// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class BinaryExtension
    {
        /// <summary>
        /// Determines if the array is null or zero length
        /// </summary>
        /// <returns>True if the array is either null or zero length, false otherwise</returns>
        public static bool IsNullOrEmpty(this byte[] array)
        {
            return (array == null || array.Length == 0);
        }
    }
}