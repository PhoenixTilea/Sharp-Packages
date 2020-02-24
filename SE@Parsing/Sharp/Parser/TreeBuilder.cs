// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// Assembles tokens of certain grammar along the provided ruleset
    /// </summary>
    public partial class TreeBuilder<TokenId, TokenizerStateId, ParserStateId> where TokenId : struct, IConvertible, IComparable, IFormattable
                                                                               where TokenizerStateId : struct, IConvertible, IComparable, IFormattable
                                                                               where ParserStateId : struct, IConvertible, IComparable, IFormattable
    {
        List<Tuple<TokenId, string, TextPointer>> preservationQueue;
        object currentContext;

        protected StreamTokenizer<TokenId, TokenizerStateId> tokenizer;
        /// <summary>
        /// Returns the tokenizer state used while processing
        /// </summary>
        protected ProcessingState<TokenizerStateId> StreamState
        {
            get
            {
                if (tokenizer == null) return null;
                else return tokenizer.State;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the current stream position is at the end
        /// of the stream
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                if (tokenizer == null) return true;
                else return (tokenizer.EndOfStream && preservationQueue.Count == 0);
            }
        }

        string current;
        /// <summary>
        /// Returns last successfully read token from the stream
        /// </summary>
        public string Current
        {
            get
            {
                if (current != null) return current;
                else
                {
                    if (tokenizer == null) return null;
                    else return tokenizer.Buffer;
                }
            }
        }

        TextPointer textPointer;
        /// <summary>
        /// Returns current file position
        /// </summary>
        public TextPointer Carret
        {
            get { return textPointer; }
        }

        ProcessingState<ParserStateId> state;
        /// <summary>
        /// Returns current processing state
        /// </summary>
        public ProcessingState<ParserStateId> BuilderState
        {
            get { return state; }
        }

        /// <summary>
        /// Creates a new tree builder instance
        /// </summary>
        /// <param name="initialState">An initial parser state to start at</param>
        public TreeBuilder(ParserStateId initialState)
        {
            this.state = new ProcessingState<ParserStateId>();
            this.state.Set(initialState);
        }
        /// <summary>
        /// Creates a new tree builder instance
        /// </summary>
        public TreeBuilder()
            :this(default(ParserStateId))
        { }

        /// <summary>
        /// Preserves any next token and pushes it to a buffer
        /// </summary>
        protected TokenId PreserveToken(int index = 0)
        {
            if (preservationQueue.Count > index) return preservationQueue[index].Item1;
            else
            {
                TokenId token = default(TokenId);
                for (; preservationQueue.Count <= index;)
                {
                    if (tokenizer.EndOfStream) return default(TokenId);
                    else
                    {
                        TextPointer current = tokenizer.Carret;
                        token = tokenizer.Read(currentContext);

                        if (DiscardToken(token, currentContext))
                            continue;

                        preservationQueue.Add(Tuple.Create(token, tokenizer.Buffer, current));
                    }
                }
                return token;
            }
        }
        /// <summary>
        /// Preserves the given token and pushes it to a stack
        /// </summary>
        protected void PushToken(Tuple<TokenId, string, TextPointer> token)
        {
            preservationQueue.Insert(0, token);
        }

        /// <summary>
        /// Sets the stream pointer to the end of the stream
        /// </summary>
        protected void MoveToEnd()
        {
            tokenizer.BaseStream.Position = tokenizer.BaseStream.Length;
            tokenizer.RawDataBuffer.Flush();
        }
    }
}