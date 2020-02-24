// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SE
{
    public partial class ResourceDescriptor
    {
        /// <summary>
        /// Returns a data blob read from this element's embedded content
        /// </summary>
        /// <returns>The data blob read</returns>
        public byte[] GetBytes()
        {
            using (Stream stream = Open())
            {
                byte[] result = new byte[stream.Length];
                stream.Read(result, 0, result.Length);

                return result;
            }
        }
        /// <summary>
        /// Returns this element's embedded content line by line
        /// </summary>
        /// <param name="encoding">An optional encoding used to read the embedded content</param>
        /// <returns>A list of lines</returns>
        public IEnumerable<string> GetLines(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            List<string> lines = new List<string>();

            using (Stream stream = Open())
            using (StreamReader sr = new StreamReader(stream, encoding))
                while (sr.Peek() > 0)
                    lines.Add(sr.ReadLine());

            return lines;
        }
        /// <summary>
        /// Returns a data blob read from this element's embedded content as text
        /// </summary>
        /// <param name="encoding">An optional encoding used to read the embedded content</param>
        /// <returns>The data blob read</returns>
        public string GetText(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            using (Stream stream = Open())
            using (StreamReader sr = new StreamReader(stream, encoding))
                return sr.ReadToEnd();
        }
    }
}
