// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace SE.Storage.Zip
{
    /// <summary>
    /// Streamed Zip Reader 
    /// </summary>
    public class ZipInputStream : Stream
    {
        const int DefaultBlockSize = 16 * 1024;

        Stream stream;
        BinaryReader reader;
        MemoryStream buffer;
        int bufferSize;

        ZipEncoding encoding;

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

        List<ZipEncoding.Entry> entries = new List<ZipEncoding.Entry>();
        /// <summary>
        /// A list of data entries found in Zip header
        /// </summary>
        public IEnumerable<ZipEncoding.Entry> Entries
        {
            get 
            {
                if (entries.Count == 0) entries = encoding.Decode(stream, reader);
                return entries;
            }
        }

        /// <summary>
        /// Opens a new Zip stream from certain stream
        /// </summary>
        /// <param name="stream">Base stream to process Zip data from</param>
        /// <param name="blockSize">The size of internal processing buffer</param>
        public ZipInputStream(Stream stream, int blockSize = DefaultBlockSize)
        {
            this.stream = stream;
            this.reader = new BinaryReader(stream);
            this.bufferSize = blockSize;
            this.encoding = new ZipEncoding();
        }

        public override int ReadByte()
        {
            return buffer.ReadByte();
        }
        public override int Read(byte[] buff, int offset, int count)
        {
            return buffer.Read(buff, offset, count);
        }

        /// <summary>
        /// Moves the stream pointer to the start of the given entry
        /// </summary>
        /// <param name="entry">An entry object that should be read</param>
        /// <returns>The position of the stream pointer</returns>
        public long Seek(ZipEncoding.Entry entry)
        {
            buffer = new MemoryStream(Math.Max(16, bufferSize));
            encoding.Decode(stream, reader, entry, buffer);

            buffer.Position = 0;
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
