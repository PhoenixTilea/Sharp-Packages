// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class StringExtension
    {
        /// <summary>
        /// Removes the first amount of leading occurrences of a set of characters specified in an array 
        /// from the current string
        /// </summary>
        /// <param name="count">The amount of characters to be tested from the beginning of the string</param>
        /// <param name="parameters">An array of Unicode characters to remove</param>
        /// <returns>The string that remains after all occurrences of characters in the trimChars parameter 
        /// are removed from the start of the current string. If trimChars is null or an empty array, white-space 
        /// characters are removed instead. If no characters can be trimmed from the current instance, the method 
        /// returns the current instance unchanged</returns>
        public static string TrimStart(this string str, int count, params char[] parameters)
        {
            int i = 0;
            for (; i < str.Length && i < count; i++)
            {
                bool end = true;
                foreach (char c in parameters)
                    if (str[i] == c)
                    {
                        end = false;
                        break;
                    }
                if (end) break;
            }
            return str.Substring(i, str.Length - i);
        }
    }
}
