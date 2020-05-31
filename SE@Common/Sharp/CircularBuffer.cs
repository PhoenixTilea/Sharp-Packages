// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// Provides a round robin type of storage
    /// </summary>
    public class CircularBuffer<T>
    {
        T[] buffer;
        /// <summary>
        /// The internal data buffer used by this storage type
        /// </summary>
        public T[] Buffer
        {
            get { return buffer; }
        }

        int head;
        /// <summary>
        /// Current position of the producer
        /// </summary>
        public int WritePointer
        {
            get { return head; }
        }

        int tail;
        /// <summary>
        /// Current position of the consumer
        /// </summary>
        public int ReadPointer
        {
            get { return tail; }
        }

        bool full;
        /// <summary>
        /// Determines if this instance can't read any further data
        /// </summary>
        public bool Full
        {
            get { return full; }
        }

        /// <summary>
        /// Current item count
        /// </summary>
        public int Count
        {
            get
            {
                if(full) return buffer.Length;
                else if (head >= tail) return head - tail;
                else return buffer.Length - (tail - head);
            }
        }
        /// <summary>
        /// The maximum number of items that can be stored
        /// </summary>
        public int Capacity
        {
            get { return buffer.Length; }
        }

        /// <summary>
        /// Creates a new instance with the given capacity
        /// </summary>
        /// <param name="capacity">A capacity to provide by this stoirage type</param>
        public CircularBuffer(int capacity)
        {
            capacity--;
            capacity |= capacity >> 1;
            capacity |= capacity >> 2;
            capacity |= capacity >> 4;
            capacity |= capacity >> 8;
            capacity |= capacity >> 16;
            capacity |= capacity >> 32;
            capacity++;

            buffer = new T[capacity];
        }

        /// <summary>
        /// Reads a sequence of bytes from the current storage type
        /// </summary>
        /// <param name="items">
        /// An array of items. When this method returns, the buffer contains the specified
        /// byte array with the values between offset and (offset + count - 1) replaced by the
        /// items read from the current source.</param>
        /// <param name="offset">
        /// The zero-based items offset in buffer at which to begin storing the data read from
        /// the current stream.</param>
        /// <param name="count">The maximum number of items to be read from the current stream.</param>
        /// <returns>The amount of items read</returns>
        public int Read(T[] items, int offset, int count)
        {
            int size = Count;
            if (count > size)
                count = size;

            if (tail + count > buffer.Length)
            {
                int overlap = (tail + count) & (buffer.Length - 1);

                Array.Copy(buffer, tail, items, offset, count - overlap);
                Array.Copy(buffer, 0, items, count - overlap + offset, overlap);
            }
            else Array.Copy(buffer, tail, items, offset, count);

            tail = (tail + count) & (buffer.Length - 1);
            full = !(count > 0);
            return count;
        }

        /// <summary>
        /// Clears the data
        /// </summary>
        public void Clear()
        {
            head = 0;
            tail = 0;
            full = false;
        }

        /// <summary>
        /// Changes the data range to a value between the given minimum and maximum
        /// </summary>
        public void SetRange(int writePointer, int readPointer)
        {
            head = writePointer & (buffer.Length - 1);
            tail = readPointer & (buffer.Length - 1);
            full = (head == tail && writePointer != readPointer);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current storage type
        /// </summary>
        /// <param name="items">An array of items. This method copies count items from buffer to the current stream.</param>
        /// <param name="offset">The zero-based items offset in buffer at which to begin copying items to the current stream.</param>
        /// <param name="count">The number of items to be written to the current stream.</param>
        /// <returns>The amount of items written</returns>
        public int Write(byte[] items, int offset, int count)
        {
            int size = Count;
            if (count > buffer.Length - size)
                count = buffer.Length - size;

            if (head + count > buffer.Length)
            {
                int overlap = (head + count) & (buffer.Length - 1);

                Array.Copy(items, offset, buffer, head, count - overlap);
                Array.Copy(items, count - overlap + offset, buffer, 0, overlap);
            }
            else Array.Copy(items, offset, buffer, head, count);

            head = (head + count) & (buffer.Length - 1);
            full = (head == tail);
            return count;
        }
    }
}