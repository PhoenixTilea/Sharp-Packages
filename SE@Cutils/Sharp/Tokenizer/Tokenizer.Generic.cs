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
        /// C++ preprocessing-token
        /// https://www.nongnu.org/hcb/#preprocessing-token
        /// </summary>
        Token ReadGenericTokens()
        {
            switch (GetCharacter())
            {
                case '/': switch (GetCharacter())
                    {
                        #region SingleLineComment = '//'(~'\\\n') *;
                        case '/':
                            {
                                if (Rules.SingleLineComment(this))
                                    return Token.Comment;
                            }
                            break;
                        #endregion

                        #region MultiLineComment = '/*'(~'*/') * '*/' ?;
                        case '*':
                            {
                                if (Rules.MultiLineComment(this))
                                    return Token.Comment;
                            }
                            break;
                        #endregion

                        #region DivAssign (/=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.DivAssign;
                            }
                        #endregion

                        #region Div (/)
                        default:
                            {
                                return Token.Div;
                            }
                        #endregion
                    }
                    goto default;

                #region StringLiteral = '\"' ('\\"' | ~'\"')* '\"';
                case '\"':
                    {
                        if (Rules.StringLiteral(this))
                        {
                            return Token.StringLiteral;
                        }
                        else return Token.BogusStringLiteral;
                    }
                #endregion

                #region RawStringLiteral = 'R\"(' delimiter (~'delimiter)')* ')\"';
                case 'R':
                    {
                        if (PeekCharacter() == '\"')
                        {
                            RawDataBuffer.Position++;
                            if (Rules.RawStringLiteral(this))
                            {
                                return Token.StringLiteral;
                            }
                            else return Token.BogusStringLiteral;
                        }
                        RawDataBuffer.Position = 1;
                    }
                    goto Identifier;
                #endregion

                #region CharacterLiteral = '\'' ('\\'' | ~'\'')+ '\'';
                case '\'':
                    {
                        if (Rules.CharacterLiteral(this))
                        {
                            return Token.CharacterLiteral;
                        }
                        else return Token.BogusCharacterLiteral;
                    }
                #endregion

                #region UniversalCharacterName = ('u' DIGIT DIGIT DIGIT DIGIT) | ('U' DIGIT DIGIT DIGIT DIGIT DIGIT DIGIT DIGIT DIGIT);
                case '\\':
                    {
                        if (Rules.UniversalCharacterName(this))
                            goto Identifier;

                        RawDataBuffer.Position = 1;
                    }
                    goto default;
                #endregion

                #region Identifier = (['a','z'] | ['A','Z'] | '_' | UniversalCharacterName) (['a','z'] | ['A','Z'] | ['0','9'] | '_' | UniversalCharacterName)*;
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
                        switch (PreviewBufferedData())
                        {
                            #region LogicalAnd (&&) alternative
                            case "and": return Token.LogicalAnd;
                            #endregion

                            #region BitwiseAndAssign (&=) alternative
                            case "and_eq": return Token.BitwiseAndAssign;
                            #endregion

                            #region BitwiseAnd (&) alternative
                            case "bitand": return Token.BitwiseAnd;
                            #endregion

                            #region BitwiseOr (|) alternative
                            case "bitor": return Token.BitwiseOr;
                            #endregion

                            #region BitwiseNot (~) alternative
                            case "compl": return Token.BitwiseNot;
                            #endregion

                            #region Delete
                            case "delete": return Token.Delete;
                            #endregion

                            #region LogicalNot (!) alternative
                            case "not": return Token.LogicalNot;
                            #endregion

                            #region New
                            case "new": return Token.New;
                            #endregion

                            #region NotEqual (!=) alternative
                            case "not_eq": return Token.NotEqual;
                            #endregion

                            #region LogicalOr (||) alternative
                            case "or": return Token.LogicalOr;
                            #endregion

                            #region BitwiseOrAssign (|=) alternative
                            case "or_eq": return Token.BitwiseOrAssign;
                            #endregion

                            #region BitwiseXor (^) alternative
                            case "xor": return Token.BitwiseXor;
                            #endregion

                            #region BitwiseXorAssign (^=) alternative
                            case "xor_eq": return Token.BitwiseXorAssign;
                            #endregion

                            #region Identifier
                            default: return Token.Identifier;
                                #endregion
                        }
                    }
                #endregion

                #region Numeric = (['0', '9'] | '.') ((('E' | 'e') ('+' | '-')) | ['a','z'] | ['A','Z'] | ['0','9'] | '_' | '.' | UniversalCharacterName)*;
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

                case ':': switch(PeekCharacter())
                    {
                        #region ScopeResolution (::)
                        case ':':
                            {
                                RawDataBuffer.Position++;
                                return Token.ScopeResolution;
                            }
                        #endregion

                        #region RightBracket ] alternative (:>)
                        case '>':
                            {
                                RawDataBuffer.Position++;
                                return Token.RightSquareBracket;
                            }
                        #endregion

                        #region Colon (:)
                        default:
                            {
                                return Token.Colon;
                            }
                        #endregion
                    }

                case '+': switch(PeekCharacter())
                    {
                        #region Increment (++)
                        case '+':
                            {
                                RawDataBuffer.Position++;
                                return Token.Increment;
                            }
                        #endregion

                        #region AddAssign (+=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.AddAssign;
                            }
                        #endregion

                        #region Add (+)
                        default:
                            {
                                return Token.Add;
                            }
                        #endregion
                    }

                case '-': switch(PeekCharacter())
                    {
                        #region Decrement (--)
                        case '-':
                            {
                                RawDataBuffer.Position++;
                                return Token.Decrement;
                            }
                        #endregion

                        case '>':
                            {
                                RawDataBuffer.Position++;
                                switch (PeekCharacter())
                                {
                                    #region PointerToMember (->*)
                                    case '*':
                                        {
                                            RawDataBuffer.Position++;
                                            return Token.PointerToMember;
                                        }
                                    #endregion

                                    #region Pointer (->)
                                    default:
                                        {
                                            return Token.Pointer;
                                        }
                                    #endregion
                                }
                            }

                        #region SubAssign (-=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.SubAssign;
                            }
                        #endregion

                        #region Sub (-)
                        default:
                            {
                                return Token.Sub;
                            }
                        #endregion
                    }

                #region LeftBrace {
                case '{':
                    {
                        return Token.LeftCurlyBracket;
                    }
                #endregion

                #region RightBrace }
                case '}':
                    {
                        return Token.RightCurlyBracket;
                    }
                #endregion

                #region LeftParentheses (
                case '(':
                    {
                        return Token.LeftRoundBracket;
                    }
                #endregion

                #region RightParentheses )
                case ')':
                    {
                        return Token.RightRoundBracket;
                    }
                #endregion

                #region LeftBracket [
                case '[':
                    {
                        return Token.LeftSquareBracket;
                    }
                #endregion

                #region RightBracket ]
                case ']':
                    {
                        return Token.RightSquareBracket;
                    }
                #endregion

                case '.': switch(PeekCharacter())
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

                        #region ReferenceToMember (.*)
                        case '*':
                            {
                                RawDataBuffer.Position++;
                                return Token.ReferenceToMember;
                            }
                        #endregion

                        #region VariableArgs (...)
                        case '.':
                            {
                                RawDataBuffer.Position++;
                                switch (PeekCharacter())
                                {
                                    case '.':
                                        {
                                            if (PeekCharacter() == '.')
                                            {
                                                RawDataBuffer.Position++;
                                                return Token.VariableArgs;
                                            }
                                            RawDataBuffer.Position--;
                                        }
                                        break;
                                }
                                RawDataBuffer.Position--;
                            }
                            goto default;
                        #endregion

                        #region Dot (.)
                        default:
                            {
                                return Token.Dot;
                            }
                        #endregion
                    }

                case '!': switch(PeekCharacter())
                    {
                        #region NotEqual (!=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.NotEqual;
                            }
                        #endregion

                        #region LogicalNot (!)
                        default:
                            {
                                return Token.LogicalNot;
                            }
                        #endregion
                    }

                #region BitwiseNot (~)
                case '~':
                    {
                        return Token.BitwiseNot;
                    }
                #endregion

                case '*': switch(PeekCharacter())
                    {
                        #region MultAssign (*=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.MultAssign;
                            }
                        #endregion

                        #region Mult (*)
                        default:
                            {
                                return Token.Mult;
                            }
                        #endregion
                    }

                case '%': switch(PeekCharacter())
                    {
                        #region RightBrace } alternative (%>)
                        case '>':
                            {
                                RawDataBuffer.Position++;
                                return Token.RightCurlyBracket;
                            }
                        #endregion

                        case ':':
                            {
                                #region Concatenation (##) alternative (%:%:)
                                RawDataBuffer.Position++;
                                switch (PeekCharacter())
                                {
                                    case '%':
                                        {
                                            RawDataBuffer.Position++;
                                            if (PeekCharacter() == ':')
                                            {
                                                RawDataBuffer.Position++;
                                                return Token.Concat;
                                            }
                                            RawDataBuffer.Position--;
                                        }
                                        break;
                                }
                                #endregion

                                #region Stringify (#) alternative (%:)
                                return Token.Stringify;
                                #endregion
                            }

                        #region ModAssign (%=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.ModAssign;
                            }
                        #endregion

                        #region Mod (%)
                        default:
                            {
                                return Token.Mod;
                            }
                        #endregion
                    }

                case '<': switch(PeekCharacter())
                    {
                        case '<':
                            {
                                RawDataBuffer.Position++;
                                switch (PeekCharacter())
                                {
                                    #region LeftShiftAssign (<<=)
                                    case '=':
                                        {
                                            RawDataBuffer.Position++;
                                            return Token.LeftShiftAssign;
                                        }
                                    #endregion

                                    #region LeftShift (<<)
                                    default:
                                        {
                                            return Token.LeftShift;
                                        }
                                    #endregion
                                }
                            }

                        case '=':
                            {
                                RawDataBuffer.Position++;
                                switch (PeekCharacter())
                                {
                                    #region ThreeWayComparsion (<=>)
                                    case '>':
                                        {
                                            RawDataBuffer.Position++;
                                            return Token.ThreeWayComparsion;
                                        }
                                    #endregion

                                    #region LessEqual (<=)
                                    default:
                                        {
                                            return Token.LessEqual;
                                        }
                                    #endregion
                                }
                            }

                        #region LeftBrace { alternative (<%)
                        case '%':
                            {
                                RawDataBuffer.Position++;
                                return Token.LeftCurlyBracket;
                            }
                        #endregion

                        #region LeftBracket [ alternative (<:)
                        case ':':
                            {
                                RawDataBuffer.Position++;
                                if (PeekCharacter() == ':')
                                {
                                    RawDataBuffer.Position++;
                                    switch (PeekCharacter())
                                    {
                                        case ':':
                                        case '>':
                                            {
                                                RawDataBuffer.Position--;
                                                return Token.LeftSquareBracket;
                                            }
                                        default:
                                            {
                                                RawDataBuffer.Position--;
                                            }
                                            break;
                                    }
                                    RawDataBuffer.Position--;
                                }
                                else return Token.LeftSquareBracket;
                            }
                            goto default;
                        #endregion

                        #region LessThan (<)
                        default:
                            {
                                return Token.LessThan;
                            }
                        #endregion
                    }

                case '>': switch(PeekCharacter())
                    {
                        case '>':
                            {
                                RawDataBuffer.Position++;
                                switch (PeekCharacter())
                                {
                                    #region RightShiftAssign (>>=)
                                    case '=':
                                        {
                                            RawDataBuffer.Position++;
                                            return Token.RightShiftAssign;
                                        }
                                    #endregion

                                    #region RightShift (>>)
                                    default:
                                        {
                                            return Token.RightShift;
                                        }
                                    #endregion
                                }
                            }

                        #region GreaterEqual (>=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.GreaterEqual;
                            }
                        #endregion

                        #region GreaterThan (>)
                        default:
                            {
                                return Token.GreaterThan;
                            }
                        #endregion
                    }

                case '&': switch(PeekCharacter())
                    {
                        #region LogicalAnd (&&)
                        case '&':
                            {
                                RawDataBuffer.Position++;
                                return Token.LogicalAnd;
                            }
                        #endregion

                        #region BitwiseAndAssign (&=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.BitwiseAndAssign;
                            }
                        #endregion

                        #region BitwiseAnd (&)
                        default:
                            {
                                return Token.BitwiseAnd;
                            }
                        #endregion
                    }

                case '^': switch(PeekCharacter())
                    {
                        #region BitwiseXorAssign (^=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.BitwiseXorAssign;
                            }
                        #endregion

                        #region BitwiseXor (^)
                        default:
                            {
                                return Token.BitwiseXor;
                            }
                        #endregion
                    }

                case '|': switch(PeekCharacter())
                    {
                        #region LogicalOr (||)
                        case '|':
                            {
                                RawDataBuffer.Position++;
                                return Token.LogicalOr;
                            }
                        #endregion

                        #region BitwiseOrAssign (|=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.BitwiseOrAssign;
                            }
                        #endregion

                        #region BitwiseOr (|)
                        default:
                            {
                                return Token.BitwiseOr;
                            }
                        #endregion
                    }

                case '=': switch(PeekCharacter())
                    {
                        #region Equal (==)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.Equal;
                            }
                        #endregion

                        #region Assign (=)
                        default:
                            {
                                return Token.Assign;
                            }
                        #endregion
                    }

                case '#': switch(PeekCharacter())
                    {
                        #region Concatenation (##)
                        case '#':
                            {
                                RawDataBuffer.Position++;
                                return Token.Concat;
                            }
                        #endregion

                        #region Stringify (#)
                        default:
                            {
                                return Token.Stringify;
                            }
                        #endregion
                    }

                #region Ternary (?)
                case '?':
                    {
                        return Token.Ternary;
                    }
                #endregion

                #region Semicolon (;)
                case ';':
                    {
                        return Token.Semicolon;
                    }
                #endregion

                #region Comma (,)
                case ',':
                    {
                        return Token.Comma;
                    }
                #endregion

                #region Whitespace = ' ' | '\t' | '\v' | '\f';
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
                                default: return Token.Whitespace;
                            }
                        }
                        while (!EndOfStream);
                    }
                    return Token.Whitespace;
                #endregion

                #region NewLine = '\n';
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
