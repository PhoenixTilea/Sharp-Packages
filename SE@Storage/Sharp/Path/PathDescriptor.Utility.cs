// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SE;

namespace SE.Storage
{
    public partial class PathDescriptor
    {
        /// <summary>
        /// Returns current working location
        /// </summary>
        public static PathDescriptor Current
        {
            get { return new PathDescriptor(Environment.CurrentDirectory); }
        }

        /// <summary>
        /// Returns a string description of the relative route to a given
        /// file system entry
        /// </summary>
        /// <param name="root">The file system entry to start routing</param>
        /// <param name="location">An URI location to route to</param>
        /// <returns>The string location descriptor</returns>
        public static string GetRelativePath(FileSystemDescriptor root, Uri location)
        {
            Uri pathRoot; if (root.Type == FileSystemEntryType.File) pathRoot = new Uri((root as FileDescriptor).Location.GetAbsolutePath().TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            else pathRoot = new Uri((root as PathDescriptor).GetAbsolutePath().TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);

            return Uri.UnescapeDataString(pathRoot.MakeRelativeUri(new Uri(location.LocalPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar)).ToString()).Replace('/', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Returns a string description to a string descriptors top
        /// parent location
        /// </summary>
        /// <param name="path">The string descriptor to process</param>
        /// <returns>The string location descriptor</returns>
        public static string GetRootFolder(string path)
        {
            string root = path;
            while (!string.IsNullOrWhiteSpace(path))
            {
                root = path;
                path = Path.GetDirectoryName(path);
            }
            return root;
        }

        /// <summary>
        /// Tries to isolate the most common parent of a given set of locations
        /// </summary>
        /// <param name="paths">A set of paths to process</param>
        /// <returns>The most common parent location if successful, null otherwise</returns>
        public static string GetCommonParent(IEnumerable<FileSystemDescriptor> paths)
        {
            HashSet<string> result = new HashSet<string>(paths.Select(x => ((x.IsFile) ? (x as FileDescriptor).Location.GetAbsolutePath() : x.GetAbsolutePath())));
            if (result.Count > 1)
            {
                HashSet<string>.Enumerator enumerator = result.GetEnumerator();
                int maxLength = int.MaxValue;
                while (enumerator.MoveNext())
                {
                    int length = enumerator.Current.Length;
                    if (length < maxLength)
                    {
                        enumerator = result.GetEnumerator();
                        maxLength = length;
                    }
                    else if (length > maxLength)
                    {
                        string path = enumerator.Current;
                        result.Remove(path);

                        while (path.Length > maxLength)
                            path = Path.GetDirectoryName(path);

                        result.Add(path);
                        if (length < maxLength)
                            maxLength = length;

                        enumerator = result.GetEnumerator();
                    }
                }
            }
            if (result.Count > 1)
            {
                HashSet<string> cache = new HashSet<string>();
                do
                {
                    foreach (string path in result.Select(x => Path.GetDirectoryName(x)))
                        cache.Add(path);

                    result.Clear();

                    HashSet<string> tmp = cache;
                    cache = result;
                    result = tmp;
                }
                while (result.Count > 1);
            }
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Adds a final directory separator character to the end of the given 
        /// file system location
        /// </summary>
        /// <param name="path">The string descriptor to process</param>
        /// <returns>The escaped string location descriptor</returns>
        public static string Escape(string path)
        {
            if (path.LastOrDefault() != (Path.DirectorySeparatorChar))
                return Path.Combine(path, " ").TrimEnd();

            return path;
        }

        /// <summary>
        /// Unifies directory separator characters found in the string
        /// </summary>
        /// <param name="path">The string descriptor to process</param>
        /// <returns>The normalized string location descriptor</returns>
        public static string Normalize(string path)
        {
            return path.Replace('\\', '/');
        }
    }
}
