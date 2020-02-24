// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Bridge
{
    /// <summary>
    /// Marks certain interface inside a class as runtime native binding
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
    public class NativeBindingAttribute : Attribute
    {
        string assemblyName;
        /// <summary>
        /// The library name to initialize native bindings to
        /// </summary>
        public string AssemblyName 
        {
            get { return assemblyName; }
        }

        PlatformSwitch targetPlatform;
        /// <summary>
        /// The target platform architecture to initialize the binding for
        /// </summary>
        public PlatformSwitch TargetPlatform 
        {
            get { return targetPlatform; }
            set { targetPlatform = value; }
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="assemblyName">The library name to initialize interopt bindings to</param>
        public NativeBindingAttribute(string assemblyName)
        {
            this.assemblyName = assemblyName;
        }
    }
}
