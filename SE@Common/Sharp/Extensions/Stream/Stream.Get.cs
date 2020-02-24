// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SE
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Returns the next character from this Stream
        /// </summary>
        /// <returns>The character read from the stream or zero</returns>
        public static char Get(this Stream s)
        {
            if (s.Eof()) return (char)0;
            return (char)s.ReadByte();
        }

        /// <summary>
        /// Determines the streams encoding if possible
        /// </summary>
        /// <returns>An encoding object or default</returns>
        public static Encoding GetEncoding(this Stream s)
        {
            byte[] bom = new byte[4];
            int read = s.Read(bom, 0, 4);

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
            {
                s.Position -= 1;
                return Encoding.UTF7;
            }
            else if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
            {
                s.Position -= 1;
                return Encoding.UTF8;
            }
            else if (bom[0] == 0xff && bom[1] == 0xfe)
            {
                s.Position -= 2;
                return Encoding.Unicode;
            }
            else if (bom[0] == 0xfe && bom[1] == 0xff)
            {
                s.Position -= 2;
                return Encoding.BigEndianUnicode;
            }
            else if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            else s.Position -= read;

            return Encoding.ASCII;
        }
    }
}