// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Json
{
    public partial class JsonTokenizer
    {
        protected static class Rules
        {
            /// <summary>
            /// Identifier = '\"' [^\"]* '\"';
            /// </summary>
            public static bool Identifier(JsonTokenizer data)
            {
                while (!data.EndOfStream && data.PeekCharacter() != '\"')
                    data.Position++;

                return (data.GetCharacter() == '\"');
            }

            /// <summary>
            /// TrueConstant = 'true';
            /// </summary>
            public static bool TrueConstant(JsonTokenizer data)
            {
                switch (data.State.Current)
                {
                    case JsonParserState.Resolver:
                    case JsonParserState.Array:
                    case JsonParserState.Value:
                        {
                            ComparisonState state = 0;
                            UInt32 c; for (; ; )
                            {
                                c = data.PeekCharacter();
                                switch (state)
                                {
                                    case (ComparisonState)0:
                                        {
                                            if (c == 'r') goto case ComparisonState.Next;
                                            else goto case ComparisonState.Failure;
                                        }
                                    case (ComparisonState)1:
                                        {
                                            if (c == 'u') goto case ComparisonState.Next;
                                            else goto case ComparisonState.Failure;
                                        }
                                    case (ComparisonState)2:
                                        {
                                            if (c == 'e') goto case ComparisonState.Next;
                                            else goto case ComparisonState.Failure;
                                        }
                                    case (ComparisonState)3: return true;
                                    default:
                                    case ComparisonState.Failure: return false;
                                    case ComparisonState.Next: state++; break;
                                }
                                data.Position++;
                            }
                        }
                    default: return false;
                }
            }
            /// <summary>
            /// FalseConstant = 'false';
            /// </summary>
            public static bool FalseConstant(JsonTokenizer data)
            {
                switch (data.State.Current)
                {
                    case JsonParserState.Resolver:
                    case JsonParserState.Array:
                    case JsonParserState.Value:
                        {
                            ComparisonState state = 0;
                            UInt32 c; for (; ; )
                            {
                                c = data.PeekCharacter();
                                switch (state)
                                {
                                    case (ComparisonState)0:
                                        {
                                            if (c == 'a') goto case ComparisonState.Next;
                                            else goto case ComparisonState.Failure;
                                        }
                                    case (ComparisonState)1:
                                        {
                                            if (c == 'l') goto case ComparisonState.Next;
                                            else goto case ComparisonState.Failure;
                                        }
                                    case (ComparisonState)2:
                                        {
                                            if (c == 's') goto case ComparisonState.Next;
                                            else goto case ComparisonState.Failure;
                                        }
                                    case (ComparisonState)3:
                                        {
                                            if (c == 'e') goto case ComparisonState.Next;
                                            else goto case ComparisonState.Failure;
                                        }
                                    case (ComparisonState)4: return true;
                                    default:
                                    case ComparisonState.Failure: return false;
                                    case ComparisonState.Next: state++; break;
                                }
                                data.Position++;
                            }
                        }
                    default: return false;
                }
            }
            /// <summary>
            /// NullConstant = 'null';
            /// </summary>
            public static bool NullConstant(JsonTokenizer data)
            {
                switch (data.State.Current)
                {
                    case JsonParserState.Resolver:
                    case JsonParserState.Array:
                    case JsonParserState.Value:
                        {
                            ComparisonState state = 0;
                            UInt32 c; for (; ; )
                            {
                                c = data.PeekCharacter();
                                switch (state)
                                {
                                    case (ComparisonState)0:
                                        {
                                            if (c == 'u') goto case ComparisonState.Next;
                                            else goto case ComparisonState.Failure;
                                        }
                                    case (ComparisonState)1:
                                    case (ComparisonState)2:
                                        {
                                            if (c == 'l') goto case ComparisonState.Next;
                                            else goto case ComparisonState.Failure;
                                        }
                                    case (ComparisonState)3: return true;
                                    default:
                                    case ComparisonState.Failure: return false;
                                    case ComparisonState.Next: state++; break;
                                }
                                data.Position++;
                            }
                        }
                    default: return false;
                }
            }
        }
    }
}
