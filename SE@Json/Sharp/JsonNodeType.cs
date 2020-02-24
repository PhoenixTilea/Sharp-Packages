// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Json
{
    /// <summary>
    /// Defines valid JSON DOM nodes
    /// </summary>
    public enum JsonNodeType : byte
    {
        Empty = 0,
        Object,
        Array,

        Bool,
        Integer,
        Decimal,
        String,

        Undefined
    }
}