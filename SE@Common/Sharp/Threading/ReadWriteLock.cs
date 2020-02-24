// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;

namespace SE
{
    /// <summary>
    /// Spin awaits until desired access is passed
    /// </summary>
    public class ReadWriteLock
    {
        const Int32 WriteMask = 0x7ffffff;
        const Int32 ReaderMask = 0x8000000;

        atomic_int @lock;
        /// <summary>
        /// Gets current lock state
        /// </summary>
        public Int32 State
        {
            get { return @lock.Value; }
        }

        Int32 scopeId;

        atomic_int writeReferences;
        /// <summary>
        /// Gets the amount of write access nesting in this scope
        /// </summary>
        public int RefCount
        {
            get { return writeReferences.Value; }
        }

        /// <summary>
        /// Creates a new lock instance
        /// </summary>
        public ReadWriteLock()
        { }

        public void ReadLock()
        {
            for (;;)
            {
                if (scopeId == ThreadContext.LocalId)
                    return;

                // Wait until there's no active writer
                while ((@lock & ReaderMask) != 0)
                    Thread.Sleep(0);

                Int32 oldLock = (@lock & WriteMask);
                Int32 newLock = oldLock + 1;

                if (@lock.CompareExchange(newLock, oldLock) == oldLock)
                    return;
            }
        }
        /// <summary>
        /// Try to acquire read access to the critical section
        /// </summary>
        /// <returns>True if successfully locked, false otherwise</returns>
        public bool TryGetReadLock()
        {
            if (scopeId == ThreadContext.LocalId)
                return true;

            // Wait until there's no active writer
            if ((@lock & ReaderMask) != 0)
                return false;

            Int32 oldLock = (@lock & WriteMask);
            Int32 newLock = oldLock + 1;
            return (@lock.CompareExchange(newLock, oldLock) == oldLock);
        }

        public void ReadRelease()
        {
            if (scopeId != ThreadContext.LocalId)
                @lock.Decrement();
        }

        public void WriteLock()
        {
            for (;;)
            {
                if (scopeId == ThreadContext.LocalId && (@lock & WriteMask) == 0)
                {
                    writeReferences.Increment();
                    return;
                }

                // Wait until there's no active writer
                while ((@lock & ReaderMask) != 0)
                    Thread.Sleep(0);

                Int32 oldLock = (@lock & WriteMask);
                Int32 newLock = (oldLock | ReaderMask);

                if (@lock.CompareExchange(newLock, oldLock) == oldLock)
                {
                    // Wait for active readers to release their locks
                    while ((@lock & WriteMask) != 0)
                        Thread.Sleep(0);

                    scopeId = ThreadContext.LocalId;
                    return;
                }
            }
        }
        /// <summary>
        /// Try to acquire write access to the critical section
        /// </summary>
        /// <returns>True if successfully locked, false otherwise</returns>
        public bool TryGetWriteLock()
        {
            if (scopeId == ThreadContext.LocalId && (@lock & WriteMask) == 0)
            {
                writeReferences.Increment();
                return true;
            }

            // Wait until there's no active writer
            if ((@lock & ReaderMask) != 0)
                return false;

            Int32 oldLock = (@lock & WriteMask);
            Int32 newLock = (oldLock | ReaderMask);

            if (@lock.CompareExchange(newLock, oldLock) == oldLock)
            {
                // Wait for active readers to release their locks
                while ((@lock & WriteMask) != 0)
                {
                    if (@lock.CompareExchange(oldLock, newLock) == newLock)
                        return false;
                }

                scopeId = ThreadContext.LocalId;
                return true;
            }
            else return false;
        }

        public void WriteRelease()
        {
            if (writeReferences > 0) writeReferences.Decrement();
            else
            {
                scopeId = ~scopeId;
                @lock = 0;
            }
        }
    }
}