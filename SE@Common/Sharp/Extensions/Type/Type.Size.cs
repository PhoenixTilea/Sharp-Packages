// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class TypeExtension
    {
        /// <summary>
        /// Determines the size in bytes of the given type
        /// </summary>
        /// <returns>The size in bytes for primitive types, zero otherwise</returns>
        public static int Size(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte: return sizeof(Byte);
                case TypeCode.Char: return sizeof(Char);
                case TypeCode.Int16:
                case TypeCode.UInt16: return sizeof(UInt16);
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Single: return sizeof(UInt32);
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Double: return sizeof(UInt64);
                default: return 0;
            }
        }
    }
}
