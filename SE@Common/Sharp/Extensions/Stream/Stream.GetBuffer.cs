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
        /// Wraps this stream into a StreamBuffer instance of defined sice
        /// </summary>
        public static StreamBuffer GetBuffer(this Stream s, int size)
        {
            return new StreamBuffer(s, size);
        }
    }
}