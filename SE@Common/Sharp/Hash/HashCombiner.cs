// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;

namespace SE
{
    /// <summary>
    /// Provides a combinator for different .Net hash codes
    /// </summary>
    public struct HashCombiner
    {
        const Int64 HashCombinerBias = 0x1505L;
        Int64 combinedHash64;

        /// <summary>
        /// The final hash
        /// </summary>
        public int Value
        {
            get { return combinedHash64.GetHashCode(); }
        }

        /// <summary>
        /// Creates a new combiner instance
        /// </summary>
        public HashCombiner(Int64 offsetBias)
        {
            this.combinedHash64 = offsetBias;
        }

        /// <summary>
        /// Adds a set of hash codes to the final result
        /// </summary>
        public HashCombiner Add(IEnumerable e)
        {
            if (e == null)
            {
                Add(0);
            }
            else
            {
                int count = 0;
                foreach (object o in e)
                {
                    Add(o);
                    count++;
                }
                Add(count);
            }
            return this;
        }
        /// <summary>
        /// Adds a single hash code to the final result
        /// </summary>
        public HashCombiner Add(int i)
        {
            combinedHash64 = ((combinedHash64 << 5) + combinedHash64) ^ i;
            return this;
        }
        /// <summary>
        /// Adds an object's hash code to the final result
        /// </summary>
        public HashCombiner Add(object o)
        {
            int hashCode = (o != null) ? o.GetHashCode() : 0;
            Add(hashCode);
            return this;
        }

        /// <summary>
        /// Initializes a new instance of HashCombiner
        /// </summary>
        /// <returns></returns>
        public static HashCombiner Initialize()
        {
            return new HashCombiner(HashCombinerBias);
        }
    }
}