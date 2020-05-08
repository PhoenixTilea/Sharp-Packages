// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SE;

namespace SE.Text.Parsing
{
    public abstract partial class TreeBuilder<TokenId, TokenizerStateId, ParserStateId> where TokenId : struct, IConvertible, IComparable
                                                                                        where TokenizerStateId : struct, IConvertible, IComparable
                                                                                        where ParserStateId : struct, IConvertible, IComparable
    {
        /// <summary>
        /// Creates the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        /// <param name="discardStream">determines if the stream should be closed after processing ends</param>
        /// <returns>True if successfully processed the stream, false otherwise</returns>
        public bool Parse(Stream stream, bool discardStream, object context = null)
        {
            bool result = true;
            if(BuilderState.Count == 0)
                BuilderState.Set(default(ParserStateId));

            Encoding encoding = stream.GetEncoding();
            if (encoding != Encoding.ASCII && encoding != Encoding.UTF8)
                throw new FormatException();

            currentContext = context;
            List<Tuple<TokenId, string, TextPointer>> defaultQueue = preservationQueue;
            using (tokenizer = Begin(stream, encoding == Encoding.UTF8))
                try
                {
                    tokenizer.Discard = discardStream;
                    preservationQueue = CollectionPool<List<Tuple<TokenId, string, TextPointer>>, Tuple<TokenId, string, TextPointer>>.Get();
                    while (!EndOfStream)
                    {
                        TokenId token;
                        if (preservationQueue.Count > 0)
                        {
                            Tuple<TokenId, string, TextPointer> item = preservationQueue[0];
                            preservationQueue.RemoveAt(0);

                            current = item.Item2;
                            textPointer = item.Item3;
                            token = item.Item1;
                        }
                        else
                        {
                            textPointer = tokenizer.Carret;
                            token = tokenizer.Read(context);
                            current = null;
                        }

                        if (!DiscardToken(token, context))
                            result &= ProcessToken(token, context);
                    }
                    result = Finalize(result, context);
                }
                finally
                {
                    CollectionPool<List<Tuple<TokenId, string, TextPointer>>, Tuple<TokenId, string, TextPointer>>.Return(preservationQueue);

                    preservationQueue = defaultQueue;
                    currentContext = null;
                    tokenizer = null;
                }
            return result;
        }
        /// <summary>
        /// Creates the tree of tokens from the provided text stream
        /// </summary>
        /// <param name="stream">An ASCII or UTF8 text stream to process</param>
        /// <returns>True if successfully processed the stream, false otherwise</returns>
        public bool Parse(Stream stream, object context = null)
        {
            return Parse(stream, true, context);
        }

        /// <summary>
        /// Prepares the parser for a clean run and returns an instance of the desired tokenizer
        /// </summary>
        protected abstract StreamTokenizer<TokenId, TokenizerStateId> Begin(Stream stream, bool isUtf8);
        /// <summary>
        /// Finalizes the parser after a run and returns the final result
        /// </summary>
        protected abstract bool Finalize(bool result, object context);

        /// <summary>
        /// Checks current token for being discarded
        /// </summary>
        /// <returns>True if discarded, false otherwise</returns>
        protected virtual bool DiscardToken(TokenId token, object context)
        {
            return false;
        }
        /// <summary>
        /// Processes current token and returns success or failure. The returned state doesn't has
        /// any impact on the processing loop itself. Set the stream to the end to cancel further
        /// processing
        /// </summary>
        /// <returns>True if successfully handled, false otherwise</returns>
        protected abstract bool ProcessToken(TokenId token, object context);
    }
}
