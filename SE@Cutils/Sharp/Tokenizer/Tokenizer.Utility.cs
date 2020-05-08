// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Cpp
{
    public partial class Tokenizer
    {
        /// <summary>
        /// Base rules used by this tokenizer
        /// </summary>
        protected static class Rules
        {
            /// <summary>
            /// SingleLineComment = '//' (~'\\\n')*;
            /// </summary>
            public static bool SingleLineComment(Tokenizer data)
            {
                while (!data.EndOfStream && data.PeekCharacter() != '\n')
                    data.Position++;

                return true;
            }
            /// <summary>
            /// MultiLineComment = '/*' (~'*/')* '*/'?;
            /// </summary>
            public static bool MultiLineComment(Tokenizer data)
            {
                while (!data.EndOfStream)
                {
                    while (!data.EndOfStream && data.PeekCharacter() != '*')
                        data.Position++;

                    data.Position++;
                    if (data.PeekCharacter() == '/')
                        break;
                }

                data.Position++;
                return true;
            }

            /// <summary>
            /// StringLiteral = '\"' ('\\"' | ~'\"')* '\"';
            /// </summary>
            public static bool StringLiteral(Tokenizer data)
            {
                for (bool skip = false; !data.EndOfStream;)
                {
                    Char32 c = data.PeekCharacter();
                    switch (c)
                    {
                        case '\"':
                            {
                                if (!skip)
                                {
                                    data.RawDataBuffer.Buffer.RemoveAt(0);
                                    data.RawDataBuffer.Discard(1);

                                    return true;
                                }
                            }
                            break;
                        case '\n':
                            {
                                if (!skip)
                                    return false;
                            }
                            break;
                    }
                    skip = !skip && (c == '\\');
                    data.Position++;
                }
                return false;
            }

            /// <summary>
            /// RawStringLiteral = 'R\"(' delimiter (~'delimiter)')* ')\"';
            /// </summary>
            public static bool RawStringLiteral(Tokenizer data)
            {
                try
                {
                    ComparisonState state = 0;
                    data.textBuffer.Clear();
                    bool isBogus = false;

                    string delimiter = string.Empty;
                    Char32 c; while (!data.EndOfStream)
                    {
                        data.allowUcnConversion = ((int)state != 1);

                        c = data.PeekCharacter();
                        switch (state)
                        {
                            #region Introducing sequence
                            case (ComparisonState)0:
                                switch (c)
                                {
                                    #region Bogus literal
                                    case '\"':
                                        {
                                            data.Position++;
                                            return false;
                                        }
                                    #endregion

                                    #region Escape sequence
                                    case '\\':
                                        {
                                            //Illegal escape sequence in delimiter
                                            isBogus = true;
                                        }
                                        goto default;
                                    #endregion

                                    #region Raw character sequence
                                    case '(':
                                        {
                                            delimiter = data.textBuffer.ToString();
                                            state++;

                                            data.textBuffer.Clear();
                                        }
                                        break;
                                    #endregion

                                    #region Anything else
                                    default:
                                        {
                                            data.textBuffer.Append(Char.ConvertFromUtf32((Int32)c));
                                        }
                                        break;
                                    #endregion
                                }
                                break;
                            #endregion

                            #region Raw character sequence
                            case (ComparisonState)1:
                                switch (c)
                                {
                                    #region Closing sequence
                                    case ')':
                                        {
                                            data.textBuffer.Clear();
                                            state++;
                                        }
                                        break;
                                        #endregion
                                }
                                break;
                            #endregion

                            #region Closing sequence
                            case (ComparisonState)2:
                                switch (c)
                                {
                                    #region String literal
                                    case '\"':
                                        {
                                            if (data.textBuffer.IsEqual(delimiter))
                                            {
                                                data.RawDataBuffer.Buffer.RemoveRange(0, 3 + delimiter.Length);
                                                data.RawDataBuffer.Discard(2 + delimiter.Length);

                                                return !isBogus;
                                            }
                                            else state--;
                                        }
                                        break;
                                    #endregion

                                    #region Bogus literal
                                    case '\n':
                                        {
                                            return false;
                                        }
                                    #endregion

                                    #region Closing sequence
                                    case ')':
                                        {
                                            data.textBuffer.Clear();
                                        }
                                        break;
                                    #endregion

                                    #region Anything else
                                    default:
                                        {
                                            if (data.textBuffer.Length >= delimiter.Length)
                                            {
                                                state--;
                                            }
                                            else data.textBuffer.Append(Char.ConvertFromUtf32((Int32)c));
                                        }
                                        break;
                                        #endregion
                                }
                                break;
                                #endregion
                        }
                        data.Position++;
                    }
                    return false;
                }
                finally
                {
                    data.allowUcnConversion = true;
                }
            }

            /// <summary>
            /// CharacterLiteral = '\'' ('\\'' | ~'\'')+ '\'';
            /// </summary>
            public static bool CharacterLiteral(Tokenizer data)
            {
                for (bool skip = false, hasCharacter = false; !data.EndOfStream; hasCharacter = true)
                {
                    Char32 c = data.PeekCharacter();
                    switch (c)
                    {
                        case '\'':
                            {
                                if (!skip)
                                {
                                    data.RawDataBuffer.Buffer.RemoveAt(0);
                                    data.RawDataBuffer.Discard(1);

                                    return hasCharacter;
                                }
                            }
                            break;
                        case '\n':
                            {
                                if (!skip)
                                    return false;
                            }
                            break;
                    }
                    skip = !skip && (c == '\\');
                    data.Position++;
                }
                return false;
            }

            /// <summary>
            /// Identifier = (['a','z'] | ['A','Z'] | '_' | UniversalCharacterName) (['a','z'] | ['A','Z'] | ['0','9'] | '_' | UniversalCharacterName)*;
            /// </summary>
            public static bool Identifier(Tokenizer data)
            {
                for (int count = 0; ; count++)
                {
                    Char32 c = data.PeekCharacter();
                    switch (c)
                    {
                        case '\\':
                            {
                                long streamPos = data.Position;
                                if (Rules.UniversalCharacterName(data))
                                    break;

                                data.Position = streamPos;
                            }
                            goto default;
                        case 'R':
                            {
                                data.Position++;
                                if (data.PeekCharacter() == '\"')
                                {
                                    data.Position--;
                                    return (count > 0);
                                }
                            }
                            break;
                        default:
                            {
                                if (!IsIdentifierChar(c))
                                {
                                    return (count > 0);
                                }
                                else data.Position++;
                            }
                            break;
                    }
                }
            }

            /// <summary>
            /// Numeric = (['0', '9'] | '.') ((('E' | 'e') ('+' | '-')) | '\'' | ['a','z'] | ['A','Z'] | ['0','9'] | '_' | '.' | UniversalCharacterName)*;
            /// </summary>
            public static bool Numeric(Tokenizer data)
            {
                for (int count = 0; ; count++)
                {
                    Char32 c = data.PeekCharacter();
                    switch (c)
                    {
                        case '\'':
                        case '.':
                            {
                                data.Position++;
                            }
                            break;
                        case 'e':
                        case 'E':
                            {
                                data.Position++;
                                switch (data.PeekCharacter())
                                {
                                    case '+':
                                    case '-':
                                        {
                                            data.Position++;
                                        }
                                        break;
                                }
                            }
                            break;
                        case '\\':
                            {
                                long streamPos = data.Position;
                                if (Rules.UniversalCharacterName(data))
                                    break;

                                data.Position = streamPos;
                            }
                            goto default;
                        default:
                            {
                                if (!IsIdentifierChar(c))
                                {
                                    return (count > 0);
                                }
                                else data.Position++;
                            }
                            break;
                    }
                }
            }

            /// <summary>
            /// UniversalCharacterName = ('\u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT) | ('\U' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT);
            /// </summary>
            public static bool UniversalCharacterName(Tokenizer data)
            {
                int count;
                switch (data.GetCharacter())
                {
                    case 'u':
                        {
                            count = 4;
                        }
                        goto Resolve;
                    case 'U':
                        {
                            count = 8;
                        }
                        goto Resolve;
                    default: return false;
                }

            Resolve:
                for (int i = 0; i < count; i++)
                {
                    switch (data.PeekCharacter())
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case 'a':
                        case 'A':
                        case 'b':
                        case 'B':
                        case 'c':
                        case 'C':
                        case 'd':
                        case 'D':
                        case 'e':
                        case 'E':
                        case 'f':
                        case 'F': break;
                        default: return (i > 0);
                    }
                    data.Position++;
                }
                return true;
            }

            /// <summary>
            /// IfdefDirective = '#ifdef';
            /// </summary>
            public static bool IfdefDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// IfndefDirective = '#ifndef';
            /// </summary>
            public static bool IfndefDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'd') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// IncludeDirective = '#include';
            /// </summary>
            public static bool IncludeDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'c') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'l') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'u') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 'd') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)4:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)5:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// ElifDirective = '#elif';
            /// </summary>
            public static bool ElifDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// ElseDirective = '#else';
            /// </summary>
            public static bool ElseDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// ErrorDirective = '#error';
            /// </summary>
            public static bool ErrorDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'r') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'o') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'r') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// EndifDirective = '#endif';
            /// </summary>
            public static bool EndifDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'd') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'i') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// DefineDirective = '#define';
            /// </summary>
            public static bool DefineDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)4:
                        case (ComparisonState)0:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'i') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 'n') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)5:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// UndefDirective = '#undef';
            /// </summary>
            public static bool UndefDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'n') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'd') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)4:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// LineDirective = '#line';
            /// </summary>
            public static bool LineDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'i') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'n') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// PragmaDirective = '#pragma';
            /// </summary>
            public static bool PragmaDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'r') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'a') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'g') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 'm') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)4:
                            {
                                if (c == 'a') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)5:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }
        }

        /// <summary>
        /// Determines if a character is a valid identifier character to this tokenizer
        /// </summary>
        public static bool IsIdentifierChar(Char32 c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_');
        }
    }
}
