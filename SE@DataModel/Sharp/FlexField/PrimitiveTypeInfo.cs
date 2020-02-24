// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SE;

namespace SE.DataModel
{
    /// <summary>
    /// An info struct to describe a primitive type container
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct PrimitiveTypeInfo
    {
        [FieldOffset(0)]
        public UInt16 Segment0_UInt32;

        [FieldOffset(0)]
        public bool IsArrayReference;

        [FieldOffset(2)]
        public Int16 Length;
    }
}
