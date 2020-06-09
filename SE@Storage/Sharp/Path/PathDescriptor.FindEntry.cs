// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE;

namespace SE.Storage
{
    public partial class PathDescriptor
    {
        /// <summary>
        /// Returns a list of file system entries that match a given filter
        /// </summary>
        /// <param name="directory">The location to start lookup for entries</param>
        /// <param name="filter">A filter that will be applied to the lookup</param>
        /// <param name="option">An option flag to define the kind of entries to seek for</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public static void FindEntries(PathDescriptor directory, Filter filter, PathEntryOption option, PathSeekOptions options, ICollection<FileSystemDescriptor> items)
        {
            if (directory.Exists())
            {
                DirectoryInfo dir = new DirectoryInfo(directory.GetAbsolutePath());
                if ((option & PathEntryOption.Directory) == PathEntryOption.Directory) FindDirectories(filter, dir, "", (options & PathSeekOptions.Backward) == PathSeekOptions.Backward, (options & PathSeekOptions.RootLevel) != PathSeekOptions.RootLevel, items);
                if ((option & PathEntryOption.File) == PathEntryOption.File) FindFiles(filter, dir, "", (options & PathSeekOptions.Backward) == PathSeekOptions.Backward, (options & PathSeekOptions.RootLevel) != PathSeekOptions.RootLevel, items);
            }
        }
        /// <summary>
        /// Returns a list of file system entries that match a given pattern
        /// </summary>
        /// <param name="directory">The location to start lookup for entries</param>
        /// <param name="pattern">A pattern that will be translated into a filter object</param>
        /// <param name="option">An option flag to define the kind of entries to seek for</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public static void FindEntries(PathDescriptor directory, string pattern, PathEntryOption option, PathSeekOptions direction, ICollection<FileSystemDescriptor> items)
        {
            Filter filter = new Filter();

            pattern = PathDescriptor.Normalize(pattern);
            if (pattern.StartsWith("/"))
                pattern = pattern.Substring(1);
            else if (pattern.StartsWith("./"))
                pattern = ".." + pattern;

            FilterToken last = null;
            string[] tiles = pattern.Split('/');
            foreach (string tile in tiles)
            {
                FilterToken current = null;
                if (last != null)
                    current = last.GetChild(tile);
                if (current == null)
                {
                    if (last != null) current = filter.Add(last, tile);
                    else current = filter.Add(tile);
                }
                last = current;
            }

            FindEntries(directory, filter, option, direction, items);
        }
        /// <summary>
        /// Returns a list of file system entries that match a given filter
        /// </summary>
        /// <param name="directory">The location to start lookup for entries</param>
        /// <param name="filter">A filter that will be applied to the lookup</param>
        /// <param name="option">An option flag to define the kind of entries to seek for</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public static List<FileSystemDescriptor> FindEntries(PathDescriptor directory, Filter filter, PathEntryOption option, PathSeekOptions direction)
        {
            List<FileSystemDescriptor> items = new List<FileSystemDescriptor>();
            FindEntries(directory, filter, option, direction, items);

            return items;
        }
        /// <summary>
        /// Returns a list of file system entries that match a given pattern
        /// </summary>
        /// <param name="directory">The location to start lookup for entries</param>
        /// <param name="pattern">A pattern that will be translated into a filter object</param>
        /// <param name="option">An option flag to define the kind of entries to seek for</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public static List<FileSystemDescriptor> FindEntries(PathDescriptor directory, string pattern, PathEntryOption option, PathSeekOptions direction)
        {
            List<FileSystemDescriptor> items = new List<FileSystemDescriptor>();
            FindEntries(directory, pattern, option, direction, items);

            return items;
        }

        private static FilterToken GetExactMatch(FilterToken currentToken, string[] tiles, ref int tileIndex)
        {
            FilterToken match = null;
            FilterToken current = currentToken;
            while (current != null)
            {
                bool isMatch = false;
                int index = tileIndex;
                if (current.Child != null)
                {
                    FilterToken token = GetExactMatch(current.Child, tiles, ref index);
                    if (token != null)
                    {
                        index++;
                        match = token;
                        isMatch = true;
                    }
                    else
                    {
                        while (current.Next != null && current.Type == FilterType.And)
                            current = current.Next;

                        if (current != null)
                            current = current.Next;

                        match = null;
                        continue;
                    }
                }

                if (index >= tiles.Length)
                {
                    if (isMatch)
                        isMatch = (current.Pattern == "...");
                }
                else
                {
                    bool tmp = current.IsMatch(tiles[index]);
                    isMatch = (tmp && !current.Exclude || !tmp && current.Exclude);
                }

                if (isMatch)
                {
                    if (match == null)
                        match = current;

                    tileIndex = index;

                    if (current.Type != FilterType.And)
                        break;
                }
                else
                {
                    while (current.Next != null && current.Type == FilterType.And)
                        current = current.Next;

                    match = null;
                }
                if (current != null)
                    current = current.Next;
            }
            return match;
        }
        private static bool IsExactMatch(Filter filter, string[] tiles)
        {
            Array.Reverse(tiles);

            int i = 0;
            FilterToken token = GetExactMatch(filter.Root, tiles, ref i);
            return (token != null);
        }
    }
}
