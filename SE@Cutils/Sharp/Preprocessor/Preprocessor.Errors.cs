// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Cpp
{
    public partial class Preprocessor
    {
        /// <summary>
        /// A collection of error messages that are used by the preprocessor
        /// </summary>
        protected static class ErrorMessages
        {
            public const string InvalidDirective = "{0} ({1}): Invalid preprocessor directive '{2}'";

            public const string UnterminatedDirective = "{0} ({1}): Unterminated conditional directive";
            public const string UnexpectedElifConditional = "{0} ({1}): #elif without #if";
            public const string UnexpectedElseConditional = "{0} ({1}): #else without #if";
            public const string UnexpectedEndConditional = "{0} ({1}): #endif without #if";

            public const string PreprocessorError = "{0} ({1}): {2}";
            public const string InvalidLineNumber = "{0} ({1}): #line directive requires a positive integer argument";
            public const string InvalidFileName = "{0} ({1}): Invalid filename for {2} directive";
            public const string UnterminatedFileName = "{0} ({1}): Unterminated filename for {2} directive";

            public const string FileNotFound = "{0} ({1}): '{2}' file not found";

            public const string InvalidMacroName = "{0} ({1}): Macro name must be an identifier";
            public const string MacroRedefinition = "{0} ({1}): Macro '{2}' redefinition";
            public const string MacroUndefined = "{0} ({1}): Macro '{2}' is undefined";

            public const string InvalidParameter = "{0} ({1}): Invalid token in macro parameter list";
            public const string MissingParameterListSeparator = "{0} ({1}): Expected comma in macro parameter list";
            public const string MissingParameterListTerminator = "{0} ({1}): Missing ')' in macro parameter list";
            public const string BogusFunctionMacro = "{0} ({1}): Unterminated function-like macro invocation";

            public const string ParameterMismatch = "{0} ({1}): Invalid number of arguments for macro '{2}'";
            public const string ParameterTerminatorMissing = "{0} ({1}): Unterminated function-like macro invocation";

            public const string InvalidPastingOperatorStart = "{0} ({1}): '##' cannot appear at start of macro expansion";
            public const string InvalidPastingOperatorEnd = "{0} ({1}): '##' cannot appear at end of macro expansion";
            public const string InvalidPastingToken = "{0} ({1}): Pasting formed '{2}', an invalid preprocessing token";
            public const string InvalidStringificationToken = "{0} ({1}): '#' is not followed by a macro parameter";

            public const string MissingTernarySeparator = "{0} ({1}): Expected ':' to match this '?'";
            public const string MissingExpressionValue = "{0} ({1}): Expected value in expression";
            public const string InvalidExpressionValue = "{0} ({1}): Invalid token in expression";
            public const string InvalidNumberExpression = "{0} ({1}): Expected an integer value in expression";

            public const string MissingDefinedTerminator = "{0} ({1}): Missing ')' after 'defined'";
        }
    }
}
