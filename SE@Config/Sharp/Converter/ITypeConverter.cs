// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Config
{
    /// <summary>
    /// Provides access to custom property conversion of command line arguments
    /// </summary>
    public interface ITypeConverter
    {
        /// <summary>
        /// Provides a conditional converter function incoming data is passed to if
        /// detected while matching arguments to properties and fields
        /// </summary>
        /// <returns>True if data was added to <paramref name="result"/>, false otherwise</returns>
        bool TryParseValue(Type memberType, string value, out object result);
    }
}
