// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Crypto
{
    /// <summary>
    /// 256 bit Elliptic Curve Cryptography Curve Config
    /// </summary>
    public class Secp256r1 : EcCurve
    {
        public Secp256r1()
            : base(4)
        {
            P[0] = 0xFFFFFFFFFFFFFFFFul;
				P[1] = 0x00000000FFFFFFFFul;
				P[2] = 0x0000000000000000ul;
				P[3] = 0xFFFFFFFF00000001ul;

				B[0] = 0x3BCE3C3E27D2604Bul;
				B[1] = 0x651D06B0CC53B0F6ul;
				B[2] = 0xB3EBBD55769886BCul;
				B[3] = 0x5AC635D8AA3A93E7ul;

				G.X[0] = 0xF4A13945D898C296ul;
				G.X[1] = 0x77037D812DEB33A0ul;
				G.X[2] = 0xF8BCE6E563A440F2ul;
				G.X[3] = 0x6B17D1F2E12C4247ul;
				G.Y[0] = 0xCBB6406837BF51F5ul;
				G.Y[1] = 0x2BCE33576B315ECEul;
				G.Y[2] = 0x8EE7EB4A7C0F9E16ul;
				G.Y[3] = 0x4FE342E2FE1A7F9Bul;

				N[0] = 0xF3B9CAC2FC632551ul;
				N[1] = 0xBCE6FAADA7179E84ul;
				N[2] = 0xFFFFFFFFFFFFFFFFul;
				N[3] = 0xFFFFFFFF00000000ul;
        }

        public override void ModMultFast(UInt64[] a, UInt64[] b)
        {
            UInt64[] tmp = new UInt64[Words];
            Int64 carry;

            /* t */
            LongMath.Assign(a, b, Words);

            /* s1 */
            tmp[0] = 0;
            tmp[1] = b[5] & 0xffffffff00000000ul;
            tmp[2] = b[6];
            tmp[3] = b[7];
            carry = (Int64)LongMath.LeftShift(tmp, tmp, 1, Words);
            carry += (Int64)LongMath.Add(a, a, tmp, Words);

            /* s2 */
            tmp[1] = b[6] << 32;
            tmp[2] = (b[6] >> 32) | (b[7] << 32);
            tmp[3] = b[7] >> 32;
            carry += (Int64)LongMath.LeftShift(tmp, tmp, 1, Words);
            carry += (Int64)LongMath.Add(a, a, tmp, Words);

            /* s3 */
            tmp[0] = b[4];
            tmp[1] = b[5] & 0xffffffff;
            tmp[2] = 0;
            tmp[3] = b[7];
            carry += (Int64)LongMath.Add(a, a, tmp, Words);

            /* s4 */
            tmp[0] = (b[4] >> 32) | (b[5] << 32);
            tmp[1] = (b[5] >> 32) | (b[6] & 0xffffffff00000000ul);
            tmp[2] = b[7];
            tmp[3] = (b[6] >> 32) | (b[4] << 32);
            carry += (Int64)LongMath.Add(a, a, tmp, Words);

            /* d1 */
            tmp[0] = (b[5] >> 32) | (b[6] << 32);
            tmp[1] = (b[6] >> 32);
            tmp[2] = 0;
            tmp[3] = (b[4] & 0xffffffff) | (b[5] << 32);
            carry -= (Int64)LongMath.Sub(a, a, tmp, Words);

            /* d2 */
            tmp[0] = b[6];
            tmp[1] = b[7];
            tmp[2] = 0;
            tmp[3] = (b[4] >> 32) | (b[5] & 0xffffffff00000000ul);
            carry -= (Int64)LongMath.Sub(a, a, tmp, Words);

            /* d3 */
            tmp[0] = (b[6] >> 32) | (b[7] << 32);
            tmp[1] = (b[7] >> 32) | (b[4] << 32);
            tmp[2] = (b[4] >> 32) | (b[5] << 32);
            tmp[3] = (b[6] << 32);
            carry -= (Int64)LongMath.Sub(a, a, tmp, Words);

            /* d4 */
            tmp[0] = b[7];
            tmp[1] = b[4] & 0xffffffff00000000ul;
            tmp[2] = b[5];
            tmp[3] = b[6] & 0xffffffff00000000ul;
            carry -= (Int64)LongMath.Sub(a, a, tmp, Words);

            if (carry < 0)
            {
                do
                {
                    carry += (Int64)LongMath.Add(a, a, P, Words);
                }
                while (carry < 0);
            }
            else
            {
                while (carry != 0 || LongMath.Compare(P, a, Words) != 1)
                    carry -= (Int64)LongMath.Sub(a, a, P, Words);
            }
        }
    }
}
