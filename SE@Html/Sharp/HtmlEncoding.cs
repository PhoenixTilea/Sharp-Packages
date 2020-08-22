// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Html
{
    public partial struct HtmlEncoding
    {
        public readonly static Char32 ReplacementCharacter = 0xFFFD;

        HtmlTokenizer tokenizer;
        long streamPosition;
        List<Char32> buffer;

        public HtmlEncoding(HtmlTokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
            this.streamPosition = tokenizer.RawDataBuffer.Position;
            this.buffer = new List<Char32>();
        }

        /// <summary>
        /// HTML5 8.2.4.73. Numeric character reference state
        /// https://www.w3.org/TR/html53/syntax.html#numeric-character-reference-state
        /// </summary>
        public bool ConvertNumericCharacter()
        {
        #region 8.2.4.73. Numeric character reference state
            switch (tokenizer.PeekCharacter())
            {
                #region 8.2.4.74. Hexadecimal character reference start state
                case 'x':
                case 'X':
                    {
                        buffer.Add(tokenizer.GetCharacter());
                    }
                    goto HexStart;
                #endregion

                #region 8.2.4.75. Decimal character reference start state
                default:
                    {

                    }
                    goto DecimalStart;
                #endregion
            }
        #endregion

        #region 8.2.4.74. Hexadecimal character reference start state
        HexStart:
            switch (tokenizer.PeekCharacter())
            {
                #region 8.2.4.76. Hexadecimal character reference state
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
                case 'F':
                    {
                        return ConvertHexCharacter();
                    }
                #endregion

                #region 8.2.4.79. Character reference end state
                default:
                    {

                    }
                    goto End;
                #endregion
            }
        #endregion

        #region 8.2.4.75. Decimal character reference start state
        DecimalStart:
            switch (tokenizer.PeekCharacter())
            {
                #region 8.2.4.77. Decimal character reference state
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
                    {
                        return ConvertDecimalCharacter();
                    }
                #endregion

                #region 8.2.4.79. Character reference end state
                default:
                    {

                    }
                    goto End;
                #endregion
            }
        #endregion

        End:
            return false;
        }

        /// <summary>
        /// HTML5 8.2.4.76. Hexadecimal character reference state
        /// https://www.w3.org/TR/html53/syntax.html#hexadecimal-character-reference-state
        /// </summary>
        bool ConvertHexCharacter()
        {
            UInt64 result = 0;
            while(!tokenizer.EndOfStream)
            {
                switch (tokenizer.PeekCharacter())
                {
                    #region ASCII digit
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
                        {
                            Char32 c = tokenizer.GetCharacter();
                            buffer.Add(c);

                            c -= 0x0030;
                            result *= 16;
                            result += c;
                        }
                        break;
                    #endregion

                    #region Uppercase ASCII hex digit
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                        {
                            Char32 c = tokenizer.GetCharacter();
                            buffer.Add(c);

                            c -= 0x0037;
                            result *= 16;
                            result += c;
                        }
                        break;
                    #endregion

                    #region Lowercase ASCII hex digit
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                        {
                            Char32 c = tokenizer.GetCharacter();
                            buffer.Add(c);

                            c -= 0x0057;
                            result *= 16;
                            result += c;
                        }
                        break;
                    #endregion

                    #region U+003B SEMICOLON character (;)
                    case ';':
                        {
                            buffer.Add(tokenizer.GetCharacter());
                            GetNumericEntityValue((int)result);
                        }
                        return true;
                    #endregion

                    #region Anything else
                    default:
                        {
                            if (buffer.Count > 1)
                            {
                                GetNumericEntityValue((int)result);
                                return true;
                            }
                        }
                        return false;
                    #endregion
                }
            }
            return false;
        }

        /// <summary>
        /// HTML5 8.2.4.77. Decimal character reference state
        /// https://www.w3.org/TR/html53/syntax.html#decimal-character-reference-state
        /// </summary>
        /// <returns></returns>
        bool ConvertDecimalCharacter()
        {
            UInt64 result = 0;
            while (!tokenizer.EndOfStream)
            {
                switch (tokenizer.PeekCharacter())
                {
                    #region ASCII digit
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
                        {
                            Char32 c = tokenizer.GetCharacter();
                            buffer.Add(c);

                            c -= 0x0030;
                            result *= 10;
                            result += c;
                        }
                        break;
                    #endregion

                    #region U+003B SEMICOLON character (;)
                    case ';':
                        {
                            buffer.Add(tokenizer.GetCharacter());
                            GetNumericEntityValue((int)result);
                        }
                        return true;
                    #endregion

                    #region Anything else
                    default:
                        {
                            if (buffer.Count > 1)
                            {
                                GetNumericEntityValue((int)result);
                                return true;
                            }
                        }
                        return false;
                        #endregion
                }
            }
            return false;
        }

        /// <summary>
        /// HTML5 8.2.4.78. Numeric character reference end state
        /// https://www.w3.org/TR/html53/syntax.html#numeric-character-reference-end-state
        /// </summary>
        void GetNumericEntityValue(int value)
        {
            switch (value)
            {
                #region REPLACEMENT CHARACTER
                case 0x00:
                    {
                        buffer.Clear();
                        buffer.Add(ReplacementCharacter);
                    }
                    break;
                #endregion

                #region EURO SIGN (€)
                case 0x80:
                    {
                        buffer.Clear();
                        buffer.Add(0x20AC);
                    }
                    break;
                #endregion

                #region SINGLE LOW-9 QUOTATION MARK (‚)
                case 0x82:
                    {
                        buffer.Clear();
                        buffer.Add(0x201A);
                    }
                    break;
                #endregion

                #region LATIN SMALL LETTER F WITH HOOK (ƒ)
                case 0x83:
                    {
                        buffer.Clear();
                        buffer.Add(0x0192);
                    }
                    break;
                #endregion

                #region DOUBLE LOW-9 QUOTATION MARK („)
                case 0x84:
                    {
                        buffer.Clear();
                        buffer.Add(0x201E);
                    }
                    break;
                #endregion

                #region HORIZONTAL ELLIPSIS (…)
                case 0x85:
                    {
                        buffer.Clear();
                        buffer.Add(0x2026);
                    }
                    break;
                #endregion

                #region DAGGER (†)
                case 0x86:
                    {
                        buffer.Clear();
                        buffer.Add(0x2020);
                    }
                    break;
                #endregion

                #region DOUBLE DAGGER (‡)
                case 0x87:
                    {
                        buffer.Clear();
                        buffer.Add(0x2021);
                    }
                    break;
                #endregion

                #region MODIFIER LETTER CIRCUMFLEX ACCENT (ˆ)
                case 0x88:
                    {
                        buffer.Clear();
                        buffer.Add(0x02C6);
                    }
                    break;
                #endregion

                #region PER MILLE SIGN (‰)
                case 0x89:
                    {
                        buffer.Clear();
                        buffer.Add(0x2030);
                    }
                    break;
                #endregion

                #region LATIN CAPITAL LETTER S WITH CARON (Š)
                case 0x8A:
                    {
                        buffer.Clear();
                        buffer.Add(0x0160);
                    }
                    break;
                #endregion

                #region SINGLE LEFT-POINTING ANGLE QUOTATION MARK (‹)
                case 0x8B:
                    {
                        buffer.Clear();
                        buffer.Add(0x2039);
                    }
                    break;
                #endregion

                #region LATIN CAPITAL LIGATURE OE (Œ)
                case 0x8C:
                    {
                        buffer.Clear();
                        buffer.Add(0x0152);
                    }
                    break;
                #endregion

                #region LATIN CAPITAL LETTER Z WITH CARON (Ž)
                case 0x8E:
                    {
                        buffer.Clear();
                        buffer.Add(0x017D);
                    }
                    break;
                #endregion

                #region LEFT SINGLE QUOTATION MARK (‘)
                case 0x91:
                    {
                        buffer.Clear();
                        buffer.Add(0x2018);
                    }
                    break;
                #endregion

                #region RIGHT SINGLE QUOTATION MARK (’)
                case 0x92:
                    {
                        buffer.Clear();
                        buffer.Add(0x2019);
                    }
                    break;
                #endregion

                #region LEFT DOUBLE QUOTATION MARK (“)
                case 0x93:
                    {
                        buffer.Clear();
                        buffer.Add(0x201C);
                    }
                    break;
                #endregion

                #region RIGHT DOUBLE QUOTATION MARK (”)
                case 0x94:
                    {
                        buffer.Clear();
                        buffer.Add(0x201D);
                    }
                    break;
                #endregion

                #region BULLET (•)
                case 0x95:
                    {
                        buffer.Clear();
                        buffer.Add(0x2022);
                    }
                    break;
                #endregion

                #region EN DASH (–)
                case 0x96:
                    {
                        buffer.Clear();
                        buffer.Add(0x2013);
                    }
                    break;
                #endregion

                #region EM DASH (—)
                case 0x97:
                    {
                        buffer.Clear();
                        buffer.Add(0x2014);
                    }
                    break;
                #endregion

                #region SMALL TILDE (˜)
                case 0x98:
                    {
                        buffer.Clear();
                        buffer.Add(0x02DC);
                    }
                    break;
                #endregion

                #region TRADE MARK SIGN (™)
                case 0x99:
                    {
                        buffer.Clear();
                        buffer.Add(0x2122);
                    }
                    break;
                #endregion

                #region LATIN SMALL LETTER S WITH CARON (š)
                case 0x9A:
                    {
                        buffer.Clear();
                        buffer.Add(0x0161);
                    }
                    break;
                #endregion

                #region SINGLE RIGHT-POINTING ANGLE QUOTATION MARK (›)
                case 0x9B:
                    {
                        buffer.Clear();
                        buffer.Add(0x203A);
                    }
                    break;
                #endregion

                #region LATIN SMALL LIGATURE OE (œ)
                case 0x9C:
                    {
                        buffer.Clear();
                        buffer.Add(0x0153);
                    }
                    break;
                #endregion

                #region LATIN SMALL LETTER Z WITH CARON (ž)
                case 0x9E:
                    {
                        buffer.Clear();
                        buffer.Add(0x017E);
                    }
                    break;
                #endregion

                #region LATIN CAPITAL LETTER Y WITH DIAERESIS (Ÿ)
                case 0x9F:
                    {
                        buffer.Clear();
                        buffer.Add(0x0178);
                    }
                    break;
                #endregion

                #region Parser error
                case 0x0000B:
                case 0x0FFFE:
                case 0x1FFFE:
                case 0x1FFFF:
                case 0x2FFFE:
                case 0x2FFFF:
                case 0x3FFFE:
                case 0x3FFFF:
                case 0x4FFFE:
                case 0x4FFFF:
                case 0x5FFFE:
                case 0x5FFFF:
                case 0x6FFFE:
                case 0x6FFFF:
                case 0x7FFFE:
                case 0x7FFFF:
                case 0x8FFFE:
                case 0x8FFFF:
                case 0x9FFFE:
                case 0x9FFFF:
                case 0xAFFFE:
                case 0xAFFFF:
                case 0xBFFFE:
                case 0xBFFFF:
                case 0xCFFFE:
                case 0xCFFFF:
                case 0xDFFFE:
                case 0xDFFFF:
                case 0xEFFFE:
                case 0xEFFFF:
                case 0xFFFFE:
                case 0xFFFFF:
                case 0x10FFFE:
                case 0x10FFFF:
                    {
                        
                    }
                    break;
                #endregion

                #region Anything else
                default:
                    {
                        if ((value >= 0xD800 && value <= 0xDFFF) || value > 0x10FFFF)
                            goto case 0;

                        if ((value >= 0x0001 && value <= 0x0008) || (value >= 0x000D && value <= 0x001F) ||
                            (value >= 0x007F && value <= 0x009F) || (value >= 0xFDD0 && value <= 0xFDEF))
                        {
                            
                        }
                        else
                        {
                            buffer.Clear();
                            foreach (char c in char.ConvertFromUtf32(value))
                            {
                                buffer.Add((int)c);
                            }
                        }
                    }
                    break;
                #endregion
            }
        }

        public void Flush()
        {
            tokenizer.RawDataBuffer.Buffer.AddRange(buffer);
            tokenizer.RawDataBuffer.Position += buffer.Count;
        }
    }
}
