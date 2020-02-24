// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// Endianess Encoding Extension
    /// </summary>
    public static class Endianess
    {
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Set(this byte[] data, Int16 value, int offset = 0)
        {
            data[offset] = (byte)((value >> 8) & 0xff);
            data[offset + 1] = (byte)(value & 0xff);
        }
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Set(this byte[] data, UInt16 value, int offset = 0)
        {
            data[offset] = (byte)((value >> 8) & 0xff);
            data[offset + 1] = (byte)(value & 0xff);
        }
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Set(this byte[] data, Int32 value, int offset = 0)
        {
            data[offset] = (byte)((value >> 24) & 0xff);
            data[offset + 1] = (byte)((value >> 16) & 0xff);
            data[offset + 2] = (byte)((value >> 8) & 0xff);
            data[offset + 3] = (byte)(value & 0xff);
        }
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Set(this byte[] data, UInt32 value, int offset = 0)
        {
            data[offset] = (byte)((value >> 24) & 0xff);
            data[offset + 1] = (byte)((value >> 16) & 0xff);
            data[offset + 2] = (byte)((value >> 8) & 0xff);
            data[offset + 3] = (byte)(value & 0xff);
        }
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Set(this byte[] data, Int64 value, int offset = 0)
        {
            Set(data, (UInt32)(value >> 32), offset);
            Set(data, (UInt32)(value & 0xffffffff), offset + 4);
        }
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Set(this byte[] data, UInt64 value, int offset = 0)
        {
            Set(data, (UInt32)(value >> 32), offset);
            Set(data, (UInt32)(value & 0xffffffff), offset + 4);
        }

        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static Int16 ToInt16(this byte[] data, int offset = 0)
        {
            return (Int16)(data[offset] << 8 | data[offset + 1]);
        }
        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static UInt16 ToUInt16(this byte[] data, int offset = 0)
        {
            return (UInt16)(data[offset] << 8 | data[offset + 1]);
        }
        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static Int32 ToInt32(this byte[] data, int offset = 0)
        {
            Int32 result = data[offset++];
            result = (result << 8) | data[offset++];
            result = (result << 8) | data[offset++];
            result = (result << 8) | data[offset];
            return result;
        }
        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static UInt32 ToUInt32(this byte[] data, int offset = 0)
        {
            UInt32 result = data[offset++];
            result = (result << 8) | data[offset++];
            result = (result << 8) | data[offset++];
            result = (result << 8) | data[offset];
            return result;
        }
        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static Int64 ToInt64(this byte[] data, int offset = 0)
        {
            UInt32 hipart = ToUInt32(data, offset);
            UInt32 lopart = ToUInt32(data, offset + 4);
            return (((Int64)hipart) << 32) | lopart;
        }
        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static UInt64 ToUInt64(this byte[] data, int offset = 0)
        {
            UInt32 hipart = ToUInt32(data, offset);
            UInt32 lopart = ToUInt32(data, offset + 4);
            return (((UInt64)hipart) << 32) | lopart;
        }

        /// <summary>
        /// Converts a given integer into big endian byte order if necessary
        /// </summary>
        /// <typeparam name="T">The type of integer to change encoding</typeparam>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static T ToBigEndian<T>(T value)
        {
            if (BitConverter.IsLittleEndian)
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Int32: return (T)Convert.ChangeType(ByteSwap32((Int32)Convert.ChangeType(value, typeof(Int32))), typeof(T));
                    case TypeCode.UInt32: return (T)Convert.ChangeType(ByteSwap32((UInt32)Convert.ChangeType(value, typeof(UInt32))), typeof(T));
                    case TypeCode.Int64: return (T)Convert.ChangeType(ByteSwap64((Int64)Convert.ChangeType(value, typeof(Int64))), typeof(T));
                    case TypeCode.UInt64: return (T)Convert.ChangeType(ByteSwap64((UInt64)Convert.ChangeType(value, typeof(UInt64))), typeof(T));
                    default: return value;
                }
            else return value;
        }
        /// <summary>
        /// Converts a given integer into little endian byte order if necessary
        /// </summary>
        /// <typeparam name="T">The type of integer to change encoding</typeparam>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static T FromBigEndian<T>(T value)
        {
            if (BitConverter.IsLittleEndian)
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Int32: return (T)Convert.ChangeType(ByteSwap32((Int32)Convert.ChangeType(value, typeof(Int32))), typeof(T));
                    case TypeCode.UInt32: return (T)Convert.ChangeType(ByteSwap32((UInt32)Convert.ChangeType(value, typeof(UInt32))), typeof(T));
                    case TypeCode.Int64: return (T)Convert.ChangeType(ByteSwap64((Int64)Convert.ChangeType(value, typeof(Int64))), typeof(T));
                    case TypeCode.UInt64: return (T)Convert.ChangeType(ByteSwap64((UInt64)Convert.ChangeType(value, typeof(UInt64))), typeof(T));
                    default: return value;
                }
            else return value;
        }

        /// <summary>
        /// Converts a given integer into big endian byte order if necessary
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static Int32 ToBigEndian(Int32 value)
        {
            if (BitConverter.IsLittleEndian) return ByteSwap32(value);
            else return value;
        }
        /// <summary>
        /// Converts a given integer into big endian byte order if necessary
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static UInt32 ToBigEndian(UInt32 value)
        {
            if (BitConverter.IsLittleEndian) return ByteSwap32(value);
            else return value;
        }
        /// <summary>
        /// Converts a given integer into big endian byte order if necessary
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static Int64 ToBigEndian(Int64 value)
        {
            if (BitConverter.IsLittleEndian) return ByteSwap64(value);
            else return value;
        }
        /// <summary>
        /// Converts a given integer into big endian byte order if necessary
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static UInt64 ToBigEndian(UInt64 value)
        {
            if (BitConverter.IsLittleEndian) return ByteSwap64(value);
            else return value;
        }

        /// <summary>
        /// Converts a given integer into little endian byte order if necessary
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static Int32 FromBigEndian(Int32 value)
        {
            if (BitConverter.IsLittleEndian) return ByteSwap32(value);
            else return value;
        }
        /// <summary>
        /// Converts a given integer into little endian byte order if necessary
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static UInt32 FromBigEndian(UInt32 value)
        {
            if (BitConverter.IsLittleEndian) return ByteSwap32(value);
            else return value;
        }
        /// <summary>
        /// Converts a given integer into little endian byte order if necessary
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static Int64 FromBigEndian(Int64 value)
        {
            if (BitConverter.IsLittleEndian) return ByteSwap64(value);
            else return value;
        }
        /// <summary>
        /// Converts a given integer into little endian byte order if necessary
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static UInt64 FromBigEndian(UInt64 value)
        {
            if (BitConverter.IsLittleEndian) return ByteSwap64(value);
            else return value;
        }

        /// <summary>
        /// Swaps the given integer bytes without checking
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static Int32 ByteSwap32(Int32 value)
        {
            return (Int32)(((UInt32)value << 24) | ((UInt32)value >> 24) | (((UInt32)value & 0xff00) << 8) | (((UInt32)value & 0xff0000) >> 8));
        }
        /// <summary>
        /// Swaps the given integer bytes without checking
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static UInt32 ByteSwap32(UInt32 value)
        {
            return (UInt32)(((UInt32)value << 24) | ((UInt32)value >> 24) | (((UInt32)value & 0xff00) << 8) | (((UInt32)value & 0xff0000) >> 8));
        }
        /// <summary>
        /// Swaps the given integer bytes without checking
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static Int64 ByteSwap64(Int64 value)
        {
            return (Int64)((((UInt64)ByteSwap32((UInt32)value)) << 32) | ByteSwap32((UInt32)((UInt64)value >> 32)));
        }
        /// <summary>
        /// Swaps the given integer bytes without checking
        /// </summary>
        /// <param name="value">An integer to change encoding</param>
        /// <returns>The integer result</returns>
        public static UInt64 ByteSwap64(UInt64 value)
        {
            return (UInt64)((((UInt64)ByteSwap32((UInt32)value)) << 32) | ByteSwap32((UInt32)((UInt64)value >> 32)));
        }
    }
}
