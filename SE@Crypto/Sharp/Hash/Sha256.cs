// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Security.Cryptography;
using System.IO;

namespace SE.Crypto
{
    /// <summary>
    /// 256 bit Secure Hash Algorithm
    /// </summary>
    public sealed class Sha256
    {
        private SHA256Managed hash;
        byte[] state = new byte[32];

        /// <summary>
        /// Current hash value based on last concatenation
        /// </summary>
        public byte[] Value
        {
            get { return hash.Hash; }
        }

        /// <summary>
        /// Creates a new initial hash value instance
        /// </summary>
        public Sha256()
        {
            hash = new SHA256Managed();
            hash.Initialize();
        }

        /// <summary>
        /// Hashes certain block of data into current state hash
        /// </summary>
        /// <param name="data">A block of data to create hash values from</param>
        public void AddHash(byte[] data)
        {
            int length = data.Length;
            for (int i = 0; i < length; i += 32)
                hash.TransformBlock(data, i, Math.Min(length - i, 32), state, 0);
        }
        /// <summary>
        /// Completes current state hash and returns the final result
        /// </summary>
        /// <returns>The final 256 bit hash value</returns>
        public byte[] Finalize(byte[] data)
        {
            hash.TransformFinalBlock(data, 0, data.Length);
            state = null;

            return Value;
        }

        /// <summary>
        /// Computes a hash value for the given block of data
        /// </summary>
        /// <param name="data">A block of data to create a hash value from</param>
        /// <returns>The final 256 bit hash value</returns>
        public static byte[] Hash(byte[] data)
        {
            return new SHA256Managed().ComputeHash(data);
        }
        /// <summary>
        /// Computes a hash value for the given block of data
        /// </summary>
        /// <param name="data">A block of data to create a hash value from</param>
        /// <returns>The final 256 bit hash value</returns>
        public static byte[] Hash(Stream data)
        {
            return new SHA256Managed().ComputeHash(data);
        }
    }
}
