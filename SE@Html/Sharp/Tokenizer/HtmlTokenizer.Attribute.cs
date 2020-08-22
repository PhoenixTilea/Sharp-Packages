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
        ///<summary>
        ///HTML5 8.2.4.32. Before attribute name state
        ///https://www.w3.org/TR/html53/syntax.html#before-attribute-name-state
        ///</summary>
        bool ReadBeforeAttributeName()
        {
            do
            {
                if (EndOfStream)
                {
                    State.Set(HtmlTokenizerState.AfterAttributeName);
                    currentAttributeName = string.Empty;
                    break;
                }

                switch (PeekCharacter())
                {
                    #region Ignore the character
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                        {
                            RawDataBuffer.Position++;
                        }
                        break;
                    #endregion

                    #region 8.2.4.34. After attribute name state
                    case '/':
                    case '>':
                        {
                            State.Set(HtmlTokenizerState.AfterAttributeName);
                            currentAttributeName = string.Empty;
                        }
                        break;
                    #endregion

                    #region 8.2.4.33. Attribute name state
                    case '=':
                        {
                            State.Set(HtmlTokenizerState.AttributeName);
                            RawDataBuffer.Position++;

                            textBuffer.Clear();
                            textBuffer.Append('=');
                        }
                        break;
                    default:
                        {
                            State.Set(HtmlTokenizerState.AttributeName);
                            textBuffer.Clear();
                        }
                        break;
                        #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.BeforeAttributeName);
            return ProcessingFlags.Continue;
        }

        ///<summary>
        ///HTML5 8.2.4.33. Attribute name state
        ///https://www.w3.org/TR/html53/syntax.html#attribute-name-state
        ///</summary>
        bool ReadAttributeName()
        {
            do
            {
                if (EndOfStream)
                {
                    State.Set(HtmlTokenizerState.AfterAttributeName);
                    currentAttributeName = textBuffer.ToString();
                    break;
                }

                switch (GetCharacter())
                {
                    #region 8.2.4.34. After attribute name state
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                    case '/':
                    case '>':
                        {
                            State.Set(HtmlTokenizerState.AfterAttributeName);
                            currentAttributeName = textBuffer.ToString();
                            RawDataBuffer.Position--;
                        }
                        break;
                    #endregion

                    #region 8.2.4.35. Before attribute value state
                    case '=':
                        {
                            State.Set(HtmlTokenizerState.BeforeAttributeValue);
                            currentAttributeName = textBuffer.ToString();
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
                    case '\"':
                    case '\'':
                    case '<':
                        {
                            //Parser error
                        }
                        goto default;
                    default:
                        {
                            textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.AttributeName);
            return ProcessingFlags.Continue;
        }

        ///<summary>
        ///HTML5 8.2.4.34. After attribute name state
        ///https://www.w3.org/TR/html53/syntax.html#after-attribute-name-state
        ///</summary>
        bool ReadAfterAttributeName()
        {
            do
            {
                if (EndOfStream)
                {
                    State.Set(HtmlTokenizerState.Data);
                    break;
                }
                switch (GetCharacter())
                {
                    #region Ignore the character
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                        {

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

                    #region 8.2.4.35. Before attribute value state
                    case '=':
                        {
                            State.Set(HtmlTokenizerState.BeforeAttributeValue);
                            currentAttributeName = textBuffer.ToString();
                        }
                        break;
                    #endregion

                    #region 8.2.4.1. Data state
                    case '>':
                        {

                        }
                        return ProcessingFlags.Complete;
                    #endregion

                    #region 8.2.4.33. Attribute name state
                    default:
                        {
                            State.Set(HtmlTokenizerState.AttributeName);
                            RawDataBuffer.Position--;

                            textBuffer.Clear();
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.BeforeAttributeName);
            return ProcessingFlags.Continue;
        }

        ///<summary>
        ///HTML5 8.2.4.35. Before attribute value state
        ///https://www.w3.org/TR/html53/syntax.html#before-attribute-value-state
        ///</summary>
        bool ReadBeforeAttributeValue()
        {
            do
            {
                switch (PeekCharacter())
                {
                    #region Ignore the character
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                        {
                            RawDataBuffer.Position++;
                        }
                        break;
                    #endregion

                    #region 8.2.4.36. Attribute value (double-quoted) state
                    case '\"':
                    #endregion

                    #region 8.2.4.37. Attribute value (single-quoted) state
                    case '\'':

                        {
                            State.Set(HtmlTokenizerState.AttributeValueQuoted);
                            textBuffer.Clear();
                        }
                        break;
                    #endregion

                    #region 8.2.4.38. Attribute value (unquoted) state
                    case '>':
                        {
                            //parser error
                        }
                        goto default;
                    default:
                        {
                            State.Set(HtmlTokenizerState.AttributeValueUnquoted);
                            textBuffer.Clear();
                        }
                        break;
                        #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.BeforeAttributeValue);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.36. Attribute value (double-quoted) state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-attribute-value-double-quoted-state
        /// 
        /// HTML5 8.2.4.37. Attribute value (single-quoted) state
        /// https://www.w3.org/TR/html53/syntax.html#attribute-value-single-quoted-state
        /// </summary>
        bool ReadAttributeValueQuoted()
        {
            Char32 quotationMark = GetCharacter();
            do
            {
                if(EndOfStream)
                {
                    State.Set(HtmlTokenizerState.AfterAttributeValueQuoted);
                    break;
                }

                Char32 c = GetCharacter();
                switch (c)
                {
                    #region 8.2.4.72. Character reference state
                    case '&':
                        {
                            State.Add(HtmlTokenizerState.CharacterReference);
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

                    #region 8.2.4.39. After attribute value (quoted) state
                    default:
                        {
                            if (c == quotationMark)
                            {
                                State.Set(HtmlTokenizerState.AfterAttributeValueQuoted);
                                AppendAttribute();
                            }
                            else textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.AttributeValueQuoted);
            return ProcessingFlags.Continue;
        }

        ///<summary>
        ///HTML5 8.2.4.38. Attribute value (unquoted) state
        ///https://www.w3.org/TR/html53/syntax.html#attribute-value-unquoted-state
        ///</summary>
        bool ReadAttributeValueUnquoted()
        {
            do
            {
                if (EndOfStream)
                {
                    State.Set(HtmlTokenizerState.AfterAttributeName);
                    AppendAttribute();

                    break;
                }
                switch (GetCharacter())
                {
                    #region 8.2.4.32. Before attribute name state
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                        {
                            State.Set(HtmlTokenizerState.BeforeAttributeName);
                        }
                        break;
                    #endregion

                    #region 8.2.4.72. Character reference state
                    case '&':
                        {
                            State.Add(HtmlTokenizerState.CharacterReference);
                        }
                        break;
                    #endregion

                    #region 8.2.4.34. After attribute name state
                    case '>':
                        {
                            State.Set(HtmlTokenizerState.AfterAttributeName);
                            AppendAttribute();

                            RawDataBuffer.Position--;
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

                    #region Anything else
                    case '\"':
                    case '\'':
                    case '<':
                    case '=':
                    case '`':
                        {
                            //Parser error
                        }
                        goto default;
                    default:
                        {
                            textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.AttributeValueUnquoted);
            return ProcessingFlags.Continue;
        }

        ///<summary>
        ///HTML5 8.2.4.39. After attribute value (quoted) state
        ///https://www.w3.org/TR/html53/syntax.html#after-attribute-value-quoted-state
        ///</summary>
        bool ReadAfterAttributeValueQuoted()
        {
            if (EndOfStream)
            {
                State.Set(HtmlTokenizerState.Data);
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

                    }
                    return ProcessingFlags.Complete;
                #endregion

                #region Anything else
                default:
                    {
                        State.Set(HtmlTokenizerState.BeforeAttributeName);
                        RawDataBuffer.Position--;
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }
    }
}