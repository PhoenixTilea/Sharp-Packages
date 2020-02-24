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
        /// Creates a list of non-unique permutations from an input vector
        /// </summary>
        /// <returns>The list of non-unique permutations</returns>
        public static ICollection<ICollection<T>> GetCombinations<T>(this ICollection<T> items, int count, int start = 1)
        {
            return GetCombinations<T>(items as IEnumerable<T>, count, start);
        }
        /// <summary>
        /// Creates a list of non-unique permutations from an input vector
        /// </summary>
        /// <returns>The list of non-unique permutations</returns>
        public static ICollection<ICollection<T>> GetCombinations<T>(this ICollection<T> items, int start = 1)
        {
            return GetCombinations<T>(items, items.Count(), start);
        }
        /// <summary>
        /// Creates a list of non-unique permutations from an input vector
        /// </summary>
        /// <returns>The list of non-unique permutations</returns>
        public static ICollection<ICollection<T>> GetCombinations<T>(this T[] items, int start = 1)
        {
            List<ICollection<T>> result = new List<ICollection<T>>();
            if (start <= 1)
                foreach (T item in items)
                    result.Add(new T[] { item });

            for (int i = Math.Max(start, 2); i < items.Length; i++)
                result.AddRange(CombinationsHelper(items, i));

            if (start <= items.Length) result.Add(items.ToArray());
            return result;
        }
        /// <summary>
        /// Creates a list of non-unique permutations from an input vector
        /// </summary>
        /// <returns>The list of non-unique permutations</returns>
        public static ICollection<ICollection<T>> GetCombinations<T>(this IEnumerable<T> items, int count, int start = 1)
        {
            List<ICollection<T>> result = new List<ICollection<T>>();
            if (start <= 1)
                foreach (T item in items)
                    result.Add(new T[] { item });

            for (int i = Math.Max(start, 2); i < count; i++)
                result.AddRange(CombinationsHelper(items, i, count));

            if (start <= count) result.Add(items.ToArray());
            return result;
        }
        /// <summary>
        /// Creates a list of non-unique permutations from an input vector
        /// </summary>
        /// <returns>The list of non-unique permutations</returns>
        public static ICollection<ICollection<T>> GetCombinations<T>(this IEnumerable<T> items, int start = 1)
        {
            return GetCombinations<T>(items, items.Count(), start);
        }

        private static IEnumerable<int[]> CombinationsHelper(int i, int count)
        {
            int[] result = new int[i];
            Stack<int> stack = new Stack<int>(i);
            stack.Push(0);

            do
            {
                int index = stack.Count - 1;
                int value = stack.Pop();

                while (value < count)
                {
                    result[index++] = value++;
                    stack.Push(value);
                    if (index != i)
                        continue;

                    yield return result;
                    break;
                }
            }
            while (stack.Count > 0);
        }
        private static IEnumerable<T[]> CombinationsHelper<T>(T[] array, int i)
        {
            T[] result = new T[i];
            foreach (int[] j in CombinationsHelper(i, array.Length))
            {
                for (int n = 0; n < i; n++)
                    result[n] = array[j[n]];

                yield return result.Clone() as T[];
            }
        }
        private static IEnumerable<ICollection<T>> CombinationsHelper<T>(IEnumerable<T> items, int i, int count)
        {
            Dictionary<int, List<ICollection<T>>> cache = new Dictionary<int, List<ICollection<T>>>(count);
            List<ICollection<T>> result = new List<ICollection<T>>(count * 2);

            foreach (int[] j in CombinationsHelper(i, count))
            {
                List<T> item = new List<T>(i);
                result.Add(item);

                foreach (int n in j)
                {
                    List<ICollection<T>> helper; if (!cache.TryGetValue(n, out helper))
                    {
                        helper = new List<ICollection<T>>(count);
                        cache.Add(n, helper);
                    }
                    helper.Add(item);
                }
            }

            IEnumerator<T> enumerator = items.GetEnumerator();
            for (int n = 0; enumerator.MoveNext(); n++)
            {
                List<ICollection<T>> helper; if (!cache.TryGetValue(n, out helper))
                {
                    helper = new List<ICollection<T>>(count);
                    cache.Add(n, helper);
                }
                foreach (ICollection<T> collection in helper)
                    collection.Add(enumerator.Current);
            }

            return result;
        }
    }
}
