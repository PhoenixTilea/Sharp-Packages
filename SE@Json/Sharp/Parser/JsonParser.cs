// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SE;
using SE.Text.Parsing;

namespace SE.Json
{
    /// <summary>
    /// JSON DOM Assembler
    /// </summary>
    public class JsonParser : TreeBuilder<JsonToken, JsonParserState, int>
    {
        Stack<Json.JsonNode> nodeStack;
        JsonDocument document;

        List<string> errors;
        /// <summary>
        /// A collection of error messages occurred during build
        /// </summary>
        public List<string> Errors
        {
            get { return errors; }
        }

        /// <summary>
        /// Creates a new JSON DOM assembler
        /// </summary>
        /// <param name="document">A document hosting the desired DOM</param>
        public JsonParser(JsonDocument document)
        {
            this.nodeStack = new Stack<JsonNode>();
            this.errors = new List<string>();
            this.document = document;
        }

        protected override StreamTokenizer<JsonToken, JsonParserState> Begin(Stream stream, bool isUtf8)
        {
            nodeStack.Clear();
            errors.Clear();

            return new JsonTokenizer(stream, isUtf8);
        }

        protected override bool ProcessToken(JsonToken token, object context)
        {
            try
            {
                switch (token)
                {
                    case JsonToken.BeginObject:
                        {
                            if (StreamState == JsonParserState.Initial && document.Root != null)
                                goto case JsonToken.Invalid;

                            if (StreamState == JsonParserState.Value) nodeStack.Peek().Type = JsonNodeType.Object;
                            else AddNode(JsonNodeType.Object);
                            StreamState.Add(JsonParserState.Object);
                        }
                        break;
                    case JsonToken.BeginArray:
                        {
                            if (StreamState == JsonParserState.Initial && document.Root != null)
                                goto case JsonToken.Invalid;

                            if (StreamState == JsonParserState.Value) nodeStack.Peek().Type = JsonNodeType.Array;
                            else AddNode(JsonNodeType.Array);
                            StreamState.Add(JsonParserState.Array);
                        }
                        break;
                    case JsonToken.Property:
                        {
                            Json.JsonNode node = AddNode(JsonNodeType.Undefined);
                            node.Name = Current.Trim('\"');

                            StreamState.Add(JsonParserState.Property);
                        }
                        break;
                    case JsonToken.Colon:
                        {
                            StreamState.Add(JsonParserState.Value);
                        }
                        break;
                    case JsonToken.Comma:
                        {
                            StreamState.Remove();
                            nodeStack.Pop();
                        }
                        break;
                    case JsonToken.Null: SetValue(JsonNodeType.Empty, null); break;
                    case JsonToken.Boolean: SetValue(JsonNodeType.Bool, Convert.ToBoolean(Current)); break;
                    case JsonToken.Integer: SetValue(JsonNodeType.Integer, Decimal.Parse(Current, NumberStyles.Integer)); break;
                    case JsonToken.Decimal: SetValue(JsonNodeType.Decimal, Decimal.Parse(Current, NumberStyles.Float, CultureInfo.InvariantCulture)); break;
                    case JsonToken.String: SetValue(JsonNodeType.String, Current.Trim('\"')); break;
                    case JsonToken.EndObject:
                    case JsonToken.EndArray:
                        {
                            if (StreamState == JsonParserState.Separator)
                            {
                                StreamState.Remove();
                                nodeStack.Pop();
                            }
                            if (StreamState.Remove() == JsonParserState.Initial || (StreamState != JsonParserState.Value && StreamState != JsonParserState.Array))
                                nodeStack.Pop();

                            ResetValueState();
                            if (context is bool && (bool)context && nodeStack.Count == 0)
                                MoveToEnd();
                        }
                        break;
                    case JsonToken.Invalid:
                        {
                            if (!TryRestoreStream())
                                MoveToEnd();

                            return false;
                        }
                }
                return true;
            }
            catch(Exception er)
            {
                errors.Add(er.Message);

                MoveToEnd();
                return false;
            }
        }
        bool TryRestoreStream()
        {
            StreamState.Add(JsonParserState.Resolver);
            TextPointer location = Carret;

            bool skip = true;
            string token; switch (tokenizer.Read())
            {
                case JsonToken.Invalid: token = tokenizer.Buffer; break;
                default:
                    {
                        location = Carret;
                        token = Current;
                        skip = false;
                    }
                    break;
            }

            string source; if (tokenizer.BaseStream is FileStream)
                source = (tokenizer.BaseStream as FileStream).Name;
            else
                source = tokenizer.BaseStream.ToString();

            errors.Add(string.Format("Invalid token {0} found at {1} ({2}, {3})", token, source, location.Line, location.Column));

            StreamState.Remove();
            switch (StreamState.Current)
            {
                case JsonParserState.Object:
                case JsonParserState.Array:
                    {
                        if(skip)
                            tokenizer.SkipToken();
                    }
                    break;
                case JsonParserState.Property:
                case JsonParserState.Value:
                case JsonParserState.Separator:
                    {
                        if (skip)
                            tokenizer.SkipToken((c) =>
                            {
                                switch (c)
                                {
                                    case ',':
                                    case ']':
                                    case '}': return true;
                                    default: return false;
                                }
                            });
                        if (skip) tokenizer.FlushBufferedData();
                        switch (StreamState.Current)
                        {
                            case JsonParserState.Value: ResetValueState(); break;
                            case JsonParserState.Separator:
                                {
                                    if (skip)
                                        tokenizer.SkipCharacter((c) =>
                                        {
                                            return (c == ',');

                                        });
                                }
                                goto case JsonParserState.Property;
                            case JsonParserState.Property:
                                {
                                    if (skip)
                                        nodeStack.Pop();
                                    StreamState.Remove();
                                }
                                break;
                        }
                    }
                    break;
                case JsonParserState.Initial: return false;
            }
            return true;
        }

        protected override bool Finalize(bool result, object context)
        {
            if (result && nodeStack.Count > 0)
            {
                string source; if (tokenizer.BaseStream is FileStream)
                    source = (tokenizer.BaseStream as FileStream).Name;
                else
                    source = tokenizer.BaseStream.ToString();

                errors.Add(string.Format("Document not closed properly at {0}", source));
                return false;
            }
            else return result;
        }

        Json.JsonNode AddNode(JsonNodeType type)
        {
            Json.JsonNode node; if (nodeStack.Count == 0)
                node = document.AddNode(type);
            else
                node = document.AddNode(nodeStack.Peek(), type);

            nodeStack.Push(node);
            return node;
        }
        void SetValue(JsonNodeType valueType, object value)
        {
            if (nodeStack.Peek().Type != JsonNodeType.Undefined)
                AddNode(JsonNodeType.Undefined);

            Json.JsonNode node = nodeStack.Peek();
            node.Type = valueType;
            node.RawValue = value;

            ResetValueState();
        }

        void ResetValueState()
        {
            if (StreamState == JsonParserState.Value)
            {
                StreamState.Remove();
                if (StreamState == JsonParserState.Property)
                    StreamState.Remove();
            }
            StreamState.Add(JsonParserState.Separator);
        }
    }
}
