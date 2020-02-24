// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Security.Cryptography;
using System.IO;

namespace SE.Crypto
{
    /// <summary>
    /// 160 bit RACE Integrity Primitives Evaluation Message Digest
    /// </summary>
    public sealed class Ripemd160
    {
        private RIPEMD160Managed hash;
        byte[] state = new byte[20];

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
        public Ripemd160()
        {
            hash = new RIPEMD160Managed();
            hash.Initialize();
        }

        /// <summary>
        /// Hashes certain block of data into current state hash
        /// </summary>
        /// <param name="data">A block of data to create hash values from</param>
        public void AddHash(byte[] data)
        {
            int length = data.Length;
            for (int i = 0; i < length; i += 20)
                hash.TransformBlock(data, i, Math.Min(length - i, 20), state, 0);
        }
        /// <summary>
        /// Completes current state hash and returns the final result
        /// </summary>
        /// <returns>The final 160 bit hash value</returns>
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
        /// <returns>The final 160 bit hash value</returns>
        public static byte[] Hash(byte[] data)
        {
            return new RIPEMD160Managed().ComputeHash(data);
        }
        /// <summary>
        /// Computes a hash value for the given block of data
        /// </summary>
        /// <param name="data">A block of data to create a hash value from</param>
        /// <returns>The final 160 bit hash value</returns>
        public static byte[] Hash(Stream data)
        {
            return new RIPEMD160Managed().ComputeHash(data);
        }
    }
}
