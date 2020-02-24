// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class IntegerExtension
    {
        /// <summary>
        /// Returns the largest possible value of a numeric type
        /// </summary>
        /// <returns>The largest possible value of the numeric type</returns>
        public static T MaxValue<T>(this T i) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            switch (i.GetTypeCode())
            {
                case TypeCode.Byte: return (T)Convert.ChangeType(byte.MaxValue, typeof(T));
                case TypeCode.SByte: return (T)Convert.ChangeType(sbyte.MaxValue, typeof(T));
                case TypeCode.Char: return (T)Convert.ChangeType(char.MaxValue, typeof(T));
                case TypeCode.Int16: return (T)Convert.ChangeType(Int16.MaxValue, typeof(T));
                case TypeCode.UInt16: return (T)Convert.ChangeType(UInt16.MaxValue, typeof(T));
                case TypeCode.Int32: return (T)Convert.ChangeType(Int32.MaxValue, typeof(T));
                case TypeCode.UInt32: return (T)Convert.ChangeType(UInt32.MaxValue, typeof(T));
                case TypeCode.Int64: return (T)Convert.ChangeType(Int64.MaxValue, typeof(T));
                case TypeCode.UInt64: return (T)Convert.ChangeType(UInt64.MaxValue, typeof(T));
                case TypeCode.Single: return (T)Convert.ChangeType(float.MaxValue, typeof(T));
                case TypeCode.Double: return (T)Convert.ChangeType(double.MaxValue, typeof(T));
                case TypeCode.Decimal: return (T)Convert.ChangeType(decimal.MaxValue, typeof(T));
                default: return default(T);
            }
        }
    }
}
