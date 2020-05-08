// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    public static partial class CollectionExtension
    {
        /// <summary>
        /// Converts the value of objects to strings based on the formats specified
        /// and inserts them into a string template to this collection
        /// </summary>
        public static void AddFormatted(this ICollection<string> items, IFormatProvider provider, String format, params object[] args)
        {
            items.Add(string.Format(provider, format, args));
        }
        /// <summary>
        /// Converts the value of objects to strings based on the formats specified
        /// and inserts them into a string template to this collection
        /// </summary>
        public static void AddFormatted(this ICollection<string> items, String format, params object[] args)
        {
            items.Add(string.Format(format, args));
        }
        /// <summary>
        /// Converts the value of objects to strings based on the formats specified
        /// and inserts them into a string template to this collection
        /// </summary>
        public static void AddFormatted(this ICollection<string> items, String format, object arg0, object arg1, object arg2)
        {
            items.Add(string.Format(format, arg0, arg1, arg2));
        }
        /// <summary>
        /// Converts the value of objects to strings based on the formats specified
        /// and inserts them into a string template to this collection
        /// </summary>
        public static void AddFormatted(this ICollection<string> items, String format, object arg0)
        {
            items.Add(string.Format(format, arg0));
        }
        /// <summary>
        /// Converts the value of objects to strings based on the formats specified
        /// and inserts them into a string template to this collection
        /// </summary>
        public static void AddFormatted(this ICollection<string> items, String format, object arg0, object arg1)
        {
            items.Add(string.Format(format, arg0, arg1));
        }
    }
}