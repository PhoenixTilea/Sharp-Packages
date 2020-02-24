// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE;

namespace SE.Storage
{
    /// <summary>
    /// A basic file system entry of defined type
    /// </summary>
    public abstract class FileSystemDescriptor
    {
        private readonly static string[] EmptyExtensions = new string[0];

        /// <summary>
        /// A considered to be unique ID addressed to this file system object
        /// </summary>
        public abstract UInt32 Id { get; }

        /// <summary>
        /// The type this file system entry is of
        /// </summary>
        public abstract FileSystemEntryType Type { get; }

        /// <summary>
        /// Returns true if this file system entry is of type File
        /// </summary>
        public bool IsFile
        {
            get { return (Type == FileSystemEntryType.File); }
        }
        /// <summary>
        /// Returns true if this file system entry is of type Directory
        /// </summary>
        public bool IsDirectory
        {
            get { return (Type == FileSystemEntryType.Directory); }
        }

        /// <summary>
        /// The name of this file system entry without extensions
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// The name of this file system entry including extensions
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// Replaces all locations in this file system entry's location to match the exact
        /// spelling on file system
        /// </summary>
        public abstract void Equalize();

        /// <summary>
        /// Gets last modification timestamp
        /// </summary>
        public virtual DateTime Timestamp
        {
            get { return DateTime.UtcNow; }
        }

        /// <summary>
        /// Returns a string description of the full qualified route
        /// to this file system entry
        /// </summary>
        /// <returns>The string location descriptor</returns>
        public abstract string GetAbsolutePath();
        /// <summary>
        /// Returns a string description of the relative route to a given
        /// file system entry
        /// </summary>
        /// <param name="root">The file system entry to start routing</param>
        /// <returns>The string location descriptor</returns>
        public abstract string GetRelativePath(FileSystemDescriptor root);

        /// <summary>
        /// Creates this file system entry at grouping location
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// Determines if this file system entry points to a valid ressource
        /// </summary>
        /// <returns>True if the located ressource exists, false otherwise</returns>
        public abstract bool Exists();

        /// <summary>
        /// Deletes this file system entry from physical storage
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// Obtains the exact spelling path from a file system location
        /// </summary>
        /// <param name="directory">The file system root location</param>
        /// <returns>The exact spelling path if possible</returns>
        public static string GetExactPath(DirectoryInfo directory)
        {
            DirectoryInfo parentDirInfo = directory.Parent;
            if (null == parentDirInfo) return directory.Name;
            else
            {
                parentDirInfo = new DirectoryInfo(directory.Parent.FullName);
                DirectoryInfo[] di = parentDirInfo.GetDirectories(directory.Name);

                if (di.Length == 0) return directory.Name;
                else return Path.Combine(GetExactPath(parentDirInfo), di[0].Name);
            }
        }
        /// <summary>
        /// Obtains the exact spelling path from a file system item
        /// </summary>
        /// <param name="directory">The file system item</param>
        /// <returns>The exact spelling path if possible</returns>
        public static string GetExactPath(FileInfo file)
        {
            DirectoryInfo dirInfo = file.Directory;
            return Path.Combine(GetExactPath(dirInfo), dirInfo.GetFiles(file.Name)[0].Name);
        }

        private static bool Find(string path, ref string target, IEnumerable<string> extensions)
        {
            string tmp = Path.Combine(path, target);
            if (!string.IsNullOrEmpty(Path.GetExtension(tmp)))
                tmp += ".";

            IEnumerator<string> extension = extensions.GetEnumerator();
            if (!extension.MoveNext())
                return Directory.Exists(target);
            else do
            {
                string ttmp = Path.ChangeExtension(tmp, extension.Current).Trim('.');
                if (File.Exists(ttmp))
                {
                    target = ttmp;
                    return true;
                }
            }
            while (extension.MoveNext());
            return false;
        }
        /// <summary>
        /// Searches any target in a set of paths provided and set using environment variables that 
        /// match any of the provided extensions. Target will be changed into a full qualified file
        /// system path if successfull
        /// </summary>
        /// <param name="target">The target file system object to find</param>
        /// <param name="paths">The list of paths to search</param>
        /// <param name="extensions">An optional list of file extensions to match</param>
        /// <returns>True if target was found, false otherwise</returns>
        public static bool Find(ref string target, IEnumerable<string> paths = null, IEnumerable<string> extensions = null)
        {
            if (extensions == null)
                extensions = EmptyExtensions;

            if (paths != null)
                foreach (string path in paths)
                    if (Find(path, ref target, extensions))
                        return true;
            
            foreach(string path in Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine).Split(';'))
                if (Find(path, ref target, extensions))
                    return true;

            return false;
        }
    }
}
