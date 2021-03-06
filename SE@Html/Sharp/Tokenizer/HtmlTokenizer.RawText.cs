﻿// Copyright (C) 2017 Schroedinger Entertainment
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
        /// HTML5 8.2.4.12. RAWTEXT less-than sign state
        /// https://www.w3.org/TR/html53/syntax.html#rawtext-less-than-sign-state
        /// </summary>
        bool ReadRawTextLessThan()
        {
            switch (GetCharacter())
            {
                #region 8.2.4.13. RAWTEXT end tag open state
                case '/':
                    {
                        State.Set(HtmlTokenizerState.RawTextEndTagOpen);
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.3. RAWTEXT state
                default:
                    {
                        State.Set(HtmlTokenizerState.RawText);
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
        /// HTML5 8.2.4.13. RAWTEXT end tag open state
        /// https://www.w3.org/TR/html53/syntax.html#rawtext-end-tag-open-state
        /// </summary>
        bool ReadRawTextEndTagOpen()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.14. RAWTEXT end tag name state
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

                        State.Set(HtmlTokenizerState.RawTextEndTagName);
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.3. RAWTEXT state
                default:
                    {
                        State.Set(HtmlTokenizerState.RawText);
                        RawDataBuffer.Buffer.Insert(0, (int)'<');
                        RawDataBuffer.Position++;
                    }
                    break;
                    #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.14. RAWTEXT end tag name state
        /// https://www.w3.org/TR/html53/syntax.html#rawtext-end-tag-name-state
        /// </summary>
        bool ReadRawTextEndTagName()
        {
            do
            {
                switch (GetCharacter())
                {
                    #region 8.2.4.32. Before attribute name state
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                        {
                            if (textBuffer.IsEqual(currentStartTag))
                            {
                                State.Set(HtmlTokenizerState.BeforeAttributeName);
                                CreateTagToken();
                            }
                            else goto default;
                        }
                        break;
                    #endregion

                    #region 8.2.4.40. Self-closing start tag state
                    case '/':
                        {
                            if (textBuffer.IsEqual(currentStartTag))
                            {
                                State.Set(HtmlTokenizerState.SelfClosingStartTag);
                            }
                            else goto default;
                        }
                        break;
                    #endregion

                    #region 8.2.4.1. Data state
                    case '>':
                        {
                            if (textBuffer.IsEqual(currentStartTag))
                            {
                                CreateTagToken();
                            }
                            else goto default;
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
                        goto case 'a';
                    #endregion

                    #region Lowercase ASCII letter
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
                        {
                            textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                    #endregion

                    #region 8.2.4.3. RAWTEXT state
                    default:
                        {
                            State.Set(HtmlTokenizerState.RawText);
                            RawDataBuffer.Buffer.Insert(0, (int)'<');
                            RawDataBuffer.Position++;
                        }
                        break;
                        #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.RawTextEndTagName);
            return ProcessingFlags.Continue;
        }
    }
}
