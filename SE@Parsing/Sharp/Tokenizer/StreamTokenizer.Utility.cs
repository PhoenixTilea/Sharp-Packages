// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    public partial class StreamTokenizer<TokenId, StateId> where TokenId : struct, IConvertible, IComparable, IFormattable
                                                           where StateId : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// A collection of basic token rules
        /// </summary>
        protected static class BaseRules
        {
            /// <summary>
            /// Determines if current character is an ASCII (a-z|A-Z) character
            /// </summary>
            public static bool Letter(StreamTokenizer<TokenId, StateId> data)
            {
                if (IsLetter(data.PeekCharacter()))
                {
                    data.Position++;
                    return true;
                }
                else return false;
            }
            /// <summary>
            /// Determines if current token is an ASCII literal (a-z|A-Z)+ token
            /// </summary>
            public static bool Literal(StreamTokenizer<TokenId, StateId> data)
            {
                int count = 0;
                for (; IsLetter(data.PeekCharacter()); count++)
                    data.Position++;

                return (count > 0);
            }

            private static bool IsLetter(Char32 c)
            {
                return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
            }

            /// <summary>
            /// Determines if current character is a number digit (0-9) character
            /// </summary>
            public static bool Digitt(StreamTokenizer<TokenId, StateId> data)
            {
                if (IsDigit(data.PeekCharacter()))
                {
                    data.Position++;
                    return true;
                }
                else return false;
            }
            /// <summary>
            /// Determines if current token is a numeral (0-9)* token
            /// </summary>
            public static bool Numeral(StreamTokenizer<TokenId, StateId> data)
            {
                int count = 0;
                for (; IsDigit(data.PeekCharacter()); count++)
                    data.Position++;

                return (count > 0);
            }

            private static bool IsDigit(Char32 c)
            {
                return (c >= '0' && c <= '9');
            }

            /// <summary>
            /// Determines if current character is any whitespace character
            /// </summary>
            public static bool Whitspace(StreamTokenizer<TokenId, StateId> data)
            {
                if (Char32.IsWhiteSpace(data.PeekCharacter()))
                {
                    data.Position++;
                    return true;
                }
                else return false;
            }

            /// <summary>
            /// Determines if current token is a whitespace token
            /// </summary>
            public static bool NullLiteral(StreamTokenizer<TokenId, StateId> data)
            {
                int count = 0;
                for (; !data.EndOfStream && Char32.IsWhiteSpace(data.PeekCharacter()); count++)
                    data.Position++;

                return (count > 0);
            }
        }

        /// <summary>
        /// Skips current character
        /// </summary>
        /// <returns>The character skipped</returns>
        public Char32 SkipCharacter()
        {
            Char32 result = GetCharacter();
            DiscardBufferedData();

            return result;
        }
        /// <summary>
        /// Skips current character limited by the delimiter
        /// </summary>
        /// <returns>True if the token ended properly, false otherwise</returns>
        public bool SkipCharacter(SkipDelimiter delimiter)
        {
            if (delimiter(PeekCharacter()))
                Position++;

            DiscardBufferedData();
            return !EndOfStream;
        }

        /// <summary>
        /// Skips current token limited by the delimiter callback
        /// </summary>
        /// <param name="delimeter">A callback to determine if the end of the token was reached</param>
        /// <returns>True if the token ended properly, false otherwise</returns>
        public bool SkipToken(SkipDelimiter delimiter)
        {
            while (!EndOfStream && !delimiter(PeekCharacter()))
                Position++;

            return !EndOfStream;
        }
        /// <summary>
        /// Skips current token limited by any whitespace character
        /// </summary>
        /// <returns>True if the token ended properly, false otherwise</returns>
        public bool SkipToken()
        {
            while (!EndOfStream && !Char32.IsWhiteSpace(PeekCharacter()))
                Position++;

            return !EndOfStream;
        }
    }
}
