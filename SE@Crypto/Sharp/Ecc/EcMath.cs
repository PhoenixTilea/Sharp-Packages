// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Crypto
{
    /// <summary>
    /// Elliptic Curve Cryptography Math Utilities
    /// </summary>
    public static class EcMath
    {
        /// <summary>
		/// Determines if a is zero
		/// </summary>
		public static bool IsZero(EcPoint a)
		{
			return (LongMath.IsZero(a.X, a.Size) && 
				    LongMath.IsZero(a.Y, a.Size));
		}

		/// <summary>
		/// Assigns product of b and c modulo P to a
		/// </summary>
		public static void ModMult(EcCurve curve, UInt64[] a, UInt64[] b, UInt64[] c)
		{
			UInt64[] tmp = new UInt64[curve.Words * 2];
			LongMath.Mult(tmp, b, c, curve.Words);
			curve.ModMultFast(a, tmp);
		}
		/// <summary>
		/// Assigns sqrt of b modulo P to a
		/// </summary>
		public static void ModSqrt(EcCurve curve, UInt64[] a, UInt64[] b)
		{
			UInt64[] tmp = new UInt64[curve.Words];
            tmp[0] = 1;
			UInt64[] result = new UInt64[curve.Words];
            result[0] = 1;

			LongMath.Add(tmp, curve.P, tmp, curve.Words);
			for(int i = (LongMath.GetBits(tmp, curve.Words) - 1); i > 1; i--)
			{
				ModMult(curve, result, result, result);
				if(LongMath.GetBitLong(tmp, i, curve.Words))
					ModMult(curve, result, result, b);
			}
			LongMath.Assign(a, result, curve.Words);
		}

		/// <summary>
		/// Encodes a into its binary representation
		/// </summary>
		public static void Encode(byte[] bytes, UInt64[] a, int words)
		{
			for(int i = 0; i < words; i++)
			{
				int index = (8 * (words - 1 - i));
				bytes[0 + index] = (byte)(a[i] >> 56);
				bytes[1 + index] = (byte)(a[i] >> 48);
				bytes[2 + index] = (byte)(a[i] >> 40);
				bytes[3 + index] = (byte)(a[i] >> 32);
				bytes[4 + index] = (byte)(a[i] >> 24);
				bytes[5 + index] = (byte)(a[i] >> 16);
				bytes[6 + index] = (byte)(a[i] >> 8);
				bytes[7 + index] = (byte)(a[i]);
			}
		}
        /// <summary>
		/// Encodes a into its binary representation
		/// </summary>
        public static void Encode(byte[] bytes, int bIndex, UInt64[] a, int aIndex, int words)
        {
            for (int i = 0; i < words; i++)
            {
                int index = (8 * (words - 1 - i)) + bIndex;
                bytes[0 + index] = (byte)(a[i + aIndex] >> 56);
                bytes[1 + index] = (byte)(a[i + aIndex] >> 48);
                bytes[2 + index] = (byte)(a[i + aIndex] >> 40);
                bytes[3 + index] = (byte)(a[i + aIndex] >> 32);
                bytes[4 + index] = (byte)(a[i + aIndex] >> 24);
                bytes[5 + index] = (byte)(a[i + aIndex] >> 16);
                bytes[6 + index] = (byte)(a[i + aIndex] >> 8);
                bytes[7 + index] = (byte)(a[i + aIndex]);
            }
        }
        /// <summary>
        /// Decodes a from its binary representation
        /// </summary>
        public static void Decode(UInt64[] a, byte[] bytes, int words)
        {
            for (int i = 0; i < words; i++)
            {
                int index = (8 * (words - 1 - i));
                a[i] = ((UInt64)bytes[0 + index] << 56) | ((UInt64)bytes[1 + index] << 48) | ((UInt64)bytes[2 + index] << 40) | ((UInt64)bytes[3 + index] << 32) |
                       ((UInt64)bytes[4 + index] << 24) | ((UInt64)bytes[5 + index] << 16) | ((UInt64)bytes[6 + index] << 8) | (UInt64)bytes[7 + index];
            }
        }
		/// <summary>
		/// Decodes a from its binary representation
		/// </summary>
		public static void Decode(UInt64[] a, int aIndex, byte[] bytes, int bIndex, int words)
		{
			for(int i = 0; i < words; i++)
			{
                int index = (8 * (words - 1 - i)) + bIndex;
                a[i + aIndex] = ((UInt64)bytes[0 + index] << 56) | ((UInt64)bytes[1 + index] << 48) | ((UInt64)bytes[2 + index] << 40) | ((UInt64)bytes[3 + index] << 32) |
					            ((UInt64)bytes[4 + index] << 24) | ((UInt64)bytes[5 + index] << 16) | ((UInt64)bytes[6 + index] << 8)  |  (UInt64)bytes[7 + index];
			}
		}

		/// <summary>
		/// Converts a point into jacobian coords 
		/// (X, Y) = (X * Z^2, Y * Z^3)
		/// </summary>
	 	public static void Add(EcCurve curve, UInt64[] x, UInt64[] y, UInt64[] z)
		{
			UInt64[] tmp = new UInt64[curve.Words];
			ModMult(curve, tmp, z, z);
			ModMult(curve, x, x, tmp);
			ModMult(curve, tmp, tmp, z);
			ModMult(curve, y, y, tmp);
		}
		/// <summary>
		/// Adds a point to another point via jacobian transform
        /// (X2, Y2) = (X1, Y1) + (X2, Y2)
		/// (X1, Y1) = (X1', Y1')
		/// </summary>
		public static void Add(EcCurve curve, UInt64[] x1, UInt64[] y1, UInt64[] x2, UInt64[] y2)
		{
			UInt64[] tmp = new UInt64[curve.Words];
			LongMath.ModSub(tmp, x2, x1, curve.P, curve.Words);
			ModMult(curve, tmp, tmp, tmp);
			ModMult(curve, x1, x1, tmp);
			ModMult(curve, x2, x2, tmp);
			LongMath.ModSub(y2, y2, y1, curve.P, curve.Words);
			ModMult(curve, tmp, y2, y2);
    
			LongMath.ModSub(tmp, tmp, x1, curve.P, curve.Words);
			LongMath.ModSub(tmp, tmp, x2, curve.P, curve.Words);
			LongMath.ModSub(x2, x2, x1, curve.P, curve.Words);
			ModMult(curve, y1, y1, x2);
			LongMath.ModSub(x2, x1, tmp, curve.P, curve.Words);
			ModMult(curve, y2, y2, x2);
			LongMath.ModSub(y2, y2, y1, curve.P, curve.Words);
			LongMath.Assign(x2, tmp, curve.Words);
		}
		/// <summary>
		/// Adds a point to another point via jacobian transform over c
        /// (X1, Y1) = (X1, Y1) - (X2, Y2)
		/// (X2, Y2) = (X1, Y1) + (X2, Y2)
		/// </summary>
		public static void Add2(EcCurve curve, UInt64[] x1, UInt64[] y1, UInt64[] x2, UInt64[] y2)
		{
			UInt64[] p1 = new UInt64[curve.Words];
			UInt64[] p2 = new UInt64[curve.Words];
			UInt64[] p3 = new UInt64[curve.Words];
			LongMath.ModSub(p1, x2, x1, curve.P, curve.Words);
			ModMult(curve, p1, p1, p1);
			ModMult(curve, x1, x1, p1);
			ModMult(curve, x2, x2, p1);
			LongMath.ModAdd(p1, y2, y1, curve.P, curve.Words);
			LongMath.ModSub(y2, y2, y1, curve.P, curve.Words);

			LongMath.ModSub(p2, x2, x1, curve.P, curve.Words);
			ModMult(curve, y1, y1, p2);
			LongMath.ModAdd(p2, x1, x2, curve.P, curve.Words);
			ModMult(curve, x2, y2, y2);
			LongMath.ModSub(x2, x2, p2, curve.P, curve.Words);
    
			LongMath.ModSub(p3, x1, x2, curve.P, curve.Words);
			ModMult(curve, y2, y2, p3);
			LongMath.ModSub(y2, y2, y1, curve.P, curve.Words);
    
			ModMult(curve, p3, p1, p1);
			LongMath.ModSub(p3, p3, p2, curve.P, curve.Words);
			LongMath.ModSub(p2, p3, x1, curve.P, curve.Words);
			ModMult(curve, p2, p2, p1);
			LongMath.ModSub(y1, p2, y1, curve.P, curve.Words);
			LongMath.Assign(x1, p3, curve.Words);
		}
		/// <summary>
		/// Helper function.
		/// Adds a point to another point sharing the same z coord
		/// </summary>
		public static void Add(EcCurve curve, EcPoint a, EcPoint b, EcPoint c)
		{
			UInt64[] tX1 = new UInt64[curve.Words];
			UInt64[] tY1 = new UInt64[curve.Words];
			UInt64[] tX2 = new UInt64[curve.Words];
			UInt64[] tY2 = new UInt64[curve.Words];

			LongMath.Assign(tX1, b.X, curve.Words);
			LongMath.Assign(tY1, b.Y, curve.Words);
			LongMath.Assign(tX2, c.X, curve.Words);
			LongMath.Assign(tY2, c.Y, curve.Words);
			Add(curve, tX1, tY1, tX2, tY2);

			UInt64[] z = new UInt64[curve.Words];
			ModMult(curve, z, b.X, tY1);
			LongMath.ModInv(z, z, curve.P, curve.Words);
			ModMult(curve, z, z, tX1);
			ModMult(curve, z, z, b.Y);
			Add(curve, tX2, tY2, z);

			LongMath.Assign(a.X, tX2, curve.Words);
			LongMath.Assign(a.Y, tY2, curve.Words);
		}

		/// <summary>
		/// Adds an ECC point in place to itself
		/// </summary>
		public static void Double(EcCurve curve, UInt64[] x, UInt64[] y, UInt64[] z)
		{
			UInt64[] p1 = new UInt64[curve.Words];
			UInt64[] p2 = new UInt64[curve.Words];
    
			if(LongMath.IsZero(z, curve.Words))
				return;
    
			ModMult(curve, p1, y, y);
			ModMult(curve, p2, x, p1);
			ModMult(curve, p1, p1, p1);
			ModMult(curve, y, y, z);
			ModMult(curve, z, z, z);
			LongMath.ModAdd(x, x, z, curve.P, curve.Words);
			LongMath.ModAdd(z, z, z, curve.P, curve.Words);
			LongMath.ModSub(z, x, z, curve.P, curve.Words);
			ModMult(curve, x, x, z);
			LongMath.ModAdd(z, x, x, curve.P, curve.Words);
			LongMath.ModAdd(x, x, z, curve.P, curve.Words);

			if(LongMath.GetBitLong(x, 0, curve.Words))
			{
				UInt64 carry = LongMath.Add(x, x, curve.P, curve.Words);
				LongMath.RightShift(x, x, 1, curve.Words);
				x[curve.Words - 1] |= (carry << 63);
			}
			else LongMath.RightShift(x, x, 1, curve.Words);

			ModMult(curve, z, x, x);
			LongMath.ModSub(z, z, p2, curve.P, curve.Words);
			LongMath.ModSub(z, z, p2, curve.P, curve.Words);
			LongMath.ModSub(p2, p2, z, curve.P, curve.Words);
			ModMult(curve, x, x, p2);
			LongMath.ModSub(p1, x, p1, curve.P, curve.Words);
			LongMath.Assign(x, z, curve.Words);
			LongMath.Assign(z, y, curve.Words);
			LongMath.Assign(y, p1, curve.Words);
		}
		/// <summary>
		/// Assigns an ECC point twice to another point
		/// </summary>
		public static void Double(EcCurve curve, UInt64[] x1, UInt64[] y1, UInt64[] x2, UInt64[] y2)
		{
			UInt64[] z = new UInt64[curve.Words];
			LongMath.Assign(x2, x1, curve.Words);
			LongMath.Assign(y2, y1, curve.Words);
    
			LongMath.AssignZero(z, curve.Words);
			z[0] = 1;

			Add(curve, x1, y1, z);
			Double(curve, x1, y1, z);
			Add(curve, x2, y2, z);
		}

		/// <summary>
		/// Assigns product of b and scalar to a
		/// </summary>
		public static void Mult(EcCurve curve, EcPoint a, EcPoint b, UInt64[] scalar)
		{
			UInt64[][] rX = new UInt64[][]
            {
                new UInt64[curve.Words],
                new UInt64[curve.Words]
            };
			UInt64[][] rY = new UInt64[][]
            {
                new UInt64[curve.Words],
                new UInt64[curve.Words]
            };
			UInt64[] z = new UInt64[curve.Words];

			LongMath.Assign(rX[1], b.X, curve.Words);
			LongMath.Assign(rY[1], b.Y, curve.Words);
			Double(curve, rX[1], rY[1], rX[0], rY[0]);

			int nb = 0;
			for(int i = (LongMath.GetBits(scalar, curve.Words) - 2); i > 0; i--)
			{
				nb = ((!LongMath.GetBitLong(scalar, i, curve.Words)) ? 1 : 0);
				Add2(curve, rX[1 - nb], rY[1 - nb], rX[nb], rY[nb]);
				Add(curve, rX[nb], rY[nb], rX[1 - nb], rY[1 - nb]);
			}
			nb = ((!LongMath.GetBitLong(scalar, 0, curve.Words)) ? 1 : 0);
			Add2(curve, rX[1 - nb], rY[1 - nb], rX[nb], rY[nb]);
    
			LongMath.ModSub(z, rX[1], rX[0], curve.P, curve.Words);
			ModMult(curve, z, z, rY[1 - nb]);
			ModMult(curve, z, z, b.X);
			LongMath.ModInv(z, z, curve.P, curve.Words);
			ModMult(curve, z, z, b.Y);
			ModMult(curve, z, z, rX[1 - nb]);
			Add(curve, rX[nb], rY[nb], rX[1 - nb], rY[1 - nb]);
			Add(curve, rX[0], rY[0], z);
			LongMath.Assign(a.X, rX[0], curve.Words);
			LongMath.Assign(a.Y, rY[0], curve.Words);
		}

		/// <summary>
		/// Compresses an ECC point into half its size + 1 in bytes
		/// </summary>
		public static void Compress(EcPoint a, byte[] compressed)
		{
			Encode(compressed, 1, a.X, 0, a.Size);
		 	compressed[0] = (byte)(2 + (a.Y[0] & 0x01));
		}
		/// <summary>
        /// Decompresses a range of bytes into an ECC point
		/// </summary>
		public static bool Decompress(EcCurve curve, EcPoint a, byte[] compressed)
		{
			if (compressed[0] == 0x04)
				return false;

            Decode(a.X, 0, compressed, 1, a.Size);
			Expand(curve, a, a.X, compressed[0]);

			return true;
		}

		/// <summary>
		/// Expands the given X-coord b into an ECC point a
		/// </summary>
		public static void Expand(EcCurve curve, EcPoint a, UInt64[] b, byte signalBit)
		{
			UInt64[] _3 = new UInt64[curve.Words];
			_3[0] = 3;

			LongMath.Assign(a.X, b, curve.Words);
			ModMult(curve, a.Y, a.X, a.X);
			LongMath.ModSub(a.Y, a.Y, _3, curve.P, curve.Words);
			ModMult(curve, a.Y, a.Y, a.X);
			LongMath.ModAdd(a.Y, a.Y, curve.B, curve.P, curve.Words);

			ModSqrt(curve, a.Y, a.Y);
			if ((int)(a.Y[0] & 0x01) != (signalBit & 0x01))
			{
				LongMath.Sub(a.Y, curve.P, a.Y, curve.Words);
			}
		}
	}
}
