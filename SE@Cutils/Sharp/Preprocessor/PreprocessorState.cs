// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Cpp
{
    /// <summary>
    /// Defines the states of the preprocessor
    /// </summary>
    public enum CppPreprocessorState : byte
    {
        Master = 0,

        Pragma,
        Ifndef,
        Ifdef,
        If,
        Include,
        Error,
        Endif,
        Else,
        Elif,
        Line,
        Undefine,
        Define,
        FunctionMacro,

        Failure
    }
}
