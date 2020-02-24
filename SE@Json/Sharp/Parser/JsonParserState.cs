// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Json
{
    /// <summary>
    /// Defines valid JSON parser rules
    /// </summary>
    public enum JsonParserState : byte
    {
        Initial = 0,
        Resolver = 1,

        Object = 3,
        Array = 4,

        Property = 7,
        Value = 8,

        Separator = 11
    }
}
