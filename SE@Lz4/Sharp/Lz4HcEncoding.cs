// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Storage.Lz4
{
    /// <summary>
    /// Lz4 High Compression Encoding
    /// </summary>
    public class Lz4HcEncoding : Lz4Encoding
    {
        class CompressionContext
        {
            public ushort[] chainTable;
            public byte[] dst;
            public int dst_base;
            public int dst_end;
            public int dst_len;
            public int[] hashTable;
            public int nextToUpdate;
            public byte[] src;
            public int src_LASTLITERALS;
            public int src_base;
            public int src_end;

            public CompressionContext(byte[] source, int sourceIndex, int sourceLength, byte[] destination, int destinationIndex, int destinationLength)
            {
                this.src = source;
                this.src_base = sourceIndex;
                this.src_end = sourceIndex + sourceLength;
                this.src_LASTLITERALS = (sourceIndex + sourceLength - Lz4Utils.LastLiterals);
                this.dst = destination;
                this.dst_base = destinationIndex;
                this.dst_len = destinationLength;
                this.dst_end = destinationIndex + destinationLength;
                this.hashTable = new int[Lz4Utils.HashTableSizeHC];
                this.chainTable = new ushort[Lz4Utils.MaxD];
                this.nextToUpdate = sourceIndex + 1;

                for (int i = chainTable.Length - 1; i >= 0; i--)
                    chainTable[i] = unchecked((ushort)-1);
            }
        };

        /// <summary>
        /// Compresses a block of data into the provided buffer
        /// </summary>
        /// <param name="input">The block of data to be compressed</param>
        /// <param name="inputOffset">An offset to start reading from</param>
        /// <param name="inputLength">The length of data to be compressed</param>
        /// <param name="output">An output buffer to compress to</param>
        /// <param name="outputOffset">An offset to start writing</param>
        /// <param name="outputLength">The length of data to be written</param>
        /// <returns>The number of data bytes in compressed block</returns>
        public override int Encode(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength)
        {
            if (inputLength == 0) {
                return 0;
            }

            Transform(
                input, inputOffset, ref inputLength,
                output, outputOffset, ref outputLength);

            int length = Compress(new CompressionContext(input, inputOffset, inputLength, output, outputOffset, outputLength));
            return length <= 0 ? -1 : length;
        }
        /// <summary>
        /// Compresses a block of data into a new buffer
        /// </summary>
        /// <param name="input">The block of data to be compressed</param>
        /// <param name="inputOffset">An offset to start reading from</param>
        /// <param name="inputLength">The length of data to be compressed</param>
        /// <returns>A buffer object containing the block of data</returns>
        public override byte[] Encode(byte[] input, int inputOffset, int inputLength)
        {
            if (inputLength == 0)
            {
                return new byte[0];
            }
            int outputLength = MaximumOutputLength(inputLength);
            byte[] result = new byte[outputLength];
            int length = Encode(input, inputOffset, inputLength, result, 0, outputLength);

            if (length < 0)
            {
                throw new ArgumentException("Provided data seems to be corrupted.");
            }

            if (length != outputLength)
            {
                byte[] buffer = new byte[length];
                Buffer.BlockCopy(result, 0, buffer, 0, length);
                result = buffer;
            }

            return result;
        }
        int Encode(CompressionContext ctx, ref int src_p, ref int dst_p, ref int src_anchor, int matchLength, int src_ref, int dst_end)
        {
            int len;
            byte[] src = ctx.src;
            byte[] dst = ctx.dst;

            // Encode Literal length
            int length = src_p - src_anchor;
            int dst_token = dst_p++;
            if ((dst_p + length + (2 + 1 + Lz4Utils.LastLiterals) + (length >> 8)) > dst_end)
            {
                return 1; // Check output limit
            }
            if (length >= Lz4Utils.RunMask)
            {
                dst[dst_token] = (Lz4Utils.RunMask << Lz4Utils.MLBits);
                len = length - Lz4Utils.RunMask;
                for (; len > 254; len -= 255)
                {
                    dst[dst_p++] = 255;
                }
                dst[dst_p++] = (byte)len;
            }
            else
            {
                dst[dst_token] = (byte)(length << Lz4Utils.MLBits);
            }

            // Copy Literals
            if (length > 0)
            {
                int _i = dst_p + length;
                src_anchor += Lz4Utils.WildCopy(src, src_anchor, dst, dst_p, _i);
                dst_p = _i;
            }

            // Encode Offset
            Lz4Utils.Poke2(dst, dst_p, (ushort)(src_p - src_ref));
            dst_p += 2;

            // Encode MatchLength
            len = (matchLength - Lz4Utils.MinMatch);
            if (dst_p + (1 + Lz4Utils.LastLiterals) + (length >> 8) > dst_end)
            {
                return 1; // Check output limit
            }
            if (len >= Lz4Utils.MLMask)
            {
                dst[dst_token] += Lz4Utils.MLMask;
                len -= Lz4Utils.MLMask;
                for (; len > 509; len -= 510)
                {
                    dst[(dst_p)++] = 255;
                    dst[(dst_p)++] = 255;
                }
                if (len > 254)
                {
                    len -= 255;
                    dst[(dst_p)++] = 255;
                }
                dst[(dst_p)++] = (byte)len;
            }
            else
            {
                dst[dst_token] += (byte)len;
            }

            // Prepare next loop
            src_p += matchLength;
            src_anchor = src_p;

            return 0;
        }

        int Compress(CompressionContext ctx)
        {
            byte[] src = ctx.src;
            byte[] dst = ctx.dst;
            int src_0 = ctx.src_base;
            int src_end = ctx.src_end;
            int dst_0 = ctx.dst_base;
            int dst_len = ctx.dst_len;
            int dst_end = ctx.dst_end;

            int src_p = src_0;
            int src_anchor = src_p;
            int src_mflimit = src_end - Lz4Utils.MFLimit;

            int dst_p = dst_0;

            int src_ref = 0;
            int start2 = 0;
            int ref2 = 0;
            int start3 = 0;
            int ref3 = 0;

            src_p++;

            // Main Loop
            while (src_p < src_mflimit)
            {
                int ml = Insert(ctx, src_p, ref src_ref);
                if (ml == 0)
                {
                    src_p++;
                    continue;
                }

                // saved, in case we would skip too much
                int start0 = src_p;
                int ref0 = src_ref;
                int ml0 = ml;

            _Search2:
                int ml2 = src_p + ml < src_mflimit
                    ? Insert(ctx, src_p + ml - 2, src_p + 1, ml, ref ref2, ref start2)
                    : ml;

                if (ml2 == ml) // No better match
                {
                    if (Encode(ctx, ref src_p, ref dst_p, ref src_anchor, ml, src_ref, dst_end) != 0)
                    {
                        return 0;
                    }
                    continue;
                }

                if (start0 < src_p)
                {
                    if (start2 < src_p + ml0) // empirical
                    {
                        src_p = start0;
                        src_ref = ref0;
                        ml = ml0;
                    }
                }

                // Here, start0==ip
                if ((start2 - src_p) < 3) // First Match too small : removed
                {
                    ml = ml2;
                    src_p = start2;
                    src_ref = ref2;
                    goto _Search2;
                }

            _Search3:
                // Currently we have :
                // ml2 > ml1, and
                // ip1+3 <= ip2 (usually < ip1+ml1)
                if ((start2 - src_p) < Lz4Utils.OptimalML)
                {
                    int new_ml = ml;
                    if (new_ml > Lz4Utils.OptimalML)
                    {
                        new_ml = Lz4Utils.OptimalML;
                    }
                    if (src_p + new_ml > start2 + ml2 - Lz4Utils.MinMatch)
                    {
                        new_ml = (start2 - src_p) + ml2 - Lz4Utils.MinMatch;
                    }
                    int correction = new_ml - (start2 - src_p);
                    if (correction > 0)
                    {
                        start2 += correction;
                        ref2 += correction;
                        ml2 -= correction;
                    }
                }
                // Now, we have start2 = ip+new_ml, with new_ml=min(ml, OPTIMAL_ML=18)

                int ml3 = start2 + ml2 < src_mflimit
                    ? Insert(ctx, start2 + ml2 - 3, start2, ml2, ref ref3, ref start3)
                    : ml2;

                if (ml3 == ml2) // No better match : 2 sequences to encode
                {
                    // ip & ref are known; Now for ml
                    if (start2 < src_p + ml)
                    {
                        ml = (start2 - src_p);
                    }
                    // Now, encode 2 sequences
                    if (Encode(ctx, ref src_p, ref dst_p, ref src_anchor, ml, src_ref, dst_end) != 0)
                    {
                        return 0;
                    }
                    src_p = start2;
                    if (Encode(ctx, ref src_p, ref dst_p, ref src_anchor, ml2, ref2, dst_end) != 0)
                    {
                        return 0;
                    }
                    continue;
                }

                if (start3 < src_p + ml + 3) // Not enough space for match 2 : remove it
                {
                    if (start3 >= (src_p + ml)) // can write Seq1 immediately ==> Seq2 is removed, so Seq3 becomes Seq1
                    {
                        if (start2 < src_p + ml)
                        {
                            int correction = (src_p + ml - start2);
                            start2 += correction;
                            ref2 += correction;
                            ml2 -= correction;
                            if (ml2 < Lz4Utils.MinMatch)
                            {
                                start2 = start3;
                                ref2 = ref3;
                                ml2 = ml3;
                            }
                        }

                        if (Encode(ctx, ref src_p, ref dst_p, ref src_anchor, ml, src_ref, dst_end) != 0)
                        {
                            return 0;
                        }
                        src_p = start3;
                        src_ref = ref3;
                        ml = ml3;

                        start0 = start2;
                        ref0 = ref2;
                        ml0 = ml2;
                        goto _Search2;
                    }

                    start2 = start3;
                    ref2 = ref3;
                    ml2 = ml3;
                    goto _Search3;
                }

                // OK, now we have 3 ascending matches; let's write at least the first one
                // ip & ref are known; Now for ml
                if (start2 < src_p + ml)
                {
                    if ((start2 - src_p) < Lz4Utils.MLMask)
                    {
                        if (ml > Lz4Utils.OptimalML)
                        {
                            ml = Lz4Utils.OptimalML;
                        }
                        if (src_p + ml > start2 + ml2 - Lz4Utils.MinMatch)
                        {
                            ml = (start2 - src_p) + ml2 - Lz4Utils.MinMatch;
                        }
                        int correction = ml - (start2 - src_p);
                        if (correction > 0)
                        {
                            start2 += correction;
                            ref2 += correction;
                            ml2 -= correction;
                        }
                    }
                    else
                    {
                        ml = (start2 - src_p);
                    }
                }
                if (Encode(ctx, ref src_p, ref dst_p, ref src_anchor, ml, src_ref, dst_end) != 0)
                {
                    return 0;
                }

                src_p = start2;
                src_ref = ref2;
                ml = ml2;

                start2 = start3;
                ref2 = ref3;
                ml2 = ml3;

                goto _Search3;
            }

            // Encode Last Literals
            {
                int lastRun = (src_end - src_anchor);
                if ((dst_p - dst_0) + lastRun + 1 + ((lastRun + 255 - Lz4Utils.RunMask) / 255) > (UInt32)dst_len)
                {
                    return 0; // Check output limit
                }
                if (lastRun >= Lz4Utils.RunMask)
                {
                    dst[dst_p++] = (Lz4Utils.RunMask << Lz4Utils.MLBits);
                    lastRun -= Lz4Utils.RunMask;
                    for (; lastRun > 254; lastRun -= 255)
                    {
                        dst[dst_p++] = 255;
                    }
                    dst[dst_p++] = (byte)lastRun;
                }
                else
                {
                    dst[dst_p++] = (byte)(lastRun << Lz4Utils.MLBits);
                }
                Lz4Utils.BlockCopy(src, src_anchor, dst, dst_p, src_end - src_anchor);
                dst_p += src_end - src_anchor;
            }

            return (dst_p - dst_0);
        }

        void Insert(CompressionContext ctx, int src_p)
        {
            ushort[] chainTable = ctx.chainTable;
            int[] hashTable = ctx.hashTable;
            int nextToUpdate = ctx.nextToUpdate;
            byte[] src = ctx.src;
            int src_base = ctx.src_base;

            while (nextToUpdate < src_p)
            {
                int p = nextToUpdate;
                int delta = (p) - (hashTable[(((Lz4Utils.Peek4(src, p)) * 2654435761u) >> Lz4Utils.HashAdjustHC)] + src_base);
                if (delta > Lz4Utils.MaxDistance)
                {
                    delta = Lz4Utils.MaxDistance;
                }
                chainTable[(p) & Lz4Utils.MaxDMask] = (ushort)delta;
                hashTable[(((Lz4Utils.Peek4(src, p)) * 2654435761u) >> Lz4Utils.HashAdjustHC)] = ((p) - src_base);
                nextToUpdate++;
            }

            ctx.nextToUpdate = nextToUpdate;
        }
        int Insert(CompressionContext ctx, int src_p, ref int src_match)
        {
            ushort[] chainTable = ctx.chainTable;
            int[] hashTable = ctx.hashTable;
            byte[] src = ctx.src;
            int src_base = ctx.src_base;

            int nbAttempts = Lz4Utils.MaxNBAttempts;
            int repl = 0, ml = 0;
            ushort delta = 0;

            // HC4 match finder
            Insert(ctx, src_p);
            int src_ref = (hashTable[(((Lz4Utils.Peek4(src, src_p)) * 2654435761u) >> Lz4Utils.HashAdjustHC)] + src_base);


            // Detect repetitive sequences of length <= 4
            if (src_ref >= src_p - 4) // potential repetition
            {
                if (Lz4Utils.Equal4(src, src_ref, src_p)) // confirmed
                {
                    delta = (ushort)(src_p - src_ref);
                    repl = ml = CommonLength(ctx, src_p + Lz4Utils.MinMatch, src_ref + Lz4Utils.MinMatch) + Lz4Utils.MinMatch;
                    src_match = src_ref;
                }
                src_ref = ((src_ref) - chainTable[(src_ref) & Lz4Utils.MaxDMask]);
            }

            while ((src_ref >= src_p - Lz4Utils.MaxDistance) && (nbAttempts != 0))
            {
                nbAttempts--;
                if (src[(src_ref + ml)] == src[(src_p + ml)])
                {
                    if (Lz4Utils.Equal4(src, src_ref, src_p))
                    {
                        int mlt = CommonLength(ctx, src_p + Lz4Utils.MinMatch, src_ref + Lz4Utils.MinMatch) + Lz4Utils.MinMatch;
                        if (mlt > ml)
                        {
                            ml = mlt;
                            src_match = src_ref;
                        }
                    }
                }
                src_ref = ((src_ref) - chainTable[(src_ref) & Lz4Utils.MaxDMask]);
            }


            // Complete table
            if (repl != 0)
            {
                int src_ptr = src_p;

                int end = src_p + repl - (Lz4Utils.MinMatch - 1);
                while (src_ptr < end - delta)
                {
                    chainTable[(src_ptr) & Lz4Utils.MaxDMask] = delta; // Pre-Load
                    src_ptr++;
                }
                do
                {
                    chainTable[(src_ptr) & Lz4Utils.MaxDMask] = delta;
                    hashTable[(((Lz4Utils.Peek4(src, src_ptr)) * 2654435761u) >> Lz4Utils.HashAdjustHC)] = ((src_ptr) - src_base);
                    // Head of chain
                    src_ptr++;
                } while (src_ptr < end);
                ctx.nextToUpdate = end;
            }

            return ml;
        }
        int Insert(CompressionContext ctx, int src_p, int startLimit, int longest, ref int matchpos, ref int startpos)
        {
            ushort[] chainTable = ctx.chainTable;
            int[] hashTable = ctx.hashTable;
            byte[] src = ctx.src;
            int src_base = ctx.src_base;
            int src_LASTLITERALS = ctx.src_LASTLITERALS;
            int[] debruijn32 = Lz4Utils.DebrujinTable32;
            int nbAttempts = Lz4Utils.MaxNBAttempts;
            int delta = (src_p - startLimit);

            // First Match
            Insert(ctx, src_p);
            int src_ref = (hashTable[(((Lz4Utils.Peek4(src, src_p)) * 2654435761u) >> Lz4Utils.HashAdjustHC)] + src_base);

            while ((src_ref >= src_p - Lz4Utils.MaxDistance) && (nbAttempts != 0))
            {
                nbAttempts--;
                if (src[(startLimit + longest)] == src[(src_ref - delta + longest)])
                {
                    if (Lz4Utils.Equal4(src, src_ref, src_p))
                    {
                        int reft = src_ref + Lz4Utils.MinMatch;
                        int ipt = src_p + Lz4Utils.MinMatch;
                        int startt = src_p;

                        while (ipt < src_LASTLITERALS - (Lz4Utils.StepSize32 - 1))
                        {
                            int diff = (int)Lz4Utils.Xor4(src, reft, ipt);
                            if (diff == 0)
                            {
                                ipt += Lz4Utils.StepSize32;
                                reft += Lz4Utils.StepSize32;
                                continue;
                            }
                            ipt += debruijn32[((UInt32)((diff) & -(diff)) * 0x077CB531u) >> 27];
                            goto _endCount;
                        }
                        if ((ipt < (src_LASTLITERALS - 1)) && (Lz4Utils.Equal2(src, reft, ipt)))
                        {
                            ipt += 2;
                            reft += 2;
                        }
                        if ((ipt < src_LASTLITERALS) && (src[reft] == src[ipt]))
                        {
                            ipt++;
                        }

                    _endCount:
                        reft = src_ref;

                        while ((startt > startLimit) && (reft > src_base) && (src[startt - 1] == src[reft - 1]))
                        {
                            startt--;
                            reft--;
                        }

                        if ((ipt - startt) > longest)
                        {
                            longest = (ipt - startt);
                            matchpos = reft;
                            startpos = startt;
                        }
                    }
                }
                src_ref = ((src_ref) - chainTable[(src_ref) & Lz4Utils.MaxDMask]);
            }

            return longest;
        }

        int CommonLength(CompressionContext ctx, int p1, int p2)
        {
            int[] debruijn32 = Lz4Utils.DebrujinTable32;
            byte[] src = ctx.src;
            int src_LASTLITERALS = ctx.src_LASTLITERALS;

            int p1t = p1;

            while (p1t < src_LASTLITERALS - (Lz4Utils.StepSize32 - 1))
            {
                int diff = (int)Lz4Utils.Xor4(src, p2, p1t);
                if (diff == 0)
                {
                    p1t += Lz4Utils.StepSize32;
                    p2 += Lz4Utils.StepSize32;
                    continue;
                }
                p1t += debruijn32[((UInt32)((diff) & -(diff)) * 0x077CB531u) >> 27];
                return (p1t - p1);
            }
            if ((p1t < (src_LASTLITERALS - 1)) && (Lz4Utils.Equal2(src, p2, p1t)))
            {
                p1t += 2;
                p2 += 2;
            }
            if ((p1t < src_LASTLITERALS) && (src[p2] == src[p1t]))
            {
                p1t++;
            }
            return (p1t - p1);
        }
    }
}
