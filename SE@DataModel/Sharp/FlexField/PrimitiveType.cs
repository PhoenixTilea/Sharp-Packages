// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SE;

namespace SE.DataModel
{
    /// <summary>
    /// A type struct able to store primitive value types
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct PrimitiveType
    {
        [FieldOffset(0)]
        public IntPtr Segment0_IntPtr;

        [FieldOffset(0)]
        public UInt32 Segment0_UInt32;

        [FieldOffset(0)]
        public Int32 Segment0_Int32;

        [FieldOffset(0)]
        public UInt16 Segment0_UInt16;
        [FieldOffset(2)]
        public UInt16 Segment1_UInt16;

        [FieldOffset(0)]
        public Int16 Segment0_Int16;
        [FieldOffset(2)]
        public Int16 Segment1_Int16;

        [FieldOffset(0)]
        public byte Segment0_Byte;
        [FieldOffset(1)]
        public byte Segment1_Byte;
        [FieldOffset(2)]
        public byte Segment2_Byte;
        [FieldOffset(3)]
        public byte Segment3_Byte;
    }
}
