// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using SE;

namespace SE.Text.Parsing
{
    public abstract partial class StreamTokenizer<TokenId, StateId> where TokenId : struct, IConvertible, IComparable, IFormattable
                                                                    where StateId : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// Moves the stream pointer to next available position if possible
        /// </summary>
        /// <returns>True if the pointer was moved, false otherwise</returns>
        protected bool MoveNext()
        {
            if (secondaryStream.Eof())
            {
                UInt32 result = ReadCharacter();
                if (result > 0)
                {
                    secondaryStream.Buffer.Add(result);
                    return true;
                }
                else return false;
            }
            else
            {
                secondaryStream.Position++;
                return true;
            }
        }

        /// <summary>
        /// Returns the next available UTF8 character but does not consume it
        /// </summary>
        protected Char32 PeekCharacter()
        {
            if (secondaryStream.Eof())
            {
                Char32 result = ReadCharacter();
                if (result > 0)
                    secondaryStream.Buffer.Add(result);

                return result;
            }
            else return secondaryStream.Current;
        }
        /// <summary>
        /// Reads the next UTF8 character from the input stream and advances it's position by one
        /// </summary>
        protected Char32 GetCharacter()
        {
            Char32 result = PeekCharacter();
            secondaryStream.Position++;

            return result;
        }

        /// <summary>
        /// Walks through the tokenization rules to find the next matching token
        /// </summary>
        protected abstract TokenId GetToken(object context);

        string GetBuffer(bool update)
        {
            if (textBuffer != null) textBuffer.Clear();
            else textBuffer = new StringBuilder((int)secondaryStream.Length);

            long position = textPointer.Column;
            for (int i = 0; i < secondaryStream.Position && i < secondaryStream.Length; i++)
            {
                Char32 character = secondaryStream.Buffer[i];
                if (update)
                {
                    if (Char32.IsNewLine(character))
                    {
                        textPointer = new TextPointer(textPointer.Line + 1, 0);
                        position = 0;
                    }
                    if (!Char.IsControl((char)character.Value) || Char32.IsWhiteSpace(character.Value))
                        position++;
                }
                textBuffer.Append(Char.ConvertFromUtf32((Int32)character));
            }

            if (update)
                textPointer = new TextPointer(textPointer.Line, position);

            return textBuffer.ToString();
        }
        Char32 ReadCharacter()
        {
            if (isUtf8) return Char32.Decode(primaryStream);
            else return new Char32(primaryStream.Get());
        }
    }
}
