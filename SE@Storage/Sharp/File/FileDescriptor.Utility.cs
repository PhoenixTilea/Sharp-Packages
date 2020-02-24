// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE;

namespace SE.Storage
{
    public partial class FileDescriptor
    {
        /// <summary>
        /// Creates a file system entry descriptor object from a given string
        /// </summary>
        /// <param name="path">The string to make a descriptor object from</param>
        /// <returns>A file system entry descriptor object</returns>
        public static FileDescriptor Create(string path)
        {
            PathDescriptor location = new PathDescriptor(Path.GetDirectoryName(path));
            return new FileDescriptor(location, Path.GetFileName(path));
        }
        /// <summary>
        /// Creates a file system entry descriptor object from a given string
        /// </summary>
        /// <param name="location">A base location to append path to</param>
        /// <param name="path">The string to make a descriptor object from</param>
        /// <returns>A file system entry descriptor object</returns>
        public static FileDescriptor Create(PathDescriptor location, string path)
        {
            location = new PathDescriptor(location, Path.GetDirectoryName(path));
            return new FileDescriptor(location, Path.GetFileName(path));
        }
    }
}
