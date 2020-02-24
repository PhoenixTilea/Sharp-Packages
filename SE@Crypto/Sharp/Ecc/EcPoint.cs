// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Crypto
{
    /// <summary>
    /// Elliptic Curve Cryptography Curve Point
    /// </summary>
    public class EcPoint
    {
        int size;
        /// <summary>
        /// Size of 64 bit segments per value in this curve point
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        UInt64[] x;
        /// <summary>
        /// X Coord of this curve point
        /// </summary>
        public UInt64[] X
        {
            get { return x; }
        }

        UInt64[] y;
        /// <summary>
        /// Y Coord of this curve point
        /// </summary>
        public UInt64[] Y
        {
            get { return y; }
        }

        /// <summary>
        /// Creates a new curve point
        /// </summary>
        /// <param name="size">Size of 64 bit segments per value</param>
        public EcPoint(int size)
        {
            this.size = size;
            this.x = new UInt64[size];
            this.y = new UInt64[size];
        }
    }
}
