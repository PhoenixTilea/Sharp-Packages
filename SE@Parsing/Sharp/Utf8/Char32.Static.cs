// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE;

namespace SE.Text.Parsing
{
    public partial struct Char32
    {
        /// <summary>
        /// Determines if the given character is a new line character
        /// </summary>
        public static bool IsNewLine(Char32 c)
        {
            switch (c)
            {
                case NewLineGroup.LineFeed:
                case NewLineGroup.NextLine:
                case NewLineGroup.LineSeparator:
                case NewLineGroup.ParagraphSeparator: return true;
                default: return false;
            }
        }
        /// <summary>
        /// Determines if the given character is in whitespace character range
        /// </summary>
        public static bool IsWhiteSpace(Char32 c)
        {
            switch (c)
            {
                case '\r':

                case NewLineGroup.LineFeed:
                case NewLineGroup.NextLine:
                case NewLineGroup.LineSeparator:
                case NewLineGroup.ParagraphSeparator:

                case WhiteSpaceGroup.Space:
                case WhiteSpaceGroup.HorizontalTab:
                case WhiteSpaceGroup.VerticalTab:
                case WhiteSpaceGroup.FormFeed:
                case WhiteSpaceGroup.NoBreakingSpace:
                case WhiteSpaceGroup.OghamSpace:
                case WhiteSpaceGroup.MongolianVowelSeparator:
                case WhiteSpaceGroup.EnQuad:
                case WhiteSpaceGroup.EmQuad:
                case WhiteSpaceGroup.EnSpace:
                case WhiteSpaceGroup.EmSpace:
                case WhiteSpaceGroup.ThreePerEmSpace:
                case WhiteSpaceGroup.FourPerEmSpace:
                case WhiteSpaceGroup.SixPerEmSpace:
                case WhiteSpaceGroup.PunctuationSpace:
                case WhiteSpaceGroup.ThinSpace:
                case WhiteSpaceGroup.HairSpace:
                case WhiteSpaceGroup.NarrowSpace:
                case WhiteSpaceGroup.IdeographicSpace:
                case WhiteSpaceGroup.MediumMathematicalSpace: return true;
                default: return false;
            }
        }

        /// <summary>
        /// Decodes next character from an ASCII or UTF8 stream
        /// </summary>
        public static Char32 Decode(Stream stream)
        {
            char c = stream.Get();
            int seqlen;
            UInt32 uc;

            if ((c & 0x80) == 0) //7 bits encoded ASCII [1 mask - 7 bits]
            {
                return (UInt32)(c & 0x7F);
            }
            else if ((c & 0xE0) == 0xC0) //11 bits encoded [110 mask - 5 bits][10 mask - 6 bits]
            {
                uc = (UInt32)(c & 0x1F);
                seqlen = 2;
            }
            else if ((c & 0xF0) == 0xE0) //16 bits encoded [1110 mask - 4 bits][10 mask - 6 bits][10 mask - 6 bits]
            {
                uc = (UInt32)(c & 0x0F);
                seqlen = 3;
            }
            else if ((c & 0xF8) == 0xF0) //21 bits encoded [11110 mask - 3 bits][10 mask - 6 bits][10 mask - 6 bits]
            {
                uc = (UInt32)(c & 0x07);
                seqlen = 4;
            }
            else throw new FormatException();
            for (int i = 1; i < seqlen; i++)
                uc = ((uc << 6) | (UInt32)(stream.Get() & 0x3F));

            return new Char32(uc);
        }
    }
}
