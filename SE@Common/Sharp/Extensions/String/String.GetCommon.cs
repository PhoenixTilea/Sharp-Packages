// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class StringExtension
    {
        /// <summary>
        /// Creates the most common string along a list of strings
        /// </summary>
        /// <param name="items">A list of strings</param>
        /// <returns>The most common string or the first element</returns>
        public static string GetCommon(this IEnumerable<string> items)
        {
            IEnumerator<string> it = items.GetEnumerator();
            if (it.MoveNext())
            {
                string orig = it.Current;
                string ident = orig.ToLowerInvariant();

                int k = ident.Length;
                while (it.MoveNext())
                {
                    string local = it.Current.ToLowerInvariant();
                    k = Math.Min(k, local.Length);
                    for (int j = 0; j < k; j++)
                        if (local[j] != ident[j])
                        {
                            k = j;
                            break;
                        }
                }
                return orig.Substring(0, k);
            }
            else return string.Empty;
        }

        /// <summary>
        /// Creates the statistical most common string along a list of strings
        /// </summary>
        /// <param name="items">A list of strings</param>
        /// <returns>The most common string or the first element</returns>
        public static string GetMostCommon(this IEnumerable<string> items)
        {
            IEnumerator<string> it = items.GetEnumerator();
            if (it.MoveNext())
            {
                string orig = it.Current;
                string ident = orig.ToLowerInvariant();

                Dictionary<int, int> matches = new Dictionary<int, int>();

                int k = ident.Length;
                while (it.MoveNext())
                {
                    string local = it.Current.ToLowerInvariant();
                    k = Math.Min(k, local.Length);
                    for (int j = 0; j < k; j++)
                        if (local[j] != ident[j])
                        {
                            k = j;
                            break;
                        }

                    if (matches.ContainsKey(k)) matches[k]++;
                    else matches.Add(k, 1);
                    k = ident.Length;
                }
                if (matches.Count > 1)
                {
                    KeyValuePair<int, int> max = new KeyValuePair<int, int>();
                    foreach (KeyValuePair<int, int> match in matches)
                        if (max.Value < match.Value)
                            max = match;

                    return orig.Substring(0, max.Key);
                }
                else return orig.Substring(0, k);
            }
            else return string.Empty;
        }
    }
}
