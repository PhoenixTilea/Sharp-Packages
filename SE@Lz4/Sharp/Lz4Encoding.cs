// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Storage.Lz4
{
    /// <summary>
    /// Lz4 Data Encoding
    /// </summary>
    public class Lz4Encoding
    {
        protected int MaximumOutputLength(int inputLength)
        {
            return inputLength + (inputLength / 255) + 16;
        }
        protected void Transform(byte[] input, int inputOffset, ref int inputLength, byte[] output, int outputOffset, ref int outputLength)
        {
            if (inputLength < 0) inputLength = input.Length - inputOffset;
            else if (inputLength == 0) 
            {
                outputLength = 0;
                return;
            }

            if (inputOffset < 0) throw new ArgumentOutOfRangeException("InputOffset");
            else if (inputOffset + inputLength > input.Length) throw new ArgumentOutOfRangeException("InputLength");
            else if (outputOffset < 0) throw new ArgumentOutOfRangeException("OutputLength");
            else if (outputOffset + outputLength > output.Length) throw new ArgumentOutOfRangeException("OutputLength");
            else if (outputLength < 0) outputLength = output.Length - outputOffset;
        }

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
        public virtual int Encode(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength)
        {
            Transform(input, inputOffset, ref inputLength, output, outputOffset, ref outputLength);
            if (outputLength == 0) 
                return 0;

            if (inputLength < Lz4Utils.Limit64)
            {
                UInt16[] hashTable = new UInt16[Lz4Utils.HashTableSize64];
                return Compress64(hashTable, input, inputOffset, inputLength, output, outputOffset, outputLength);
            }
            else
            {
                int[] hashTable = new int[Lz4Utils.HashTableSize];
                return Compress(hashTable, input, inputOffset, inputLength, output, outputOffset, outputLength);
            }
        }
        /// <summary>
        /// Compresses a block of data into a new buffer
        /// </summary>
        /// <param name="input">The block of data to be compressed</param>
        /// <param name="inputOffset">An offset to start reading from</param>
        /// <param name="inputLength">The length of data to be compressed</param>
        /// <returns>A buffer object containing the block of data</returns>
        public virtual byte[] Encode(byte[] input, int inputOffset, int inputLength)
        {
            byte[] result = new byte[MaximumOutputLength(inputLength)];
            int length = Encode(input, inputOffset, inputLength, result, 0, result.Length);

            if (length != result.Length)
            {
                if (length < 0) throw new Lz4DataException();
                
                byte[] buffer = new byte[length];
                Buffer.BlockCopy(result, 0, buffer, 0, length);
                return buffer;
            }
            return result;
        }

        int Compress(int[] hashTable, byte[] source, int sourceIndex, int sourceLength, byte[] destination, int destinationIndex, int destinationMaxLength)
        {
            int[] debruijn32 = Lz4Utils.DebrujinTable32;
            int _i;

            // r93
            int src_p = sourceIndex;
            int src_base = sourceIndex;
            int src_anchor = src_p;
            int src_end = src_p + sourceLength;
            int src_mflimit = src_end - Lz4Utils.MFLimit;

            int dst_p = destinationIndex;
            int dst_end = dst_p + destinationMaxLength;

            int src_LASTLITERALS = src_end - Lz4Utils.LastLiterals;
            int src_LASTLITERALS_1 = src_LASTLITERALS - 1;

            int src_LASTLITERALS_STEPSIZE_1 = src_LASTLITERALS - (Lz4Utils.StepSize32 - 1);
            int dst_LASTLITERALS_1 = dst_end - (1 + Lz4Utils.LastLiterals);
            int dst_LASTLITERALS_3 = dst_end - (2 + 1 + Lz4Utils.LastLiterals);

            int length;

            UInt32 h, h_fwd;

            // Init
            if (sourceLength < Lz4Utils.MinLength)
            {
                goto _last_literals;
            }

            // First Byte
            hashTable[(((Lz4Utils.Peek4(source, src_p)) * 2654435761u) >> Lz4Utils.HashAdjust)] = (src_p - src_base);
            src_p++;
            h_fwd = (((Lz4Utils.Peek4(source, src_p)) * 2654435761u) >> Lz4Utils.HashAdjust);

            // Main Loop
            while (true)
            {
                int findMatchAttempts = (1 << Lz4Utils.SkipStrength) + 3;
                int src_p_fwd = src_p;
                int src_ref;
                int dst_token;

                // Find a match
                do
                {
                    h = h_fwd;
                    int step = findMatchAttempts++ >> Lz4Utils.SkipStrength;
                    src_p = src_p_fwd;
                    src_p_fwd = src_p + step;

                    if (src_p_fwd > src_mflimit)
                    {
                        goto _last_literals;
                    }

                    h_fwd = (((Lz4Utils.Peek4(source, src_p_fwd)) * 2654435761u) >> Lz4Utils.HashAdjust);
                    src_ref = src_base + hashTable[h];
                    hashTable[h] = (src_p - src_base);
                } while ((src_ref < src_p - Lz4Utils.MaxDistance) || (!Lz4Utils.Equal4(source, src_ref, src_p)));

                // Catch up
                while ((src_p > src_anchor) && (src_ref > sourceIndex) && (source[src_p - 1] == source[src_ref - 1]))
                {
                    src_p--;
                    src_ref--;
                }

                // Encode Literal length
                length = (src_p - src_anchor);
                dst_token = dst_p++;

                if (dst_p + length + (length >> 8) > dst_LASTLITERALS_3)
                {
                    return 0; // Check output limit
                }

                if (length >= Lz4Utils.RunMask)
                {
                    int len = length - Lz4Utils.RunMask;
                    destination[dst_token] = (Lz4Utils.RunMask << Lz4Utils.MLBits);
                    if (len > 254)
                    {
                        do
                        {
                            destination[dst_p++] = 255;
                            len -= 255;
                        } while (len > 254);
                        destination[dst_p++] = (byte)len;
                        Lz4Utils.BlockCopy(source, src_anchor, destination, dst_p, length);
                        dst_p += length;
                        goto _next_match;
                    }
                    else
                    {
                        destination[dst_p++] = (byte)len;
                    }
                }
                else
                {
                    destination[dst_token] = (byte)(length << Lz4Utils.MLBits);
                }

                // Copy Literals
                if (length > 0)
                {
                    _i = dst_p + length;
                    Lz4Utils.WildCopy(source, src_anchor, destination, dst_p, _i);
                    dst_p = _i;
                }

            _next_match:
                // Encode Offset
                Lz4Utils.Poke2(destination, dst_p, (ushort)(src_p - src_ref));
                dst_p += 2;

                // Start Counting
                src_p += Lz4Utils.MinMatch;
                src_ref += Lz4Utils.MinMatch; // MinMatch already verified
                src_anchor = src_p;

                while (src_p < src_LASTLITERALS_STEPSIZE_1)
                {
                    int diff = (int)Lz4Utils.Xor4(source, src_ref, src_p);
                    if (diff == 0)
                    {
                        src_p += Lz4Utils.StepSize32;
                        src_ref += Lz4Utils.StepSize32;
                        continue;
                    }
                    src_p += debruijn32[((UInt32)((diff) & -(diff)) * 0x077CB531u) >> 27];
                    goto _endCount;
                }

                if ((src_p < src_LASTLITERALS_1) && (Lz4Utils.Equal2(source, src_ref, src_p)))
                {
                    src_p += 2;
                    src_ref += 2;
                }
                if ((src_p < src_LASTLITERALS) && (source[src_ref] == source[src_p]))
                {
                    src_p++;
                }

            _endCount:
                // Encode MatchLength
                length = (src_p - src_anchor);

                if (dst_p + (length >> 8) > dst_LASTLITERALS_1)
                {
                    return 0; // Check output limit
                }

                if (length >= Lz4Utils.MLMask)
                {
                    destination[dst_token] += Lz4Utils.MLMask;
                    length -= Lz4Utils.MLMask;
                    for (; length > 509; length -= 510)
                    {
                        destination[dst_p++] = 255;
                        destination[dst_p++] = 255;
                    }
                    if (length > 254)
                    {
                        length -= 255;
                        destination[dst_p++] = 255;
                    }
                    destination[dst_p++] = (byte)length;
                }
                else
                {
                    destination[dst_token] += (byte)length;
                }

                // Test end of chunk
                if (src_p > src_mflimit)
                {
                    src_anchor = src_p;
                    break;
                }

                // Fill table
                hashTable[(((Lz4Utils.Peek4(source, src_p - 2)) * 2654435761u) >> Lz4Utils.HashAdjust)] = (src_p - 2 - src_base);

                // Test next position

                h = (((Lz4Utils.Peek4(source, src_p)) * 2654435761u) >> Lz4Utils.HashAdjust);
                src_ref = src_base + hashTable[h];
                hashTable[h] = (src_p - src_base);

                if ((src_ref > src_p - (Lz4Utils.MaxDistance + 1)) && (Lz4Utils.Equal4(source, src_ref, src_p)))
                {
                    dst_token = dst_p++;
                    destination[dst_token] = 0;
                    goto _next_match;
                }

                // Prepare next loop
                src_anchor = src_p++;
                h_fwd = (((Lz4Utils.Peek4(source, src_p)) * 2654435761u) >> Lz4Utils.HashAdjust);
            }

        _last_literals:
            // Encode Last Literals
            {
                int lastRun = (src_end - src_anchor);

                if (dst_p + lastRun + 1 + ((lastRun + 255 - Lz4Utils.RunMask) / 255) > dst_end)
                {
                    return 0;
                }

                if (lastRun >= Lz4Utils.RunMask)
                {
                    destination[dst_p++] = (Lz4Utils.RunMask << Lz4Utils.MLBits);
                    lastRun -= Lz4Utils.RunMask;
                    for (; lastRun > 254; lastRun -= 255)
                    {
                        destination[dst_p++] = 255;
                    }
                    destination[dst_p++] = (byte)lastRun;
                }
                else
                {
                    destination[dst_p++] = (byte)(lastRun << Lz4Utils.MLBits);
                }
                Lz4Utils.BlockCopy(source, src_anchor, destination, dst_p, src_end - src_anchor);
                dst_p += src_end - src_anchor;
            }

            return ((dst_p) - destinationIndex);
        }
        int Compress64(ushort[] hashTable, byte[] source, int sourceIndex, int sourceLength, byte[] destination, int destinationIndex, int destinationMaxLength)
        {
            int[] debruijn32 = Lz4Utils.DebrujinTable32;
            int _i;

            // r93
            int src_p = sourceIndex;
            int src_anchor = src_p;
            int src_base = src_p;
            int src_end = src_p + sourceLength;
            int src_mflimit = src_end - Lz4Utils.MFLimit;

            int dst_p = destinationIndex;
            int dst_end = dst_p + destinationMaxLength;

            int src_LASTLITERALS = src_end - Lz4Utils.LastLiterals;
            int src_LASTLITERALS_1 = src_LASTLITERALS - 1;

            int src_LASTLITERALS_STEPSIZE_1 = src_LASTLITERALS - (Lz4Utils.StepSize32 - 1);
            int dst_LASTLITERALS_1 = dst_end - (1 + Lz4Utils.LastLiterals);
            int dst_LASTLITERALS_3 = dst_end - (2 + 1 + Lz4Utils.LastLiterals);

            int len, length;

            UInt32 h, h_fwd;

            // Init
            if (sourceLength < Lz4Utils.MinLength)
            {
                goto _last_literals;
            }

            // First Byte
            src_p++;
            h_fwd = (((Lz4Utils.Peek4(source, src_p)) * 2654435761u) >> Lz4Utils.HashAdjust64);

            // Main Loop
            while (true)
            {
                int findMatchAttempts = (1 << Lz4Utils.SkipStrength) + 3;
                int src_p_fwd = src_p;
                int src_ref;
                int dst_token;

                // Find a match
                do
                {
                    h = h_fwd;
                    int step = findMatchAttempts++ >> Lz4Utils.SkipStrength;
                    src_p = src_p_fwd;
                    src_p_fwd = src_p + step;

                    if (src_p_fwd > src_mflimit)
                    {
                        goto _last_literals;
                    }

                    h_fwd = (((Lz4Utils.Peek4(source, src_p_fwd)) * 2654435761u) >> Lz4Utils.HashAdjust64);
                    src_ref = src_base + hashTable[h];
                    hashTable[h] = (ushort)(src_p - src_base);
                } while (!Lz4Utils.Equal4(source, src_ref, src_p));

                // Catch up
                while ((src_p > src_anchor) && (src_ref > sourceIndex) && (source[src_p - 1] == source[src_ref - 1]))
                {
                    src_p--;
                    src_ref--;
                }

                // Encode Literal length
                length = (src_p - src_anchor);
                dst_token = dst_p++;

                if (dst_p + length + (length >> 8) > dst_LASTLITERALS_3)
                {
                    return 0; // Check output limit
                }

                if (length >= Lz4Utils.RunMask)
                {
                    len = length - Lz4Utils.RunMask;
                    destination[dst_token] = (Lz4Utils.RunMask << Lz4Utils.MLBits);
                    if (len > 254)
                    {
                        do
                        {
                            destination[dst_p++] = 255;
                            len -= 255;
                        } while (len > 254);
                        destination[dst_p++] = (byte)len;
                        Lz4Utils.BlockCopy(source, src_anchor, destination, dst_p, length);
                        dst_p += length;
                        goto _next_match;
                    }
                    else
                    {
                        destination[dst_p++] = (byte)len;
                    }
                }
                else
                {
                    destination[dst_token] = (byte)(length << Lz4Utils.MLBits);
                }

                // Copy Literals
                if (length > 0)
                {
                    _i = dst_p + length;
                    Lz4Utils.WildCopy(source, src_anchor, destination, dst_p, _i);
                    dst_p = _i;
                }

            _next_match:
                // Encode Offset
                Lz4Utils.Poke2(destination, dst_p, (ushort)(src_p - src_ref));
                dst_p += 2;

                // Start Counting
                src_p += Lz4Utils.MinMatch;
                src_ref += Lz4Utils.MinMatch; // MinMatch verified
                src_anchor = src_p;

                while (src_p < src_LASTLITERALS_STEPSIZE_1)
                {
                    int diff = (int)Lz4Utils.Xor4(source, src_ref, src_p);
                    if (diff == 0)
                    {
                        src_p += Lz4Utils.StepSize32;
                        src_ref += Lz4Utils.StepSize32;
                        continue;
                    }
                    src_p += debruijn32[((UInt32)((diff) & -(diff)) * 0x077CB531u) >> 27];
                    goto _endCount;
                }

                if ((src_p < src_LASTLITERALS_1) && (Lz4Utils.Equal2(source, src_ref, src_p)))
                {
                    src_p += 2;
                    src_ref += 2;
                }
                if ((src_p < src_LASTLITERALS) && (source[src_ref] == source[src_p]))
                {
                    src_p++;
                }

            _endCount:

                // Encode MatchLength
                len = (src_p - src_anchor);

                if (dst_p + (len >> 8) > dst_LASTLITERALS_1)
                {
                    return 0; // Check output limit
                }

                if (len >= Lz4Utils.MLMask)
                {
                    destination[dst_token] += Lz4Utils.MLMask;
                    len -= Lz4Utils.MLMask;
                    for (; len > 509; len -= 510)
                    {
                        destination[dst_p++] = 255;
                        destination[dst_p++] = 255;
                    }
                    if (len > 254)
                    {
                        len -= 255;
                        destination[dst_p++] = 255;
                    }
                    destination[dst_p++] = (byte)len;
                }
                else
                {
                    destination[dst_token] += (byte)len;
                }

                // Test end of chunk
                if (src_p > src_mflimit)
                {
                    src_anchor = src_p;
                    break;
                }

                // Fill table
                hashTable[(((Lz4Utils.Peek4(source, src_p - 2)) * 2654435761u) >> Lz4Utils.HashAdjust64)] =
                    (ushort)(src_p - 2 - src_base);

                // Test next position

                h = (((Lz4Utils.Peek4(source, src_p)) * 2654435761u) >> Lz4Utils.HashAdjust64);
                src_ref = src_base + hashTable[h];
                hashTable[h] = (ushort)(src_p - src_base);

                if (Lz4Utils.Equal4(source, src_ref, src_p))
                {
                    dst_token = dst_p++;
                    destination[dst_token] = 0;
                    goto _next_match;
                }

                // Prepare next loop
                src_anchor = src_p++;
                h_fwd = (((Lz4Utils.Peek4(source, src_p)) * 2654435761u) >> Lz4Utils.HashAdjust64);
            }

        _last_literals:
            // Encode Last Literals
            int lastRun = (src_end - src_anchor);
            if (dst_p + lastRun + 1 + (lastRun - Lz4Utils.RunMask + 255) / 255 > dst_end) return 0;
            if (lastRun >= Lz4Utils.RunMask)
            {
                destination[dst_p++] = (Lz4Utils.RunMask << Lz4Utils.MLBits);
                lastRun -= Lz4Utils.RunMask;
                for (; lastRun > 254; lastRun -= 255)
                {
                    destination[dst_p++] = 255;
                }
                destination[dst_p++] = (byte)lastRun;
            }
            else destination[dst_p++] = (byte)(lastRun << Lz4Utils.MLBits);

            Lz4Utils.BlockCopy(source, src_anchor, destination, dst_p, src_end - src_anchor);
            dst_p += src_end - src_anchor;
            return ((dst_p) - destinationIndex);
        }

        /// <summary>
        /// Uncompresses a block of data into the provided buffer
        /// </summary>
        /// <param name="input">The block of data to be uncompressed</param>
        /// <param name="inputOffset">An offset to start reading from</param>
        /// <param name="inputLength">The length of data to be uncompressed</param>
        /// <param name="output">An output buffer to uncompress to</param>
        /// <param name="outputOffset">An offset to start writing</param>
        /// <param name="outputLength">The length of data to be written</param>
        /// <returns>The number of data bytes in uncompressed block</returns>
        public virtual int Decode(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength, bool knownOutputLength)
        {
            Transform(input, inputOffset, ref inputLength, output, outputOffset, ref outputLength);
            if (outputLength == 0)
                return 0;

            if (knownOutputLength)
            {
                int length = Decompress(input, inputOffset, output, outputOffset, outputLength);
                if (length != inputLength) throw new Lz4DataException();
                return outputLength;
            }
            else
            {
                int length = Decompress(input, inputOffset, inputLength, output, outputOffset, outputLength);
                if (length < 0) throw new Lz4DataException();
                return length;
            }
        }
        /// <summary>
        /// Uncompresses a block of data into a new buffer
        /// </summary>
        /// <param name="input">The block of data to be uncompressed</param>
        /// <param name="inputOffset">An offset to start reading from</param>
        /// <param name="inputLength">The length of data to be uncompressed</param>
        /// <param name="outputLength">The size of uncompressed data to write</param>
        /// <returns>A buffer object containing the block of data</returns>
        public virtual byte[] Decode(byte[] input, int inputOffset, int inputLength, int outputLength)
        {
            byte[] result = new byte[outputLength];
            int length = Decode(input, inputOffset, inputLength, result, 0, outputLength, true);

            if (length != outputLength) throw new Lz4DataException();
            return result;
        }

        int Decompress(byte[] source, int sourceIndex, byte[] destination, int destinationIndex, int destinationMaxLength)
        {
            int[] dec32table = Lz4Utils.DecodeTable32;
            int[] dec64table = Lz4Utils.DecodeTable64;
            int _i;

            // ---- preprocessed source start here ----
            // r93
            int src_p = sourceIndex;
            int dst_ref;

            int dst_p = destinationIndex;
            int dst_end = dst_p + destinationMaxLength;
            int dst_cpy;

            int dst_LASTLITERALS = dst_end - Lz4Utils.LastLiterals;
            int dst_COPYLENGTH = dst_end - Lz4Utils.CopyLength;
            int dst_COPYLENGTH_STEPSIZE_4 = dst_end - Lz4Utils.CopyLength - (Lz4Utils.StepSize - 4);

            UInt32 token;

            // Main Loop
            while (true)
            {
                int length;

                // get runlength
                token = source[src_p++];
                if ((length = (byte)(token >> Lz4Utils.MLBits)) == Lz4Utils.RunMask)
                {
                    int len;
                    for (; (len = source[src_p++]) == 255; length += 255)
                    {
                        /* do nothing */
                    }
                    length += len;
                }

                // copy literals
                dst_cpy = dst_p + length;

                if (dst_cpy > dst_COPYLENGTH)
                {
                    if (dst_cpy != dst_end)
                    {
                        goto _output_error; // Error : not enough place for another match (min 4) + 5 literals
                    }
                    Lz4Utils.BlockCopy(source, src_p, destination, dst_p, length);
                    src_p += length;
                    break; // EOF
                }
                if (dst_p < dst_cpy) /*?*/
                {
                    _i = Lz4Utils.WildCopy(source, src_p, destination, dst_p, dst_cpy);
                    src_p += _i;
                    dst_p += _i;
                }
                src_p -= (dst_p - dst_cpy);
                dst_p = dst_cpy;

                // get offset
                dst_ref = (dst_cpy) - Lz4Utils.Peek2(source, src_p);
                src_p += 2;
                if (dst_ref < destinationIndex)
                {
                    goto _output_error; // Error : offset outside destination buffer
                }

                // get matchlength
                if ((length = (byte)(token & Lz4Utils.MLMask)) == Lz4Utils.MLMask)
                {
                    for (; source[src_p] == 255; length += 255)
                    {
                        src_p++;
                    }
                    length += source[src_p++];
                }

                // copy repeated sequence
                if ((dst_p - dst_ref) < Lz4Utils.StepSize)
                {
                    int dec64 = dec64table[dst_p - dst_ref];

                    destination[dst_p + 0] = destination[dst_ref + 0];
                    destination[dst_p + 1] = destination[dst_ref + 1];
                    destination[dst_p + 2] = destination[dst_ref + 2];
                    destination[dst_p + 3] = destination[dst_ref + 3];
                    dst_p += 4;
                    dst_ref += 4;
                    dst_ref -= dec32table[dst_p - dst_ref];
                    Lz4Utils.Copy4(destination, dst_ref, dst_p);
                    dst_p += Lz4Utils.StepSize - 4;
                    dst_ref -= dec64;
                }
                else
                {
                    Lz4Utils.Copy8(destination, dst_ref, dst_p);
                    dst_p += 8;
                    dst_ref += 8;
                }
                dst_cpy = dst_p + length - (Lz4Utils.StepSize - 4);

                if (dst_cpy > dst_COPYLENGTH_STEPSIZE_4)
                {
                    if (dst_cpy > dst_LASTLITERALS)
                    {
                        goto _output_error; // Error : last 5 bytes must be literals
                    }
                    if (dst_p < dst_COPYLENGTH)
                    {
                        _i = Lz4Utils.SecureCopy(destination, dst_ref, dst_p, dst_COPYLENGTH);
                        dst_ref += _i;
                        dst_p += _i;
                    }

                    while (dst_p < dst_cpy)
                    {
                        destination[dst_p++] = destination[dst_ref++];
                    }
                    dst_p = dst_cpy;
                    continue;
                }

                if (dst_p < dst_cpy)
                {
                    Lz4Utils.SecureCopy(destination, dst_ref, dst_p, dst_cpy);
                }
                dst_p = dst_cpy; // correction
            }

            // end of decoding
            return ((src_p) - sourceIndex);

        _output_error:
            // write overflow error detected
            return (-((src_p) - sourceIndex));
        }
        int Decompress(byte[] source, int sourceIndex, int sourceLength, byte[] destination, int destinationIndex, int destinationMaxLength)
        {
            int[] dec32table = Lz4Utils.DecodeTable32;
            int[] dec64table = Lz4Utils.DecodeTable64;
            int _i;

            // ---- preprocessed source start here ----
            // r93
            int src_p = sourceIndex;
            int src_end = src_p + sourceLength;
            int dst_ref;

            int dst_p = destinationIndex;
            int dst_end = dst_p + destinationMaxLength;
            int dst_cpy;

            int src_LASTLITERALS_3 = (src_end - (2 + 1 + Lz4Utils.LastLiterals));
            int src_LASTLITERALS_1 = (src_end - (Lz4Utils.LastLiterals + 1));
            int dst_COPYLENGTH = (dst_end - Lz4Utils.CopyLength);
            int dst_COPYLENGTH_STEPSIZE_4 = (dst_end - (Lz4Utils.CopyLength + (Lz4Utils.StepSize - 4)));
            int dst_LASTLITERALS = (dst_end - Lz4Utils.LastLiterals);
            int dst_MFLIMIT = (dst_end - Lz4Utils.MFLimit);

            // Special case
            if (src_p == src_end)
            {
                goto _output_error; // A correctly formed null-compressed LZ4 must have at least one byte (token=0)
            }

            // Main Loop
            while (true)
            {
                byte token;
                int length;

                // get runlength
                token = source[src_p++];
                if ((length = (token >> Lz4Utils.MLBits)) == Lz4Utils.RunMask)
                {
                    int s = 255;
                    while ((src_p < src_end) && (s == 255))
                    {
                        length += (s = source[src_p++]);
                    }
                }

                // copy literals
                dst_cpy = dst_p + length;

                if ((dst_cpy > dst_MFLIMIT) || (src_p + length > src_LASTLITERALS_3))
                {
                    if (dst_cpy > dst_end)
                    {
                        goto _output_error; // Error : writes beyond output buffer
                    }
                    if (src_p + length != src_end)
                    {
                        goto _output_error;
                        // Error : LZ4 format requires to consume all input at this stage (no match within the last 11 bytes, and at least 8 remaining input bytes for another match+literals)
                    }
                    Lz4Utils.BlockCopy(source, src_p, destination, dst_p, length);
                    dst_p += length;
                    break; // Necessarily EOF, due to parsing restrictions
                }
                if (dst_p < dst_cpy) /*?*/
                {
                    _i = Lz4Utils.WildCopy(source, src_p, destination, dst_p, dst_cpy);
                    src_p += _i;
                    dst_p += _i;
                }
                src_p -= (dst_p - dst_cpy);
                dst_p = dst_cpy;

                // get offset
                dst_ref = (dst_cpy) - Lz4Utils.Peek2(source, src_p);
                src_p += 2;
                if (dst_ref < destinationIndex)
                {
                    goto _output_error; // Error : offset outside of destination buffer
                }

                // get matchlength
                if ((length = (token & Lz4Utils.MLMask)) == Lz4Utils.MLMask)
                {
                    while (src_p < src_LASTLITERALS_1)
                    // Error : a minimum input bytes must remain for LASTLITERALS + token
                    {
                        int s = source[src_p++];
                        length += s;
                        if (s == 255)
                        {
                            continue;
                        }
                        break;
                    }
                }

                // copy repeated sequence
                if (dst_p - dst_ref < Lz4Utils.StepSize)
                {
                    int dec64 = dec64table[dst_p - dst_ref];
                    destination[dst_p + 0] = destination[dst_ref + 0];
                    destination[dst_p + 1] = destination[dst_ref + 1];
                    destination[dst_p + 2] = destination[dst_ref + 2];
                    destination[dst_p + 3] = destination[dst_ref + 3];
                    dst_p += 4;
                    dst_ref += 4;
                    dst_ref -= dec32table[dst_p - dst_ref];
                    Lz4Utils.Copy4(destination, dst_ref, dst_p);
                    dst_p += Lz4Utils.StepSize - 4;
                    dst_ref -= dec64;
                }
                else
                {
                    Lz4Utils.Copy8(destination, dst_ref, dst_p);
                    dst_p += 8;
                    dst_ref += 8;
                }
                dst_cpy = dst_p + length - (Lz4Utils.StepSize - 4);

                if (dst_cpy > dst_COPYLENGTH_STEPSIZE_4)
                {
                    if (dst_cpy > dst_LASTLITERALS)
                    {
                        goto _output_error; // Error : last 5 bytes must be literals
                    }
                    if (dst_p < dst_COPYLENGTH)
                    {
                        _i = Lz4Utils.SecureCopy(destination, dst_ref, dst_p, dst_COPYLENGTH);
                        dst_ref += _i;
                        dst_p += _i;
                    }
                    while (dst_p < dst_cpy)
                    {
                        destination[dst_p++] = destination[dst_ref++];
                    }
                    dst_p = dst_cpy;
                    continue;
                }

                if (dst_p < dst_cpy)
                {
                    Lz4Utils.SecureCopy(destination, dst_ref, dst_p, dst_cpy);
                }
                dst_p = dst_cpy; // correction
            }

            // end of decoding
            return ((dst_p) - destinationIndex);

            // write overflow error detected
        _output_error:
            return (-((src_p) - sourceIndex));
        }
    }
}
