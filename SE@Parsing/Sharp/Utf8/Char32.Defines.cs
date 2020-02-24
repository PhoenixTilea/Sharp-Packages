// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    public partial struct Char32
    {
        /// <summary>
        /// The group of new line characters
        /// </summary>
        public static class NewLineGroup
        {
            public const char LineFeed = '\n';
            public const char NextLine = '\u0085';
            public const char LineSeparator = '\u2028';
            public const char ParagraphSeparator = '\u2029';
        }
        /// <summary>
        /// The group of whitespace characters
        /// </summary>
        public static class WhiteSpaceGroup
        {
            public const char Space = ' ';
            public const char HorizontalTab = '\u0009';
            public const char VerticalTab = '\u000B';
            public const char FormFeed = '\u000C';
            public const char NoBreakingSpace = '\u00A0';
            public const char OghamSpace = '\u1680';
            public const char MongolianVowelSeparator = '\u180E';
            public const char EnQuad = '\u2000';
            public const char EmQuad = '\u2001';
            public const char EnSpace = '\u2002';
            public const char EmSpace = '\u2003';
            public const char ThreePerEmSpace = '\u2004';
            public const char FourPerEmSpace = '\u2005';
            public const char SixPerEmSpace = '\u2006';
            public const char PunctuationSpace = '\u2008';
            public const char ThinSpace = '\u2009';
            public const char HairSpace = '\u200A';
            public const char NarrowSpace = '\u202F';
            public const char IdeographicSpace = '\u3000';
            public const char MediumMathematicalSpace = '\u205F';
        }
    }
}
