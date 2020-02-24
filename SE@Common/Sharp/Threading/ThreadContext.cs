// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;

namespace SE
{
    public static class ThreadContext
    {
        private static ThreadLocal<Int32> globalId;
        private static atomic_int idBase = 0;

        /// <summary>
        /// A per context Thread and Service wide unique Id
        /// </summary>
        public static Int32 LocalId
        {
            get
            {
                Int32 scopeId; if (!globalId.IsValueCreated)
                {
                    scopeId = idBase.Increment();
                    globalId.Value = scopeId;
                }
                else scopeId = globalId.Value;
                return scopeId;
            }
        }

        static ThreadContext()
        {
            globalId = new ThreadLocal<Int32>();
        }

        /// <summary>
        /// Creates a lock region to be used in a using statement
        /// </summary>
        public static LockScope Lock(IScopeable scopeObject)
        {
            return new LockScope(scopeObject);
        }

        /// <summary>
        /// Creates a read-only region to be used in a using statement
        /// </summary>
        public static ReadScope ReadLock(ReadWriteLock @lock)
        {
            return new ReadScope(@lock);
        }
        /// <summary>
        /// Creates a write-exclusive region to be used in a using statement
        /// </summary>
        public static WriteScope WriteLock(ReadWriteLock @lock)
        {
            return new WriteScope(@lock);
        }
    }
}
