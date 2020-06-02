// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reactive
{
    public static partial class EnumerableExtension
    {
        /// <summary>
        /// Converts the collection of items into a push-based notification stream
        /// </summary>
        public static IReactiveStream<T> ToStream<T>(this ICollection<T> items)
        {
            return new EnumerableStream<T>(items);
        }
        /// <summary>
        /// Converts the collection of items into a push-based notification stream
        /// </summary>
        public static IReactiveStream<T> ToStream<T>(this T[] items)
        {
            return new ArrayStream<T>(items);
        }
        /// <summary>
        /// Converts the collection of items into a push-based notification stream
        /// </summary>
        public static IReactiveStream<T> ToStream<T>(this IEnumerable<T> items)
        {
            return new EnumerableStream<T>(items);
        }
    }
}
