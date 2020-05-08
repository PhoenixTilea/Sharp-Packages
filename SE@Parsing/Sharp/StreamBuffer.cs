// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// Wraps a buffer of primitive types into a streaming interface
    /// </summary>
    public class StreamBuffer<T> : Stream, IDisposable where T : struct, IComparable, IConvertible, IComparable<T>, IEquatable<T>
    {
        private readonly static Func<object, object> internalBuffer;
        private readonly static int byteSize;

        List<T> buffer;
        /// <summary>
        /// The underlaying data buffer
        /// </summary>
        public List<T> Buffer
        {
            get { return buffer; }
        }
        public override long Length
        {
            get { return buffer.Count; }
        }

        long position;
        public override long Position
        {
            get { return position; }
            set
            {
                if (value < 0) position = 0;
                else if (value > buffer.Count) position = buffer.Count;
                else position = value;
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
        /// An element considered as currently pointing to
        /// </summary>
        public T Current
        {
            get
            {
                if (position >= buffer.Count) return default(T);
                else return buffer[(int)position];
            }
            set { buffer[(int)position] = value; }
        }

        /// <summary>
        /// The head element of the buffer
        /// </summary>
        public T Head
        {
            get
            {
                if (buffer.Count == 0) return default(T);
                else return buffer[buffer.Count - 1];
            }
        }

        static StreamBuffer()
        {
            FieldInfo buffer = typeof(List<T>).GetField("_items", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            internalBuffer = (Func<object, object>)buffer.CreateGetter();

            byteSize = typeof(T).Size();
        }
        /// <summary>
        /// Creates a new buffer with provided capacity
        /// </summary>
        /// <param name="capacity">The capacity in elements the buffer should manage</param>
        public StreamBuffer(int capacity)
        {
            this.buffer = new List<T>(capacity);
        }
        /// <summary>
        /// Creates a new buffer containing elements
        /// </summary>
        /// <param name="buffer">The content to add to this buffer</param>
        public StreamBuffer(IEnumerable<T> buffer)
        {
            this.buffer = new List<T>(buffer);
        }
        /// <summary>
        /// Creates a new buffer instance
        /// </summary>
        public StreamBuffer()
            :this(16)
        { }

        public new void Dispose()
        {
            Flush();
            base.Dispose();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int remaining = (int)(Length - position) * byteSize;
            count = Math.Min(count, remaining);

            if (count > 0)
            {
                System.Buffer.BlockCopy
                (
                    internalBuffer(this.buffer) as Array,
                    (int)(position * byteSize),
                    buffer,
                    offset,
                    count
                );
                Position += (count / byteSize) + ((count % byteSize > 0) ? 1 : 0);
            }
            return count;
        }
        public override void Flush()
        {
            buffer.Clear();
            position = 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin: Position = offset; break;
                case SeekOrigin.Current: Position += offset; break;
                case SeekOrigin.End: Position = (Length - offset); break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            int delta = (int)(value / byteSize) + ((value % byteSize > 0) ? 1 : 0);
            delta = delta - buffer.Count;

            if (delta < 0)
            {
                buffer.RemoveRange(buffer.Count + delta, -delta);
                if (position > -delta)
                    Position = -delta;
            }
            else buffer.AddRange(FillBuffer(delta));    
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            int elementCount = (count / byteSize) + ((count % byteSize > 0) ? 1 : 0);
            int delta = (int)(elementCount - position);

            this.buffer.AddRange(FillBuffer(delta));
            System.Buffer.BlockCopy
            (
                buffer,
                offset, 
                internalBuffer(this.buffer) as Array, 
                (int)(position * byteSize), 
                count
            );
            Position += elementCount;
        }

        /// <summary>
        /// Replaces the last item added to the buffer
        /// </summary>
        /// <param name="value">The value to replace the item with</param>
        /// <returns>The value that has been replaced</returns>
        public T Replace(T value)
        {
            if (buffer.Count > 0)
            {
                T result = buffer[buffer.Count - 1];
                buffer[buffer.Count - 1] = value;

                return result;
            }
            else
            {
                buffer.Add(value);
                position++;

                return default(T);
            }
        }

        /// <summary>
        /// Discards items from the end of this buffer
        /// </summary>
        /// <param name="count">An amount of items to discard</param>
        public void Discard(int count)
        {
            buffer.RemoveRange(buffer.Count - count, count);
            position = buffer.Count;
        }

        IEnumerable<T> FillBuffer(int length)
        {
            while (length > 0)
            {
                length--;
                yield return default(T);
            }

            yield break;
        }
    }
}
