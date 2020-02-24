// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;
using SE;

namespace SE.Parallel
{
    /// <summary>
    /// A lock-free multi reader/multi writer queue of certain Type
    /// </summary>
    /// <typeparam name="T">The object Type to handle</typeparam>
    public class ConcurrentQueue<T>
    {
        public readonly int BufferLength;
        readonly UInt32 Mask;

        T[] buffer;
        /// <summary>
        /// Returns this queues internal data buffer
        /// </summary>
        public T[] InternalBuffer
        {
            get { return buffer; }
        }

        atomic_long readPtr;
        /// <summary>
        /// A pointer pointing to the next element to read
        /// </summary>
        public int ReadPointer
        {
            get { return (int)readPtr; }
            set { readPtr.Exchange((readPtr + value) & Mask); }
        }

        atomic_long writePtr;
        /// <summary>
        /// A pointer pointing to the next free slot to write
        /// </summary>
        public int WritePointer
        {
            get { return (int)writePtr; }
            set { writePtr.Exchange((writePtr + value) & Mask); }
        }

        Spinlock writeLock = new Spinlock();
        Spinlock readLock = new Spinlock();

        /// <summary>
        /// The length between reading pointer and writing pointer
        /// </summary>
        public int Length
        {
            get
            {
                UInt32 reader = (UInt32)readPtr.Value;
                UInt32 writer = (UInt32)writePtr.Value;
                return (int)((reader > writer) ? ((UInt32)BufferLength - reader + writer) : (writer - reader));
            }
        }

        /// <summary>
        /// Creates a new lock-free queue of certain size
        /// </summary>
        /// <param name="size"></param>
        public ConcurrentQueue(int size)
        {
            this.readPtr = 0;
            this.writePtr = 0;
            this.BufferLength = size;
            this.Mask = ((UInt32)size - 1u);
            buffer = new T[BufferLength];
        }

        /// <summary>
        /// Tries to enqueue an element into the buffer
        /// </summary>
        /// <param name="value">The element to enqueue</param>
        /// <returns>True if element was successfully enqueued, otherwise false</returns>
        public bool Enqueue(T value)
        {
            using (ThreadContext.Lock(writeLock))
            {
                UInt32 current = (UInt32)writePtr;
                UInt32 next = current + 1;
                UInt32 reader = (UInt32)readPtr.Value;

                if (((reader > current) ? ((UInt32)BufferLength - reader + next) : (next - reader)) > Mask)
                    return false;

                buffer[current] = value;
                writePtr.Exchange(next & Mask);
                return true;
            }
        }

        /// <summary>
        /// Tries to dequeue the next element from the buffer
        /// </summary>
        /// <param name="value">The resulting element dequeued</param>
        /// <returns>True if there was at least one element to dequeue, false otherwise</returns>
        public bool Dequeue(out T value)
        {
            UInt32 current;
            using (ThreadContext.Lock(readLock))
            {
                current = (UInt32)readPtr;
            }

            if (current == writePtr.Value)
            {
                value = default(T);
                return false;
            }

            value = buffer[current];
            bool result = true;

            using (ThreadContext.Lock(readLock))
            {
                if (readPtr != current) result = false;
                else
                {
                    buffer[current] = default(T);
                    readPtr.Exchange((readPtr + 1) & Mask);
                }
            }

            return result;
        }
    }
}
