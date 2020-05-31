// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Cpp
{
    /// <summary>
    /// ISO/IEC C++20 Standard Compliant Preprocessor
    /// https://en.cppreference.com/w/cpp/preprocessor
    /// </summary>
    public partial class Preprocessor : TreeBuilder<Token, CppTokenizerState, CppPreprocessorState>
    {
        HashSet<UInt32> resolverStack;

        Dictionary<UInt32, Macro> defines;
        /// <summary>
        /// A collection of macros defined for this preprocessor unit
        /// </summary>
        public Dictionary<UInt32, Macro> Defines
        {
            get { return defines; }
        }

        List<string> errors;
        /// <summary>
        /// A collection of error messages occurred during build
        /// </summary>
        public List<string> Errors
        {
            get { return errors; }
        }

        string file;
        /// <summary>
        /// The file currently processed
        /// </summary>
        public string File
        {
            get { return file; }
        }

        /// <summary>
        /// Creates a new preprocessor instance
        /// </summary>
        /// <param name="source">A defined location the preprocessor will push created tokens to</param>
        public Preprocessor()
        {
            this.defines = new Dictionary<UInt32, Macro>();
            this.resolverStack = new HashSet<uint>();
            this.errors = new List<string>();
            this.discardNonControlTokens = false;
            this.discardWhitespaceToken = true;
            this.discardNewLineToken = true;
        }

        /// <summary>
        /// Defines a macro for this preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, params CompilerToken[] replacementList)
        {
            UInt32 id = name.Fnv32();
            if (!defines.ContainsKey(id))
            {
                Macro macro = new Macro(id, name);
                macro.ReplacementList.AddRange(replacementList);
                defines.Add(macro.Id, macro);

                return true;
            }
            else return false;
        }
        /// <summary>
        /// Defines a macro for this preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, string replacementList)
        {
            if (!defines.ContainsKey(name.Fnv32()))
            {
                MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(replacementList));
                Tokenizer t = new Tokenizer(ms, false);
                t.State.Set(CppTokenizerState.AfterWhitespace);

                List<CompilerToken> tokens = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
                try
                {
                    TextPointer carret = t.Carret;
                    Token token = t.Read();

                    while (!t.EndOfStream && token != Token.NewLine)
                    {
                        tokens.Add(new CompilerToken
                        (
                            token,
                            t.Buffer,
                            carret
                        ));
                        carret = t.Carret;
                        token = t.Read();
                    }
                    return Define(name, tokens.ToArray());
                }
                finally
                {
                    CollectionPool<List<CompilerToken>, CompilerToken>.Return(tokens);
                }
            }
            else return false;
        }
        /// <summary>
        /// Defines a function like macro for this preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="parameter">A collection of input parameters for this macro</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, string[] parameter, params CompilerToken[] replacementList)
        {
            UInt32 id = name.Fnv32();
            if (!defines.ContainsKey(id))
            {
                Macro macro = new Macro(id, name);
                foreach (string p in parameter)
                {
                    macro.Parameter.Add(p.Fnv32());
                }
                macro.HasParameter = true;
                macro.ReplacementList.AddRange(replacementList);
                defines.Add(macro.Id, macro);

                return true;
            }
            else return false;
        }
        /// <summary>
        /// Defines a function like macro for this preprocessor unit
        /// </summary>
        /// <param name="name">The name of the macro to be defined</param>
        /// <param name="parameter">A collection of input parameters for this macro</param>
        /// <param name="replacementList">An optional collection of tokens to add to the source stream</param>
        /// <returns>False if a macro with the same name already exists, true otherwise</returns>
        public bool Define(string name, string[] parameter, string replacementList)
        {
            if (!defines.ContainsKey(name.Fnv32()))
            {
                MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(replacementList));
                Tokenizer t = new Tokenizer(ms, false);
                t.State.Set(CppTokenizerState.AfterWhitespace);

                List<CompilerToken> tokens = CollectionPool<List<CompilerToken>, CompilerToken>.Get();
                try
                {
                    TextPointer carret = t.Carret;
                    Token token = t.Read();

                    while (!t.EndOfStream && token != Token.NewLine)
                    {
                        tokens.Add(new CompilerToken
                        (
                            token,
                            t.Buffer,
                            carret
                        ));
                        carret = t.Carret;
                        token = t.Read();
                    }
                    return Define(name, parameter, tokens.ToArray());
                }
                finally
                {
                    CollectionPool<List<CompilerToken>, CompilerToken>.Return(tokens);
                }
            }
            else return false;
        }

        protected override StreamTokenizer<Token, CppTokenizerState> Begin(Stream stream, bool isUtf8)
        {
            BuilderState.Reset();
            errors.Clear();
            
            this.discardNonControlTokens = false;
            this.discardWhitespaceToken = true;
            this.discardNewLineToken = true;

            return new Tokenizer(stream, isUtf8);
        }

        protected override bool DiscardToken(Token token, object context)
        {
            switch (token)
            {
                case Token.IfdefDirective:
                case Token.IfndefDirective:
                case Token.IfDirective:
                case Token.ElifDirective:
                case Token.ElseDirective:
                case Token.EndifDirective:
                    {
                        discardNonControlTokens = false;
                        return false;
                    }
                case Token.NewLine: switch (BuilderState.Current)
                        {
                            case CppPreprocessorState.Ifdef:
                            case CppPreprocessorState.Ifndef:
                            case CppPreprocessorState.If:
                            case CppPreprocessorState.Include:
                            case CppPreprocessorState.Elif:
                            case CppPreprocessorState.Else:
                            case CppPreprocessorState.Endif:
                            case CppPreprocessorState.Error:
                            case CppPreprocessorState.Define:
                                {

                                }
                                return false;
                        default:
                            {
                                if (discardNewLineToken && BuilderState.Current == CppPreprocessorState.Master)
                                {
                                    OnNextCompilerToken(new CompilerToken(Token.Whitespace, string.Empty, Carret));
                                }
                                return discardNewLineToken;
                            }
                        }
                case Token.Whitespace:
                    {
                        if (discardWhitespaceToken && BuilderState.Current == CppPreprocessorState.Master)
                        {
                            OnNextCompilerToken(new CompilerToken(Token.Whitespace, string.Empty, Carret));
                        }
                        return discardWhitespaceToken;
                    }
                case Token.Comment:
                    {
                        if (BuilderState.Current == CppPreprocessorState.Master)
                        {
                            OnNextCompilerToken(new CompilerToken(Token.Whitespace, string.Empty, Carret));
                        }
                        return true;
                    }
                default:
                    {
                        return discardNonControlTokens;
                    }
            }
        }

        protected override bool ProcessToken(Token token, object context)
        {
            file = context as string;

        Head:
            switch (BuilderState.Current)
            {
                #region Master
                case CppPreprocessorState.Master:
                    {
                        if (Master(ref token)) break;
                        else goto Head;
                    }
                #endregion

                #region #ifdef
                case CppPreprocessorState.Ifdef:
                    {
                        if (Ifdef(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #ifndef
                case CppPreprocessorState.Ifndef:
                    {
                        if (Ifndef(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #if
                case CppPreprocessorState.If:
                    {
                        if (If(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #include
                case CppPreprocessorState.Include:
                    {
                        if (Include(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #elif
                case CppPreprocessorState.Elif:
                    {
                        if (Elif(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #else
                case CppPreprocessorState.Else:
                    {
                        if (Else(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #endif
                case CppPreprocessorState.Endif:
                    {
                        if (Endif(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #error
                case CppPreprocessorState.Error:
                    {
                        if (Error(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #define
                case CppPreprocessorState.Define:
                    {
                        if (Define(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #undef
                case CppPreprocessorState.Undefine:
                    {
                        if (Undefine(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #line
                case CppPreprocessorState.Line:
                    {
                        if (Line(token)) break;
                        else goto Head;
                    }
                #endregion

                #region #pragma
                case CppPreprocessorState.Pragma:
                    {
                        if (Pragma(token)) break;
                        else goto Head;
                    }
                #endregion

                #region FunctionMacro = '(' Identifier (',' Identifier)* ')';
                case CppPreprocessorState.FunctionMacro:
                    {
                        if (FunctionMacro(token)) break;
                        else goto Head;
                    }
                #endregion

                #region Failure
                case CppPreprocessorState.Failure:
                    {
                        MoveToEnd();
                    }
                    return false;
                #endregion

                default: throw new ArgumentException(BuilderState.Current.ToString());
            }
            return true;
        }

        #region Master
        protected virtual ProductionState Master(ref int state, ref Token token)
        {
            switch (token)
            {
                #region BogusDirective
                case Token.BogusDirective:
                    {
                        token = MoveNext();
                        for (bool @break = false; !EndOfStream && !@break; )
                        {
                            switch (token)
                            {
                                case Token.NewLine:
                                    {
                                        @break = true;
                                    }
                                    break;
                                default:
                                    {
                                        errors.AddFormatted(ErrorMessages.InvalidDirective, file, Carret, Current);
                                        @break = true;
                                    }
                                    break;
                            }
                        }
                        while (!tokenizer.EndOfStream && token != Token.NewLine)
                        {
                            token = MoveNext();
                        }
                    }
                    break;
                #endregion

                #region Empty
                case Token.Empty:
                    break;
                #endregion

                #region #ifdef
                case Token.IfdefDirective:
                    {
                        BuilderState.Set(CppPreprocessorState.Ifdef);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #ifndef
                case Token.IfndefDirective:
                    {
                        BuilderState.Set(CppPreprocessorState.Ifndef);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #if
                case Token.IfDirective:
                    {
                        BuilderState.Set(CppPreprocessorState.If);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #include
                case Token.IncludeDirective:
                    {
                        BuilderState.Set(CppPreprocessorState.Include);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #elif
                case Token.ElifDirective:
                    {
                        BuilderState.Set(CppPreprocessorState.Elif);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #else
                case Token.ElseDirective:
                    {
                        BuilderState.Set(CppPreprocessorState.Else);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #endif
                case Token.EndifDirective:
                    {
                        BuilderState.Set(CppPreprocessorState.Endif);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #error
                case Token.Error:
                    {
                        BuilderState.Set(CppPreprocessorState.Error);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #define
                case Token.DefineDirective:
                    {
                        BuilderState.Set(CppPreprocessorState.Define);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #undef
                case Token.UndefDirective:
                    {
                        BuilderState.Set(CppPreprocessorState.Undefine);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #line
                case Token.Line:
                    {
                        BuilderState.Set(CppPreprocessorState.Line);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #pragma
                case Token.Pragma:
                    {
                        BuilderState.Set(CppPreprocessorState.Pragma);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region Macro
                case Token.Identifier:
                    {
                        if (!ExpandMacro(Current))
                            goto default;
                    }
                    break;
                #endregion

                #region Translate Concat (##) => DoubleHash (##)
                case Token.Concat:
                    {
                        token = Token.DoubleHash;
                    }
                    return ProductionState.Preserve;
                #endregion

                #region Translate Stringify (#) => Hash (#)
                case Token.Stringify:
                    {
                        token = Token.Hash;
                    }
                    return ProductionState.Preserve;
                #endregion

                default:
                    {
                        OnNextCompilerToken(new CompilerToken(token, Current, Carret));
                    }
                    break;
            }
            return ProductionState.Success;
        }
        bool Master(ref Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Master(ref state, ref token), state);
        }
        #endregion

        #region #ifdef
        protected virtual ProductionState Ifdef(ref int state, Token token)
        {
            if (GetConditionalScope(false))
            {
                switch (token)
                {
                    case Token.Identifier:
                    case Token.New:
                    case Token.Delete:
                        {
                            BeginConditional(Token.IfdefDirective, defines.ContainsKey(Current.Fnv32()));
                        }
                        break;
                    default:
                        {
                            errors.AddFormatted(ErrorMessages.InvalidMacroName, file, Carret);
                        }
                        return ProductionState.Failure;
                }
            }
            else BeginConditional(Token.IfDirective, false);
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Ifdef(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Ifdef(ref state, token), state);
        }
        #endregion

        #region #ifndef
        protected virtual ProductionState Ifndef(ref int state, Token token)
        {
            if (GetConditionalScope(false))
            {
                switch (token)
                {
                    case Token.Identifier:
                    case Token.New:
                    case Token.Delete:
                        {
                            BeginConditional(Token.IfndefDirective, !defines.ContainsKey(Current.Fnv32()));
                        }
                        break;
                    default:
                        {
                            errors.AddFormatted(ErrorMessages.InvalidMacroName, file, Carret);
                        }
                        return ProductionState.Failure;
                }
            }
            else BeginConditional(Token.IfDirective, false);
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Ifndef(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Ifndef(ref state, token), state);
        }
        #endregion

        #region #if
        protected virtual ProductionState If(ref int state, Token token)
        {
            if (GetConditionalScope(false))
            {
                switch (token)
                {
                    default:
                        {
                            PushToken(Tuple.Create(token, Current, Carret));
                            bool result; if (EvaluateExpression(out result))
                            {
                                BeginConditional(Token.IfDirective, result);
                            }
                            else BeginConditional(Token.IfDirective, false);
                        }
                        break;
                    case Token.NewLine:
                        {
                            errors.AddFormatted(ErrorMessages.MissingExpressionValue, file, Carret);
                            BeginConditional(Token.IfDirective, false);
                        }
                        break;
                }
            }
            else BeginConditional(Token.IfDirective, false);
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool If(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(If(ref state, token), state);
        }
        #endregion

        #region #include
        protected virtual ProductionState Include(ref int state, Token token)
        {
            switch (token)
            {
                #region Macro
                case Token.Identifier:
                    {
                        if (!ExpandMacro(Current))
                            goto default;
                    }
                    return ProductionState.Next;
                #endregion

                #region Fallback UnqoutedHeaderName
                case Token.AngleBracketOpen:
                    {
                        TextPointer carret = Carret;
                        StringBuilder sb = new StringBuilder();
                        token = MoveNext();

                        for(bool @break = false; !@break;)
                        {
                            switch (token)
                            {
                                case Token.NewLine:
                                    {
                                        sb.Clear();
                                    }
                                    goto case Token.AngleBracketClose;
                                case Token.AngleBracketClose:
                                    {
                                        @break = true;
                                    }
                                    break;
                                default:
                                    {
                                        sb.Append(Current);
                                    }
                                    break;
                            }
                            token = MoveNext();
                        }
                        if (sb.Length > 0)
                        {
                            PushToken(Tuple.Create(token, Current, Carret));
                            PushToken(Tuple.Create(Token.UnqoutedHeaderName, sb.ToString(), carret));
                            return ProductionState.Next;
                        }
                        else goto default;
                    }
                #endregion

                #region UnqoutedHeaderName
                case Token.UnqoutedHeaderName:
                #endregion

                #region StringLiteral
                case Token.StringLiteral:
                    {
                        Stream stream;
                        string path;

                        if (ResolveFileReference(new CompilerToken(token, Current, Carret), out path, out stream))
                        {
                            if (!ExpandFileReference(path, stream))
                            {
                                return ProductionState.Failure;
                            }
                            else break;
                        }
                        else if (!string.IsNullOrWhiteSpace(path))
                        {
                            errors.AddFormatted(ErrorMessages.FileNotFound, file, Carret, path);
                            return ProductionState.Failure;
                        }
                        else break;
                    }
                #endregion

                #region UnqoutedHeaderName
                case Token.BogusUnqoutedHeaderName:
                #endregion

                #region BogusStringLiteral
                case Token.BogusStringLiteral:
                    {
                        errors.AddFormatted(ErrorMessages.UnterminatedFileName, file, Carret, "#include");
                    }
                    break;
                #endregion

                default:
                    {
                        errors.AddFormatted(ErrorMessages.InvalidFileName, file, Carret, "#include");
                    }
                    break;
            }
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Include(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Include(ref state, token), state);
        }
        #endregion

        #region #elif
        protected virtual ProductionState Elif(ref int state, Token token)
        {
            if (scopeStack.Count > 0)
            {
                if (scopeStack.Peek().Item1 != Token.ElseDirective)
                {
                    if (GetConditionalScope(true) && !GetConditionalState())
                    {
                        switch (token)
                        {
                            default:
                                {
                                    PushToken(Tuple.Create(token, Current, Carret));
                                    bool result; if (EvaluateExpression(out result))
                                    {
                                        BeginConditional(Token.ElifDirective, result);
                                    }
                                    else BeginConditional(Token.ElifDirective, false);
                                }
                                break;
                            case Token.NewLine:
                                {
                                    errors.AddFormatted(ErrorMessages.MissingExpressionValue, file, Carret);
                                    BeginConditional(Token.ElifDirective, false);
                                }
                                break;
                        }
                    }
                    else BeginConditional(Token.ElifDirective, false);
                }
                else errors.AddFormatted(ErrorMessages.UnexpectedElifConditional, file, new TextPointer(Carret.Line, 0));
            }
            else errors.AddFormatted(ErrorMessages.UnexpectedElifConditional, file, new TextPointer(Carret.Line, 0));
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Elif(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Elif(ref state, token), state);
        }
        #endregion

        #region #else
        protected virtual ProductionState Else(ref int state, Token token)
        {
            if (scopeStack.Count > 0)
            {
                Tuple<Token, TextPointer, bool> scope = scopeStack.Peek();
                if (scope.Item1 == Token.ElseDirective)
                {
                    errors.AddFormatted(ErrorMessages.UnexpectedElseConditional, file, new TextPointer(Carret.Line, 0));
                    BeginConditional(Token.ElseDirective, false);
                }
                else BeginConditional(Token.ElseDirective, !GetConditionalState());
            }
            else errors.AddFormatted(ErrorMessages.UnexpectedElseConditional, file, new TextPointer(Carret.Line, 0));
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Else(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Else(ref state, token), state);
        }
        #endregion

        #region #endif
        protected virtual ProductionState Endif(ref int state, Token token)
        {
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            EndConditional();
            return ProductionState.Success;
        }
        bool Endif(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Endif(ref state, token), state);
        }
        #endregion

        #region #error
        protected virtual ProductionState Error(ref int state, Token token)
        {
            StringBuilder sb = new StringBuilder();
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                sb.Append(Current);
                token = MoveNext();
            }
            errors.AddFormatted(ErrorMessages.PreprocessorError, file, Carret, sb.ToString());
            scopeStack.Clear();
            return ProductionState.Failure;
        }
        bool Error(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Error(ref state, token), state);
        }
        #endregion

        #region #define
        protected virtual ProductionState Define(ref int state, Token token)
        {
            switch (state)
            {
                case 0: switch (token)
                    {
                        case Token.Identifier:
                        case Token.New:
                        case Token.Delete:
                            {
                                id = Current.Fnv32();
                                if (defines.ContainsKey(id))
                                {
                                    errors.AddFormatted(ErrorMessages.MacroRedefinition, file, Carret, Current);
                                    goto default;
                                }
                                else
                                {
                                    discardNewLineToken = false;
                                    discardWhitespaceToken = false;
                                    defines.Add(id, new Macro(id, Current));
                                }
                            }
                            break;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.InvalidMacroName, file, Carret);
                            }
                            return ProductionState.Failure;
                    }
                    return ProductionState.Shift;

                case 1:
                    {
                        discardWhitespaceToken = true;
                        switch (token)
                        {
                            case Token.NewLine:
                                {
                                    state = 3;
                                }
                                break;
                            case Token.RoundBracketOpen:
                                {
                                    BuilderState.Add(CppPreprocessorState.FunctionMacro);
                                }
                                return ProductionState.Shift | ProductionState.Reduce;
                            default:
                                {
                                    return ProductionState.Shift | ProductionState.Preserve;
                                }
                        }
                    }
                    goto default;

                case 2:
                    {
                        Macro macro; if (defines.TryGetValue(id, out macro))
                        {
                            List<CompilerToken> replacementList = macro.ReplacementList;
                            bool expectsDefinedParameter = false;
                            while (!tokenizer.EndOfStream && token != Token.NewLine)
                            {
                                switch (token)
                                {
                                    case Token.Whitespace:
                                    case Token.Comment:
                                        {
                                            if (replacementList.Count > 0 && replacementList[replacementList.Count - 1].Type != Token.Whitespace)
                                            {
                                                replacementList.Add(new CompilerToken(Token.Whitespace, Current, Carret));
                                            }
                                        }
                                        break;
                                    case Token.Identifier:
                                        {
                                            expectsDefinedParameter = false;
                                        }
                                        goto default;
                                    case Token.Stringify:
                                        {
                                            if (macro.HasParameter)
                                            {
                                                replacementList.Add(new CompilerToken(token, Current, Carret));
                                                expectsDefinedParameter = true;
                                            }
                                            else replacementList.Add(new CompilerToken(Token.Hash, Current, Carret));
                                        }
                                        break;
                                    default:
                                        {
                                            if (expectsDefinedParameter)
                                            {
                                                errors.AddFormatted(ErrorMessages.InvalidStringificationToken, file, Carret);
                                                return ProductionState.Failure;
                                            }
                                            replacementList.Add(new CompilerToken(token, Current, Carret));
                                        }
                                        break;
                                }
                                token = MoveNext();
                            }
                            while (replacementList.Count > 0 && replacementList[replacementList.Count - 1].Type == Token.Whitespace)
                            {
                                replacementList.RemoveAt(replacementList.Count - 1);
                            }
                            if (replacementList.Count > 0)
                            {
                                if (replacementList[0].Type == Token.Concat)
                                {
                                    errors.AddFormatted(ErrorMessages.InvalidPastingOperatorStart, file, replacementList[0].Carret);
                                    defines.Remove(id);
                                }
                                else if (replacementList[replacementList.Count - 1].Type == Token.Concat)
                                {
                                    errors.AddFormatted(ErrorMessages.InvalidPastingOperatorEnd, file, replacementList[replacementList.Count - 1].Carret);
                                    defines.Remove(id);
                                }
                            }
                        }
                    }
                    goto default;

                default:
                    {
                        while (!tokenizer.EndOfStream && token != Token.NewLine)
                        {
                            token = MoveNext();
                        }
                        discardWhitespaceToken = true;
                        discardNewLineToken = true;
                    }
                    return ProductionState.Success;
            }
        }
        bool Define(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Define(ref state, token), state);
        }
        #endregion

        #region #undef
        protected virtual ProductionState Undefine(ref int state, Token token)
        {
            switch (token)
            {
                case Token.Identifier:
                case Token.New:
                case Token.Delete:
                    {
                        if (!defines.Remove(Current.Fnv32()))
                        {
                            errors.AddFormatted(ErrorMessages.MacroUndefined, file, Carret, Current);
                        }
                    }
                    break;
                default:
                    {
                        errors.AddFormatted(ErrorMessages.InvalidMacroName, file, Carret);
                    }
                    return ProductionState.Failure;
            }
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Undefine(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Undefine(ref state, token), state);
        }
        #endregion

        #region #line
        protected virtual ProductionState Line(ref int state, Token token)
        {
            switch (state)
            {
                #region Number | >>Identifier
                case 0: switch (token)
                    {
                        case Token.Identifier:
                            {
                                if (!ExpandMacro(Current))
                                    goto default;

                                discardNewLineToken = false;
                            }
                            return ProductionState.Next;
                        case Token.Numeric:
                            {
                                if (!decimal.TryParse(Current, NumberStyles.Integer | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out line) || line < 0)
                                    goto default;

                                discardNewLineToken = false;
                            }
                            return ProductionState.Shift;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.InvalidLineNumber, file, Carret);
                            }
                            return ProductionState.Failure;
                    }
                #endregion

                #region (Literal | >>Identifier)?
                case 1:
                    {
                        switch (token)
                        {
                            case Token.Identifier:
                                {
                                    if (!ExpandMacro(Current))
                                    {
                                        errors.AddFormatted(ErrorMessages.InvalidFileName, file, Carret, "#line");
                                    }
                                }
                                return ProductionState.Next;
                            case Token.StringLiteral:
                                {
                                    file = Current;
                                }
                                break;
                            case Token.BogusStringLiteral:
                                {
                                    errors.AddFormatted(ErrorMessages.UnterminatedFileName, file, Carret, "#line");
                                }
                                return ProductionState.Failure;
                        }
                        tokenizer.Carret = new TextPointer(decimal.ToUInt32(line), Carret.Column);
                    }
                    break;
                #endregion
            }
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            discardNewLineToken = true;
            return ProductionState.Success;
        }
        bool Line(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Line(ref state, token), state);
        }
        #endregion

        #region #pragma
        protected virtual ProductionState Pragma(ref int state, Token token)
        {
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Pragma(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Pragma(ref state, token), state);
        }
        #endregion

        #region FunctionMacro = '(' (Identifier (',' Identifier)* )? ((',' VariadicArguments) | VariadicArguments)? ')';
        protected virtual ProductionState FunctionMacro(ref int state, Token token)
        {
            switch (state)
            {
                #region (Identifier ... )? (... | VariadicArguments)? ')'
                case 0: switch (token)
                    {
                        case Token.RoundBracketClose:
                            {
                                defines[id].HasParameter = true;
                                discardWhitespaceToken = false;
                            }
                            return ProductionState.Success;
                        case Token.Identifier:
                        case Token.New:
                        case Token.Delete:
                            {
                                defines[id].Parameter.Add(Current.Fnv32());
                            }
                            return ProductionState.Shift;
                        case Token.VariableArgs:
                            {
                                defines[id].IsVariadic = true;
                                state = 3;
                            }
                            return ProductionState.Next;
                        case Token.NewLine:
                            {
                                errors.AddFormatted(ErrorMessages.MissingParameterListTerminator, file, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.InvalidParameter, file, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                    }
                #endregion

                #region ... ( ... (',' ...)* )? ...
                case 1: switch (token)
                    {
                        case Token.RoundBracketClose:
                            {
                                defines[id].HasParameter = true;
                                discardWhitespaceToken = false;
                            }
                            return ProductionState.Success;
                        case Token.Comma:
                            return ProductionState.Shift;
                        case Token.NewLine:
                            {
                                errors.AddFormatted(ErrorMessages.MissingParameterListTerminator, file, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.MissingParameterListSeparator, file, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                    }
                #endregion

                #region ... ( ... (... Identifier)* )? ((... VariadicArgs) | ...)? ...
                case 2: switch (token)
                    {
                        case Token.Identifier:
                        case Token.New:
                        case Token.Delete:
                            {
                                defines[id].Parameter.Add(Current.Fnv32());
                                state = 1;
                            }
                            return ProductionState.Next;
                        case Token.VariableArgs:
                            {
                                defines[id].IsVariadic = true;
                            }
                            return ProductionState.Shift;
                        case Token.NewLine:
                            {
                                errors.AddFormatted(ErrorMessages.MissingParameterListTerminator, file, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.InvalidParameter, file, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                    }
                #endregion

                #region ')'
                case 3: switch (token)
                    {
                        case Token.RoundBracketClose:
                            {
                                defines[id].HasParameter = true;
                                discardWhitespaceToken = false;
                            }
                            return ProductionState.Success;
                        default:
                            {
                                errors.AddFormatted(ErrorMessages.MissingParameterListTerminator, file, Carret);
                                defines.Remove(id);
                            }
                            return ProductionState.Revert;
                    }
                #endregion

                default: return ProductionState.Failure;
            }
        }
        bool FunctionMacro(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(FunctionMacro(ref state, token), state);
        }
        #endregion

        /// <summary>
        /// Tries to evaluate upcoming tokens into a logical state
        /// </summary>
        /// <param name="result">The result of the evaluation</param>
        /// <returns>True if the line was read entirely, false otherwise</returns>
        protected bool EvaluateExpression(out bool result)
        {
            try
            {
                discardNewLineToken = false;
                decimal tmp; if (TernaryConditional(out tmp))
                {
                    if (EndOfStream || PreserveToken() == Token.NewLine)
                    {
                        result = (tmp != 0);
                        return true;
                    }
                    else errors.AddFormatted(ErrorMessages.InvalidExpressionValue, file, Carret);
                }
                result = false;
                return false;
            }
            finally
            {
                discardNewLineToken = true;
            }
        }

        /// <summary>
        /// Tries to expand the given macro into the stream
        /// </summary>
        /// <param name="name">The name of any defined macro in this preprocessor</param>
        /// <returns>True if the macro could be expanded, false otherwise</returns>
        protected bool ExpandMacro(string name)
        {
            return ExpandMacro(name, null, true, true, true);
        }
        /// <summary>
        /// A function to be overridden in any inherited class. Will be executed
        /// if a reserved macro name was found. The inheriting class should pass
        /// values to the resolverList or preservation queue in order to succeed
        /// </summary>
        /// <returns>True if the macro was resolved properly, false otherwise</returns>
        protected virtual bool ResolveReservedMacro(string name, List<CompilerToken> resolverList)
        {
            return false;
        }

        /// <summary>
        /// Tries to resolve a file reference token into a valid path and a stream to
        /// be processed by this preprocessor instance
        /// </summary>
        /// <param name="source">A file specification to include</param>
        /// <param name="path">The absolute path to the specified file or null</param>
        /// <param name="stream">A stream to the specified file's data or null</param>
        /// <returns>True if the file's content was load properly, false otherwise</returns>
        protected virtual bool ResolveFileReference(CompilerToken source, out string path, out Stream stream)
        {
            stream = null;
            path = null;
            return false;
        }
        /// <summary>
        /// Preserves current state and parses an embedded file reference into the 
        /// current source stream
        /// </summary>
        /// <param name="path">A path the source data is located at</param>
        /// <param name="stream">A stream of source data to be processed</param>
        /// <returns>False if a critical error occurred during parsing, true otherwise</returns>
        protected virtual bool ExpandFileReference(string path, Stream stream)
        {
            List<bool> states = CollectionPool<List<bool>, bool>.Get();
            states.Add(discardNonControlTokens);
            states.Add(discardWhitespaceToken);
            states.Add(discardNewLineToken);
            string currentFile = file;

            StreamTokenizer<Token, CppTokenizerState> tmpTokenizer = tokenizer;
            Stack<int> tmpProductionStates = productionStates;

            productionStates = StackPool<int>.Get();

            try
            {
                if (Parse(stream, true, path))
                {
                    return true;
                }
                else return false;
            }
            finally
            {
                BuilderState.Add(CppPreprocessorState.Include);

                StackPool<int>.Return(productionStates);

                productionStates = tmpProductionStates;
                tokenizer = tmpTokenizer;

                file = currentFile;
                discardNewLineToken = states[2];
                discardWhitespaceToken = states[1];
                discardNonControlTokens = states[0];
                CollectionPool<List<bool>, bool>.Return(states);
            }
        }

        /// <summary>
        /// A function to be overridden in any inherited class. Will be executed
        /// for any token ready to be processed by the compiler
        /// </summary>
        /// <param name="token"></param>
        protected virtual void OnNextCompilerToken(CompilerToken token)
        { }

        protected override bool Finalize(bool result, object context)
        {
            if (BuilderState.Current == CppPreprocessorState.FunctionMacro)
            {
                errors.AddFormatted(ErrorMessages.MissingParameterListTerminator, file, Carret);
                return false;
            }
            else
            {
                foreach (Tuple<Token, TextPointer, bool> scope in scopeStack)
                {
                    errors.AddFormatted(ErrorMessages.UnterminatedDirective, file, new TextPointer(scope.Item2.Line, 0));
                }
                return result;
            }
        }
    }
}
