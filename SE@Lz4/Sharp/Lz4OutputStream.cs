// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace SE.Storage.Lz4
{
    /// <summary>
    /// Streamed Lz4 Writer 
    /// </summary>
    public class Lz4OutputStream : Stream
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
            get { return false; }
        }
        public override bool CanSeek
        {
            get { return false; }
        }
        public override bool CanWrite
        {
            get { return true; }
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
        /// <param name="stream">Base stream to write Lz4 data to</param>
        /// <param name="blockSize">The size of internal processing buffer</param>
        public Lz4OutputStream(Stream stream, int blockSize = DefaultBlockSize)
            : this(stream, false, blockSize)
        { }
        /// <summary>
        /// Opens a new Lz4 stream from certain stream
        /// </summary>
        /// <param name="stream">Base stream to write Lz4 data to</param>
        /// <param name="highCompression">Determines if high compression mode should be used to write data</param>
        /// <param name="blockSize">The size of internal processing buffer</param>
        public Lz4OutputStream(Stream stream, bool highCompression, int blockSize = DefaultBlockSize)
        {
            this.stream = stream;
            this.blockSize = Math.Max(16, blockSize);

            if (highCompression) this.encoding = new Lz4HcEncoding();
            else this.encoding = new Lz4Encoding();
        }

        public override int ReadByte()
        {
            throw new NotSupportedException();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
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
            if (buffer == null)
            {
                buffer = new byte[blockSize];
                bufferLength = blockSize;
                bufferOffset = 0;
            }

            if (bufferOffset >= bufferLength)
                WriteChunk();

            buffer[bufferOffset++] = value;
        }
        public override void Write(byte[] buff, int offset, int count)
        {
            if (buffer == null)
            {
                buffer = new byte[blockSize];
                bufferLength = blockSize;
                bufferOffset = 0;
            }

            while (count > 0)
            {
                int chunk = Math.Min(count, bufferLength - bufferOffset);
                if (chunk > 0)
                {
                    Buffer.BlockCopy(buff, offset, buffer, bufferOffset, chunk);
                    offset += chunk;
                    count -= chunk;
                    bufferOffset += chunk;
                }
                else WriteChunk();
            }
        }

        public override void Flush()
        {
            if (bufferOffset > 0)
                WriteChunk();
        }
        protected override void Dispose(bool disposing)
        {
            Flush();
            stream.Dispose();
            base.Dispose(disposing);
        }

        void WriteChunk()
        {
            if (bufferOffset <= 0)
                return;

            byte[] compressed = new byte[bufferOffset];
            int compressedLength = encoding.Encode(buffer, 0, bufferOffset, compressed, 0, bufferOffset);
            if (compressedLength <= 0 || compressedLength >= bufferOffset)
            {
                compressed = buffer;
                compressedLength = bufferOffset;
            }

            bool isCompressed = compressedLength < bufferOffset;
            WriteVarInt((ulong)((isCompressed) ? 1 : 0));
            WriteVarInt((ulong)bufferOffset);

            if (isCompressed)
                WriteVarInt((ulong)compressedLength);

            stream.Write(compressed, 0, compressedLength);
            bufferOffset = 0;
        }
        void WriteVarInt(ulong value)
        {
            while (true)
            {
                byte bt = (byte)(value & 0x7F);

                value >>= 7;
                stream.WriteByte((byte)(bt | ((value == 0) ? 0 : 0x80)));

                if (value == 0)
                    break;
            }
        }
    }
}
