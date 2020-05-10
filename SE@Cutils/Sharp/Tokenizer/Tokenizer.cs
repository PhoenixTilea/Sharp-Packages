// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Cpp
{
    /// <summary>
    /// ISO/IEC C++20 Standard Compliant Preprocessor Tokenizer
    /// https://en.cppreference.com/w/cpp/language/translation_phases
    /// https://www.nongnu.org/hcb/
    /// </summary>
    public partial class Tokenizer : StreamTokenizer<Token, CppTokenizerState>
    {
        /// <summary>
        /// Creates a new tokenizer instance
        /// </summary>
        public Tokenizer(Stream stream, bool isUtf8)
            : base(stream, isUtf8)
        {
            this.allowUcnConversion = true;
            this.newLineCharacter = (stream.Position == 0);
        }

        /// <summary>
        /// C++ preprocessor directives
        /// https://www.nongnu.org/hcb/#if-section
        /// </summary>
        protected override Token GetToken(object context)
        {
            Token result = ProcessGetToken();
            switch (result)
            {
                case Token.NewLine:
                    {
                        State.Set(CppTokenizerState.Initial);
                    }
                    break;
                case Token.Comment:
                case Token.Whitespace:
                    break;
                default:
                    {
                        State.Set(CppTokenizerState.AfterWhitespace);
                    }
                    break;
            }
            return result;
        }

        Token ProcessGetToken()
        {
            allowUcnConversion = true;
            switch (PeekCharacter())
            {
                case '#': if(State.Current == CppTokenizerState.Initial)
                    {
                        RawDataBuffer.Position++;
                        BaseRules.NullLiteral(this);
                        switch (PeekCharacter())
                        {
                            case 'i':
                                {
                                    RawDataBuffer.Position++;
                                    switch (PeekCharacter())
                                    {
                                        case 'f':
                                            {
                                                RawDataBuffer.Position++;
                                                Char32 c = PeekCharacter();
                                                switch (c)
                                                {
                                                    #region #ifdef
                                                    case 'd':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.IfdefDirective(this))
                                                                return Token.IfdefDirective;
                                                        }
                                                        break;
                                                    #endregion

                                                    #region #ifndef
                                                    case 'n':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.IfndefDirective(this))
                                                                return Token.IfndefDirective;
                                                        }
                                                        break;
                                                    #endregion

                                                    #region #if
                                                    default:
                                                        {
                                                            if (!IsIdentifierChar(c))
                                                                return Token.IfDirective;
                                                        }
                                                        break;
                                                        #endregion
                                                }
                                            }
                                            break;

                                        #region #include
                                        case 'n':
                                            {
                                                RawDataBuffer.Position++;
                                                if (Rules.IncludeDirective(this))
                                                {
                                                    State.Set(CppTokenizerState.Include);
                                                    return Token.IncludeDirective;
                                                }
                                            }
                                            break;
                                        #endregion
                                    }
                                    RawDataBuffer.Position = 1;
                                }
                                goto default;

                            case 'e':
                                {
                                    RawDataBuffer.Position++;
                                    switch (PeekCharacter())
                                    {
                                        case 'l':
                                            {
                                                RawDataBuffer.Position++;
                                                switch (PeekCharacter())
                                                {
                                                    #region #elif
                                                    case 'i':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.ElifDirective(this))
                                                                return Token.ElifDirective;
                                                        }
                                                        break;
                                                    #endregion

                                                    #region #else
                                                    case 's':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.ElseDirective(this))
                                                                return Token.ElseDirective;
                                                        }
                                                        break;
                                                    #endregion
                                                }
                                            }
                                            break;

                                        #region #error
                                        case 'r':
                                            {
                                                RawDataBuffer.Position++;
                                                if (Rules.ErrorDirective(this))
                                                {
                                                    return Token.Error;
                                                }
                                            }
                                            break;
                                        #endregion

                                        #region #endif
                                        case 'n':
                                            {
                                                RawDataBuffer.Position++;
                                                if (Rules.EndifDirective(this))
                                                {
                                                    return Token.EndifDirective;
                                                }
                                            }
                                            break;
                                        #endregion
                                    }
                                    RawDataBuffer.Position = 1;
                                }
                                goto default;

                            #region #define
                            case 'd':
                                {
                                    RawDataBuffer.Position++;
                                    if (Rules.DefineDirective(this))
                                    {
                                        return Token.DefineDirective;
                                    }
                                    else RawDataBuffer.Position = 1;
                                }
                                goto default;
                            #endregion

                            #region #undef
                            case 'u':
                                {
                                    RawDataBuffer.Position++;
                                    if (Rules.UndefDirective(this))
                                    {
                                        return Token.UndefDirective;
                                    }
                                    else RawDataBuffer.Position = 1;
                                }
                                goto default;
                            #endregion

                            #region #line
                            case 'l':
                                {
                                    RawDataBuffer.Position++;
                                    if (Rules.LineDirective(this))
                                    {
                                        return Token.Line;
                                    }
                                    else RawDataBuffer.Position = 1;
                                }
                                goto default;
                            #endregion

                            #region #pragma
                            case 'p':
                                {
                                    RawDataBuffer.Position++;
                                    if (Rules.PragmaDirective(this))
                                    {
                                        return Token.Pragma;
                                    }
                                    else RawDataBuffer.Position = 1;
                                }
                                goto default;
                            #endregion

                            #region <Empty>
                            case '\n':
                                {
                                    return Token.Empty;
                                }
                            #endregion

                            #region BogusDirective
                            default:
                                {
                                    return Token.BogusDirective;
                                }
                            #endregion
                        }
                    }
                    goto default;

                #region Header name
                case '<':
                    {
                        if (State.Current == CppTokenizerState.Include)
                            return ReadUnquotedHeaderName();
                    }
                    goto default;
                #endregion

                #region Anything else
                default:
                    {
                        return ReadGenericTokens();
                    }
                #endregion
            }
        }

        /// <summary>
        /// C++ h-char-sequence
        /// https://www.nongnu.org/hcb/#h-char-sequence
        /// </summary>
        Token ReadUnquotedHeaderName()
        {
            DiscardCharacter();
            do
            {
                switch (GetCharacter())
                {
                    #region UnqoutedHeaderName
                    case '>':
                        {
                            RawDataBuffer.Discard(1);
                            return Token.UnqoutedHeaderName;
                        }
                    #endregion

                    #region BogusUnqoutedHeaderName
                    case '\n':
                        {
                            RawDataBuffer.Position--;
                            return Token.BogusUnqoutedHeaderName;
                        }
                    #endregion
                }
            }
            while (!EndOfStream);
            return Token.BogusUnqoutedHeaderName;
        }
    }
}
