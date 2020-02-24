// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// A line and column based text indexer
    /// </summary>
    public struct TextPointer
    {
        /// <summary>
        /// The initial index in a text stream
        /// </summary>
        public readonly static TextPointer Initial = new TextPointer(0, 0);

        long line;
        /// <summary>
        /// Current line of the text stream
        /// </summary>
        public long Line
        {
            get { return line; }
        }

        long column;
        /// <summary>
        /// Current column of the text stream
        /// </summary>
        public long Column
        {
            get { return column; }
        }

        /// <summary>
        /// Creates a new index at the given position
        /// </summary>
        public TextPointer(long line, long column)
        {
            this.column = column;
            this.line = line;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", line, column);
        }
    }
}
