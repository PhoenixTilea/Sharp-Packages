// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// Base34 Text Encoding
    /// </summary>
    public static class Base34
    {
        const string digits = "123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";

        /// <summary>
        /// Encodes a block of data into a base34 string
        /// </summary>
        /// <param name="data">A block of data to be encoded</param>
        /// <returns>The coresponding base34 type string</returns>
        public static string Encode(byte[] data)
        {
            if (data == null)
                return "1";

            UInt32 start = 0; for (; start < data.Length && data[start] == 0; start++)
                ;

            List<char> result = new List<char>();
            for (UInt32 i = start; i < data.Length; i++)
            {
                UInt32 intData = data[i];
                for (UInt32 j = 0; intData != 0 || j < result.Count; j++)
                {
                    intData += 256 * ((j < result.Count) ? (UInt32)result[result.Count - (int)j - 1] : 0);

                    if (j >= result.Count) result.Insert(0, (char)(intData % 34));
                    else result[result.Count - (int)j - 1] = (char)(intData % 34);

                    intData /= 34;
                }
            }

            for (int i = 0; i < result.Count; i++)
                result[i] = digits[result[i]];
            for (int i = 0; i < start; i++)
                result.Insert(0, '1');

            return new string(result.ToArray());
        }
        /// <summary>
        /// Determines if the provided string is a valid base34 encoded stirng
        /// </summary>
        /// <param name="base34">A string to be verified</param>
        /// <returns>True if the string could be decoded, false otherwise</returns>
        public static bool Check(string base34)
        {
            int start = 0; for (; start < base34.Length && base34[start] == '1'; start++)
                ;

            for (int i = start; i < base34.Length; i++)
                if (digits.IndexOf(base34[i]) < 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Decodes the base34 string into the coresponding block of data
        /// </summary>
        /// <param name="base34">A string to be decoded</param>
        /// <returns>A block of data that contains the decoded string</returns>
        public static byte[] Decode(string base34)
        {
            int start = 0; for (; start < base34.Length && base34[start] == '1'; start++)
                ;

            List<byte> result = new List<byte>();
            for (int i = start; i < base34.Length; i++)
            {
                int intData = -1;
                for (int j = 0; j < 34; j++)
                    if (digits[j] == base34[i])
                    {
                        intData = j;
                        break;
                    }

                for (int j = 0; intData != 0 || j < result.Count; j++)
                {
                    intData += 34 * ((j < result.Count) ? result[result.Count - j - 1] : 0);

                    if (j >= result.Count) result.Insert(0, (byte)(intData % 256));
                    else result[result.Count - j - 1] = (byte)(intData % 256);

                    intData /= 256;
                }
            }

            for (int i = 0; i < start; i++)
                result.Insert(0, 0);

            return result.ToArray();
        }
    }
}
