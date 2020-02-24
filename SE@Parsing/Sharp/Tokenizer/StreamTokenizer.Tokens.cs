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
        /// Clears the internal buffer
        /// </summary>
        public void DiscardBufferedData()
        {
            secondaryStream.Buffer.RemoveRange(0, (int)Position);
            secondaryStream.Position = 0;
        }

        /// <summary>
        /// Clears the internal buffer and returns the result
        /// </summary>
        public string FlushBufferedData()
        {
            string result = GetBuffer(true);
            DiscardBufferedData();

            return result;
        }

        /// <summary>
        /// Returns the next available token but does not consume it
        /// </summary>
        /// <returns>The token type read from the stream</returns>
        public TokenId Peek(object context = null)
        {
            TokenId result = GetToken(context);
            if (result.CompareTo(default(TokenId)) != 0)
                GetBuffer(false);
            
            Position = 0;
            return result;
        }

        /// <summary>
        /// Reads the next token from the input stream and advances it's position by one
        /// </summary>
        /// <returns>The token type read from the stream</returns>
        public TokenId Read(object context = null)
        {
            TokenId result = GetToken(context);
            if (result.CompareTo(default(TokenId)) != 0)
                FlushBufferedData();

            Position = 0;
            return result;
        }
    }
}
