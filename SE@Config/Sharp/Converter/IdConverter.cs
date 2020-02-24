// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Config
{
    /// <summary>
    /// Converts incoming command line arguments into an UInt32 ID
    /// </summary>
    public class IdConverter : ITypeConverter
    {
        public bool TryParseValue(Type memberType, string value, out object result)
        {
            result = value.Fnv32();
            return true;
        }
    }
}
