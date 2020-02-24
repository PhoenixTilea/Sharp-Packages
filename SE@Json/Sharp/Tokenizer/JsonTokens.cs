// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Json
{
    /// <summary>
    /// Defines valid JSON tokens
    /// </summary>
    public enum JsonToken : byte
    {
        Invalid = 0,
        Whitespace = 1,

        BeginObject = 5,
        EndObject = 6,

        BeginArray = 10,
        EndArray = 11,

        Property = 15,
        Colon = 16,
        Comma = 17,

        Null = 19,
        Boolean = 20,
        Integer = 21,
        Decimal = 22,
        String = 23
    }
}
