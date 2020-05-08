// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Cpp
{
    /// <summary>
    /// A token created from the preprocessor. The preprocessed source stream consists of
    /// such tokens
    /// </summary>
    public struct CompilerToken
    {
        Token type;
        /// <summary>
        /// The defined type of this token
        /// </summary>
        public Token Type
        {
            get { return type; }
        }

        string buffer;
        /// <summary>
        /// A string buffer used by some token types
        /// </summary>
        public string Buffer
        {
            get
            {
                switch (type)
                {
                    case Token.CharacterLiteral:
                        {
                            return String.Concat("\'", buffer, "\'");
                        }
                    case Token.StringLiteral:
                        {
                            return String.Concat("\"", buffer, "\"");
                        }
                    default:
                        {
                            return buffer;
                        }
                }
            }
        }
        /// <summary>
        /// The raw string buffer used by some token types
        /// </summary>
        public string RawBuffer
        {
            get { return buffer; }
        }

        TextPointer carret;
        /// <summary>
        /// The location in the source data this token has been
        /// detected at
        /// </summary>
        public TextPointer Carret
        {
            get { return carret; }
        }

        /// <summary>
        /// Creates a new token instance
        /// </summary>
        public CompilerToken(Token original, string buffer, TextPointer carret)
        {
            this.type = original;
            this.buffer = buffer;
            this.carret = carret;
        }

        public override string ToString()
        {
            switch (type)
            {
                #region Character
                case Token.Character:
                #endregion

                #region BogusStringLiteral
                case Token.BogusStringLiteral:
                #endregion

                #region BogusCharacterLiteral
                case Token.BogusCharacterLiteral:
                    {
                        return string.Concat(type.ToString(), "(", buffer, ")");
                    }
                #endregion

                #region Anything else
                default:
                    {
                        return Buffer.ToString();
                    }
                #endregion
            }
        }
    }
}
