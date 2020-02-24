// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SE;

namespace SE.Storage
{
    /// <summary>
    /// A file system path entry element
    /// </summary>
    public partial class PathDescriptor : FileSystemDescriptor
    {
        public readonly static PathDescriptor Empty = new PathDescriptor();

        public override UInt32 Id
        {
            get { return GetAbsolutePath().Fnv32(); }
        }

        public override FileSystemEntryType Type
        {
            get { return FileSystemEntryType.Directory; }
        }

        protected Uri path;
        internal Uri Uri
        {
            get { return path; }
        }

        UInt16 order;
        /// <summary>
        /// Determines this location's order
        /// </summary>
        public UInt16 Order
        {
            get { return order; }
        }

        public override string Name
        {
            get { return Path.GetFileName(GetAbsolutePath()); }
        }
        public override string FullName
        {
            get { return GetAbsolutePath(); }
        }

        /// <summary>
        /// This location's parent directory
        /// </summary>
        public PathDescriptor Parent
        {
            get { return new PathDescriptor(Path.GetFullPath(Path.GetDirectoryName(FullName))); }
        }

        public static bool operator ==(PathDescriptor left, PathDescriptor right)
        {
            return (left as object == right as object ||
                   (left as object != null && right as object != null &&
                    left.order == right.order &&
                    left.path.AbsolutePath == right.path.AbsolutePath));
        }
        public static bool operator !=(PathDescriptor left, PathDescriptor right)
        {
            return (left as object != right as object &&
                   (left as object == null || right as object == null ||
                    left.order != right.order ||
                    left.path.AbsolutePath != right.path.AbsolutePath));
        }

        private PathDescriptor()
        { }

        /// <summary>
        /// Creates a new location from the given path
        /// </summary>
        /// <param name="path">A valid URI location</param>
        public PathDescriptor(Uri path)
        {
            this.path = path;
            this.order = (UInt16)path.LocalPath.Count(x => x == Path.DirectorySeparatorChar);
        }
        /// <summary>
        /// Creates a new location from the given path
        /// </summary>
        /// <param name="path">A string description of a location</param>
        public PathDescriptor(string path)
            : this(new Uri(Path.GetFullPath(path).Trim(Path.DirectorySeparatorChar)))
        { }
        /// <summary>
        /// Creates a new location from the given path
        /// </summary>
        /// <param name="path">A string description of a location</param>
        public PathDescriptor(string name, params object[] args)
            : this(string.Format(name, args))
        { }
        /// <summary>
        /// Creates a new location from the given path grouped by the
        /// given root location
        /// </summary>
        /// <param name="root">The root location to group this location into</param>
        /// <param name="path">A string description of a location</param>
        public PathDescriptor(PathDescriptor root, string path)
            : this(root.GetAbsolutePath(path))
        { }
        /// <summary>
        /// Creates a new location from the given path grouped by the
        /// given root location
        /// </summary>
        /// <param name="root">The root location to group this location into</param>
        /// <param name="path">A string description of a location</param>
        public PathDescriptor(PathDescriptor root, string name, params object[] args)
            : this(root, string.Format(name, args))
        { }
        /// <summary>
        /// Creates a copy of an existing file system location
        /// </summary>
        public PathDescriptor(PathDescriptor source)
            : this(source.Uri)
        { }

        /// <summary>
        /// Extends this file system location with a list of directories
        /// </summary>
        /// <param name="directories">A list of directories added to the result</param>
        /// <returns>The extended file system location</returns>
        public PathDescriptor Combine(params string[] directories)
        {
            return new PathDescriptor(Path.Combine(GetAbsolutePath(), Path.Combine(directories)));
        }

        public override DateTime Timestamp
        {
            get { return new DirectoryInfo(GetAbsolutePath()).LastWriteTimeUtc; }
        }

        public override string GetAbsolutePath()
        {
            if (path == null) return string.Empty;
            else return path.LocalPath;
        }

        /// <summary>
        /// Returns a string description of this location concatenated by an
        /// additional file or folder name
        /// </summary>
        /// <param name="name">A file or fodler name to concat to this location</param>
        /// <returns>The string location descriptor</returns>
        public string GetAbsolutePath(string name)
        {
            return Path.Combine(GetAbsolutePath(), name);
        }
        /// <summary>
        /// Returns a string description of this location concatenated by an
        /// additional file or folder name
        /// </summary>
        /// <param name="name">A file or fodler name to concat to this location</param>
        /// <returns>The string location descriptor</returns>
        public string GetAbsolutePath(string name, params object[] args)
        {
            return Path.Combine(GetAbsolutePath(), string.Format(name, args));
        }

        public override string GetRelativePath(FileSystemDescriptor root)
        {
            return GetRelativePath(root, path);
        }

        public override void Create()
        {
            Directory.CreateDirectory(GetAbsolutePath());
        }
        /// <summary>
        /// Creates this location as new file system entry keeping
        /// the hidden attribute
        /// </summary>
        public void CreateHidden()
        {
            if(!Name.StartsWith("."))
                path = new Uri(Path.GetFullPath(Path.Combine(Parent.GetAbsolutePath(), string.Format(".{0}", Name))).Trim(Path.DirectorySeparatorChar));

            DirectoryInfo di = Directory.CreateDirectory(GetAbsolutePath());
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        }

        public override bool Exists()
        {
            return Directory.Exists(GetAbsolutePath());
        }

        /// <summary>
        /// Determines if this location logically contains the given file system entry
        /// </summary>
        /// <param name="subFolder">The file system entry to test for</param>
        /// <returns>True if the passed file system entry is grouped by this location, false otherwise</returns>
        public bool Contains(PathDescriptor subFolder)
        {
            return (order <= subFolder.order && subFolder.GetAbsolutePath().StartsWith(GetAbsolutePath(), StringComparison.Ordinal));
        }

        /// <summary>
        /// Returns a list file system entries with the File type directly grouped
        /// by this location
        /// </summary>
        /// <returns>The list of found file system entries</returns>
        public List<FileSystemDescriptor> GetFiles()
        {
            return GetEntires(this, PathEntryOption.File);
        }

        /// <summary>
        /// Returns a list file system entries with the Directory type directly grouped
        /// by this location
        /// </summary>
        /// <returns>The list of found file system entries</returns>
        public List<FileSystemDescriptor> GetDirectories()
        {
            return GetEntires(this, PathEntryOption.Directory);
        }

        /// <summary>
        /// Returns a short string location descriptor from a given location
        /// that is grouped by this location
        /// </summary>
        /// <param name="subFolder">The location to rebase to this location</param>
        /// <returns>A short string locationd escriptor</returns>
        public string Rebase(PathDescriptor subFolder)
        {
            return subFolder.GetAbsolutePath().Substring(GetAbsolutePath().Length + 1);
        }
        /// <summary>
        /// Returns a short string location descriptor from a given string location descriptor
        /// that is grouped by this location
        /// </summary>
        /// <param name="subFolder">The location to rebase to this location</param>
        /// <returns>A short string location descriptor</returns>
        public string Rebase(string subFolder)
        {
            return Rebase(new PathDescriptor(subFolder));
        }

        public override void Equalize()
        {
            DirectoryInfo di = new DirectoryInfo(GetAbsolutePath());
            path = new Uri(Path.GetFullPath(FileSystemDescriptor.GetExactPath(di)));
        }

        /// <summary>
        /// Removes all elements parented by this location
        /// </summary>
        public void Clear()
        {
            foreach (PathDescriptor subFolder in GetDirectories())
                subFolder.Delete();

            foreach (FileDescriptor file in GetFiles())
                file.Delete();
        }

        public override void Delete()
        {
            DirectoryInfo di = new DirectoryInfo(GetAbsolutePath());
            di.Attributes = FileAttributes.Normal;
            di.Delete(true);
        }

        public override int GetHashCode()
        {
            return path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (this == obj as PathDescriptor);
        }

        /// <summary>
        /// Returns a list of file system entries equal to the specified option flag
        /// grouped by given location
        /// </summary>
        /// <param name="directory">The location to seek file system entries for</param>
        /// <param name="option">An option flag to define the kind of entries to seek for</param>
        /// <returns>The resulting list of file system entries</returns>
        public static List<FileSystemDescriptor> GetEntires(PathDescriptor directory, PathEntryOption option)
        {
            List<FileSystemDescriptor> items = new List<FileSystemDescriptor>();
            if (directory.Exists())
            {
                DirectoryInfo dir = new DirectoryInfo(directory.GetAbsolutePath());
                if ((option & PathEntryOption.Directory) == PathEntryOption.Directory)
                {
                    foreach (DirectoryInfo di in dir.EnumerateDirectories())
                        items.Add(new PathDescriptor(di.FullName));
                }
                if ((option & PathEntryOption.File) == PathEntryOption.File)
                {
                    foreach (FileInfo fi in dir.EnumerateFiles())
                        items.Add(new FileDescriptor(new PathDescriptor(fi.DirectoryName), fi.Name));
                }
            }
            return items;
        }

        public override string ToString()
        {
            return GetAbsolutePath();
        }
    }
}
