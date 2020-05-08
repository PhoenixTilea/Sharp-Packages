// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class IntegerExtension
    {
        /// <summary>
        /// Determines the number of bytes used by this integer
        /// </summary>
        /// <returns>The number of bytes used</returns>
        public static int Count(this Int16 i)
        {
            int n = 0;
            do
            {
                i >>= 8;
                n++;
            }
            while (i > 0);
            return n;
        }
        /// <summary>
        /// Determines the number of bytes used by this integer
        /// </summary>
        /// <returns>The number of bytes used</returns>
        public static int Count(this UInt16 i)
        {
            int n = 0;
            do
            {
                i >>= 8;
                n++;
            }
            while (i > 0);
            return n;
        }
        /// <summary>
        /// Determines the number of bytes used by this integer
        /// </summary>
        /// <returns>The number of bytes used</returns>
        public static int Count(this Int32 i)
        {
            int n = 0;
            do
            {
                i >>= 8;
                n++;
            }
            while (i > 0);
            return n;
        }
        /// <summary>
        /// Determines the number of bytes used by this integer
        /// </summary>
        /// <returns>The number of bytes used</returns>
        public static int Count(this UInt32 i)
        {
            int n = 0;
            do
            {
                i >>= 8;
                n++;
            }
            while (i > 0);
            return n;
        }
        /// <summary>
        /// Determines the number of bytes used by this integer
        /// </summary>
        /// <returns>The number of bytes used</returns>
        public static int Count(this Int64 i)
        {
            int n = 0;
            do
            {
                i >>= 8;
                n++;
            }
            while (i > 0);
            return n;
        }
        /// <summary>
        /// Determines the number of bytes used by this integer
        /// </summary>
        /// <returns>The number of bytes used</returns>
        public static int Count(this UInt64 i)
        {
            int n = 0;
            do
            {
                i >>= 8;
                n++;
            }
            while (i > 0);
            return n;
        }
    }
}