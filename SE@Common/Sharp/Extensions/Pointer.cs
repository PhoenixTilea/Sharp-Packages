// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SE.Reflection
{
    public static partial class PointerExtension
    {
        [StructLayout(LayoutKind.Explicit)]
        struct ObjectReinterpreter
        {
            [FieldOffset(0)]
            public ObjectHandle Instance;
            [FieldOffset(0)]
            public ObjectPointer Pointer;
        }
        class ObjectPointer
        {
            public IntPtr Value;
        }
        class ObjectHandle
        {
            public object Value;
        }

        [ThreadStatic]
        private static ObjectReinterpreter reinterpreter;
    }
}
