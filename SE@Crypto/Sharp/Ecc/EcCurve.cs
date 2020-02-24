// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Crypto
{
    /// <summary>
    /// Elliptic Curve Cryptography Curve Descriptor
    /// </summary>
    public abstract class EcCurve
    {
        int size;
        /// <summary>
        /// Size of 64 bit segments per value in this curve
        /// </summary>
        public int Words
        {
            get { return size; }
        }
        /// <summary>
        /// Size of bytes per value in this curve
        /// </summary>
        public int Bytes
        {
            get { return (size * 8); }
        }

        UInt64[] p;
        /// <summary>
        /// This curves modulus
        /// </summary>
        public UInt64[] P
        {
            get { return p; }
        }
        UInt64[] b;
		/// <summary>
		/// The curve parameter
		/// </summary>
        public UInt64[] B
        {
            get { return b; }
        }

        EcPoint g;
		/// <summary>
		/// A base point describing the curve
		/// </summary>
        public EcPoint G
        {
            get { return g; }
        }

        UInt64[] n;
		/// <summary>
		/// Order of point G. The number of points on the cuvre that could
		/// be generated through multiplication of G
		/// </summary>
        public UInt64[] N
        {
            get { return n; }
        }

        /// <summary>
        /// Declares a new curve descriptor
        /// </summary>
        /// <param name="size">Size in 64 bit segments per value</param>
        public EcCurve(int size)
        {
            this.size = size;
            this.p = new UInt64[size];
            this.b = new UInt64[size];
            this.g = new EcPoint(size);
            this.n = new UInt64[size];
        }

        /// <summary>
        /// Fast modulo b over p
		/// </summary>
		public abstract void ModMultFast(UInt64[] a, UInt64[] b);
    }
}
