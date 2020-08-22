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
        /// HTML5 8.2.4.53. DOCTYPE state
        /// https://www.w3.org/TR/html53/syntax.html#doctype-state
        /// </summary>
        bool ReadDoctype()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.54. Before DOCTYPE name state
                case '\t':
                case '\r':
                case '\n':
                case '\f':
                case ' ':
                    {
                        State.Set(HtmlTokenizerState.BeforeDoctypeName);
                    }
                    break;
                #endregion       

                #region Anything else 
                default:
                    {
                        //parser error
                    }
                    goto case ' ';
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.54. Before DOCTYPE name state
        /// https://www.w3.org/TR/html53/syntax.html#before-doctype-name-state
        /// </summary>
        bool ReadBeforeDoctypeName()
        {
            do
            {
                if (EndOfStream)
                    goto Complete;

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

                    #region 8.2.4.1. Data state
                    case '>':
                        {

                        }
                        goto Complete;
                    #endregion

                    #region 8.2.4.55. DOCTYPE name state
                    default:
                        {
                            State.Set(HtmlTokenizerState.DoctypeName);

                            textBuffer.Clear();
                            textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                        #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.BeforeDoctypeName);
            return ProcessingFlags.Continue;

        Complete:
            metaToken.QuirksFlag = true;

            CreateDoctypeToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.55. DOCTYPE name state
        /// https://www.w3.org/TR/html53/syntax.html#doctype-name-state
        /// </summary>
        bool ReadDoctypeName()
        {
            do
            {
                if (EndOfStream)
                {
                    metaToken.Name = textBuffer.ToString();
                    metaToken.QuirksFlag = true;

                    CreateDoctypeToken();
                    return ProcessingFlags.Complete;
                }
                else switch (GetCharacter())
                {
                    #region 8.2.4.56. After DOCTYPE name state
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                        {
                            State.Set(HtmlTokenizerState.AfterDoctypeName);
                            metaToken.Name = textBuffer.ToString();
                        }
                        break;
                    #endregion

                    #region 8.2.4.1. Data state
                    case '>':
                        {
                            metaToken.Name = textBuffer.ToString();
                            CreateDoctypeToken();
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
            while (State.Current == HtmlTokenizerState.DoctypeName);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.56. After DOCTYPE name state
        /// https://www.w3.org/TR/html53/syntax.html#after-doctype-name-state
        /// </summary>
        bool ReadAfterDoctypeName()
        {
            do
            {
                if (EndOfStream)
                {
                    metaToken.QuirksFlag = true;

                    CreateDoctypeToken();
                    return ProcessingFlags.Complete;
                }
                else switch (GetCharacter())
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

                    #region 8.2.4.1. Data state
                    case '>':
                        {
                            CreateDoctypeToken();
                        }
                        return ProcessingFlags.Complete;
                    #endregion

                    #region 8.2.4.57. After DOCTYPE public keyword state
                    case 'P':
                        {
                            if (Rules.Public(this))
                            {
                                State.Set(HtmlTokenizerState.AfterDoctypePublicKeyword);
                                textBuffer.Clear();

                                break;
                            }
                        }
                        goto default;
                    #endregion

                    #region 8.2.4.63. After DOCTYPE system keyword state
                    case 'S':
                        {
                            if (Rules.System(this))
                            {
                                State.Set(HtmlTokenizerState.AfterDoctypeSystemKeyword);
                                textBuffer.Clear();

                                break;
                            }
                        }
                        goto default;
                    #endregion

                    #region 8.2.4.68. Bogus DOCTYPE state
                    default:
                        {
                            State.Set(HtmlTokenizerState.BogusDoctype);
                            metaToken.QuirksFlag = true;
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.AfterDoctypeName);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.57. After DOCTYPE public keyword state
        /// https://www.w3.org/TR/html53/syntax.html#after-doctype-public-keyword-state
        /// </summary>
        bool ReadAfterDoctypePublicKeyword()
        {
            if (EndOfStream)
                goto Complete;

            switch (GetCharacter())
            {
                #region 8.2.4.58. Before DOCTYPE public identifier state
                case '\t':
                case '\r':
                case '\n':
                case '\f':
                case ' ':
                    {
                        State.Set(HtmlTokenizerState.BeforeDoctypePublicIdentifier);
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.59. DOCTYPE public identifier (double-quoted) state
                case '\"':
                #endregion

                #region 8.2.4.60. DOCTYPE public identifier (single-quoted) state
                case '\'':
                    {
                        State.Set(HtmlTokenizerState.DoctypePublicIdentifierQuoted);
                        RawDataBuffer.Position--;

                        currentAttributeName = "PUBLIC";
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.1. Data state
                case '>':
                    {

                    }
                    goto Complete;
                #endregion

                #region 8.2.4.68. Bogus DOCTYPE state
                default:
                    {
                        State.Set(HtmlTokenizerState.BogusDoctype);
                        metaToken.QuirksFlag = true;
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;

        Complete:
            metaToken.QuirksFlag = true;

            CreateDoctypeToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.58. Before DOCTYPE public identifier state
        /// https://www.w3.org/TR/html53/syntax.html#before-doctype-public-identifier-state
        /// </summary>
        bool ReadBeforeDoctypePublicIdentifier()
        {
            do
            {
                if (EndOfStream)
                    goto Complete;

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

                    #region 8.2.4.59. DOCTYPE public identifier (double-quoted) state
                    case '\"':
                    #endregion

                    #region 8.2.4.60. DOCTYPE public identifier (single-quoted) state
                    case '\'':
                        {
                            State.Set(HtmlTokenizerState.DoctypePublicIdentifierQuoted);
                            RawDataBuffer.Position--;

                            currentAttributeName = "PUBLIC";
                            textBuffer.Clear();
                        }
                        break;
                    #endregion

                    #region 8.2.4.1. Data state
                    case '>':
                        {

                        }
                        goto Complete;
                    #endregion

                    #region 8.2.4.68. Bogus DOCTYPE state
                    default:
                        {
                            State.Set(HtmlTokenizerState.BogusDoctype);
                            metaToken.QuirksFlag = true;
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.BeforeDoctypePublicIdentifier);
            return ProcessingFlags.Continue;

        Complete:
            metaToken.QuirksFlag = true;

            CreateDoctypeToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.59. DOCTYPE public identifier (double-quoted) state
        /// https://www.w3.org/TR/html53/syntax.html#doctype-public-identifier-double-quoted-state
        /// 
        /// HTML5 8.2.4.60. DOCTYPE public identifier (single-quoted) state
        /// https://www.w3.org/TR/html53/syntax.html#doctype-public-identifier-single-quoted-state
        /// </summary>
        bool ReadDoctypePublicIdentifierQuoted()
        {
            Char32 quotationMark = GetCharacter();
            do
            {
                if (EndOfStream)
                    goto Complete;

                Char32 c = GetCharacter();
                switch (c)
                {
                    #region U+0000 NULL
                    case 0:
                        {
                            RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                            RawDataBuffer.Position++;
                        }
                        goto default;
                    #endregion

                    #region 8.2.4.1. Data state
                    case '>':
                        {

                        }
                        goto Complete;
                    #endregion

                    #region 8.2.4.61. After DOCTYPE public identifier state
                    default:
                        {
                            if (c == quotationMark)
                            {
                                State.Set(HtmlTokenizerState.AfterDoctypePublicIdentifier);
                                AppendAttribute();
                            }
                            else textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.DoctypePublicIdentifierQuoted);
            return ProcessingFlags.Continue;

        Complete:
            metaToken.QuirksFlag = true;
            
            CreateDoctypeToken();
            AppendAttribute();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.61. After DOCTYPE public identifier state
        /// https://www.w3.org/TR/html53/syntax.html#after-doctype-public-identifier-state
        /// </summary>
        bool ReadAfterDoctypePublicIdentifier()
        {
            if (EndOfStream)
            {
                metaToken.QuirksFlag = true;

                CreateDoctypeToken();
                return ProcessingFlags.Complete;
            }
            else switch (GetCharacter())
            {
                #region 8.2.4.62. Between DOCTYPE public and system identifiers state
                case '\t':
                case '\r':
                case '\n':
                case '\f':
                case ' ':
                    {
                        State.Set(HtmlTokenizerState.BetweenDoctypePublicAndSystemIdentifiers);
                    }
                    break;
                #endregion

                #region 8.2.4.1. Data state
                case '>':
                    {
                        CreateDoctypeToken();
                    }
                    return ProcessingFlags.Complete;
                #endregion

                #region 8.2.4.65. DOCTYPE system identifier (double-quoted) state
                case '\"':
                #endregion

                #region 8.2.4.66. DOCTYPE system identifier (single-quoted) state
                case '\'':
                    {
                        State.Set(HtmlTokenizerState.DoctypeSystemIdentifierQuoted);
                        RawDataBuffer.Position--;

                        currentAttributeName = "SYSTEM";
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.68. Bogus DOCTYPE state
                default:
                    {
                        State.Set(HtmlTokenizerState.BogusDoctype);
                        metaToken.QuirksFlag = true;
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.62. Between DOCTYPE public and system identifiers state
        /// https://www.w3.org/TR/html53/syntax.html#between-doctype-public-and-system-identifiers-state
        /// </summary>
        bool ReadBetweenDoctypePublicAndSystemIdentifiers()
        {
            do
            {
                if (EndOfStream)
                    goto Complete;

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
                    
                    #region 8.2.4.1. Data state
                    case '>':
                        {

                        }
                        goto Complete;
                    #endregion

                    #region 8.2.4.65. DOCTYPE system identifier (double-quoted) state
                    case '\"':
                    #endregion

                    #region 8.2.4.66. DOCTYPE system identifier (single-quoted) state
                    case '\'':
                        {
                            State.Set(HtmlTokenizerState.DoctypeSystemIdentifierQuoted);
                            RawDataBuffer.Position--;

                            currentAttributeName = "SYSTEM";
                            textBuffer.Clear();
                        }
                        break;
                    #endregion

                    #region 8.2.4.68. Bogus DOCTYPE state
                    default:
                        {
                            State.Set(HtmlTokenizerState.BogusDoctype);
                            metaToken.QuirksFlag = true;
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.BetweenDoctypePublicAndSystemIdentifiers);
            return ProcessingFlags.Continue;

        Complete:
            metaToken.QuirksFlag = true;

            CreateDoctypeToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.63. After DOCTYPE system keyword state
        /// https://www.w3.org/TR/html53/syntax.html#after-doctype-system-keyword-state
        /// </summary>
        bool ReadAfterDoctypeSystemKeyword()
        {
            if (EndOfStream)
                goto Complete;

            switch (GetCharacter())
            {
                #region 8.2.4.64. Before DOCTYPE system identifier state
                case '\t':
                case '\r':
                case '\n':
                case '\f':
                case ' ':
                    {
                        State.Set(HtmlTokenizerState.BeforeDoctypeSystemIdentifier);
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.65. DOCTYPE system identifier (double-quoted) state
                case '\"':
                #endregion

                #region 8.2.4.66. DOCTYPE system identifier (single-quoted) state
                case '\'':
                    {
                        State.Set(HtmlTokenizerState.DoctypeSystemIdentifierQuoted);
                        RawDataBuffer.Position--;

                        currentAttributeName = "SYSTEM";
                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.1. Data state
                case '>':
                    {

                    }
                    goto Complete;
                #endregion

                #region 8.2.4.68. Bogus DOCTYPE state
                default:
                    {
                        State.Set(HtmlTokenizerState.BogusDoctype);
                        metaToken.QuirksFlag = true;
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;

        Complete:
            metaToken.QuirksFlag = true;

            CreateDoctypeToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.64. Before DOCTYPE system identifier state
        /// https://www.w3.org/TR/html53/syntax.html#before-doctype-system-identifier-state
        /// </summary>
        bool ReadBeforeDoctypeSystemIdentifier()
        {
            do
            {
                if (EndOfStream)
                    goto Complete;

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

                    #region 8.2.4.65. DOCTYPE system identifier (double-quoted) state
                    case '\"':
                    #endregion

                    #region 8.2.4.66. DOCTYPE system identifier (single-quoted) state
                    case '\'':
                        {
                            State.Set(HtmlTokenizerState.DoctypeSystemIdentifierQuoted);
                            RawDataBuffer.Position--;

                            currentAttributeName = "SYSTEM";
                            textBuffer.Clear();
                        }
                        break;
                    #endregion

                    #region 8.2.4.1. Data state
                    case '>':
                        {

                        }
                        goto Complete;
                    #endregion

                    #region 8.2.4.68. Bogus DOCTYPE state
                    default:
                        {
                            State.Set(HtmlTokenizerState.BogusDoctype);
                            metaToken.QuirksFlag = true;
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.BeforeDoctypeSystemIdentifier);
            return ProcessingFlags.Continue;

        Complete:
            metaToken.QuirksFlag = true;

            CreateDoctypeToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.65. DOCTYPE system identifier (double-quoted) state
        /// https://www.w3.org/TR/html53/syntax.html#doctype-system-identifier-double-quoted-state
        /// 
        /// HTML5 8.2.4.66. DOCTYPE system identifier (single-quoted) state
        /// https://www.w3.org/TR/html53/syntax.html#doctype-system-identifier-single-quoted-state
        /// </summary>
        bool ReadDoctypeSystemIdentifierQuoted()
        {
            Char32 quotationMark = GetCharacter();
            do
            {
                if (EndOfStream)
                    goto Complete;

                Char32 c = GetCharacter();
                switch (c)
                {
                    #region U+0000 NULL
                    case 0:
                        {
                            RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                            RawDataBuffer.Position++;
                        }
                        goto default;
                    #endregion

                    #region 8.2.4.1. Data state
                    case '>':
                        {

                        }
                        goto Complete;
                    #endregion

                    #region 8.2.4.67. After DOCTYPE system identifier state
                    default:
                        {
                            if (c == quotationMark)
                            {
                                State.Set(HtmlTokenizerState.AfterDoctypeSystemIdentifier);
                                AppendAttribute();
                            }
                            else textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.DoctypeSystemIdentifierQuoted);
            return ProcessingFlags.Continue;

        Complete:
            metaToken.QuirksFlag = true;
            
            CreateDoctypeToken();
            AppendAttribute();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.67. After DOCTYPE system identifier state
        /// https://www.w3.org/TR/html53/syntax.html#after-doctype-system-identifier-state
        /// </summary>
        bool ReadAfterDoctypeSystemIdentifier()
        {
            if (EndOfStream)
            {
                metaToken.QuirksFlag = true;

                CreateDoctypeToken();
                return ProcessingFlags.Complete;
            }
            else switch (GetCharacter())
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

                #region 8.2.4.1. Data state
                case '>':
                    {
                        CreateDoctypeToken();
                    }
                    return ProcessingFlags.Complete;
                #endregion

                #region 8.2.4.68. Bogus DOCTYPE state
                default:
                    {
                        State.Set(HtmlTokenizerState.BogusDoctype);
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.67. After DOCTYPE system identifier state
        /// https://www.w3.org/TR/html53/syntax.html#after-doctype-system-identifier-state
        /// </summary>
        bool ReadBogusDoctype()
        {
            do
            {
                if (EndOfStream)
                    break;

                switch (GetCharacter())
                {
                    #region 8.2.4.1. Data state
                    case '>':
                        {
                            State.Set(HtmlTokenizerState.Data);
                        }
                        break;
                    #endregion

                    #region Anything else
                    default:
                        {

                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.BogusDoctype);

            CreateDoctypeToken();
            return ProcessingFlags.Complete;
        }
    }
}
