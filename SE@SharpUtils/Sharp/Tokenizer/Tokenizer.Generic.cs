using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Sharp
{
    public partial class Tokenizer
    {
        /// <summary>
        /// C# preprocessing-token
        /// </summary>
        Token ReadGenericTokens()
        {
            switch (GetCharacter())
            {
                case '/':
                {
                    switch (GetCharacter())
                    {
                            #region SingleLineComment
                        case '/':
                        {
                            if (Rules.SingleLineComment(this)) 
                            {
                                return Token.SingleLineComment;
                            }
                        }
                        break;
                        #endregion
                        
                        #region MultipleLineComment
                        case '*':
                        {
                            if (Rules.MultiLineComment(this))
                            {
                                return Token.MultiLineComment;
                            }
                        }
                        break;
                        #endregion
                        
                        #region DivAssign
                        case '=':
                        {
                            RawDataBuffer.Position++;
                            return Token.DivAssign;
                        }
                        #endregion
                        
                        #region Divide
                        default:
                        {
                            return Token.Div;
                        }
                        #endregion
                    }
                }
                goto default;
                
                #region string literal
                case '\"':
                {
                    if (Rules.StringLiteral(this)) return Token.StringLiteral;
                    else return Token.BogusStringLiteral;
                }
                #endregion
                
                case '@':
                {
                    #region Raw string literal
                    if (PeekCharacter() == '\"') 
                    {
                        RawDataBuffer.Position++;
                        if (Rules.RawStringLiteral(this))
                        {
                            return Token.RawStringLiteral;
                        }
                        else return Token.BogusStringLiteral;
                    }
                    #endregion
                    
                    #region Verbatim Identifier
                    else if (Rules.Identifier(this))
                    {
                        return Token.VerbatimIdentifier;
                    }
                    #endregion
                    else RawDataBuffer.Position = 1;
                }
                goto default;
                
                #region character literal
                case '\'':
                {
                    if (Rules.CharacterLiteral(this))
                    {
                        return Token.CharacterLiteral;
                    }
                    else return Token.BogusCharacterLiteral;
                }
                #endregion
                
                #region Universal character name
                case '\\':
                {
                    if (Rules.UniversalCharacterName(this))
                        goto Identifier;
                    
                RawDataBuffer.Position = 1;
                }
                goto default;
                #endregion
                
                #region Identifier 
                case '_':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                Identifier:
                {
                        Rules.Identifier(this);
                    return Token.Identifier;
                }
                #endregion
                
                #region Numeric
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
                Numeric:
                {
                    Rules.Numeric(this);
                    return Token.Numeric;
                }
                #endregion
                
                #region Colon
                case ':':
                {
                    return Token.Colon;
                }
                #endregion
                
                #region String interpolation
                case '$':
                {
                    RawDataBuffer.Position++;
                    if (PeekCharacter() == '\"')
                    {   
                        if (Rules.StringLiteral(this))
                        {
                            return Token.InterpolatedStringLiteral;
                        }
                        else return Token.BogusStringLiteral;
                    }
                    else RawDataBuffer.Position--;
                }
                goto default;
                #endregion
                
                case '+':
                {
                    switch (PeekCharacter())
                    {
                        #region Increment
                        case '+':
                        {
                            RawDataBuffer.Position++;
                            return Token.Increment;
                        }
                        #endregion
                        
                        #region AddAssign
                        case '=':
                        {
                            RawDataBuffer.Position++;
                                    return Token.AddAssign;
                        }
                        #endregion
                        
                        #region Add
                        default:
                        {
                            return Token.Add;
                        }
                        #endregion
                    }
                }
                
                case '-':
                {
                    switch (PeekCharacter())
                    {
                        #region Decrement
                        case '-':
                        {
                            RawDataBuffer.Position++;
                            return Token.Decrement;
                        }
                        #endregion
                        
                        #region SubAssign
                        case '=':
                        {
                            RawDataBuffer.Position++;
                            return Token.SubAssign;
                        }
                        #endregion
                        
                        #region Sub
                        default:
                        {
                            return Token.Sub;
                        }
                        #endregion
                    }
                }
                
                case '*':
                {
                    #region MultipleAssign
                    if (PeekCharacter() == '=') 
                    {
                        RawDataBuffer.Position++;
                        return Token.MultipleAssign;
                    }
                    #endregion
                    
                    #region Multiple
                    else return Token.Multiple;
                    #endregion
                }
                
                #region LeftBrace
                case '{':
                    {
                        return Token.LeftCurlyBracket;
                    }
                    #endregion
                    
                    #region RightBrace
                case '}':
                {
                    return Token.RightCurlyBracket;
                }
                #endregion
                
                #region LeftParenthesis
                case '(':
                {
                    return Token.LeftRoundBracket;
                }
                #endregion
                
                #region RightParenthesis
                case ')':
                {
                    return Token.RightRoundBracket;
                }
                #endregion
                
                #region LeftBracket
                case '[':
                {
                    return Token.LeftSquareBracket;
                }
                #endregion
                
                #region RightBracket
                case ']':
                {
                    return Token.RightSquareBracket;
                }
                #endregion
                
                case '.':
                {
                    switch (PeekCharacter())
                    {
                        #region Numeric
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9': goto Numeric;
                        #endregion
                        
                        case '.':
                        {
                            RawDataBuffer.Position++;
                            #region RestParams
                            if (PeekCharacter() == '.')
                            {
                                RawDataBuffer.Position++;
                                return Token.RestParams;
                            }
                                    #endregion
                                    
                                    #region Range
                                    else return Token.Range;
                            #endregion
                        }
                        
                        #region Dot
                        default:
                        {
                            return Token.Dot;
                        }
                        #endregion
                    }
                }
                
                case '!':
                {
                    #region NotEqual
                    if (PeekCharacter() == '=')
                    {
                        RawDataBuffer.Position++;
                        return Token.NotEqual;
                    }
                    #endregion
                    
                    #region LogicalNot
                    else return Token.LogicalNot;
                    #endregion
                }
                
                #region BitwiseNot
                case '~':
                {
                    return Token.BitwiseNot;
                }
                #endregion
                
                case '%':
                {
                    switch (PeekCharacter())
                    {
                        #region ModAssign
                        case '=':
                        {
                            RawDataBuffer.Position++;
                            return Token.ModAssign;
                        }
                        #endregion
                        
                        #region Mod
                        default:
                        {
                            return Token.Mod;
                        }
                        #endregion
                    }
                }
                
                case '<':
                {
                    switch (PeekCharacter())
                    {
                        case '<':
                        {
                            RawDataBuffer.Position++;
                            #region LeftShiftAssign
                            if (PeekCharacter() == '=')
                            {
                                RawDataBuffer.Position++;
                                return Token.LeftShiftAssign;
                            }
                            #endregion
                            
                            #region LeftShift
                            else return Token.LeftShift;
                            #endregion
                        }
                        
                        #region LessEquals
                        case '=':
                        {
                            RawDataBuffer.Position++;
                                    return Token.LessEqual;
                        }
                        #endregion
                        
                        #region LessThan
                        default:
                        {
                            return Token.LessThan;
                        }
                        #endregion
                    }
                }
                
                case '>':
                {
                    switch (PeekCharacter())
                    {
                        case '>':
                        {
                            RawDataBuffer.Position++;
                            #region RightShiftAssign
                            if (PeekCharacter() == '=')
                            {
                                RawDataBuffer.Position++;
                                return Token.RightShiftAssign;
                            }
                            #endregion
                            
                            #region RightShift
                            else return Token.RightShift;
                            #endregion
                        }
                        
                        #region GreaterEqual
                        case '=':
                        {
                            RawDataBuffer.Position++;
                            return Token.GreaterEqual;
                        }
                        #endregion
                        
                        #region GreaterThan
                        default:
                        {
                            return Token.GreaterThan;
                        }
                        #endregion
                    }
                }
                
                case '&':
                {
                    switch (PeekCharacter())
                    {
                        #region LogicalAnd
                        case '&':
                        {
                            RawDataBuffer.Position++;
                            return Token.LogicalAnd;
                        }
                        #endregion
                        
                        #region BitwiseAndAssign
                        case '=':
                        {
                            RawDataBuffer.Position++;
                            return Token.BitwiseAndAssign;
                        }
                        #endregion
                        
                        #region BitwiseAnd
                        default:
                        {
                            return Token.BitwiseAnd;
                        }
                        #endregion
                    }
                }
                
                case '^':
                {
                    #region BitwiseXorAssign
                    if (PeekCharacter() == '=')
                    {
                        RawDataBuffer.Position++;
                        return Token.BitwiseXorAssign;
                    }
                    #endregion
                    
                    #region BitwiseXor
                    else return Token.BitwiseXor;
                    #endregion
                }
                
                case '|':
                {
                    switch (PeekCharacter())
                    {
                        #region LogicalOr
                        case '|':
                        {
                            RawDataBuffer.Position++;
                            return Token.LogicalOr;
                        }
                        #endregion
                        
                        #region BitwiseOrAssign
                        case '=':
                        {
                            RawDataBuffer.Position++;
                            return Token.BitwiseOrAssign;
                        }
                        #endregion
                        
                        #region BitwiseOr
                        default:
                        {
                            return Token.BitwiseOr;
                        }
                        #endregion
                    }
                }
                
                case '=':
                {
                    switch (PeekCharacter())
                    {
                        #region Equal
                        case '=':
                        {
                            RawDataBuffer.Position++;
                            return Token.Equal;
                        }
                        #endregion
                        
                        #region Lamda
                        case '>':
                        {
                            RawDataBuffer.Position++;
                            return Token.Lamda;
                        }
                        #endregion
                        
                        #region Assign
                        default:
                        {
                            return Token.Assign;
                        }
                        #endregion
                    }
                }
                
                case '?':
                {
                    switch (PeekCharacter())
                    {
                        case '?':
                        {
                            RawDataBuffer.Position++;
                            #region NullCoalescingAssign
                            if (PeekCharacter() == '=')
                            {
                                RawDataBuffer.Position++;
                                return Token.NullCoalescingAssign;
                            }
                            #endregion
                            
                            #region NullCoalescing
                            else return Token.NullCoalescing;
                            #endregion
                        }
                        
                        #region Ternary
                        default:
                        {
                            return Token.Ternary;
                        }
                        #endregion
                    }
                }
                
                #region Semicolon
                case ';':
                {
                    return Token.Semicolon;
                }
                #endregion
                
                #region Comma
                case ',':
                {
                    return Token.Comma;
                }
                #endregion
                
                #region Whitespace
                case Char32.WhiteSpaceGroup.Space:
                case Char32.WhiteSpaceGroup.HorizontalTab:
                case Char32.WhiteSpaceGroup.VerticalTab:
                case Char32.WhiteSpaceGroup.FormFeed:
                {
                    do
                    {
                            switch (PeekCharacter())
                            {
                                case Char32.WhiteSpaceGroup.Space:
                                case Char32.WhiteSpaceGroup.HorizontalTab:
                                case Char32.WhiteSpaceGroup.VerticalTab:
                                case Char32.WhiteSpaceGroup.FormFeed:
                                    {
                                        RawDataBuffer.Position++;
                                    }
                                    break;
                                default:
                                    {
                                        return Token.Whitespace;
                                    }
                            }
                    } while (!EndOfStream);
                }
                return Token.Whitespace;
                #endregion
                
                #region NewLine
                case Char32.NewLineGroup.LineFeed:
                {
                    State.Reset();
                    return Token.NewLine;
                }
                #endregion
                
                #region Character
                default:
                {
                    return Token.Character;
                }
                #endregion
            }
        }
    }
}