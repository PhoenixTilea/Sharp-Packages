// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// An UTF8 compliant character
    /// </summary>
    [Serializable]
    public partial struct Char32 : IComparable, IFormattable, IConvertible, IComparable<Char32>, IEquatable<Char32>
    {
        UInt32 value;
        /// <summary>
        /// The full 4 byte value of this character
        /// </summary>
        public UInt32 Value
        {
            get { return value; }
        }

        /// <summary>
        /// Creates a new character from the given value
        /// </summary>
        public Char32(UInt32 value)
        {
            this.value = value;
        }

        public static explicit operator Int32(Char32 c)
        {
            return (Int32)c.value;
        }
        public static implicit operator UInt32(Char32 c)
        {
            return c.value;
        }
        public static explicit operator String(Char32 c)
        {
            return Char.ConvertFromUtf32((int)c.value);
        }

        public static implicit operator Char32(Int32 i)
        {
            return new Char32((UInt32)i);
        }
        public static implicit operator Char32(UInt32 i)
        {
            return new Char32(i);
        }

        public int CompareTo(object obj)
        {
            return value.CompareTo(obj);
        }
        public int CompareTo(Char32 other)
        {
            return value.CompareTo(other.value);
        }

        public bool Equals(Char32 other)
        {
            return value.Equals(other.value);
        }

        public TypeCode GetTypeCode()
        {
            return value.GetTypeCode();
        }
        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(value, provider);
        }
        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(value, provider);
        }
        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(value, provider);
        }
        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(value, provider);
        }
        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(value, provider);
        }
        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(value, provider);
        }
        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(value, provider);
        }
        public UInt32 ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(value, provider);
        }
        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(value, provider);
        }
        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(value, provider);
        }
        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(value, provider);
        }
        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(value, provider);
        }
        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(value, provider);
        }
        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(value, provider);
        }
        public string ToString(IFormatProvider provider)
        {
            return Convert.ToString(value, provider);
        }
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(value, conversionType, provider);
        }

        public override string ToString()
        {
            return Char.ConvertFromUtf32((Int32)value);
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return value.ToString(format, formatProvider);
        }
    }
}
