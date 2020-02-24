// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SE;

namespace SE.Storage
{
    public partial class FileDescriptor
    {
        /// <summary>
        /// Returns a data blob read from this element's physical storage
        /// </summary>
        /// <returns>The data blob read</returns>
        public byte[] GetBytes()
        {
            return File.ReadAllBytes(GetAbsolutePath());
        }
        /// <summary>
        /// Returns this element's physical storage line by line
        /// </summary>
        /// <param name="encoding">An optional encoding used to read the physical storage</param>
        /// <returns>A list of lines</returns>
        public IEnumerable<string> GetLines(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            return File.ReadLines(GetAbsolutePath(), encoding);
        }
        /// <summary>
        /// Returns a data blob read from this element's physical storage as text
        /// </summary>
        /// <param name="encoding">An optional encoding used to read the physical storage</param>
        /// <returns>The data blob read</returns>
        public string GetText(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            return File.ReadAllText(GetAbsolutePath(), encoding);
        }

        /// <summary>
        /// Writes a data blob to this element's physical storage
        /// </summary>
        /// <param name="value">The data blob to write</param>
        public void SetBytes(byte[] value)
        {
            File.WriteAllBytes(GetAbsolutePath(), value);
        }
        /// <summary>
        /// Writes to this element's physical storage line by line
        /// </summary>
        /// <param name="value">A list of lines to write to the storage</param>
        /// <param name="encoding">An optional encoding used to write the physical storage</param>
        public void SetLines(IEnumerable<string> value, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            File.WriteAllLines(GetAbsolutePath(), value, encoding);
        }
        /// <summary>
        /// Writes a data blob as text to this element's physical storage
        /// </summary>
        /// <param name="value">The data blob to write</param>
        /// <param name="encoding">An optional encoding used to write the physical storage</param>
        public void SetText(string value, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            File.WriteAllText(GetAbsolutePath(), value, encoding);
        }
    }
}
