// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Crypto
{
    /// <summary>
    /// 384 bit Elliptic Curve Cryptography Curve Config
    /// </summary>
    public class Secp384r1 : EcCurve
    {
        public Secp384r1()
            : base(6)
        {
            P[0] = 0x00000000FFFFFFFFul;
            P[1] = 0xFFFFFFFF00000000ul;
            P[2] = 0xFFFFFFFFFFFFFFFEul;
            P[3] = 0xFFFFFFFFFFFFFFFFul;
            P[4] = 0xFFFFFFFFFFFFFFFFul;
            P[5] = 0xFFFFFFFFFFFFFFFFul;

            B[0] = 0x2A85C8EDD3EC2AEFul;
            B[1] = 0xC656398D8A2ED19Dul;
            B[2] = 0x0314088F5013875Aul;
            B[3] = 0x181D9C6EFE814112ul;
            B[4] = 0x988E056BE3F82D19ul;
            B[5] = 0xB3312FA7E23EE7E4ul;

            G.X[0] = 0x3A545E3872760AB7ul;
            G.X[1] = 0x5502F25DBF55296Cul;
            G.X[2] = 0x59F741E082542A38ul;
            G.X[3] = 0x6E1D3B628BA79B98ul;
            G.X[4] = 0x8EB1C71EF320AD74ul;
            G.X[5] = 0xAA87CA22BE8B0537ul;
            G.Y[0] = 0x7A431D7C90EA0E5Ful;
            G.Y[1] = 0x0A60B1CE1D7E819Dul;
            G.Y[2] = 0xE9DA3113B5F0B8C0ul;
            G.Y[3] = 0xF8F41DBD289A147Cul;
            G.Y[4] = 0x5D9E98BF9292DC29ul;
            G.Y[5] = 0x3617DE4A96262C6Ful;

            N[0] = 0xECEC196ACCC52973ul;
            N[1] = 0x581A0DB248B0A77Aul;
            N[2] = 0xC7634D81F4372DDFul;
            N[3] = 0xFFFFFFFFFFFFFFFFul;
            N[4] = 0xFFFFFFFFFFFFFFFFul;
            N[5] = 0xFFFFFFFFFFFFFFFFul;
        }

        void OmegaMult(UInt64[] a, int aIndex, UInt64[] b, int bIndex)
			{
				UInt64[] tmp = new UInt64[Words];
				UInt64 carry, borrow;

                LongMath.Assign(a, aIndex, b, bIndex, Words);
                carry = LongMath.LeftShift(tmp, 0, b, bIndex, 32, Words);
                a[1 + Words] = (carry + LongMath.Add(a, 1 + aIndex, a, 1 + aIndex, tmp, 0, Words));
                a[2 + Words] = LongMath.Add(a, 2 + aIndex, a, 2 + aIndex, b, bIndex, Words);
                carry += LongMath.Sub(a, aIndex, a, aIndex, tmp, 0, Words);
                borrow = (a[Words + aIndex] - carry);
                if (borrow > a[Words + aIndex])
				{
                    for (int i = Words + 1; ; i++)
					{
                        a[i + aIndex]--;
                        unchecked
                        {
                            if (a[i + aIndex] != (UInt64)(-1))
                                break;
                        }
					}
				}
                a[Words + aIndex] = borrow;
			}
			public override void ModMultFast(UInt64[] a, UInt64[] b)
			{
				UInt64[] tmp = new UInt64[Words * 2];
                while (!LongMath.IsZero(b, Words, Words))
				{
					UInt64 carry = 0;
                    LongMath.AssignZero(tmp, Words);
                    LongMath.AssignZero(tmp, Words, Words);
                    OmegaMult(tmp, 0, b, Words);
                    LongMath.AssignZero(b, Words, Words);
        
					for(int i = 0; i < (Words + 3); i++)
					{
						UInt64 sum = b[i] + tmp[i] + carry;
						if(sum != b[i]) carry = ((sum < b[i]) ? 1u : 0u);
						b[i] = sum;
					}
				}
                while (LongMath.Compare(b, P, Words) > 0)
                    LongMath.Sub(b, b, P, Words);
                LongMath.Assign(a, b, Words);
			}
    }
}
