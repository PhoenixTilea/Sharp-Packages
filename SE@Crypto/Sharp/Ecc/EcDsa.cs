// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Crypto
{
    /// <summary>
    /// Elliptic Curve Cryptography Utilities
    /// </summary>
    public static class EcDsa
    {
        private static Random rand = new Random();

        private static void RandomBytes(UInt64[] digits, int words)
        {
            byte[] tmp = new byte[words * 8];

            lock (rand)
                rand.NextBytes(tmp);

            for (int i = 0; i < words; i++)
                digits[i] = BitConverter.ToUInt64(tmp, (i * 8));
        }
        private static void GetScalarFromSecret(EcCurve curve, EcPoint publicKey, UInt64[] privateKey, UInt64[] scalar)
		{
			EcPoint tmp = new EcPoint(curve.Words);
			EcMath.Mult(curve, tmp, publicKey, privateKey);

			byte[] hash = new byte[curve.Bytes + 1];
			EcMath.Compress(tmp, hash);

			UInt64[] tc = new UInt64[curve.Words];
			byte[] tt = Sha256.Hash(hash);

            int length = Math.Min(curve.Words, 4);
            for (int i = 0; i < length; i++)
                tc[i] = BitConverter.ToUInt64(tt, (i * 8));

			LongMath.AssignZero(scalar, curve.Words);
            LongMath.Assign(scalar, tc, curve.Words);
		}

        /// <summary>
        /// Creates a new Elliptic Curve Cryptography Data Signing private key from
        /// a given seed
        /// </summary>
        /// <param name="curve">The curve in which order the key should exist</param>
        /// <param name="privateKey">An array of 64 bit segments to store the key</param>
        /// <param name="seed">The seed to use as key basic</param>
        public static bool GetPrivateKey(EcCurve curve, UInt64[] privateKey, byte[] seed)
        {
                for (int i = 0; i < curve.Words; i++)
                    privateKey[i] = BitConverter.ToUInt64(seed, (i * 8));

                if (LongMath.IsZero(privateKey, curve.Words))
                    return false;

                if (LongMath.Compare(curve.N, privateKey, curve.Words) != 1)
                    LongMath.Sub(privateKey, privateKey, curve.N, curve.Words);

            return (!LongMath.IsZero(privateKey, curve.Words));
        }
        /// <summary>
        /// Creates a new Elliptic Curve Cryptography Data Signing private key
        /// </summary>
        /// <param name="curve">The curve in which order the key should exist</param>
        /// <param name="privateKey">An array of 64 bit segments to store the key</param>
        public static void GetPrivateKey(EcCurve curve, UInt64[] privateKey)
        {
            do
            {
                RandomBytes(privateKey, curve.Words);
                if (LongMath.IsZero(privateKey, curve.Words))
                    continue;

                if (LongMath.Compare(curve.N, privateKey, curve.Words) != 1)
                    LongMath.Sub(privateKey, privateKey, curve.N, curve.Words);
            }
            while (LongMath.IsZero(privateKey, curve.Words));
        }
        /// <summary>
        /// Creates the coresponding Elliptic Curve Cryptography Data Signing public key
        /// from a given private key
        /// </summary>
        /// <param name="curve">The curve in which order the key should exist</param>
        /// <param name="privateKey">The key to generate it's public counter part for</param>
        /// <param name="publicKey">The resulting public key</param>
        public static void GetPublicKey(EcCurve curve, UInt64[] privateKey, EcPoint publicKey)
        {
            EcMath.Mult(curve, publicKey, curve.G, privateKey);
        }

        /// <summary>
        /// Creates a shared secret from two Elliptic Curve Cryptography keys
        /// </summary>
        /// <param name="curve">The curve in which order the key should exist</param>
        /// <param name="publicKey">A foreign Elliptic Curve Cryptography public key</param>
        /// <param name="selfPrivateKey">The local Elliptic Curve Cryptography private key</param>
        /// <param name="shared">An array of 64 bit segments to store the secret</param>
        /// <returns>An indicator flag to determine if the result is odd or even</returns>
        public static byte GetSharedKey(EcCurve curve, EcPoint publicKey, UInt64[] selfPrivateKey, UInt64[] shared)
	    {
		    EcPoint tmp = new EcPoint(curve.Words);
		    EcMath.Mult(curve, tmp, publicKey, selfPrivateKey);
		    LongMath.Assign(shared, tmp.X, curve.Words);

            if (EcMath.IsZero(tmp)) return 0;
		    else return (byte)(2 + (tmp.Y[0] & 0x01));
	    }
        /// <summary>
        /// Creates a private secret key from two Elliptic Curve Cryptography keys. This
        /// key could be used to sign data using Elliptic Curve Cryptography Data Signing Algorithm
        /// </summary>
        /// <param name="curve">The curve in which order the key should exist</param>
        /// <param name="publicKey">A foreign Elliptic Curve Cryptography public key</param>
        /// <param name="selfPrivateKey">The local Elliptic Curve Cryptography private key</param>
        /// <param name="sharedPrivateKey">An array of 64 bit segments to store the key</param>
        public static void GetSharedPrivateKey(EcCurve curve, EcPoint publicKey, UInt64[] selfPrivateKey, UInt64[] sharedPrivateKey)
	    {
		    UInt64[] c = new UInt64[curve.Words];
		    GetScalarFromSecret(curve, publicKey, selfPrivateKey, c);

		    LongMath.ModAdd(sharedPrivateKey, selfPrivateKey, c, curve.N, curve.Words);
	    }
        /// <summary>
        /// Creates a public secret key from two Elliptic Curve Cryptography keys. This
        /// key could be used to verify data using Elliptic Curve Cryptography Data Signing Algorithm
        /// </summary>
        /// <param name="curve">The curve in which order the key should exist</param>
        /// <param name="publicKey">A foreign Elliptic Curve Cryptography public key</param>
        /// <param name="selfPrivateKey">The local Elliptic Curve Cryptography private key</param>
        /// <param name="sharedPublicKey">The resulting public key</param>
        public static void GetSharedPublicKey(EcCurve curve, EcPoint publicKey, UInt64[] selfPrivateKey, EcPoint sharedPubKey)
	    {
		    UInt64[] c = new UInt64[curve.Words];
		    GetScalarFromSecret(curve, publicKey, selfPrivateKey, c);

		    EcPoint tmp = new EcPoint(curve.Words);
		    EcMath.Mult(curve, tmp, curve.G, c);
            EcMath.Add(curve, sharedPubKey, publicKey, tmp);
	    }

        /// <summary>
        /// Computes an Elliptic Curve Cryptography signature pair from a hash value and
        /// the local private key
        /// </summary>
        /// <param name="curve">The curve in which order the key should exist</param>
        /// <param name="privateKey">The local Elliptic Curve Cryptography private key</param>
        /// <param name="hash">A hash value to create signature from</param>
        /// <param name="signature">The resulting signature pair</param>
        public static void Sign(EcCurve curve, UInt64[] privateKey, byte[] hash, EcPoint signature)
        {
            UInt64[] k = new UInt64[curve.Words];
            do
            {
                GetPrivateKey(curve, k);
                EcMath.Mult(curve, signature, curve.G, k);

                if (LongMath.Compare(curve.N, signature.X, curve.Words) != 1)
                    LongMath.Sub(signature.X, signature.X, curve.N, curve.Words);
            }
            while (LongMath.IsZero(signature.X, curve.Words));

            LongMath.ModMult(signature.Y, signature.X, privateKey, curve.N, curve.Words);

            UInt64[] tt = new UInt64[curve.Words];
            int length = Math.Min(curve.Words, hash.Length / 8);
            for (int i = 0; i < length; i++)
                tt[i] = BitConverter.ToUInt64(hash, (i * 8));

            LongMath.ModAdd(signature.Y, tt, signature.Y, curve.N, curve.Words);
            LongMath.ModInv(k, k, curve.N, curve.Words);
            LongMath.ModMult(signature.Y, signature.Y, k, curve.N, curve.Words);
        }
        /// <summary>
        /// Verifies a signature pair to confirm the coresponding Elliptic Curve Cryptography
        /// public key
        /// </summary>
        /// <param name="curve">The curve in which order the key should exist</param>
        /// <param name="publicKey">The known coresponding public key</param>
        /// <param name="hash">The hash value the signature was computed from</param>
        /// <param name="signature">The signature pair to verify against</param>
        /// <returns>True if the signature pair matches the coresponding public key, false otherwise</returns>
        public static bool Verify(EcCurve curve, EcPoint publicKey, byte[] hash, EcPoint signature)
        {
            if (LongMath.IsZero(signature.X, curve.Words) || LongMath.IsZero(signature.Y, curve.Words)) return false;
            if (LongMath.Compare(curve.N, signature.X, curve.Words) != 1 || LongMath.Compare(curve.N, signature.Y, curve.Words) != 1) return false;

            UInt64[] z = new UInt64[curve.Words];
            UInt64[] u1 = new UInt64[curve.Words];
            UInt64[] u2 = new UInt64[curve.Words];
            LongMath.ModInv(z, signature.Y, curve.N, curve.Words);

            UInt64[] tt = new UInt64[curve.Words];
            int length = Math.Min(curve.Words, hash.Length / 8);
            for (int i = 0; i < length; i++)
                tt[i] = BitConverter.ToUInt64(hash, (i * 8));

            LongMath.ModMult(u1, tt, z, curve.N, curve.Words);
            LongMath.ModMult(u2, signature.X, z, curve.N, curve.Words);

            EcPoint pq = new EcPoint(curve.Words);
            LongMath.Assign(pq.X, publicKey.X, curve.Words);
            LongMath.Assign(pq.Y, publicKey.Y, curve.Words);

            UInt64[] tX = new UInt64[curve.Words];
            UInt64[] tY = new UInt64[curve.Words];
            UInt64[] tZ = new UInt64[curve.Words];
            LongMath.Assign(tX, curve.G.X, curve.Words);
            LongMath.Assign(tY, curve.G.Y, curve.Words);
            LongMath.ModSub(z, pq.X, curve.G.X, curve.P, curve.Words);
            EcMath.Add(curve, tX, tY, pq.X, pq.Y);
            LongMath.ModInv(z, z, curve.P, curve.Words);
            EcMath.Add(curve, pq.X, pq.Y, z);

            EcPoint[] shamirPoints = new EcPoint[4]
            { 
                null, 
                curve.G, 
                publicKey,
                pq
            };
            int bits = Math.Max(LongMath.GetBits(u1, curve.Words), LongMath.GetBits(u2, curve.Words));

            EcPoint p = shamirPoints
            [
                ((LongMath.GetBitLong(u1, bits - 1, curve.Words)) ? 1 : 0) |
                (((LongMath.GetBitLong(u2, bits - 1, curve.Words)) ? 1 : 0) << 1)
            ];

            UInt64[] rX = new UInt64[curve.Words];
            UInt64[] rY = new UInt64[curve.Words];
            LongMath.Assign(rX, p.X, curve.Words);
            LongMath.Assign(rY, p.Y, curve.Words);
            LongMath.AssignZero(z, curve.Words);
            z[0] = 1;

            for (int i = (bits - 2); i >= 0; i--)
            {
                EcMath.Double(curve, rX, rY, z);
                p = shamirPoints
                [
                    ((LongMath.GetBitLong(u1, i, curve.Words)) ? 1 : 0) |
                    (((LongMath.GetBitLong(u2, i, curve.Words)) ? 1 : 0) << 1)
                ];
                if (p != null)
                {
                    LongMath.Assign(tX, p.X, curve.Words);
                    LongMath.Assign(tY, p.Y, curve.Words);
                    EcMath.Add(curve, tX, tY, z);
                    LongMath.ModSub(tZ, rX, tX, curve.P, curve.Words);
                    EcMath.Add(curve, tX, tY, rX, rY);
                    EcMath.ModMult(curve, z, z, tZ);
                }
            }
            LongMath.ModInv(z, z, curve.P, curve.Words);
            EcMath.Add(curve, rX, rY, z);

            if (LongMath.Compare(curve.N, rX, curve.Words) != 1)
                LongMath.Sub(rX, rX, curve.N, curve.Words);

            return (LongMath.Compare(rX, signature.X, curve.Words) == 0);
        }
    }
}
