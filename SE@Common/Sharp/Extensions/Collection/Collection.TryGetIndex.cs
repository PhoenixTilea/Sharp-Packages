// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;

namespace SE
{
    public static partial class CollectionExtension
    {
        /// <summary>
        /// Tries to find the index of an defined predicate
        /// </summary>
        /// <param name="predicate">A predicate to determine success or failure</param>
        /// <param name="index">The index found</param>
        /// <returns>True if the defined predicate matched an item, false otherwise</returns>
        public static bool TryGetIndex<T>(this IList<T> items, Predicate<T> predicate , out int index)
        {
            for (index = 0; index < items.Count && !predicate(items[index]); index++)
                ;

            return (index < items.Count);
        }
        /// <summary>
        /// Tries to find the index of the defined item in this collection
        /// </summary>
        /// <param name="item">The defined item to find an index for</param>
        /// <param name="index">The index found</param>
        /// <returns>True if the defined item is contained in this collection, false otherwise</returns>
        public static bool TryGetIndex<T>(this IList<T> items, T item, out int index)
        {
            index = items.IndexOf(item);
            return (index >= 0);
        }
    }
}