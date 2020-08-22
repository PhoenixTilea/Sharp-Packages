// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Html
{
    public partial class HtmlTokenizer
    {
        /// <summary>
        /// HTML5 8.2.4.1. Data state
        /// https://www.w3.org/TR/html53/syntax.html#data-state
        /// </summary>
        bool ReadData()
        {
            do
            {
                if(EndOfStream)
                    break;

                switch (GetCharacter())
                {
                    #region 8.2.4.72. Character reference state
                    case '&':
                        {
                            State.Add(HtmlTokenizerState.CharacterReference);
                        }
                        return ProcessingFlags.Continue;
                    #endregion

                    #region 8.2.4.6. Tag open state
                    case '<':
                        {
                            State.Set(HtmlTokenizerState.TagOpen);
                            RawDataBuffer.Discard(1);
                        }
                        break;
                    #endregion

                    #region 8.2.4.1. Data state
                    case 0:
                        {
                            //parser error
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.Data);

            CreateDataToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.2. RCDATA state
        /// https://www.w3.org/TR/html53/syntax.html#RCDATA-state
        /// </summary>
        bool ReadRcData()
        {
            metaToken.Type = HtmlToken.Data;
            do
            {
                if (EndOfStream)
                    break;

                switch (GetCharacter())
                {
                    #region 8.2.4.72. Character reference state
                    case '&':
                        {
                            State.Add(HtmlTokenizerState.CharacterReference);
                        }
                        return ProcessingFlags.Continue;
                    #endregion

                    #region 8.2.4.9. RCDATA less-than sign state
                    case '<':
                        {
                            State.Set(HtmlTokenizerState.RcDataLessThan);
                            RawDataBuffer.Discard(1);
                        }
                        break;
                    #endregion

                    #region 8.2.4.2. RCDATA state
                    case 0:
                        {
                            //parser error
                            RawDataBuffer.Replace(HtmlEncoding.ReplacementCharacter);
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.RcData);

            CreateDataToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.3. RAWTEXT state
        /// https://www.w3.org/TR/html53/syntax.html#rawtext-state
        /// </summary>
        bool ReadRawText()
        {
            metaToken.Type = HtmlToken.Data;
            do
            {
                if (EndOfStream)
                    break;

                switch (GetCharacter())
                {
                    #region 8.2.4.12. RAWTEXT less-than sign state
                    case '<':
                        {
                            State.Set(HtmlTokenizerState.RawTextLessThan);
                            RawDataBuffer.Discard(1);
                        }
                        break;
                    #endregion

                    #region 8.2.4.3. RAWTEXT state
                    case 0:
                        {
                            //parser error
                            RawDataBuffer.Replace(HtmlEncoding.ReplacementCharacter);
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.RawText);

            CreateDataToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.4. Script data state
        /// https://www.w3.org/TR/html53/syntax.html#script-data-state
        /// </summary>
        bool ReadScriptData()
        {
            metaToken.Type = HtmlToken.Data;
            do
            {
                if (EndOfStream)
                    break;

                switch (GetCharacter())
                {
                    #region 8.2.4.15. Script data less-than sign state
                    case '<':
                        {
                            State.Set(HtmlTokenizerState.ScriptDataLessThan);
                            RawDataBuffer.Discard(1);
                        }
                        break;
                    #endregion

                    #region 8.2.4.4. Script data state
                    case 0:
                        {
                            //parser error
                            RawDataBuffer.Replace(HtmlEncoding.ReplacementCharacter);
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.ScriptData);

            CreateDataToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.5. PLAINTEXT state
        /// https://www.w3.org/TR/html53/syntax.html#plaintext-state
        /// </summary>
        bool ReadPlainText()
        {
            metaToken.Type = HtmlToken.Data;
            while (!EndOfStream)
            {
                switch (GetCharacter())
                {
                    #region 8.2.4.5. PLAINTEXT state
                    case 0:
                        {
                            //parser error
                            RawDataBuffer.Replace(HtmlEncoding.ReplacementCharacter);
                        }
                        break;
                    #endregion
                }
            }

            CreateDataToken();
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.69. CDATA section state
        /// https://www.w3.org/TR/html53/syntax.html#CDATA-section-state
        /// </summary>
        bool ReadCDataSection()
        {
            metaToken.Type = HtmlToken.Data;
            do
            {
                if (EndOfStream)
                {
                    CreateDataToken();
                    return ProcessingFlags.Complete;
                }
                else switch (GetCharacter())
                {
                    #region 8.2.4.70. CDATA section bracket state
                    case ']':
                        {
                            State.Set(HtmlTokenizerState.CDataSectionBracket);
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
                    default:
                        {
                            textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.CDataSection);
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.70. CDATA section bracket state
        /// https://www.w3.org/TR/html53/syntax.html#CDATA-section-bracket-state
        /// </summary>
        bool ReadCDataSectionBracket()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.71. CDATA section end state
                case ']':
                    {
                        State.Set(HtmlTokenizerState.CDataSectionEnd);
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion

                #region 8.2.4.69. CDATA section state
                default:
                    {
                        State.Set(HtmlTokenizerState.CDataSection);
                        textBuffer.Append(']');
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        /// <summary>
        /// HTML5 8.2.4.71. CDATA section end state
        /// https://www.w3.org/TR/html53/syntax.html#CDATA-section-end-state
        /// </summary>
        bool ReadCDataSectionEnd()
        {
            do
            {
                switch (PeekCharacter())
                {
                    #region U+005D RIGHT SQUARE BRACKET (])
                    case ']':
                        {
                            textBuffer.Append(']');
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

                    #region 8.2.4.69. CDATA section state
                    default:
                        {
                            State.Set(HtmlTokenizerState.CDataSection);
                            textBuffer.Append("]]");
                        }
                        return ProcessingFlags.Continue;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.CDataSectionEnd);

            CreateDataToken(false);
            return ProcessingFlags.Complete;
        }

        /// <summary>
        /// HTML5 8.2.4.72. Character reference state
        /// https://www.w3.org/TR/html53/syntax.html#character-reference-state
        /// </summary>
        bool ReadCharacterReference()
        {
            long streamPos = RawDataBuffer.Position - 1;
            if (!EndOfStream)
            {
                switch (PeekCharacter())
                {
                    #region 8.2.4.79. Character reference end state
                    case '\t':
                    case '\r':
                    case '\n':
                    case '\f':
                    case ' ':
                    case '<':
                    case '&':
                        {

                        }
                        break;
                    #endregion

                    #region 8.2.4.73. Numeric character reference state
                    case '#':
                        {
                            GetCharacter();

                            HtmlEncoding encoding = new HtmlEncoding(this);
                            if (encoding.ConvertNumericCharacter())
                            {
                                RawDataBuffer.Discard((int)(RawDataBuffer.Position - streamPos));
                                encoding.Flush();
                            }
                        }
                        break;
                    #endregion

                    #region Anything else
                    default:
                        {
                            HtmlEncoding encoding = new HtmlEncoding(this);
                            if (encoding.ConvertNamedCharacter())
                            {
                                switch (RawDataBuffer.Head)
                                {
                                    #region U+003B SEMICOLON character (;)
                                    case ';':
                                        {
                                            RawDataBuffer.Discard((int)(RawDataBuffer.Position - streamPos));
                                            encoding.Flush();
                                        }
                                        break;
                                    #endregion

                                    #region U+003D EQUALS SIGN character (=)
                                    case '=':
                                    #endregion

                                    #region Alphanumeric ASCII character
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
                                            switch (State.ElementAt(0))
                                            {
                                                case HtmlTokenizerState.AttributeValueQuoted:
                                                case HtmlTokenizerState.AttributeValueUnquoted:
                                                    {
                                                        streamPos = RawDataBuffer.Position;
                                                    }
                                                    break;

                                                default:
                                                    {
                                                        RawDataBuffer.Discard((int)(RawDataBuffer.Position - streamPos));
                                                        encoding.Flush();
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    #endregion
                                }
                            }
                        }
                        break;
                    #endregion
                }
            }
            List<Char32> buffer = RawDataBuffer.Buffer;
            for (int i = (int)streamPos; i < RawDataBuffer.Position; i++)
            {
                textBuffer.Append(Char.ConvertFromUtf32((Int32)buffer[i]));
            }
            State.Remove();
            return ProcessingFlags.Continue;
        }
    }
}
