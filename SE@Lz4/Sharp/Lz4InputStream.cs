// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace SE.Storage.Lz4
{
    /// <summary>
    /// Streamed Lz4 Reader 
    /// </summary>
    public class Lz4InputStream : Stream
    {
        const int DefaultBlockSize = 1024 * 1024;

        int blockSize;
        Stream stream;

        Lz4Encoding encoding;

        byte[] buffer;
        int bufferLength;
        int bufferOffset;

        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return false; }
        }
        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return stream.Length; }
        }
        public override long Position
        {
            get { return stream.Position; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Opens a new Lz4 stream from certain stream
        /// </summary>
        /// <param name="stream">Base stream to read Lz4 data from</param>
        /// <param name="blockSize">The size of internal processing buffer</param>
        public Lz4InputStream(Stream stream, int blockSize = DefaultBlockSize)
        {
            this.stream = stream;
            this.blockSize = Math.Max(16, blockSize);
            this.encoding = new Lz4Encoding();
        }

        public override int ReadByte()
        {
            if (bufferOffset >= bufferLength && !GetNextChunk()) return -1;
            else return buffer[bufferOffset++];
        }
        public override int Read(byte[] buff, int offset, int count)
        {
            int total = 0;
            while (count > 0)
            {
                int chunk = Math.Min(count, bufferLength - bufferOffset);
                if (chunk > 0)
                {
                    Buffer.BlockCopy(buffer, bufferOffset, buff, offset, chunk);
                    bufferOffset += chunk;
                    offset += chunk;
                    count -= chunk;
                    total += chunk;
                }
                else if (!GetNextChunk())
                    break;
            }

            return total;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        { }
        protected override void Dispose(bool disposing)
        {
            stream.Dispose();
            base.Dispose(disposing);
        }

        bool GetNextChunk()
        {
            do
            {
                ulong varint;
                if (!TryReadVarInt(out varint))
                    return false;

                bool isCompressed = (varint != 0);
                int originalLength = (int)ReadVarInt();
                int compressedLength = ((isCompressed) ? (int)ReadVarInt() : originalLength);
                if (compressedLength > originalLength) 
                    throw new EndOfStreamException();

                byte[] compressed = new byte[compressedLength];
                int chunk = ReadChunk(compressed, 0, compressedLength);

                if (chunk != compressedLength)
                    throw new EndOfStreamException();

                if (!isCompressed)
                {
                    buffer = compressed;
                    bufferLength = compressedLength;
                }
                else
                {
                    if (buffer == null || buffer.Length < originalLength)
                        buffer = new byte[originalLength];

                    encoding.Decode(compressed, 0, compressedLength, buffer, 0, originalLength, true);
                    bufferLength = originalLength;
                }

                bufferOffset = 0;
            } 
            while (bufferLength == 0);
            return true;
        }

        private ulong ReadVarInt()
        {
            ulong result; if (!TryReadVarInt(out result))
                throw new EndOfStreamException();

            return result;
        }
        bool TryReadVarInt(out ulong result)
        {
            result = 0;
            int count = 0;
            while (true)
            {
                int bt; if ((bt = stream.ReadByte()) == -1)
                {
                    if (count == 0)
                        return false;

                    throw new EndOfStreamException();
                }

                result = result + ((ulong)(bt & 0x7F) << count);
                count += 7;

                if ((bt & 0x80) == 0 || count >= 64)
                    break;
            }

            return true;
        }

        private int ReadChunk(byte[] buffer, int offset, int length)
        {
            int total = 0;

            while (length > 0)
            {
                int read = stream.Read(buffer, offset, length);
                if (read == 0)
                    break;

                offset += read;
                length -= read;
                total += read;
            }

            return total;
        }
    }
}
