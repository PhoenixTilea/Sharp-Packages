// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Cpp
{
    /// <summary>
    /// Defines the states of the tokenizer
    /// </summary>
    public enum CppTokenizerState : byte
    {
        /// <summary>
        /// C++ preprocessing-token
        /// https://www.nongnu.org/hcb/#preprocessing-token
        /// </summary>
        Initial = 0,

        /// <summary>
        /// The first token of a line
        /// </summary>
        AfterWhitespace,

        /// <summary>
        /// C++ #include header-name
        /// https://www.nongnu.org/hcb/#header-name
        /// </summary>
        Include,
    }
}