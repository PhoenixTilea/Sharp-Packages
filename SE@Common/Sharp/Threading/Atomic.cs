// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;

namespace SE
{
    /// <summary>
    /// A data container to guarantee thread-safe read/ write behavior
    /// </summary>
    public struct atomic_bool
    {
        int rawValue;
        /// <summary>
        /// Gets or sets the contained data thread-safe
        /// </summary>
        public bool Value
        {
            get { return (Interlocked.CompareExchange(ref rawValue, default(int), default(int)) == 1); }
            set { Interlocked.Exchange(ref rawValue, (value) ? 1 : 0); }
        }

        /// <summary>
        /// Gets or sets the contained data unsafe
        /// </summary>
        public bool UnsafeValue
        {
            get { return (rawValue == 1); }
            set { rawValue = ((value) ? 1 : 0); }
        }

        /// <summary>
        /// Creates a new thread-safe Int32 instance
        /// </summary>
        public atomic_bool(bool value)
        {
            this.rawValue = ((value) ? 1 : 0);
        }

        public static implicit operator atomic_bool(bool value)
        {
            return new atomic_bool(value);
        }
        public static implicit operator bool(atomic_bool b)
        {
            return (b.rawValue == 1);
        }

        /// <summary>
        /// Exchanges current value for the provided one
        /// </summary>
        /// <returns>The previous value</returns>
        public bool Exchange(bool value)
        {
            return (Interlocked.Exchange(ref rawValue, (value) ? 1 : 0) == 1);
        }

        /// <summary>
        /// Exchanges current value for the provided one if it matches comparand
        /// </summary>
        /// <returns>The previous value if exchanged successfully, the new value otherwise</returns>
        public bool CompareExchange(bool value, bool comparand)
        {
            return (Interlocked.CompareExchange(ref rawValue, (value) ? 1 : 0, (comparand) ? 1 : 0) == 1);
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// A data container to guarantee thread-safe read/ write behavior
    /// </summary>
    public struct atomic_int
    {
        Int32 rawValue;
        /// <summary>
        /// Gets or sets the contained data thread-safe
        /// </summary>
        public Int32 Value
        {
            get { return Interlocked.CompareExchange(ref rawValue, default(Int32), default(Int32)); }
            set { Interlocked.Exchange(ref rawValue, value); }
        }

        /// <summary>
        /// Gets or sets the contained data unsafe
        /// </summary>
        public Int32 UnsafeValue
        {
            get { return rawValue; }
            set { rawValue = value; }
        }

        /// <summary>
        /// Creates a new thread-safe Int32 instance
        /// </summary>
        public atomic_int(Int32 value)
        {
            this.rawValue = value;
        }

        public static implicit operator atomic_int(Int32 value)
        {
            return new atomic_int(value);
        }
        public static implicit operator Int32(atomic_int i)
        {
            return i.rawValue;
        }

        /// <summary>
        /// Thread-safe increments current value
        /// </summary>
        /// <returns>The result value</returns>
        public Int32 Increment()
        {
            return Interlocked.Increment(ref rawValue);
        }

        /// <summary>
        /// Thread-safe decrement current value
        /// </summary>
        /// <returns>The result value</returns>
        public Int32 Decrement()
        {
            return Interlocked.Decrement(ref rawValue);
        }

        /// <summary>
        /// Performs a thread-safe add operation
        /// </summary>
        /// <returns>The result value</returns>
        public Int32 Add(Int32 value)
        {
            return Interlocked.Add(ref rawValue, value);
        }

        /// <summary>
        /// Performs a thread-safe sub operation
        /// </summary>
        /// <returns>The result value</returns>
        public Int32 Sub(Int32 value)
        {
            return Interlocked.Add(ref rawValue, -value);
        }

        /// <summary>
        /// Performs a thread-safe mult operation
        /// </summary>
        /// <returns>The result value</returns>
        public Int32 Mult(Int32 value)
        {
            Int32 tmp;
            do
            {
                tmp = Value;
            }
            while (Interlocked.CompareExchange(ref rawValue, tmp * value, tmp) != tmp);
            return tmp * value;
        }

        /// <summary>
        /// Performs a thread-safe modulo operation
        /// </summary>
        /// <returns>The result value</returns>
        public Int32 Mod(Int32 value)
        {
            Int32 tmp;
            do
            {
                tmp = Value;
            }
            while (Interlocked.CompareExchange(ref rawValue, tmp % value, tmp) != tmp);
            return tmp % value;
        }

        /// <summary>
        /// Performs a thread-safe div operation
        /// </summary>
        /// <returns>The result value</returns>
        public Int32 Div(Int32 value)
        {
            Int32 tmp;
            do
            {
                tmp = Value;
            }
            while (Interlocked.CompareExchange(ref rawValue, tmp / value, tmp) != tmp);
            return tmp / value;
        }

        /// <summary>
        /// Exchanges current value for the provided one
        /// </summary>
        /// <returns>The previous value</returns>
        public Int32 Exchange(Int32 value)
        {
            return Interlocked.Exchange(ref rawValue, value);
        }

        /// <summary>
        /// Exchanges current value for the provided one if it matches comparand
        /// </summary>
        /// <returns>The previous value if exchanged successfully, the new value otherwise</returns>
        public Int32 CompareExchange(Int32 value, Int32 comparand)
        {
            return Interlocked.CompareExchange(ref rawValue, value, comparand);
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// A data container to guarantee thread-safe read/ write behavior
    /// </summary>
    public struct atomic_long
    {
        Int64 rawValue;
        /// <summary>
        /// Gets or sets the contained data thread-safe
        /// </summary>
        public Int64 Value
        {
            get { return Interlocked.CompareExchange(ref rawValue, default(Int64), default(Int64)); }
            set { Interlocked.Exchange(ref rawValue, value); }
        }

        /// <summary>
        /// Gets or sets the contained data unsafe
        /// </summary>
        public Int64 UnsafeValue
        {
            get { return rawValue; }
            set { rawValue = value; }
        }

        /// <summary>
        /// Creates a new thread-safe Int32 instance
        /// </summary>
        public atomic_long(Int64 value)
        {
            this.rawValue = value;
        }

        public static implicit operator atomic_long(Int64 value)
        {
            return new atomic_long(value);
        }
        public static implicit operator Int64(atomic_long i)
        {
            return i.rawValue;
        }

        /// <summary>
        /// Thread-safe increments current value
        /// </summary>
        /// <returns>The result value</returns>
        public Int64 Increment()
        {
            return Interlocked.Increment(ref rawValue);
        }

        /// <summary>
        /// Thread-safe decrement current value
        /// </summary>
        /// <returns>The result value</returns>
        public Int64 Decrement()
        {
            return Interlocked.Decrement(ref rawValue);
        }

        /// <summary>
        /// Performs a thread-safe add operation
        /// </summary>
        /// <returns>The result value</returns>
        public Int64 Add(Int64 value)
        {
            return Interlocked.Add(ref rawValue, value);
        }

        /// <summary>
        /// Performs a thread-safe sub operation
        /// </summary>
        /// <returns>The result value</returns>
        public Int64 Sub(Int64 value)
        {
            return Interlocked.Add(ref rawValue, -value);
        }

        /// <summary>
        /// Performs a thread-safe mult operation
        /// </summary>
        /// <returns>The result value</returns>
        public Int64 Mult(Int64 value)
        {
            Int64 tmp;
            do
            {
                tmp = Value;
            }
            while (Interlocked.CompareExchange(ref rawValue, tmp * value, tmp) != tmp);
            return tmp * value;
        }

        /// <summary>
        /// Performs a thread-safe modulo operation
        /// </summary>
        /// <returns>The result value</returns>
        public Int64 Mod(Int64 value)
        {
            Int64 tmp;
            do
            {
                tmp = Value;
            }
            while (Interlocked.CompareExchange(ref rawValue, tmp % value, tmp) != tmp);
            return tmp % value;
        }

        /// <summary>
        /// Performs a thread-safe div operation
        /// </summary>
        /// <returns>The result value</returns>
        public Int64 Div(Int64 value)
        {
            Int64 tmp;
            do
            {
                tmp = Value;
            }
            while (Interlocked.CompareExchange(ref rawValue, tmp / value, tmp) != tmp);
            return tmp / value;
        }

        /// <summary>
        /// Exchanges current value for the provided one
        /// </summary>
        /// <returns>The previous value</returns>
        public Int64 Exchange(Int64 value)
        {
            return Interlocked.Exchange(ref rawValue, value);
        }

        /// <summary>
        /// Exchanges current value for the provided one if it matches comparand
        /// </summary>
        /// <returns>The previous value if exchanged successfully, the new value otherwise</returns>
        public Int64 CompareExchange(Int64 value, Int64 comparand)
        {
            return Interlocked.CompareExchange(ref rawValue, value, comparand);
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// A data container to guarantee thread-safe read/ write behavior
    /// </summary>
    public struct atomic_reference<T> where T : class
    {
        T rawValue;
        /// <summary>
        /// Gets or sets the contained data thread-safe
        /// </summary>
        public T Value
        {
            get { return Interlocked.CompareExchange<T>(ref rawValue, default(T), default(T)); }
            set { Interlocked.Exchange<T>(ref rawValue, value); }
        }

        /// <summary>
        /// Gets or sets the contained data unsafe
        /// </summary>
        public T UnsafeValue
        {
            get { return rawValue; }
            set { rawValue = value; }
        }

        /// <summary>
        /// Creates a new thread-safe Int32 instance
        /// </summary>
        public atomic_reference(T value)
        {
            this.rawValue = value;
        }

        public static implicit operator atomic_reference<T>(T value)
        {
            return new atomic_reference<T>(value);
        }
        public static implicit operator T(atomic_reference<T> r)
        {
            return r.rawValue;
        }

        /// <summary>
        /// Exchanges current value for the provided one
        /// </summary>
        /// <returns>The previous value</returns>
        public T Exchange(T value)
        {
            return Interlocked.Exchange<T>(ref rawValue, value);
        }

        /// <summary>
        /// Exchanges current value for the provided one if it matches comparand
        /// </summary>
        /// <returns>The previous value if exchanged successfully, the new value otherwise</returns>
        public T CompareExchange(T value, T comparand)
        {
            return Interlocked.CompareExchange<T>(ref rawValue, value, comparand);
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
