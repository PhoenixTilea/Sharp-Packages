// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE;
using SE.Text.Parsing;

namespace SE.Json
{
    /// <summary>
    /// Parses JSON data into a stream of JSON tokens
    /// </summary>
    public partial class JsonTokenizer : StreamTokenizer<JsonToken, JsonParserState>
    {
        public JsonTokenizer(Stream stream, bool isUtf8)
            : base(stream, isUtf8)
        { }

        protected override JsonToken GetToken(object context)
        {
            switch (GetCharacter())
            {
                // ------- Object -------
                case '{':
                    switch (State.Current)
                    {
                        case JsonParserState.Resolver:
                        case JsonParserState.Initial:
                        case JsonParserState.Array:
                        case JsonParserState.Value: return JsonToken.BeginObject;
                    }
                    goto default;
                case '}':
                    switch (State.Current)
                    {
                        case JsonParserState.Resolver:
                        case JsonParserState.Object:
                        case JsonParserState.Separator: return JsonToken.EndObject;
                    }
                    goto default;

                // ------- Array -------
                case '[':
                    switch (State.Current)
                    {
                        case JsonParserState.Resolver:
                        case JsonParserState.Initial:
                        case JsonParserState.Array:
                        case JsonParserState.Value: return JsonToken.BeginArray;
                    }
                    goto default;
                case ']':
                    switch (State.Current)
                    {
                        case JsonParserState.Resolver:
                        case JsonParserState.Array:
                        case JsonParserState.Separator: return JsonToken.EndArray;
                    }
                    goto default;

                // ------- Property -------
                case '\"':
                    switch (State.Current)
                    {
                        case JsonParserState.Object:
                            {
                                if (Rules.Identifier(this)) return JsonToken.Property;
                                else break;
                            }
                        case JsonParserState.Resolver:
                        case JsonParserState.Array:
                        case JsonParserState.Value:
                            {
                                if (Rules.Identifier(this) || State.Current == JsonParserState.Resolver) return JsonToken.String;
                                else break;
                            }
                    }
                    goto default;
                case ':':
                    switch (State.Current)
                    {
                        case JsonParserState.Resolver:
                        case JsonParserState.Property: return JsonToken.Colon;
                    }
                    goto default;
                case ',':
                    switch (State.Current)
                    {
                        case JsonParserState.Resolver:
                        case JsonParserState.Separator: return JsonToken.Comma;
                    }
                    goto default;

                // ------- Values -------
                case 'n':
                    {
                        if (Rules.NullConstant(this))
                            return JsonToken.Null;
                    }
                    goto default;
                case 'f':
                    {
                        if (Rules.FalseConstant(this))
                            return JsonToken.Boolean;
                    }
                    goto default;
                case 't':
                    {
                        if (Rules.TrueConstant(this))
                            return JsonToken.Boolean;
                    }
                    goto default;
                case '0':
                    {
                        if (!BaseRules.Digitt(this)) goto case '1';
                        else if (State == JsonParserState.Resolver)
                            return JsonToken.Integer;
                    }
                    goto default;
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '+':
                case '-':
                    switch (State.Current)
                    {
                        case JsonParserState.Resolver:
                        case JsonParserState.Array:
                        case JsonParserState.Value:
                            {
                                BaseRules.Numeral(this);
                                switch (PeekCharacter())
                                {
                                    case '.':
                                    case 'e':
                                    case 'E':
                                        {
                                            Position++;

                                            if (BaseRules.Numeral(this)) return JsonToken.Decimal;
                                            else Position--;
                                        }
                                        goto default;
                                    default: return JsonToken.Integer;
                                }
                            }
                    }
                    goto default;

                // ------- Fallback -------
                default:
                    {
                        Position--;
                        if (BaseRules.NullLiteral(this))
                            return JsonToken.Whitespace;

                        return JsonToken.Invalid;
                    }
            }
        }
    }
}
