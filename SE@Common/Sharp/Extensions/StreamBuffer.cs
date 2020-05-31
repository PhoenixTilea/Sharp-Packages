// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace SE
{
    /// <summary>
    /// Wraps a buffer into a streaming interface
    /// </summary>
    public class StreamBuffer : Stream, IDisposable
    {
        Stream baseStream;
        CircularBuffer<byte> dataBuffer;
        int count;

        public override long Length
        {
            get
            {
                return count;
            }
        }

        public override long Position
        {
            get
            {
                if (count < 0)
                {
                    UpdateBuffer();
                }
                return (count - dataBuffer.Count);
            }
            set
            {
                if (value < 0)
                {
                    dataBuffer.SetRange(count, 0);
                }
                else if (value > count)
                {
                    dataBuffer.SetRange(count, count);
                }
                else dataBuffer.SetRange(count, (int)value);
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return true; }
        }
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a new buffer with provided capacity
        /// </summary>
        /// <param name="capacity">The capacity in elements the buffer should manage</param>
        public StreamBuffer(Stream baseStream, int capacity)
        {
            this.dataBuffer = new CircularBuffer<byte>(capacity);
            this.baseStream = baseStream;
            this.count = -1;
        }

        public new void Dispose()
        {
            Flush();
            base.Dispose();
        }

        bool UpdateBuffer()
        {
            count = baseStream.Read(dataBuffer.Buffer, 0, dataBuffer.Capacity);
            dataBuffer.SetRange(count, 0);

            return count > 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (dataBuffer.Count == 0 && !UpdateBuffer()) return 0;
            else
                try
                {
                    int read = dataBuffer.Read(buffer, offset, count);
                    if (count - read > 0)
                        read += Read(buffer, read, count - read);

                    return read;
                }
                finally
                {
                    if (Position == Length)
                        UpdateBuffer();
                }
        }
        public override void Flush()
        { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = (Length - offset);
                    break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
