using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Sharp
{
    public partial class Tokenizer
    {
        bool allowUcnConversion;
        /// <summary>
        /// Determines if the tokenizer performs universal character name conversion
        /// </summary>
        public bool AllowUcnConversion
        {
            get { return allowUcnConversion; }
            set { allowUcnConversion = value; }
        }
        
        bool isAtLineStart;
        /// <summary>
        /// Determines if the tokenizer is at the start of a line
        /// </summary>
        public bool IsAtLineStart
        {
            get { return isAtLineStart; }
            protected internal set { newLineCharacter = value; }
        }
        
        bool newLineCharacter;
        
        /// <summary>
        /// The individual bytes of the source code file are mapped (in implementation-defined manner) to
        /// the characters of the basic source character set. In particular, OS-dependent end-of-line
        /// indicators are replaced by newline characters.
        /// </summary>
        protected override Char32 PeekCharacter()
        {
            Head:
            switch (base.PeekCharacter())
            {
                #region OS-dependant end-of-line characters are replaced with newLine
                case '\r':
                    {
                        RawDataBuffer.Position++;
                        if (base.PeekCharacter() == '\n')
                        {
                            RawDataBuffer.Discard(2);
                            RawDataBuffer.Buffer.Add((int)'\n');
                        }
                        else RawDataBuffer.Position--;
                    }
                    goto Head;
                #endregion

                #region escaped new line
                case '\\':
                    {
                        RawDataBuffer.Position++;
                        if (PeekCharacter() == '\n')
                        {
                            RawDataBuffer.Discard(2);
                            Carret = new TextPointer(Carret.Line + 1, 0);

                            goto Head;
                        }
                        else RawDataBuffer.Position--;
                    }
                    break;
                #endregion

                #region Whitespace character
                case Char32.WhiteSpaceGroup.Space:
                case Char32.WhiteSpaceGroup.HorizontalTab:
                case Char32.WhiteSpaceGroup.VerticalTab:
                case Char32.WhiteSpaceGroup.FormFeed:
                    {

                    }
                    break;
                #endregion

                #region Numeric character
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
                #endregion

                #region Wording character
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
                #endregion

                #region punctuation character
                case '_':
                case '{':
                case '}':
                case '[':
                case ']':
                case '(':
                case ')':
                case '<':
                case '>':
                case '%':
                case '#':
                case ':':
                case ';':
                case '.':
                case '!':
                case ',':
                case '?':
                case '+':
                case '-':
                case '*':
                case '/':
                case '=':
                case '&':
                case '|':
                case '^':
                case '~':
                case '\"':
                case '\'':
                    {
                        newLineCharacter = false;
                    }
                    break;
                #endregion

                #region new line
                case '\n':
                    {
                        newLineCharacter = true;
                    }
                    break;
                #endregion

                #region universal character name
                default:
                    {
                        if (EndOfStream)
                        {
                            if (!newLineCharacter)
                            {
                                RawDataBuffer.Buffer.Add((int)'\n');
                            }
                        }
                        else
                        {
                            if (allowUcnConversion)
                            {
                                Char32 c = RawDataBuffer.Head;
                                RawDataBuffer.Discard(1);

                                string hex;
                                if (c.Value.Count() <= 4)
                                {
                                    hex = string.Concat("\\u", c.Value.ToString("x4"));
                                }
                                else hex = string.Concat("\\U", c.Value.ToString("x8"));
                                for (int i = 0; i < hex.Length; i++)
                                {
                                    RawDataBuffer.Buffer.Add(char.ConvertToUtf32(hex, i));
                                }
                            }
                        }
                        newLineCharacter = false;
                    }
                    break;
                    #endregion
            }
                return base.PeekCharacter();
        }
        
        /// <summary>
        /// The individual bytes of the source code file are mapped (in implementation-defined manner) to
        /// the characters of the basic source character set. In particular, OS-dependent end-of-line
        /// indicators are replaced by newline characters.
        /// </summary>
        protected override Char32 GetCharacter()
        {
            Char32 result = PeekCharacter();
            RawDataBuffer.Position++;
            return result;
        }
        
    }
}