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
        /// Creates a list of all possible permutations from an input vector
        /// </summary>
        /// <returns>The list of permutations</returns>
        public static ICollection<ICollection<T>> GetPermutations<T>(this ICollection<T> items, int count)
        {
            List<ICollection<T>> result = new List<ICollection<T>>();
            PermutationHelper(items, ref result, count);

            return result;
        }
        /// <summary>
        /// Creates a list of all possible permutations from an input vector
        /// </summary>
        /// <returns>The list of permutations</returns>
        public static ICollection<ICollection<T>> GetPermutations<T>(this ICollection<T> items)
        {
            return GetPermutations<T>(items, items.Count());
        }
        /// <summary>
        /// Creates a list of all possible permutations from an input vector
        /// </summary>
        /// <returns>The list of permutations</returns>
        public static ICollection<ICollection<T>> GetPermutations<T>(this T[] items)
        {
            List<ICollection<T>> result = new List<ICollection<T>>();
            PermutationHelper(items, ref result);

            return result;
        }
        /// <summary>
        /// Creates a list of all possible permutations from an input vector
        /// </summary>
        /// <returns>The list of permutations</returns>
        public static ICollection<ICollection<T>> GetPermutations<T>(this IEnumerable<T> items, int count)
        {
            List<ICollection<T>> result = new List<ICollection<T>>();
            PermutationHelper(items, ref result, count);

            return result;
        }
        /// <summary>
        /// Creates a list of all possible permutations from an input vector
        /// </summary>
        /// <returns>The list of permutations</returns>
        public static ICollection<ICollection<T>> GetPermutations<T>(this IEnumerable<T> items)
        {
            return GetPermutations<T>(items, items.Count());
        }

        private static void PermutationHelper<T>(T[] items, ref List<ICollection<T>> result, int i = 0)
        {
            if (i >= items.Length) result.Add(items.Clone() as T[]);
            else
            {
                PermutationHelper(items, ref result, i + 1);
                for (int j = i + 1; j < items.Length; j++)
                {
                    items.Swap(i, j);
                    PermutationHelper(items, ref result, i + 1);
                    items.Swap(i, j);
                }
            }
        }
        private static void PermutationHelper<T>(IEnumerable<T> items, ref List<ICollection<T>> result, int count, int i = 0)
        {
            if (i >= count) result.Add(items.ToArray());
            else
            {
                PermutationHelper(items, ref result, count, i + 1);
                for (int j = i + 1; j < count; j++)
                    PermutationHelper(items.Swap(i, j).ToArray(), ref result, count, i + 1);
            }
        }
    }
}
