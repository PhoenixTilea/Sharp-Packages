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
        protected static class ProcessingFlags
        {
            public const bool Continue = true;
            public const bool Complete = false;
        }

        protected static class Rules
        {
            /// <summary>
            /// [CDATA[
            /// </summary>
            public static bool CDataSection(HtmlTokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == '[') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'C') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'D') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 'A') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)4:
                            {
                                if (c == 'T') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)5:
                            {
                                if (c == 'A') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)6:
                            {
                                if (c == '[') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)7:
                            {
                                return true;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// Doctype
            /// </summary>
            public static bool Doctype(HtmlTokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'd' || c == 'D') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'o' || c == 'O') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'c' || c == 'C') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 't' || c == 'T') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)4:
                            {
                                if (c == 'y' || c == 'Y') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)5:
                            {
                                if (c == 'p' || c == 'P') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)6:
                            {
                                if (c == 'e' || c == 'E') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)7:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// Public
            /// </summary>
            public static bool Public(HtmlTokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'U') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'B') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'L') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 'I') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)4:
                            {
                                if (c == 'C') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)5:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// System
            /// </summary>
            public static bool System(HtmlTokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'Y') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'S') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'T') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 'E') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)4:
                            {
                                if (c == 'M') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)5:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }
        }

        /// <summary>
        /// Returns the next available UTF8 character but does not consume it
        /// </summary>
        internal new Char32 PeekCharacter()
        {
            return base.PeekCharacter();
        }
        /// <summary>
        /// Reads the next UTF8 character from the input stream and advances it's position by one
        /// </summary>
        internal new Char32 GetCharacter()
        {
            return base.GetCharacter();
        }

        public static bool IsIdentifierChar(Char32 c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
        }
    }
}
