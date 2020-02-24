// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace SE
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Returns the next character from this Stream without processing it
        /// </summary>
        /// <returns>The character read from the stream or zero</returns>
        public static char Peek(this Stream s)
        {
            if (s.Eof()) return (char)0;

            char rt = (char)s.ReadByte();
            s.Seek(-1, SeekOrigin.Current);

            return rt;
        }
    }
}