// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Cpp
{
    public partial class Preprocessor
    {
        /// <summary>
        /// A collection of reserved macro names that might need to be expanded
        /// implicitly
        /// </summary>
        protected static class ReservedMacros
        {
            public const string File = "__FILE__";
            public const string Line = "__LINE__";
            public const string Date = "__DATE__";
            public const string Time = "__TIME__";

            public const string VariadicArgumentList = "__VA_ARGS__";
            public const string VariadicConditional = "__VA_OPT__";
            public const string VariadicConditional_Non_Empty = "__VA_OPT__Non_Empty__";
            public const string VariadicConditional_Empty = "__VA_OPT__Empty__";

            public const string Standard = "__STDC__";
            public const string StandardVer = "__STDC_VERSION__";
            public const string StandardLib = "__STDC_HOSTED__";

            public const string Cpp = "__cplusplus";
            public const string Objective_C = "__OBJC__";
            public const string Assembly = "__ASSEMBLER__";
        }
        
        void ProcessTokenPasting(List<CompilerToken> left, List<CompilerToken> right, List<CompilerToken> result)
        {
            for (int i = 0; i < left.Count - 1; i++)
            {
                result.Add(left[i]);
            }
            {
                CompilerToken l = left[left.Count - 1];
                CompilerToken r = right[0];
                if(ProcessTokenPasting(l, r, result))
                {
                    for (int i = 0; i < result.Count; i++)
                        if (result[i].Type == Token.Concat)
                        {
                            result[i] = new CompilerToken
                            (
                                Token.DoubleHash,
                                result[i].Buffer,
                                result[i].Carret
                            );
                        }
                }
            }
            for (int i = 1; i < right.Count; i++)
            {
                result.Add(right[i]);
            }
        }
        void ProcessTokenPasting(List<CompilerToken> result)
        {
            for (int index = 0; result.TryGetIndex(x => x.Type == Token.Concat, out index);)
            {
                List<CompilerToken> left = null;
                List<CompilerToken> right = null;
                try
                {
                    left = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
                    for (int i = 0; i < index; i++)
                    {
                        left.Add(result[i]);
                    }
                    if (left.Count > 0)
                    {
                        if (left[left.Count - 1].Type == Token.Whitespace)
                        {
                            if (left.Count == 1)
                            {
                                left[left.Count - 1] = new CompilerToken(Token.Whitespace, string.Empty, left[left.Count - 1].Carret);
                            }
                            else left.RemoveAt(left.Count - 1);
                        }
                    }
                    else
                    {
                        result.RemoveAt(index);
                        break;
                    }
                    right = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
                    for (int i = (index + 1); i < result.Count; i++)
                    {
                        right.Add(result[i]);
                    }
                    if (right.Count > 0)
                    {
                        if (right[0].Type == Token.Whitespace)
                        {
                            if (right.Count == 1)
                            {
                                right[0] = new CompilerToken(Token.Whitespace, string.Empty, right[0].Carret);
                            }
                            else right.RemoveAt(0);
                        }
                    }
                    else
                    {
                        result.RemoveAt(index);
                        break;
                    }
                    result.Clear();
                    ProcessTokenPasting(left, right, result);
                }
                finally
                {
                    if (left != null)
                    {
                        CollectionPool<List<CompilerToken>, CompilerToken>.Return(left);
                    }
                    if (right != null)
                    {
                        CollectionPool<List<CompilerToken>, CompilerToken>.Return(right);
                    }
                }
            }
        }
        bool ProcessTokenPasting(CompilerToken l, CompilerToken r, List<CompilerToken> result)
        {
            #region Encoding
            Encoding encoding; if (tokenizer.IsUtf8)
            {
                encoding = Encoding.UTF8;
            }
            else encoding = Encoding.ASCII;
            #endregion

            MemoryStream ms = new MemoryStream(encoding.GetBytes(String.Concat(l.Buffer, r.Buffer)));
            Tokenizer t = new Tokenizer(ms, tokenizer.IsUtf8);
            t.IsAtLineStart = false;

            List<CompilerToken> tokens = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
            TextPointer carret = l.Carret;

            do
            {
                TextPointer tmp = t.Carret;
                Token token = t.Read();
                if (token != Token.NewLine)
                {
                    tokens.Add(new CompilerToken
                    (
                        token,
                        t.Buffer,
                        new TextPointer
                        (
                            carret.Line + tmp.Line,
                            carret.Column + tmp.Column
                        )
                    ));
                }
            }
            while (!t.EndOfStream);
            try
            {
                switch (tokens.Count)
                {
                    #region Success
                    case 1:
                        {
                            result.Add(tokens[0]);
                            return true;
                        }
                    #endregion

                    #region Check
                    case 2:
                        {
                            switch (tokens[0].Type)
                            {
                                case Token.Identifier:
                                    {
                                        switch (tokens[1].Type)
                                        {
                                            case Token.Numeric:
                                            case Token.StringLiteral:
                                            case Token.CharacterLiteral:
                                                {
                                                    result.Add(l);
                                                    result.Add(r);
                                                }
                                                return true;
                                        }
                                    }
                                    goto Ill_Formed;
                                default:
                                    goto Ill_Formed;
                            }
                        }
                    #endregion

                    #region Ill-Formed
                    default:
                    Ill_Formed:
                        {
                            errors.AddFormatted(ErrorMessages.InvalidPastingToken, file, Carret, String.Concat(l.Buffer, r.Buffer));
                            result.Add(l);
                            result.Add(r);
                        }
                        return false;
                    #endregion
                }
            }
            finally
            {
                CollectionPool<List<CompilerToken>, CompilerToken>.Return(tokens);
            }
        }

        bool ExpandFunctionMacro(Macro macro, bool flush)
        {
            if (resolverStack.Contains(macro.Id))
                return false;

            List<List<CompilerToken>> parameters; if (ReadParameterList(macro, out parameters))
            {
                List<CompilerToken> tokens = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
                tokens.AddRange(macro.ReplacementList);
                resolverStack.Add(macro.Id);

                int variadicCommaExtension = 0;
                if (parameters.Count > 0)
                {
                    for (int i = 0; i < tokens.Count; i++)
                        switch (tokens[i].Type)
                        {
                            #region Whitespace
                            case Token.Whitespace:
                            case Token.Comment:
                            case Token.NewLine:
                                break;
                            #endregion

                            #region Identifier
                            case Token.Identifier:
                                {
                                    int index;
                                    switch (tokens[i].Buffer)
                                    {
                                        #region __VA_ARGS__
                                        case ReservedMacros.VariadicArgumentList:
                                            {
                                                if (macro.IsVariadic)
                                                {
                                                    index = macro.Parameter.Count;

                                                    #region Extension
                                                    /**
                                                      A compiler extension (, ## __VA_ARGS__) that removes the leading comma if
                                                     __VA_ARGS__ is empty
                                                    */
                                                    if (variadicCommaExtension == 2)
                                                    {
                                                        while (i - 1 > 0 && tokens[i - 1].Type != Token.Comma)
                                                        {
                                                            tokens.RemoveAt(i - 1);
                                                            i--;
                                                        }
                                                        if (parameters[index].Count == 0)
                                                        {
                                                            tokens.RemoveAt(i - 1);
                                                            i--;
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else index = -1;
                                            }
                                            break;
                                        #endregion

                                        #region __VA_OPT__
                                        case ReservedMacros.VariadicConditional:
                                            {
                                                if (macro.IsVariadic)
                                                {
                                                    string name; if (parameters[macro.Parameter.Count].Count > 0)
                                                    {
                                                        name = ReservedMacros.VariadicConditional_Non_Empty;
                                                    }
                                                    else name = ReservedMacros.VariadicConditional_Empty;
                                                    if (ExpandMacro(name, tokens, false, false, false))
                                                    {
                                                        continue;
                                                    }
                                                }
                                                index = -1;
                                            }
                                            break;
                                        #endregion

                                        #region Anything else
                                        default:
                                            {
                                                if (!macro.Parameter.TryGetIndex(tokens[i].Buffer.Fnv32(), out index))
                                                {
                                                    index = -1;
                                                }
                                            }
                                            break;
                                        #endregion
                                    }
                                    if (index >= 0)
                                    {
                                        List<CompilerToken> parameter = parameters[index];

                                        tokens.RemoveAt(i);
                                        tokens.InsertRange(i, parameter);
                                        i += (parameter.Count - 1);
                                    }
                                }
                                goto default;
                            #endregion

                            #region Stringify
                            case Token.Stringify:
                                {
                                    tokens.RemoveAt(i);
                                    while (tokens[i].Type == Token.Whitespace)
                                    {
                                        tokens.RemoveAt(i);
                                    }
                                    int index; 
                                    switch (tokens[i].Buffer)
                                    {
                                        #region __VA_ARGS__
                                        case ReservedMacros.VariadicArgumentList:
                                            {
                                                if (macro.IsVariadic)
                                                {
                                                    index = macro.Parameter.Count;
                                                }
                                                else index = -1;
                                            }
                                            break;
                                        #endregion

                                        #region __VA_OPT__
                                        case ReservedMacros.VariadicConditional:
                                            {
                                                if (macro.IsVariadic)
                                                {
                                                    string name; if (parameters[macro.Parameter.Count].Count > 0)
                                                    {
                                                        name = ReservedMacros.VariadicConditional_Non_Empty;
                                                    }
                                                    else name = ReservedMacros.VariadicConditional_Empty;
                                                    if (ExpandMacro(name, tokens, false, false, false))
                                                    {
                                                        continue;
                                                    }
                                                }
                                                index = -1;
                                            }
                                            break;
                                        #endregion

                                        #region Anything else
                                        default:
                                            {
                                                if (!macro.Parameter.TryGetIndex(tokens[i].Buffer.Fnv32(), out index))
                                                {
                                                    index = -1;
                                                }
                                            }
                                            break;
                                        #endregion
                                    }
                                    if (index >= 0)
                                    {
                                        List<CompilerToken> parameter = parameters[index];
                                        StringBuilder sb = new StringBuilder();
                                        foreach (CompilerToken t in parameter)
                                        {
                                            sb.Append(t.Buffer);
                                        }
                                        tokens[i] = new CompilerToken
                                        (
                                            Token.StringLiteral,
                                            sb.ToString(),
                                            (parameter.Count > 0) ? parameter[0].Carret : tokens[i].Carret
                                        );
                                    }
                                    else tokens[i] = new CompilerToken(Token.StringLiteral, tokens[i].Buffer, tokens[i].Carret);
                                }
                                goto default;
                            #endregion

                            #region (,)
                            case Token.Comma:
                                {
                                    variadicCommaExtension = 1;
                                }
                                break;
                            #endregion

                            #region (##)
                            case Token.Concat:
                                {
                                    variadicCommaExtension <<= 1;
                                }
                                break;
                            #endregion

                            #region Anything else
                            default:
                                {
                                    variadicCommaExtension = 0;
                                }
                                break;
                            #endregion
                        }
                }
                foreach (List<CompilerToken> parameter in parameters)
                {
                    CollectionPool<List<CompilerToken>, CompilerToken>.Return(parameter);
                }
                CollectionPool<List<List<CompilerToken>>, List<CompilerToken>>.Return(parameters);

                ProcessTokenPasting(tokens);
                int count = 0;
                for (int i = tokens.Count - 1; i >= 0; i--)
                {
                    CompilerToken token = tokens[i];
                    tokens.RemoveAt(i);

                    if (token.Type == Token.Identifier && ExpandMacro(token.Buffer, null, false, false, false))
                    {
                        continue;
                    }
                    PushToken(Tuple.Create(token.Type, token.RawBuffer, Carret));
                    count++;
                }
                CollectionPool<List<CompilerToken>, CompilerToken>.Return(tokens);
                if (flush)
                {
                    Flush(ProcessToken, count);
                }
                resolverStack.Remove(macro.Id);
                return true;
            }
            else return false;
        }
        bool ReadParameterList(Macro macro, out List<List<CompilerToken>> parameters)
        {
            List<CompilerToken> preserved = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
            preserved.Add(new CompilerToken(Token.Identifier, Current, Carret));
            parameters = null;
            try
            {
                #region Detect Parameter List Start
                while (!EndOfStream)
                {
                    Token token = MoveNext();
                    switch (token)
                    {
                        case Token.Whitespace:
                        case Token.Comment:
                        case Token.NewLine:
                            {
                                preserved.Add(new CompilerToken(token, Current, Carret));
                            }
                            break;
                        case Token.RoundBracketOpen:
                            {
                                preserved.Add(new CompilerToken(token, Current, Carret));
                            }
                            goto ParameterList;
                        default:
                            {
                                for (int i = preserved.Count - 1; i >= 0; i--)
                                {
                                    CompilerToken ptoken = preserved[i];
                                    PushToken(Tuple.Create(ptoken.Type, ptoken.RawBuffer, ptoken.Carret));
                                }
                                PushToken(Tuple.Create(token, Current, Carret));
                            }
                            return false;
                    }
                }
                #endregion

                #region Parameter List Mismatch
            Error:

                for (int i = preserved.Count - 1; i >= 0; i--)
                {
                    CompilerToken ptoken = preserved[i];
                    PushToken(Tuple.Create(ptoken.Type, ptoken.RawBuffer, ptoken.Carret));
                }
                MoveNext();
                return false;
                #endregion

                #region Read Parameter List
            ParameterList:

                List<CompilerToken> parameter = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
                parameters = CollectionPool<List<List<CompilerToken>>, List<CompilerToken>>.Get();
                parameters.Add(parameter);

                bool variableArgs = (macro.IsVariadic && macro.Parameter.Count == 0);
                int scope = 0;

                while (!EndOfStream)
                {
                    Token token = MoveNext();
                    switch (token)
                    {
                        case Token.NewLine:
                        case Token.Whitespace:
                        case Token.Comment:
                            {
                                preserved.Add(new CompilerToken(token, Current, Carret));
                                if (parameter.Count > 0 && parameter[parameter.Count - 1].Type != Token.Whitespace)
                                {
                                    parameter.Add(new CompilerToken(Token.Whitespace, Current, Carret));
                                }
                            }
                            break;
                        case Token.Comma:
                            {
                                if (!variableArgs)
                                {
                                    preserved.Add(new CompilerToken(token, Current, Carret));
                                    while (parameter.Count > 0 && parameter[parameter.Count - 1].Type == Token.Whitespace)
                                    {
                                        parameter.RemoveAt(parameter.Count - 1);
                                    }
                                    parameter = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
                                    parameters.Add(parameter);

                                    variableArgs = (macro.IsVariadic && macro.Parameter.Count < parameters.Count);
                                }
                                else goto default;
                            }
                            break;
                        case Token.RoundBracketOpen:
                            {
                                scope++;
                            }
                            goto default;
                        case Token.RoundBracketClose:
                            {
                                if (scope == 0)
                                {
                                    preserved.Add(new CompilerToken(token, Current, Carret));
                                    if (parameters.Count == 1 && parameter.Count == 0)
                                    {
                                        parameters.RemoveAt(0);
                                        CollectionPool<List<CompilerToken>, CompilerToken>.Return(parameter);
                                    }
                                    goto Success;
                                }
                                else scope--;
                            }
                            goto default;
                        case Token.Concat:
                            {
                                preserved.Add(new CompilerToken(token, Current, Carret));
                                parameter.Add(new CompilerToken(Token.DoubleHash, Current, Carret));
                            }
                            break;
                        case Token.Stringify:
                            {
                                preserved.Add(new CompilerToken(token, Current, Carret));
                                parameter.Add(new CompilerToken(Token.Hash, Current, Carret));
                            }
                            break;
                        default:
                            {
                                preserved.Add(new CompilerToken(token, Current, Carret));
                                parameter.Add(new CompilerToken(token, Current, Carret));
                            }
                            break;
                    }
                }
                errors.AddFormatted(ErrorMessages.ParameterTerminatorMissing, file, Carret);
                #endregion

                #region Validate Parameter List
            Success:

                bool success = (macro.Parameter.Count == parameters.Count);
                if (macro.IsVariadic)
                {
                    if (parameters.Count > macro.Parameter.Count)
                    {
                        success = true;
                    }
                    else if (success)
                    {
                        while (parameter.Count > 0 && parameter[parameter.Count - 1].Type == Token.Whitespace)
                        {
                            parameter.RemoveAt(parameter.Count - 1);
                        }
                        parameter = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
                        parameters.Add(parameter);
                    }
                }
                if (!success)
                {
                    errors.AddFormatted(ErrorMessages.ParameterMismatch, file, Carret, macro.Name);
                    goto Error;
                }
                else
                {
                    while (parameter.Count > 0 && parameter[parameter.Count - 1].Type == Token.Whitespace)
                    {
                        parameter.RemoveAt(parameter.Count - 1);
                    }
                    return true;
                }
                #endregion
            }
            finally
            {
                CollectionPool<List<CompilerToken>, CompilerToken>.Return(preserved);
            }
        }

        bool ExpandObjectMacro(Macro macro, List<CompilerToken> resolverList, bool flush)
        {
            if (resolverStack.Contains(macro.Id))
                return false;

            List<CompilerToken> tokens = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
            tokens.AddRange(macro.ReplacementList);
            resolverStack.Add(macro.Id);

            ProcessTokenPasting(tokens);
            if (resolverList == null)
            {
                int count = tokens.Count;
                for (int i = tokens.Count - 1; i >= 0; i--)
                {
                    CompilerToken token = tokens[i];
                    PushToken(Tuple.Create(token.Type, token.RawBuffer, Carret));
                }
                CollectionPool<List<CompilerToken>, CompilerToken>.Return(tokens);
                if (flush)
                {
                    Flush(ProcessToken, count);
                }
            }
            else resolverList.AddRange(tokens);
            resolverStack.Remove(macro.Id);
            return true;
        }

        bool ExpandMacro(string name, List<CompilerToken> resolverList, bool flushObjectMacro, bool expandFunctionStyleMacro, bool flushfunctionStyleMacro)
        {
            Macro macro; if (defines.TryGetValue(name.Fnv32(), out macro))
            {
                if (macro.Parameter.Count > 0 || macro.IsVariadic)
                {
                    if (expandFunctionStyleMacro)
                    {
                        return ExpandFunctionMacro(macro, flushfunctionStyleMacro);
                    }
                    else return false;
                }
                else return ExpandObjectMacro(macro, resolverList, flushObjectMacro);
            }
            else switch (name)
                {
                    #region __FILE__
                    case ReservedMacros.File:
                        {
                            PushToken(Tuple.Create(Token.StringLiteral, file, Carret));
                        }
                        return true;
                    #endregion

                    #region __LINE__
                    case ReservedMacros.Line:
                        {
                            PushToken(Tuple.Create(Token.Numeric, Carret.Line.ToString(), Carret));
                        }
                        return true;
                    #endregion

                    #region __DATE__
                    case ReservedMacros.Date:
                        {
                            PushToken(Tuple.Create(Token.StringLiteral, String.Format("{0:MM dd yyyy}", DateTime.Now), Carret));
                        }
                        return true;
                    #endregion

                    #region __TIME__
                    case ReservedMacros.Time:
                        {
                            PushToken(Tuple.Create(Token.StringLiteral, String.Format("{0:HH:mm:ss}", DateTime.Now), Carret));
                        }
                        return true;
                    #endregion

                    #region __STDC__
                    case ReservedMacros.Standard:
                    #endregion

                    #region __STDC_VERSION__
                    case ReservedMacros.StandardVer:
                    #endregion

                    #region __STDC_HOSTED__
                    case ReservedMacros.StandardLib:
                    #endregion

                    #region __cplusplus
                    case ReservedMacros.Cpp:
                    #endregion

                    #region __OBJC__
                    case ReservedMacros.Objective_C:
                    #endregion

                    #region __ASSEMBLER__
                    case ReservedMacros.Assembly:
                        {
                            return ResolveReservedMacro(name, null);
                        }
                        #endregion
                }
            return false;
        }
    }
}
