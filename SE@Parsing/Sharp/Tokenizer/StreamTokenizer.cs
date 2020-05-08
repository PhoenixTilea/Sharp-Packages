// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// A stream reader specialized to process text into tokens of certain grammar
    /// </summary>
    public abstract partial class StreamTokenizer<TokenId, StateId> : IDisposable where TokenId : struct, IConvertible, IComparable
                                                                                  where StateId : struct, IConvertible, IComparable
    {
        public delegate bool SkipDelimiter(Char32 character);

        bool isUtf8;
        /// <summary>
        /// An indicator if the stream is processed using UTF8 encoding
        /// </summary>
        public bool IsUtf8
        {
            get { return isUtf8; }
        }

        Stream primaryStream;
        /// <summary>
        /// Returns the underlying stream
        /// </summary>
        public Stream BaseStream
        {
            get { return primaryStream; }
        }

        bool discard;
        /// <summary>
        /// Sets a flag to have the underlying stream be discarded or kept
        /// alive after the reader closes
        /// </summary>
        public bool Discard
        {
            get { return discard; }
            set { discard = value; }
        }

        StreamBuffer<Char32> secondaryStream;
        /// <summary>
        /// Returns the underlaying raw data buffer
        /// </summary>
        public StreamBuffer<Char32> RawDataBuffer
        {
            get { return secondaryStream; }
        }

        /// <summary>
        /// Gets a value that indicates whether the current stream position is at the end
        /// of the stream
        /// </summary>
        public bool EndOfStream
        {
            get { return (primaryStream.Eof() && secondaryStream.Eof()); }
        }

        /// <summary>
        /// Returns the read position used in a rule
        /// </summary>
        public long Position
        {
            get { return secondaryStream.Position; }
            set { secondaryStream.Position = value; }
        }

        ProcessingState<StateId> state;
        /// <summary>
        /// Returns the tokenizer state used while processing
        /// </summary>
        public ProcessingState<StateId> State
        {
            get { return state; }
        }

        protected StringBuilder textBuffer;
        /// <summary>
        /// Returns current buffer content for further use
        /// </summary>
        public string Buffer
        {
            get
            {
                if (textBuffer == null) return string.Empty;
                else return textBuffer.ToString();
            }
        }

        TextPointer textPointer;
        /// <summary>
        /// Returns current file position
        /// </summary>
        public TextPointer Carret
        {
            get { return textPointer; }
            set { textPointer = value; }
        }

        /// <summary>
        /// Creates a new tokenizer on the provided stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        public StreamTokenizer(Stream stream, bool isUtf8)
        {
            this.isUtf8 = isUtf8;
            this.secondaryStream = new StreamBuffer<Char32>();
            this.primaryStream = stream;

            this.textPointer = new TextPointer(1, 1);
            this.state = new ProcessingState<StateId>();
        }

        /// <summary>
        /// Closes the tokenizer instance and the underlying stream, and releases any system
        /// resources associated with the reader
        /// </summary>
        public void Close()
        {
            Dispose();
        }
        public void Dispose()
        {
            if(discard) primaryStream.Dispose();
            secondaryStream.Dispose();
        }
    }
}
