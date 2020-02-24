// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Crypto
{
    /// <summary>
    /// 192 bit Elliptic Curve Cryptography Curve Config
    /// </summary>
    public class Secp192r1 : EcCurve
    {
        public Secp192r1()
            : base(3)
        {
            P[0] = 0xFFFFFFFFFFFFFFFFul;
            P[1] = 0xFFFFFFFFFFFFFFFEul;
            P[2] = 0xFFFFFFFFFFFFFFFFul;

            B[0] = 0xFEB8DEECC146B9B1ul;
            B[1] = 0x0FA7E9AB72243049ul;
            B[2] = 0x64210519E59C80E7ul;

            G.X[0] = 0xF4FF0AFD82FF1012ul;
            G.X[1] = 0x7CBF20EB43A18800ul;
            G.X[2] = 0x188DA80EB03090F6ul;

            G.Y[0] = 0x73F977A11E794811ul;
            G.Y[1] = 0x631011ED6B24CDD5ul;
            G.Y[2] = 0x07192B95FFC8DA78ul;

            N[0] = 0x146BC9B1B4D22831ul;
            N[1] = 0xFFFFFFFF99DEF836ul;
            N[2] = 0xFFFFFFFFFFFFFFFFul;
        }

        public override void ModMultFast(UInt64[] a, UInt64[] b)
        {
            UInt64[] tmp = new UInt64[Words];
			UInt64 carry;
    
			LongMath.Assign(a, b, Words);
			LongMath.Assign(tmp, 0, b, 3, Words);
			carry = LongMath.Add(a, a, tmp, Words);
    
			tmp[0] = 0;
			tmp[1] = b[3];
			tmp[2] = b[4];
			carry += LongMath.Add(a, a, tmp, Words);
    
			tmp[0] = tmp[1] = b[5];
			tmp[2] = 0;
			carry += LongMath.Add(a, a, tmp, Words);
    
			while(carry != 0 || LongMath.Compare(P, a, Words) != 1)
                carry -= LongMath.Sub(a, a, P, Words);
        }
    }
}
