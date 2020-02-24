// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SE;

namespace SE.Reflection.Bridge
{
    /// <summary>
    /// Marks certain function inside an interface as runtime native binding
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MethodBindingAttribute : Attribute
    {
        public string EntryPoint { get; set; }

        public bool ExactSpelling { get; set; }

        public bool PreserveSig { get; set; }

        public bool SetLastError { get; set; }

        public CallingConvention CallingConvention { get; set; }

        public CharSet CharSet { get; set; }

        public bool BestFitMapping { get; set; }

        public bool ThrowOnUnmappableChar { get; set; }

        public MethodBindingAttribute()
        {
            this.PreserveSig = true;
            this.CallingConvention = CallingConvention.Cdecl;
            this.CharSet = System.Runtime.InteropServices.CharSet.Ansi;
            this.BestFitMapping = true;
        }
    }
}
