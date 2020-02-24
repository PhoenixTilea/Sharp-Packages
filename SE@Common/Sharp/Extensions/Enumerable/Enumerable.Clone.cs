// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;

namespace SE
{
    public static partial class EnumerableExtension
    {
        /// <summary>
        /// Creates a deep copy of this set of items
        /// </summary>
        /// <returns>The instance copy of items</returns>
        public static ICollection<T> Clone<T>(this ICollection<T> items)
        {
            return items.Select(item => (T)item).ToList();
        }
        /// <summary>
        /// Creates a deep copy of this set of items
        /// </summary>
        /// <returns>The instance copy of items</returns>
        public static T[] Clone<T>(this T[] items)
        {
            return items.Select(item => (T)item).ToArray();
        }
        /// <summary>
        /// Creates a deep copy of this set of items
        /// </summary>
        /// <returns>The instance copy of items</returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> items)
        {
            return items.Select(item => (T)item);
        }
    }
}
