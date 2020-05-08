// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Config
{
    /// <summary>
    /// A result context used by configuration classes
    /// </summary>
    public struct AutoConfigResult
    {
        /// <summary>
        /// A list of errors occured during congigure
        /// </summary>
        public List<string> Errors;
        /// <summary>
        /// A list of attributes that couldn't be matched to the
        /// configuration target
        /// </summary>
        public Dictionary<string, object[]> Unknown;

        /// <summary>
        /// The ammount of values that have been attached to fields and properties
        /// by default
        /// </summary>
        public int ParsedDefault;
        /// <summary>
        /// The ammount of values that haven been attached to fields and properties
        /// </summary>
        public int Parsed;

        /// <summary>
        /// Creates a new instance of AutoConfigResult
        /// </summary>
        public static AutoConfigResult Create()
        {
            AutoConfigResult result = new AutoConfigResult();
            result.Unknown = new Dictionary<string, object[]>();
            result.Errors = new List<string>();

            return result;
        }
    }
}
