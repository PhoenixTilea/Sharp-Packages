// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace SE
{
    /// <summary>
    /// 32 bit Non-Cryptographic Hash Algorithm
    /// </summary>
    public class FastHash
    {
        struct FastHashState
        {
            public UInt64 length;
            public UInt32 seed;
            public UInt32 v1;
            public UInt32 v2;
            public UInt32 v3;
            public UInt32 v4;
            public int memsize;
            public byte[] memory;
        };

        const UInt32 PRIME32_1 = 2654435761U;
        const UInt32 PRIME32_2 = 2246822519U;
        const UInt32 PRIME32_3 = 3266489917U;
        const UInt32 PRIME32_4 = 668265263U;
        const UInt32 PRIME32_5 = 374761393U;

        FastHashState state;

        /// <summary>
        /// Creates a new initial hash value instance
        /// </summary>
        public FastHash()
        {
            Initialize();
        }
        void Initialize(UInt32 seed = 0)
        {
            state.seed = seed;
            state.v1 = seed + PRIME32_1 + PRIME32_2;
            state.v2 = seed + PRIME32_2;
            state.v3 = seed + 0;
            state.v4 = seed - PRIME32_1;
            state.length = 0;
            state.memsize = 0;
            state.memory = new byte[16];
        }

        /// <summary>
        /// Hashes certain block of data into current state hash
        /// </summary>
        /// <param name="data">A block of data to create hash values from</param>
        public bool AddHash(byte[] data)
        {
            int index = 0;

            state.length += (UInt32)data.Length;
            if (state.memsize + data.Length < 16)
            {
                Array.Copy(data, 0, state.memory, state.memsize, data.Length);
                state.memsize += data.Length;

                return true;
            }
            if (state.memsize > 0)
            {
                Array.Copy(data, 0, state.memory, state.memsize, 16 - state.memsize);

                state.v1 = Hash(state.v1, state.memory, index);
                index += 4;
                state.v2 = Hash(state.v2, state.memory, index);
                index += 4;
                state.v3 = Hash(state.v3, state.memory, index);
                index += 4;
                state.v4 = Hash(state.v4, state.memory, index);
                index += 4;
                index = 0;

                state.memsize = 0;
            }
            if (index <= data.Length - 16)
            {
                int limit = data.Length - 16;
                UInt32 v1 = state.v1;
                UInt32 v2 = state.v2;
                UInt32 v3 = state.v3;
                UInt32 v4 = state.v4;
                do
                {
                    v1 = Hash(v1, data, index);
                    index += 4;
                    v2 = Hash(v2, data, index);
                    index += 4;
                    v3 = Hash(v3, data, index);
                    index += 4;
                    v4 = Hash(v4, data, index);
                    index += 4;
                }
                while (index <= limit);
                state.v1 = v1;
                state.v2 = v2;
                state.v3 = v3;
                state.v4 = v4;
            }
            if (index < data.Length)
            {
                Array.Copy(data, index, state.memory, 0, data.Length - index);
                state.memsize = data.Length - index;
            }
            return true;
        }
        /// <summary>
        /// Completes current state hash and returns the final result
        /// </summary>
        /// <returns>The final 32 bit hash value</returns>
        public UInt32 Finalize()
        {
            UInt32 h32;

            int index = 0;
            if (state.length >= 16) h32 = RotateLeft(state.v1, 1) + RotateLeft(state.v2, 7) + RotateLeft(state.v3, 12) + RotateLeft(state.v4, 18);
            else h32 = state.seed + PRIME32_5;

            h32 += (UInt32)state.length;
            while (index <= state.memsize - 4)
            {
                h32 += BitConverter.ToUInt32(state.memory, index) * PRIME32_3;
                h32 = RotateLeft(h32, 17) * PRIME32_4;
                index += 4;
            }
            while (index < state.memsize)
            {
                h32 += state.memory[index] * PRIME32_5;
                h32 = RotateLeft(h32, 11) * PRIME32_1;
                index++;
            }
            h32 ^= h32 >> 15;
            h32 *= PRIME32_2;
            h32 ^= h32 >> 13;
            h32 *= PRIME32_3;
            h32 ^= h32 >> 16;
            return h32;
        }

        /// <summary>
        /// Computes a hash value for the given block of data
        /// </summary>
        /// <param name="data">A block of data to create a hash value from</param>
        /// <returns>The final 32 bit hash value</returns>
        public static UInt32 Hash(byte[] data, UInt32 seed = 0)
        {
            UInt32 h32;

            int index = 0;
            if (data.Length >= 16)
            {
                int limit = data.Length - 16;
                UInt32 v1 = seed + PRIME32_1 + PRIME32_2;
                UInt32 v2 = seed + PRIME32_2;
                UInt32 v3 = seed + 0;
                UInt32 v4 = seed - PRIME32_1;
                do
                {
                    v1 = Hash(v1, data, index);
                    index += 4;
                    v2 = Hash(v2, data, index);
                    index += 4;
                    v3 = Hash(v3, data, index);
                    index += 4;
                    v4 = Hash(v4, data, index);
                    index += 4;
                }
                while (index <= limit);
                h32 = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
            }
            else h32 = seed + PRIME32_5;
            h32 += (UInt32)data.Length;

            while (index <= data.Length - 4)
            {
                h32 += BitConverter.ToUInt32(data, index) * PRIME32_3;
                h32 = RotateLeft(h32, 17) * PRIME32_4;
                index += 4;
            }
            while (index < data.Length)
            {
                h32 += data[index] * PRIME32_5;
                h32 = RotateLeft(h32, 11) * PRIME32_1;
                index++;
            }
            h32 ^= h32 >> 15;
            h32 *= PRIME32_2;
            h32 ^= h32 >> 13;
            h32 *= PRIME32_3;
            h32 ^= h32 >> 16;
            return h32;
        }
        /// <summary>
        /// Computes a hash value for the given block of data
        /// </summary>
        /// <param name="data">A block of data to create a hash value from</param>
        /// <param name="size">The size of data to be processed</param>
        /// <returns>The final 32 bit hash value</returns>
        public static UInt32 Hash(Stream stream, int size, UInt32 seed = 0)
        {
            UInt32 h32;

            int index = 0;
            byte[] buffer = new byte[16];
            if (size >= 16)
            {
                int limit = size - 16;
                UInt32 v1 = seed + PRIME32_1 + PRIME32_2;
                UInt32 v2 = seed + PRIME32_2;
                UInt32 v3 = seed + 0;
                UInt32 v4 = seed - PRIME32_1;
                do
                {
                    var loopIndex = 0;
                    stream.Read(buffer, 0, buffer.Length);

                    v1 = Hash(v1, buffer, loopIndex);
                    loopIndex += 4;
                    v2 = Hash(v2, buffer, loopIndex);
                    loopIndex += 4;
                    v3 = Hash(v3, buffer, loopIndex);
                    loopIndex += 4;
                    v4 = Hash(v4, buffer, loopIndex);
                    loopIndex += 4;

                    index += loopIndex;
                }
                while (index <= limit);
                h32 = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
            }
            else h32 = seed + PRIME32_5;
            h32 += (UInt32)size;

            buffer = new byte[4];
            while (index <= size - 4)
            {
                stream.Read(buffer, 0, buffer.Length);
                h32 += BitConverter.ToUInt32(buffer, 0) * PRIME32_3;
                h32 = RotateLeft(h32, 17) * PRIME32_4;
                index += 4;
            }
            buffer = new byte[1];
            while (index < size)
            {
                stream.Read(buffer, 0, buffer.Length);
                h32 += buffer[0] * PRIME32_5;
                h32 = RotateLeft(h32, 11) * PRIME32_1;
                index++;
            }
            h32 ^= h32 >> 15;
            h32 *= PRIME32_2;
            h32 ^= h32 >> 13;
            h32 *= PRIME32_3;
            h32 ^= h32 >> 16;
            return h32;
        }
        /// <summary>
        /// Computes a hash value for the given block of data
        /// </summary>
        /// <param name="data">A block of data to create a hash value from</param>
        /// <returns>The final 32 bit hash value</returns>
        public static UInt32 Hash(Stream stream, UInt32 seed = 0)
        {
            return Hash(stream, (int)stream.Length, seed);
        }

        private static UInt32 Hash(UInt32 value, byte[] data, int index)
        {
            UInt32 tmp = BitConverter.ToUInt32(data, index);
            value += tmp * PRIME32_2;
            value = RotateLeft(value, 13);
            value *= PRIME32_1;

            return value;
        }
        private static UInt32 RotateLeft(UInt32 value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }
}