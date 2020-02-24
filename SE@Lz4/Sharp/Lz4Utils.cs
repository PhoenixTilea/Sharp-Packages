// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Storage.Lz4
{
    /// <summary>
    /// Lz4 Compression/ Decompression Utils
    /// </summary>
    public static class Lz4Utils
    {
        public const int MemoryUsage = 14;
        public const int NotCompressibleLevel = 6;
        public const int BlockCopyLimit = 16;

        public const int MinMatch = 4;
        public const int SkipStrength = NotCompressibleLevel > 2 ? NotCompressibleLevel : 2;
        public const int CopyLength = 8;
        public const int LastLiterals = 5;
        public const int MFLimit = CopyLength + MinMatch;
        public const int MinLength = MFLimit + 1;
        public const int MaxDLog = 16;
        public const int MaxD = 1 << MaxDLog;
        public const int MaxDMask = MaxD - 1;
        public const int MaxDistance = (1 << MaxDLog) - 1;
        public const int MLBits = 4;
        public const int MLMask = (1 << MLBits) - 1;
        public const int RunBits = 8 - MLBits;
        public const int RunMask = (1 << RunBits) - 1;
        public const int StepSize = 8;
        public const int StepSize32 = 4;

        public const int Limit64 = (1 << 16) + (MFLimit - 1);

        public const int HashLog = MemoryUsage - 2;
        public const int HashTableSize = 1 << HashLog;
        public const int HashAdjust = (MinMatch * 8) - HashLog;

        public const int HashLog64 = HashLog + 1;
        public const int HashTableSize64 = 1 << HashLog64;
        public const int HashAdjust64 = (MinMatch * 8) - HashLog64;

        public const int HashLogHC = MaxDLog - 1;
        public const int HashTableSizeHC = 1 << HashLogHC;
        public const int HashAdjustHC = (MinMatch * 8) - HashLogHC;

        public static readonly int[] DecodeTable32 = { 0, 3, 2, 3, 0, 0, 0, 0 };
        public static readonly int[] DecodeTable64 = { 0, 0, 0, -1, 0, 1, 2, 3 };

        public static readonly int[] DebrujinTable32 = 
        {
            0, 0, 3, 0, 3, 1, 3, 0, 3, 2, 2, 1, 3, 2, 0, 1,
            3, 3, 1, 2, 2, 2, 2, 0, 3, 1, 2, 0, 1, 0, 1, 1
        };

        public const int MaxNBAttempts = 256;
        public const int OptimalML = (MLMask - 1) + MinMatch;

        public static void Poke2(byte[] buffer, int offset, ushort value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
        }
        public static ushort Peek2(byte[] buffer, int offset)
        {
            return (ushort)(((UInt32)buffer[offset]) | ((UInt32)buffer[offset + 1] << 8));
        }

        public static UInt32 Peek4(byte[] buffer, int offset)
        {
            return
                ((UInt32)buffer[offset]) |
                ((UInt32)buffer[offset + 1] << 8) |
                ((UInt32)buffer[offset + 2] << 16) |
                ((UInt32)buffer[offset + 3] << 24);
        }
        public static UInt32 Xor4(byte[] buffer, int offset1, int offset2)
        {
            UInt32 value1 =
                ((UInt32)buffer[offset1]) |
                ((UInt32)buffer[offset1 + 1] << 8) |
                ((UInt32)buffer[offset1 + 2] << 16) |
                ((UInt32)buffer[offset1 + 3] << 24);
            UInt32 value2 =
                ((UInt32)buffer[offset2]) |
                ((UInt32)buffer[offset2 + 1] << 8) |
                ((UInt32)buffer[offset2 + 2] << 16) |
                ((UInt32)buffer[offset2 + 3] << 24);
            return value1 ^ value2;
        }
        public static ulong Xor8(byte[] buffer, int offset1, int offset2)
        {
            ulong value1 =
                ((ulong)buffer[offset1]) |
                ((ulong)buffer[offset1 + 1] << 8) |
                ((ulong)buffer[offset1 + 2] << 16) |
                ((ulong)buffer[offset1 + 3] << 24) |
                ((ulong)buffer[offset1 + 4] << 32) |
                ((ulong)buffer[offset1 + 5] << 40) |
                ((ulong)buffer[offset1 + 6] << 48) |
                ((ulong)buffer[offset1 + 7] << 56);
            ulong value2 =
                ((ulong)buffer[offset2]) |
                ((ulong)buffer[offset2 + 1] << 8) |
                ((ulong)buffer[offset2 + 2] << 16) |
                ((ulong)buffer[offset2 + 3] << 24) |
                ((ulong)buffer[offset2 + 4] << 32) |
                ((ulong)buffer[offset2 + 5] << 40) |
                ((ulong)buffer[offset2 + 6] << 48) |
                ((ulong)buffer[offset2 + 7] << 56);
            return value1 ^ value2;
        }

        public static bool Equal2(byte[] buffer, int offset1, int offset2)
        {
            if (buffer[offset1] != buffer[offset2])
                return false;

            return buffer[offset1 + 1] == buffer[offset2 + 1];
        }
        public static bool Equal4(byte[] buffer, int offset1, int offset2)
        {
            if (buffer[offset1] != buffer[offset2]) return false;
            else if (buffer[offset1 + 1] != buffer[offset2 + 1]) return false;
            else if (buffer[offset1 + 2] != buffer[offset2 + 2]) return false;
            else return buffer[offset1 + 3] == buffer[offset2 + 3];
        }

        public static void Copy4(byte[] buf, int src, int dst)
        {
            buf[dst + 3] = buf[src + 3];
            buf[dst + 2] = buf[src + 2];
            buf[dst + 1] = buf[src + 1];
            buf[dst] = buf[src];
        }
        public static void Copy8(byte[] buf, int src, int dst)
        {
            buf[dst + 7] = buf[src + 7];
            buf[dst + 6] = buf[src + 6];
            buf[dst + 5] = buf[src + 5];
            buf[dst + 4] = buf[src + 4];
            buf[dst + 3] = buf[src + 3];
            buf[dst + 2] = buf[src + 2];
            buf[dst + 1] = buf[src + 1];
            buf[dst] = buf[src];
        }

        public static void BlockCopy(byte[] src, int src_0, byte[] dst, int dst_0, int len)
        {
            if (len >= BlockCopyLimit) Buffer.BlockCopy(src, src_0, dst, dst_0, len);
            else
            {
                while (len >= 8)
                {
                    dst[dst_0] = src[src_0];
                    dst[dst_0 + 1] = src[src_0 + 1];
                    dst[dst_0 + 2] = src[src_0 + 2];
                    dst[dst_0 + 3] = src[src_0 + 3];
                    dst[dst_0 + 4] = src[src_0 + 4];
                    dst[dst_0 + 5] = src[src_0 + 5];
                    dst[dst_0 + 6] = src[src_0 + 6];
                    dst[dst_0 + 7] = src[src_0 + 7];
                    len -= 8;
                    src_0 += 8;
                    dst_0 += 8;
                }

                while (len >= 4)
                {
                    dst[dst_0] = src[src_0];
                    dst[dst_0 + 1] = src[src_0 + 1];
                    dst[dst_0 + 2] = src[src_0 + 2];
                    dst[dst_0 + 3] = src[src_0 + 3];
                    len -= 4;
                    src_0 += 4;
                    dst_0 += 4;
                }

                while (len-- > 0)
                    dst[dst_0++] = src[src_0++];
            }
        }
        public static int WildCopy(byte[] src, int src_0, byte[] dst, int dst_0, int dst_end)
        {
            int len = dst_end - dst_0;
            if (len >= BlockCopyLimit) Buffer.BlockCopy(src, src_0, dst, dst_0, len);
            else
            {
                while (len >= 4)
                {
                    dst[dst_0] = src[src_0];
                    dst[dst_0 + 1] = src[src_0 + 1];
                    dst[dst_0 + 2] = src[src_0 + 2];
                    dst[dst_0 + 3] = src[src_0 + 3];
                    len -= 4;
                    src_0 += 4;
                    dst_0 += 4;
                }

                while (len-- > 0)
                    dst[dst_0++] = src[src_0++];
            }

            return len;
        }
        public static int SecureCopy(byte[] buffer, int src, int dst, int dst_end)
        {
            int diff = dst - src;
            int length = dst_end - dst;
            int len = length;

            if (diff >= BlockCopyLimit)
            {
                if (diff >= length)
                {
                    Buffer.BlockCopy(buffer, src, buffer, dst, length);
                    return length;
                }

                do
                {
                    Buffer.BlockCopy(buffer, src, buffer, dst, diff);
                    src += diff;
                    dst += diff;
                    len -= diff;
                } 
                while (len >= diff);
            }

            while (len >= 4)
            {
                buffer[dst] = buffer[src];
                buffer[dst + 1] = buffer[src + 1];
                buffer[dst + 2] = buffer[src + 2];
                buffer[dst + 3] = buffer[src + 3];
                dst += 4;
                src += 4;
                len -= 4;
            }

            while (len-- > 0)
                buffer[dst++] = buffer[src++];

            return length;
        }
    }
}
