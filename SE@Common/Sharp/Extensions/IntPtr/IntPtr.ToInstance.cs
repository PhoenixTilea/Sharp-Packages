// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reflection
{
    public static partial class PointerExtension
    {
        /// <summary>
        /// Converts the provided managed pointer instance to the object it points to
        /// </summary>
        public static T ToInstance<T>(this IntPtr address)
        {
            if (reinterpreter.Instance == null)
                reinterpreter.Instance = new ObjectHandle();

            reinterpreter.Pointer.Value = address;
            T instance = (T)reinterpreter.Instance.Value;
            reinterpreter.Instance.Value = null;

            return instance;
        }
    }
}
