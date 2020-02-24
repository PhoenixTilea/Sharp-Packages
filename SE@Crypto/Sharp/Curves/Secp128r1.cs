// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Crypto
{
    /// <summary>
    /// 128 bit Elliptic Curve Cryptography Curve Config
    /// </summary>
    public class Secp128r1 : EcCurve
    {
        public Secp128r1()
            : base(2)
        {
            P[0] = 0xFFFFFFFFFFFFFFFFul;
            P[1] = 0xFFFFFFFDFFFFFFFFul;

            B[0] = 0xD824993C2CEE5ED3ul;
            B[1] = 0xE87579C11079F43Dul;

            G.X[0] = 0x0C28607CA52C5B86ul;
            G.X[1] = 0x161FF7528B899B2Dul;
            G.Y[0] = 0xC02DA292DDED7A83ul;
            G.Y[1] = 0xCF5AC8395BAFEB13ul;

            N[0] = 0x75A30D1B9038A115ul;
            N[1] = 0xFFFFFFFE00000000ul;
        }

        public override void ModMultFast(UInt64[] a, UInt64[] b)
        {
            UInt64[] tmp = new UInt64[Words];
            UInt64 carry;

            LongMath.Assign(a, b, Words);
            tmp[0] = b[2];
            tmp[1] = (b[3] & 0x1FFFFFFFFul) | (b[2] << 33);
            carry = LongMath.Add(a, a, tmp, Words);

            tmp[0] = (b[2] >> 31) | (b[3] << 33);
            tmp[1] = (b[3] >> 31) | ((b[2] & 0xFFFFFFFF80000000ul) << 2);
            carry += LongMath.Add(a, a, tmp, Words);

            tmp[0] = (b[2] >> 62) | (b[3] << 2);
            tmp[1] = (b[3] >> 62) | ((b[2] & 0xC000000000000000ul) >> 29) | (b[3] << 35);
            carry += LongMath.Add(a, a, tmp, Words);

            tmp[0] = (b[3] >> 29);
            tmp[1] = ((b[3] & 0xFFFFFFFFE0000000ul) << 4);
            carry += LongMath.Add(a, a, tmp, Words);

            tmp[0] = (b[3] >> 60);
            tmp[1] = (b[3] & 0xFFFFFFFE00000000ul);
            carry += LongMath.Add(a, a, tmp, Words);

            tmp[0] = 0;
            tmp[1] = ((b[3] & 0xF000000000000000ul) >> 27);
            carry += LongMath.Add(a, a, tmp, Words);

            while (carry != 0 || LongMath.Compare(P, a, Words) != 1)
                carry -= LongMath.Sub(a, a, P, Words);
        }
    }
}
