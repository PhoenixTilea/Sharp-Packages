// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace SE.Storage.Tar
{
    /// <summary>
    /// Streamed Tar Reader 
    /// </summary>
    public class TarInputStream : Stream
    {
        const int DefaultBlockSize = 155;

        Stream stream;
        byte[] buffer;
        long contentBytes;
        long contentCount;
        long chunkOffset;

        TarEncoding encoding;

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
            get { return contentCount; }
        }
        public override long Position
        {
            get { return (contentCount - contentBytes); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// A list of data entries found in Tar header
        /// </summary>
        public IEnumerable<TarEncoding.Entry> Entries
        {
            get 
            {
                TarEncoding.Entry entry; while (encoding.Decode(stream, buffer, ref contentBytes, out entry))
                {
                    if (!stream.CanSeek)
                    {
                        entry.Chunk = chunkOffset;
                        chunkOffset += (1 + (contentBytes / TarEncoding.TarChunkSize));
                    }
                    else entry.Chunk = (TarEncoding.TarChunkSize - (stream.Position / TarEncoding.TarChunkSize));
                    contentCount = contentBytes;
                    yield return entry;
                }
                contentBytes = 0;
                contentCount = 0;
                yield break;
            }
        }

        /// <summary>
        /// Opens a new Tar stream from certain stream
        /// </summary>
        /// <param name="stream">Base stream to process Tar data from</param>
        public TarInputStream(Stream stream)
        {
            this.stream = stream;
            this.buffer = new byte[DefaultBlockSize];
            this.encoding = new TarEncoding();
        }

        public override int ReadByte()
        {
            if (contentBytes > 0)
            {
                int result = stream.ReadByte();
                contentBytes--;

                return result;
            }
            else return -1;
        }
        public override int Read(byte[] buff, int offset, int count)
        {
            if (contentBytes > 0)
            {
                int byteCount = stream.Read(buff, offset, count);
                contentBytes -= byteCount;

                return byteCount;
            }
            else return 0;
        }

        /// <summary>
        /// Moves the stream pointer to the start of the given entry
        /// </summary>
        /// <param name="entry">An entry object that should be read</param>
        /// <returns>The position of the stream pointer</returns>
        public long Seek(TarEncoding.Entry entry)
        {
            stream.Position = (entry.Chunk * TarEncoding.TarChunkSize) + TarEncoding.TarChunkSize;
            contentBytes = entry.Size;

            return stream.Position;
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
    }
}
