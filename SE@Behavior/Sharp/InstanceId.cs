// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    /// <summary>
    /// A 64 bit instance ID used to identify an object and optional a single component
    /// </summary>
    public struct InstanceId
    {
        private readonly UInt64 value;

        /// <summary>
        /// The underlaying object ID
        /// </summary>
        public UInt32 ObjectId
        {
            get { return (UInt32)(value >> 32); }
        }
        /// <summary>
        /// An optional component ID
        /// </summary>
        public UInt32 ComponentId
        {
            get { return (UInt32)(value & UInt32.MaxValue); }
        }

        /// <summary>
        /// Creates a new instance ID from the given integer
        /// </summary>
        /// <param name="id"></param>
        public InstanceId(UInt64 id)
        {
            this.value = id;
        }
        /// <summary>
        /// Creates a new instance ID from the given integer
        /// </summary>
        /// <param name="id"></param>
        public InstanceId(UInt32 id)
        {
            this.value = (((UInt64)id) << 32);
        }

        public static InstanceId operator |(InstanceId lhs, UInt32 rhs)
        {
            return new InstanceId(((UInt64)lhs.ObjectId << 32) | rhs);
        }

        public static implicit operator UInt64(InstanceId id)
        {
            return id.value;
        }
        public static implicit operator InstanceId(UInt32 id)
        {
            return new InstanceId(id);
        }
        public static implicit operator InstanceId(UInt64 id)
        {
            return new InstanceId(id);
        }

        public int CompareTo(InstanceId other)
        {
            return value.CompareTo(other.value);
        }
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("Object: {0}, Component: {1}", ObjectId, ComponentId);
        }
    }
}
