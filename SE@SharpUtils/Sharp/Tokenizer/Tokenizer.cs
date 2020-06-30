using System;
using System.Collections.Generic;
using System.IO;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Sharp
{
    /// <summary>
        /// Creates a new tokenizer instance
        /// </summary>
    public partial class Tokenizer : StreamTokenizer<Token, SharpTokenizerState>
    {
        public Tokenizer(Stream stream, bool isUtf8)
            : base(stream, isUtf8)
        {
            this.allowUcnConversion = true;
            this.newLineCharacter = (stream.Position == 0);
        }

        /// <summary>
        /// C# preprocessor directives
        /// </summary>
        protected override Token GetToken(object context)
        {
            Token result = ProcessGetToken();
            switch (result)
            {
                case Token.NewLine:
                    {
                        State.Set(SharpTokenizerState.Initial);
                    }
                    break;

                case Token.SingleLineComment:
                case Token.MultiLineComment:
                case Token.Whitespace:
                    break;

                default:
                    {
                        if (State.Current == SharpTokenizerState.Initial)
                        {
                            State.Set(SharpTokenizerState.AfterWhitespace);
                        }
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
                case '#':
                    if (State.Current == SharpTokenizerState.Initial)
                    {
                        RawDataBuffer.Position++;
                        BaseRules.NullLiteral(this);
                        switch (PeekCharacter())
                        {
                            #region #if
                            case 'i':
                            {
                                RawDataBuffer.Position++;
                                if (Rules.IfDirective(this))
                                {
                                    return Token.IfDirective;
                                }
                            }
                            break;
                            #endregion

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
                                                            // IfDirective performs the same checks as an ElifDirective method would
                                                            if (Rules.IfDirective(this))
                                                            {
                                                                return Token.ElifDirective;
                                                            }
                                                        }
                                                        break;
                                                    #endregion

                                                    #region #else
                                                    case 's':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.ElseDirective(this))
                                                            {
                                                                return Token.ElseDirective;
                                                            }
                                                        }
                                                        break;
                                                        #endregion

                                                }
                                            }
                                            break;

                                            case 'n':
                                            {
                                                RawDataBuffer.Position++;
                                                if (PeekCharacter() == 'd')
                                                {
                                                    RawDataBuffer.Position++;
                                                    switch (PeekCharacter())
                                                    {
                                                        #region #endif
                                                        case 'i':
                                                            {
                                                                RawDataBuffer.Position++;
                                                                if (Rules.EndifDirective(this))
                                                                {
                                                                    return Token.EndifDirective;
                                                                }
                                                            }
                                                            break;
                                                        #endregion

                                                        #region #endregion
                                                        case 'r':
                                                            {
                                                                RawDataBuffer.Position++;
                                                                // Calling Rules.RegionDirective because it would perform
                                                                // the exact same checks as an EndregionDirective() in this case
                                                                if (Rules.RegionDirective(this))
                                                                {
                                                                    return Token.Endregion;
                                                                }
                                                            }
                                                            break;
                                                            #endregion
                                                    }
                                                }
                                                RawDataBuffer.Position = 1;
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

                            #region #warning
                            case 'w':
                            {
                                RawDataBuffer.Position++;
                                if (Rules.WarningDirective(this))
                                {
                                    return Token.Warning;
                                }
                                else RawDataBuffer.Position = 1;
                            }
                            goto default;
                            #endregion

                            #region #region
                            case 'r':
                            {
                                RawDataBuffer.Position++;
                                if (Rules.RegionDirective(this))
                                {
                                    return Token.Region;
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
                    
                    #region AnythingElse
                        default:
                        {
                        return ReadGenericTokens();
                        }
                        #endregion
            }
        }
    }
}