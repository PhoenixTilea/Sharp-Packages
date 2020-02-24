// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace SE
{
    public static partial class AssemblyExtension
    {
        /// <summary>
        /// Returns a stream into an embedded data block if existing
        /// </summary>
        /// <param name="name">The name of a resource to find</param>
        /// <returns>A stream object accessing the embedded data block if existing, null otherwise</returns>
        public static Stream GetResourceStream(this Assembly assembly, string name)
        {
            Stream stream = null;
            foreach (string tmp in assembly.GetManifestResourceNames())
                if (tmp.EndsWith(name))
                {
                    stream = assembly.GetManifestResourceStream(tmp);
                    break;
                }

            return stream;
        }
    }
}
