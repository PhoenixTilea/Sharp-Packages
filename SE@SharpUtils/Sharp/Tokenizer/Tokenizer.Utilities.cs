using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Sharp
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
                    ++data.Position;
            
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
                    {
                        ++data.Position;
                    }
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
                for (bool skip = false; !data.EndOfStream; )
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
            /// RawStringLiteral = '@\"' delimiter (~'delimiter)')* '\"';
            /// </summary>
            public static bool RawStringLiteral(Tokenizer data)
            {
                try
                {
                    bool isBogus = false;
                    string delimiter = string.Empty;
                    data.textBuffer.Clear();
                    data.allowUcnConversion = false;
                    Char32 c;
                    while (!data.EndOfStream)
                    {
                        c = data.PeekCharacter();
                        if (c == '\"')
                        {
                            data.Position++;
                            #region closing sequence
                            if (data.PeekCharacter() != '\"')
                            {
                                if (data.textBuffer.IsEqual(delimiter))
                                {
                                    data.RawDataBuffer.Buffer.RemoveRange(0, 1 + delimiter.Length);
                                    data.RawDataBuffer.Discard(delimiter.Length);
                                    return !isBogus;
                                }
                                else isBogus = true;
                            }
                            #endregion
                            else if (data.textBuffer.Length < delimiter.Length)
                            {
                                data.textBuffer.Append(Char.ConvertFromUtf32((Int32)c));
                            }
                        }
                        #region Bogus string literal
                        else if (c == '\n')
                        {
                            return false;
                        }
                        #endregion
                        
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
                    
                }
                return true;
            }
            
            /// <summary>
            /// If directive = #if
            /// Elif directive = #elif
            /// </summary>
            public static bool IfDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c;
                for ( ; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState) 0:
                        {
                            if (c == 'f') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        
                        case (ComparisonState) 1:
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
            /// Else directive = #else
            /// </summary>
            public static bool ElseDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c;
                for ( ; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState) 0:
                        {
                            if (c == 'e') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        
                        case (ComparisonState) 1:
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
            /// Error directive = #error
            /// </summary>
            public static bool ErrorDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c;
                for ( ; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState) 0:
                        case (ComparisonState) 2:
                        {
                            if (c == 'r') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        
                        case (ComparisonState) 1:
                        {
                            if (c == 'o') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 3:
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
            /// Warning directive = #warning
            /// </summary>
            public static bool WarningDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c;
                for ( ; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState) 0:
                        {
                            if (c == 'a') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 1:
                        {
                            if (c == 'r') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 2:
                        case (ComparisonState) 4:
                        {
                            if (c == 'n') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 3:
                        {
                            if (c == 'i') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 5:
                        {
                            if (c == 'g') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 6:
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
            /// Endif directive = #endif
            /// </summary>
            public static bool EndifDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c;
                for ( ; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState) 0:
                        {
                            if (c == 'f') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 1:
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
            /// Define directive = #define
            /// </summary>
            public static bool DefineDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c;
                for ( ; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState) 0:
                        case (ComparisonState) 4:
                        {
                            if (c == 'e') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 1:
                        {
                            if (c == 'f') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 2:
                        {
                            if (c == 'i') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 3:
                        {
                            if (c == 'n') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 5:
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
            /// Undef directive = #undef
            /// </summary>
            public static bool UndefDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c;
                for ( ; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState) 0:
                        {
                            if (c == 'n') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 1:
                        {
                            if (c == 'd') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 2:
                        {
                            if (c == 'e') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 3:
                        {
                            if (c == 'f') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 4:
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
            /// Line directive = #line
            /// </summary>
            public static bool LineDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c;
                for ( ; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState) 0:
                        {
                            if (c == 'i') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 1:
                        {
                            if (c == 'n') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 2:
                        {
                            if (c == 'e') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 3:
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
            /// Region directive = #region
            /// Endregion directive = #endregion
            /// </summary>
            public static bool RegionDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c;
                for ( ; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState) 0:
                        {
                            if (c == 'e') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 1:
                        {
                            if (c == 'g') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 2:
                        {
                            if (c == 'i') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 3:
                        {
                            if (c == 'o') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 4:
                        {
                            if (c == 'n') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 5:
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
            /// Pragma directive = #pragma
            /// </summary>
            public static bool PragmaDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c;
                for ( ; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState) 0:
                        {
                            if (c == 'r') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 1:
                        {
                            if (c == 'a') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 2:
                        {
                            if (c == 'g') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 3:
                        {
                            if (c == 'm') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 4:
                        {
                            if (c == 'a') goto case ComparisonState.Next;
                            else goto case ComparisonState.Failure;
                        }
                        case (ComparisonState) 5:
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
        /// Determines if a character is a valid identifier character to this tokenizer
        /// </summary>
            public static bool IsIdentifierChar(Char32 c)
            {
                return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || (c == '_'));
            }
        }
    }
}