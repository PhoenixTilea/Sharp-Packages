// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reflection
{
    public static partial class PointerExtension
    {
        /// <summary>
        /// Converts this object into a managed pointer address to local heap
        /// </summary>
        public static IntPtr ToPointer(this object obj)
        {
            if (reinterpreter.Instance == null)
                reinterpreter.Instance = new ObjectHandle();

            reinterpreter.Instance.Value = obj;
            IntPtr ptr = reinterpreter.Pointer.Value;
            reinterpreter.Instance.Value = null;

            return ptr;
        }
    }
}
