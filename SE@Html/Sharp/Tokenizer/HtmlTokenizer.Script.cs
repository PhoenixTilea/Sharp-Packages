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
        /// HTML5 8.2.4.15. Script data less-than sign state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-less-than-sign-state
        /// </summary>
        bool ReadScriptDataLessThan()
        {
            switch (GetCharacter())
            {
                #region 8.2.4.16. Script data end tag open state
                case '/':
                    {
                        State.Set(HtmlTokenizerState.ScriptDataEndTagOpen);
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.18. Script data escape start state
                case '!':
                    {
                        State.Set(HtmlTokenizerState.ScriptDataEscapeStart);
                        Char32 c = RawDataBuffer.Replace((int)'<');
                        RawDataBuffer.Buffer.Add(c);
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion

                #region 8.2.4.4. Script data state
                default:
                    {
                        State.Set(HtmlTokenizerState.ScriptData);
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
        /// HTML5 8.2.4.16. Script data end tag open state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-end-tag-open-state
        /// </summary>
        bool ReadScriptDataEndTagOpen()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.17. Script data end tag name state
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

                        State.Set(HtmlTokenizerState.ScriptDataEndTagName);
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.4. Script data state
                default:
                    {
                        State.Set(HtmlTokenizerState.ScriptData);
                        RawDataBuffer.Buffer.Insert(0, (int)'<');
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.17. Script data end tag name state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-end-tag-name-state
        /// </summary>
        bool ReadScriptDataEndTagName()
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

                    #region 8.2.4.4. Script data state
                    default:
                        {
                            State.Set(HtmlTokenizerState.ScriptData);
                            RawDataBuffer.Buffer.Insert(0, (int)'<');
                            RawDataBuffer.Position++;
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.ScriptDataEndTagName);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.18. Script data escape start state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-escape-start-state
        /// </summary>
        bool ReadScriptDataEscapeStart()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.19. Script data escape start dash state
                case '-':
                    {
                        State.Set(HtmlTokenizerState.ScriptDataEscapeStartDash);
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion

                #region 8.2.4.4. Script data state
                default:
                    {
                        State.Set(HtmlTokenizerState.ScriptData);
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.19. Script data escape start dash state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-escape-start-dash-state
        /// </summary>
        bool ReadScriptDataEscapeStartDash()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.22. Script data escaped dash dash state
                case '-':
                    {
                        State.Set(HtmlTokenizerState.ScriptDataEscapedDashDash);
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion

                #region 8.2.4.4. Script data state
                default:
                    {
                        State.Set(HtmlTokenizerState.ScriptData);
                    }
                    break;
                    #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.20. Script data escaped state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-escaped-state
        /// </summary>
        bool ReadScriptDataEscaped()
        {
            do
            {
                if (EndOfStream)
                {
                    State.Set(HtmlTokenizerState.ScriptData);
                    break;
                }

                switch (GetCharacter())
                {
                    #region 8.2.4.21. Script data escaped dash state
                    case '-':
                        {
                            State.Set(HtmlTokenizerState.ScriptDataEscapedDash);
                        }
                        break;
                    #endregion

                    #region 8.2.4.23. Script data escaped less-than sign state
                    case '<':
                        {
                            State.Set(HtmlTokenizerState.ScriptDataEscapedLessThan);
                            RawDataBuffer.Discard(1);
                        }
                        break;
                    #endregion

                    #region U+0000 NULL
                    case 0:
                        {
                            RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                            RawDataBuffer.Position++;
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.ScriptDataEscaped);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.21. Script data escaped dash state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-escaped-dash-state
        /// </summary>
        bool ReadScriptDataEscapedDash()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.22. Script data escaped dash dash state
                case '-':
                    {
                        State.Set(HtmlTokenizerState.ScriptDataEscapedDashDash);
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion

                #region 8.2.4.23. Script data escaped less-than sign state
                case '<':
                    {
                        State.Set(HtmlTokenizerState.ScriptDataEscapedLessThan);
                        RawDataBuffer.Discard(1);
                    }
                    break;
                #endregion

                #region U+0000 NULL
                case 0:
                    {
                        RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                    }
                    goto default;
                #endregion

                #region 8.2.4.20. Script data escaped state
                default:
                    {
                        State.Set(HtmlTokenizerState.ScriptDataEscaped);
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.22. Script data escaped dash dash state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-escaped-dash-dash-state
        /// </summary>
        bool ReadScriptDataEscapedDashDash()
        {
            do
            {
                if (EndOfStream)
                {
                    State.Set(HtmlTokenizerState.ScriptData);
                    break;
                }

                switch (GetCharacter())
                {
                    #region U+002D HYPHEN-MINUS (-)
                    case '-':
                        {

                        }
                        break;
                    #endregion

                    #region 8.2.4.23. Script data escaped less-than sign state
                    case '<':
                        {
                            State.Set(HtmlTokenizerState.ScriptDataEscapedLessThan);
                            RawDataBuffer.Discard(1);
                        }
                        break;
                    #endregion

                    #region 8.2.4.4. Script data state
                    case '>':
                        {
                            State.Set(HtmlTokenizerState.ScriptData);
                        }
                        break;
                    #endregion

                    #region U+0000 NULL
                    case 0:
                        {
                            RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                            RawDataBuffer.Position++;
                        }
                        goto default;
                    #endregion

                    #region 8.2.4.20. Script data escaped state
                    default:
                        {
                            State.Set(HtmlTokenizerState.ScriptDataEscaped);
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.ScriptDataEscapedDashDash);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.23. Script data escaped less-than sign state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-escaped-less-than-sign-state
        /// </summary>
        bool ReadScriptDataEscapedLessThan()
        {
            switch (GetCharacter())
            {
                #region 8.2.4.24. Script data escaped end tag open state
                case '/':
                    {
                        State.Set(HtmlTokenizerState.ScriptDataEscapedEndTagOpen);
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.26. Script data double escape start state
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
                        State.Set(HtmlTokenizerState.ScriptDataDoubleEscapeStart);
                        Char32 c = RawDataBuffer.Replace((int)'<');
                        RawDataBuffer.Buffer.Add(c);

                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.20. Script data escaped state
                default:
                    {
                        State.Set(HtmlTokenizerState.ScriptDataEscaped);
                        Char32 c = RawDataBuffer.Replace((int)'<');
                        RawDataBuffer.Buffer.Add(c);
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.24. Script data escaped end tag open state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-escaped-end-tag-open-state
        /// </summary>
        bool ReadScriptDataEscapedEndTagOpen()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.25. Script data escaped end tag name state
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
                        State.Set(HtmlTokenizerState.ScriptDataEscapedEndTagName);
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.20. Script data escaped state
                default:
                    {
                        State.Set(HtmlTokenizerState.ScriptDataEscaped);
                        RawDataBuffer.Buffer.Insert(RawDataBuffer.Buffer.Count - 2, (int)'<');
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.25. Script data escaped end tag name state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-escaped-end-tag-name-state
        /// </summary>
        bool ReadScriptDataEscapedEndTagName()
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

                        }
                        goto case '>';
                    #endregion

                    #region 8.2.4.40. Self-closing start tag state
                    case '/':
                        {
                            RawDataBuffer.Discard(1);
                            BaseStream.Position--;
                        }
                        goto case '>';
                    #endregion

                    #region 8.2.4.1. Data state
                    case '>':
                        {
                            if (textBuffer.IsEqual(currentStartTag))
                            {
                                int count = RawDataBuffer.Buffer.Count;
                                while (RawDataBuffer.Head != '/')
                                    RawDataBuffer.Discard(1);

                                RawDataBuffer.Discard(1);
                                count++;

                                BaseStream.Position -= (count - RawDataBuffer.Buffer.Count);
                                State.Set(HtmlTokenizerState.ScriptData);
                            }
                            else goto default;
                        }
                        break;
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

                    #region 8.2.4.20. Script data escaped state
                    default:
                        {
                            State.Set(HtmlTokenizerState.ScriptDataEscaped);
                            RawDataBuffer.Buffer.Insert(RawDataBuffer.Buffer.Count - textBuffer.Length - 2, (int)'<');
                            RawDataBuffer.Position++;
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.ScriptDataEscapedEndTagName);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.26. Script data double escape start state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-double-escape-start-state
        /// </summary>
        bool ReadScriptDataDoubleEscapeStart()
        {
            do
            {
                switch (GetCharacter())
                {
                    #region 8.2.4.27. Script data double escaped state
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                    case '/':
                    case '>':
                        {
                            if (textBuffer.IsEqual("script"))
                            {
                                State.Set(HtmlTokenizerState.ScriptDataDoubleEscaped);
                            }
                            else State.Set(HtmlTokenizerState.ScriptDataEscaped);
                        }
                        break;
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

                    #region 8.2.4.20. Script data escaped state
                    default:
                        {
                            State.Set(HtmlTokenizerState.ScriptDataEscaped);
                            RawDataBuffer.Position--;
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.ScriptDataDoubleEscapeStart);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.27. Script data double escaped state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-script-data-double-escaped-state
        /// </summary>
        bool ReadScriptDataDoubleEscaped()
        {
            do
            {
                if (EndOfStream)
                {
                    State.Set(HtmlTokenizerState.ScriptData);
                    break;
                }

                switch (GetCharacter())
                {
                    #region 8.2.4.28. Script data double escaped dash state
                    case '-':
                        {
                            State.Set(HtmlTokenizerState.ScriptDataDoubleEscapedDash);
                        }
                        break;
                    #endregion

                    #region 8.2.4.30. Script data double escaped less-than sign state
                    case '<':
                        {
                            State.Set(HtmlTokenizerState.ScriptDataDoubleEscapedLessThan);
                        }
                        break;
                    #endregion

                    #region U+0000 NULL
                    case 0:
                        {
                            RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                            RawDataBuffer.Position++;
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.ScriptDataDoubleEscaped);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.28. Script data double escaped dash state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-double-escaped-dash-state
        /// </summary>
        bool ReadScriptDataDoubleEscapedDash()
        {
            switch (GetCharacter())
            {
                #region 8.2.4.22. Script data escaped dash dash state
                case '-':
                    {
                        State.Set(HtmlTokenizerState.ScriptDataDoubleEscapedDashDash);
                    }
                    break;
                #endregion

                #region 8.2.4.23. Script data escaped less-than sign state
                case '<':
                    {
                        State.Set(HtmlTokenizerState.ScriptDataDoubleEscapedLessThan);
                    }
                    break;
                #endregion

                #region U+0000 NULL
                case 0:
                    {
                        RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                    }
                    goto default;
                #endregion

                #region 8.2.4.20. Script data escaped state
                default:
                    {
                        State.Set(HtmlTokenizerState.ScriptDataDoubleEscaped);
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.29. Script data double escaped dash dash state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-double-escaped-dash-dash-state
        /// </summary>
        bool ReadScriptDataDoubleEscapedDashDash()
        {
            do
            {
                if (EndOfStream)
                {
                    State.Set(HtmlTokenizerState.ScriptData);
                    break;
                }

                switch (GetCharacter())
                {
                    #region U+002D HYPHEN-MINUS (-)
                    case '-':
                        {

                        }
                        break;
                    #endregion

                    #region 8.2.4.30. Script data double escaped less-than sign state
                    case '<':
                        {
                            State.Set(HtmlTokenizerState.ScriptDataDoubleEscapedLessThan);
                        }
                        break;
                    #endregion

                    #region 8.2.4.4. Script data state
                    case '>':
                        {
                            State.Set(HtmlTokenizerState.ScriptData);
                        }
                        break;
                    #endregion

                    #region U+0000 NULL
                    case 0:
                        {
                            RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                            RawDataBuffer.Position++;
                        }
                        goto default;
                    #endregion

                    #region 8.2.4.27. Script data double escaped state
                    default:
                        {
                            State.Set(HtmlTokenizerState.ScriptDataDoubleEscaped);
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.ScriptDataDoubleEscapedDashDash);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.30. Script data double escaped less-than sign state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-double-escaped-less-than-sign-state
        /// </summary>
        bool ReadScriptDataDoubleEscapedLessThan()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.31. Script data double escape end state
                case '/':
                    {
                        State.Set(HtmlTokenizerState.ScriptDataDoubleEscapeEnd);
                        RawDataBuffer.Position++;

                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.27. Script data double escaped state
                default:
                    {
                        State.Set(HtmlTokenizerState.ScriptDataDoubleEscaped);
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.31. Script data double escape end state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-double-escape-end-state
        /// </summary>
        bool ReadScriptDataDoubleEscapeEnd()
        {
            do
            {
                switch (GetCharacter())
                {
                    #region 8.2.4.20. Script data escaped state
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                    case '/':
                    case '>':
                        {
                            if (textBuffer.IsEqual("script"))
                            {
                                State.Set(HtmlTokenizerState.ScriptDataEscaped);
                            }
                            else State.Set(HtmlTokenizerState.ScriptDataDoubleEscaped);
                        }
                        break;
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

                    #region 8.2.4.27. Script data double escaped state
                    default:
                        {
                            State.Set(HtmlTokenizerState.ScriptDataDoubleEscaped);
                            RawDataBuffer.Position--;
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.ScriptDataDoubleEscapeEnd);
            return ProcessingFlags.Continue;
        }
    }
}
