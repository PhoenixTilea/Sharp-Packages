// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Html
{
    public partial class HtmlTokenizer
    {
        /// <summary>
        /// HTML5 8.2.4.6. Tag open state
        /// https://www.w3.org/TR/html53/syntax.html#tag-open-state
        /// </summary>
        bool ReadTagOpen()
        {
            switch (GetCharacter())
            {
                #region 8.2.4.42. Markup declaration open state
                case '!':
                    {
                        State.Set(HtmlTokenizerState.MarkupDeclarationOpen);
                    }
                    break;
                #endregion

                #region 8.2.4.41. Bogus comment state
                case '?':
                    {
                        //XML comment
                        State.Set(HtmlTokenizerState.BogusComment);
                        RawDataBuffer.Position--;

                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.7. End tag open state
                case '/':
                    {
                        State.Set(HtmlTokenizerState.EndTagOpen);
                    }
                    break;
                #endregion

                #region 8.2.4.8. Tag name state
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
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                    {
                        State.Set(HtmlTokenizerState.TagName);
                        RawDataBuffer.Position--;

                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.1. Data state 
                default:
                    {
                        State.Set(HtmlTokenizerState.Data);
                        Char32 c = RawDataBuffer.Replace((int)'<');
                        if (!EndOfStream)
                        {
                            RawDataBuffer.Buffer.Add(c);
                        }
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.7. End tag open state
        /// https://www.w3.org/TR/html53/syntax.html#end-tag-open-state
        /// </summary>
        bool ReadEndTagOpen()
        {
            switch (GetCharacter())
            {
                #region 8.2.4.8. Tag name state
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
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                    {
                        metaToken.Type = HtmlToken.CloseTag;

                        State.Set(HtmlTokenizerState.TagName);
                        RawDataBuffer.Position--;

                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.1. Data state
                case '>':
                    {
                        State.Set(HtmlTokenizerState.Data);
                    }
                    break;
                #endregion

                #region 8.2.4.41. Bogus comment state
                default:
                    {
                        if (EndOfStream)
                        {
                            State.Set(HtmlTokenizerState.Data);
                            Char32 c = RawDataBuffer.Replace((int)'<');
                            RawDataBuffer.Buffer.Add(c);
                        }
                        else
                        {
                            State.Set(HtmlTokenizerState.BogusComment);
                            textBuffer.Clear();
                        }
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.8. Tag name state
        /// https://www.w3.org/TR/html53/syntax.html#tag-name-state
        /// </summary>
        bool ReadTagName()
        {
            do
            {
                if (EndOfStream)
                {
                    CreateDataToken();
                    return ProcessingFlags.Complete;
                }
                else switch (GetCharacter())
                {
                    #region 8.2.4.32. Before attribute name state
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                        {
                            State.Set(HtmlTokenizerState.BeforeAttributeName);
                            CreateTagToken();
                        }
                        break;
                    #endregion

                    #region 8.2.4.40. Self-closing start tag state
                    case '/':
                        {
                            State.Set(HtmlTokenizerState.SelfClosingStartTag);
                        }
                        break;
                    #endregion

                    #region 8.2.4.1. Data state
                    case '>':
                        {
                            CreateTagToken();
                        }
                        return ProcessingFlags.Complete;
                    #endregion

                    #region Uppercase ASCII letter
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
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                        {
                            Char32 c = RawDataBuffer.Head + 0x0020;
                            RawDataBuffer.Replace(c);
                        }
                        goto default;
                    #endregion

                    #region U+0000 NULL
                    case 0:
                        {
                            RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                            RawDataBuffer.Position++;
                        }
                        goto default;
                    #endregion

                    #region Anything else 
                    default:
                        {
                            textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.TagName);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.40. Self-closing start tag state
        /// https://www.w3.org/TR/html53/syntax.html#self-closing-start-tag-state
        /// </summary>
        bool ReadSelfClosingStartTag()
        {
            if (EndOfStream)
            {
                State.Set(HtmlTokenizerState.Data);
            }
            else switch (PeekCharacter())
            {
                #region 8.2.4.1. Data state
                case '>':
                    {
                        metaToken.Type = HtmlToken.SelfCloseTag;
                        RawDataBuffer.Position++; 

                        CreateTagToken();
                    }
                    return ProcessingFlags.Complete;
                #endregion

                #region 8.2.4.32. Before attribute name state
                default:
                    {
                        State.Set(HtmlTokenizerState.BeforeAttributeName);
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.42. Markup declaration open state
        /// https://www.w3.org/TR/html53/syntax.html#markup-declaration-open-state
        /// </summary>
        bool ReadMarkupDeclarationOpen()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.43. Comment start state
                case '-':
                    {
                        GetCharacter();
                        if (PeekCharacter() == '-')
                        {
                            State.Set(HtmlTokenizerState.CommentStart);
                            RawDataBuffer.Position++;

                            textBuffer.Clear();
                            break;
                        }
                        else RawDataBuffer.Position--;
                    }
                    goto default;
                #endregion

                #region 8.2.4.53. DOCTYPE state
                case 'd':
                case 'D':
                    {
                        long streamPos = RawDataBuffer.Position;
                        if (Rules.Doctype(this))
                        {
                            State.Set(HtmlTokenizerState.Doctype);
                            textBuffer.Clear();

                            break;
                        }
                        else RawDataBuffer.Position = streamPos;
                    }
                    goto default;
                #endregion

                #region 8.2.4.69. CDATA section state
                case '[':
                    {
                        long streamPos = RawDataBuffer.Position;
                        if (Rules.CDataSection(this))
                        {
                            State.Set(HtmlTokenizerState.CDataSection);
                            textBuffer.Clear();

                            break;
                        }
                        else RawDataBuffer.Position = streamPos;
                    }
                    goto default;
                #endregion

                #region 8.2.4.41. Bogus comment state
                default:
                    {
                        State.Set(HtmlTokenizerState.BogusComment);
                        textBuffer.Clear();
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }
    }
}
