// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;

namespace SE
{
    public static partial class StringBuilderExtension
    {
        /// <summary>
        /// Compares the contents of a StringBuilder with the provided string
        /// for equality
        /// </summary>
        /// <param name="value">The value to compare the StringBuilder contents to</param>
        /// <returns>True if both character arrays are equal, false otherwise</returns>
        public static bool IsEqual(this StringBuilder sb, string value)
        {
            if (sb.Length != value.Length) return false;
            else
            {
                for (int i = 0; i < sb.Length; i++)
                    if (value[i] != sb[i])
                    {
                        return false;
                    }

                return true;
            }
        }
    }
}
