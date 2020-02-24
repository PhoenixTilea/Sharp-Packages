// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reflection
{
    /// <summary>
    /// Stores a managed pointer to an object in the heap
    /// </summary>
    public struct ManagedPtr<T>
    {
        readonly IntPtr value;
        /// <summary>
        /// The pointer address stored
        /// </summary>
        public IntPtr Value
        {
            get { return value; }
        }

        /// <summary>
        /// The target object in local managed heap
        /// </summary>
        public T Target
        {
            get { return value.ToInstance<T>(); }
        }

        /// <summary>
        /// Creates a new pointer instance from a managed address
        /// </summary>
        public ManagedPtr(IntPtr value)
        {
            this.value = value;
        }
        /// <summary>
        /// Creates a new pointer instance from a managed object instance
        /// </summary>
        /// <param name="instance"></param>
        public ManagedPtr(T instance)
        {
            this.value = instance.ToPointer();
        }

        public static implicit operator bool(ManagedPtr<T> ptr)
        {
            return (ptr.value == IntPtr.Zero);
        }

        public static implicit operator ManagedPtr<T>(IntPtr value)
        {
            return new ManagedPtr<T>(value);
        }
        public static implicit operator IntPtr(ManagedPtr<T> ptr)
        {
            return ptr.value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return value.Equals(obj);
        }

        public override string ToString()
        {
            switch (IntPtr.Size)
            {
                case 4: return value.ToString("x8");
                default: return value.ToString("x16");
            }
        }
    }
}
