using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Sharp
{
    public partial class Preprocessor : TreeBuilder<Token, SharpTokenizerState, SharpPreprocessorState>
    {
        HashSet<UInt32> resolverStack;

        List<UInt32> defines;
        /// <summary>
        /// A collection of symbols defined for this preprocessor unit
        /// </summary>
        public List<UInt32> Defines
        {
            get
            {
                return defines;
            }
        }

        List<string> errors;
        /// <summary>
        /// A collection of error messages accrued during build
        /// </summary>
        public List<string> Errors
        {
            get
            {
                return errors;
            }
        }

        string file;
        /// <summary>
        /// The file currently processed
        /// </summary>
        public string File
        {
            get
            {
                return file;
            }
        }

        string origFile = string.Empty;
        /// <summary>
        /// Stores the original name of the processed file, as "file" may be changed by a #line directive
        /// </summary
        public string OrigFile 
        {
            get
            {
                return origFile;
            }
        }
        
        decimal lineOffset = 0;
        /// <summary>
        /// Tracks the current offset from the actual line in a processed file to allow reset after being
        /// altered by a #line directive
        /// </summary>
        public decimal LineOffset
        {
            get
            {
                return lineOffset;
            }
        }

        /// <summary>
        /// Creates a new preprocessor instance
        /// </summary>
        public Preprocessor()
        {
            this.defines = new List<UInt32>();
            this.resolverStack = new HashSet<UInt32>();
            this.errors = new List<string>();
            this.discardNonControlTokens = false;
            this.discardWhitespaceToken = true;
            this.discardNewLineToken = true;
            this.lineOffset = 0;
        }

        /// <summary>
        /// Defines a preprocessor symbol
        /// </summary>
        /// <param name="name">The symbol to be defined</param>
        /// <returns>False if the named symbol already exists, true otherwise</returns>
        public bool Define(string name)
        {
            UInt32 id = name.Fnv32();
            if (!defines.Contains(id))
            {
                defines.Add(id);
                return true;
            }
            else
                return false;
        }

        protected override StreamTokenizer<Token, SharpTokenizerState> Begin(Stream stream, bool isUtf8)
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
                case Token.IfDirective:
                case Token.ElifDirective:
                case Token.ElseDirective:
                case Token.EndifDirective:
                    {
                        discardNonControlTokens = false;
                        return false;
                    }
                case Token.NewLine:
                    {
                        switch (BuilderState.Current)
                        {
                            case SharpPreprocessorState.If:
                            case SharpPreprocessorState.Elif:
                            case SharpPreprocessorState.Else:
                            case SharpPreprocessorState.Endif:
                            case SharpPreprocessorState.Error:
                            case SharpPreprocessorState.Warning:
                            case SharpPreprocessorState.Define:
                                {
                                    return false;
                                }
                            default:
                                {
                                    if (discardNewLineToken && BuilderState.Current == SharpPreprocessorState.Master)
                                    {
                                        OnNextCompilerToken(new CompilerToken(Token.Whitespace, string.Empty, Carret));
                                    }
                                    return discardNewLineToken;
                                }
                        }
                    }
                case Token.Whitespace:
                    {
                        if (discardWhitespaceToken && BuilderState.Current == SharpPreprocessorState.Master)
                        {
                            OnNextCompilerToken(new CompilerToken(Token.Whitespace, string.Empty, Carret));
                        }
                        return discardWhitespaceToken;
                    }
                case Token.SingleLineComment:
                case Token.MultiLineComment:
                    {
                        if (BuilderState.Current == SharpPreprocessorState.Master)
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
                case SharpPreprocessorState.Master:
                    {
                        if (Master(ref token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #if
                case SharpPreprocessorState.If:
                    {
                        if (If(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #elif
                case SharpPreprocessorState.Elif:
                    {
                        if (Elif(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #else
                case SharpPreprocessorState.Else:
                    {
                        if (Else(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #endif
                case SharpPreprocessorState.Endif:
                    {
                        if (Endif(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #error
                case SharpPreprocessorState.Error:
                    {
                        if (Error(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #warning
                case SharpPreprocessorState.Warning:
                    {
                        if (Warning(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #define
                case SharpPreprocessorState.Define:
                    {
                        if (Define(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #undef
                case SharpPreprocessorState.Undef:
                    {
                        if (Undef(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #line
                case SharpPreprocessorState.Line:
                    {
                        if (Line(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #region
                case SharpPreprocessorState.Region:
                    {
                        if (Region(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region endregion
                case SharpPreprocessorState.Endregion:
                    {
                        if (Endregion(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region #pragma
                case SharpPreprocessorState.Pragma:
                    {
                        if (Pragma(token))
                            break;
                        else
                            goto Head;
                    }
                #endregion

                #region failure
                case SharpPreprocessorState.Failure:
                    {
                        MoveToEnd();
                    }
                    return false;
                #endregion

                default:
                    throw new ArgumentException(BuilderState.Current.ToString());
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
                        for (bool @break = false; !EndOfStream && !@break;)
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

                #region #if
                case Token.IfDirective:
                    {
                        BuilderState.Set(SharpPreprocessorState.If);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #elif
                case Token.ElifDirective:
                    {
                        BuilderState.Set(SharpPreprocessorState.Elif);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #else
                case Token.ElseDirective:
                    {
                        BuilderState.Set(SharpPreprocessorState.Else);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #endif
                case Token.EndifDirective:
                    {
                        BuilderState.Set(SharpPreprocessorState.Endif);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #define
                case Token.DefineDirective:
                    {
                        BuilderState.Set(SharpPreprocessorState.Define);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #undef
                case Token.UndefDirective:
                    {
                        BuilderState.Set(SharpPreprocessorState.Undef);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #error
                case Token.Error:
                    {
                        BuilderState.Set(SharpPreprocessorState.Error);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #warning
                case Token.Warning:
                    {
                        BuilderState.Set(SharpPreprocessorState.Warning);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #line
                case Token.Line:
                    {
                        BuilderState.Set(SharpPreprocessorState.Line);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #pragma
                case Token.Pragma:
                    {
                        BuilderState.Set(SharpPreprocessorState.Pragma);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region #region
                case Token.Region:
                    {
                        BuilderState.Set(SharpPreprocessorState.Region);
                    }
                    return ProductionState.Reduce;
                #endregion

                #region endregion
                case Token.Endregion:
                    {
                        BuilderState.Set(SharpPreprocessorState.Endregion);
                    }
                    return ProductionState.Reduce;
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
                            bool result;
                            if (EvaluateExpression(out result))
                            {
                                BeginConditional(Token.IfDirective, result);
                            }
                            else
                                BeginConditional(Token.IfDirective, false);
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
            else
                BeginConditional(Token.IfDirective, false);
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

        #region  #elif
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
                            case Token.Identifier:
                                {
                                    BeginConditional(Token.ElifDirective, defines.Contains(Current.Fnv32()));
                                }
                                break;
                            default:
                                {
                                    errors.AddFormatted(ErrorMessages.InvalidMacroName, file, Carret);
                                }
                                return ProductionState.Failure;
                        }
                    }
                    else
                        BeginConditional(Token.ElifDirective, false);
                }
                else
                    errors.AddFormatted(ErrorMessages.UnexpectedElifConditional, file, new TextPointer(Carret.Line, 0));
            }
            else
                errors.AddFormatted(ErrorMessages.UnexpectedElifConditional, file, new TextPointer(Carret.Line, 0));
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
                else
                    BeginConditional(Token.ElseDirective, !GetConditionalState());
            }
            else
                errors.AddFormatted(ErrorMessages.UnexpectedElseConditional, file, new TextPointer(Carret.Line, 0));
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

        #region #warning
        protected virtual ProductionState Warning(ref int state, Token token)
        {
            StringBuilder sb = new StringBuilder();
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                sb.Append(Current);
                token = MoveNext();
            }
            errors.AddFormatted(ErrorMessages.PreprocessorWarning, file, Carret, sb.ToString());
            return ProductionState.Success;
        }
        bool Warning(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Warning(ref state, token), state);
        }
        #endregion

        #region #define
        protected virtual ProductionState Define(ref int state, Token token)
        {
            switch (state)
            {
                case 0:
                    {
                        switch (token)
                        {
                            case Token.Identifier:
                                {
                                    id = Current.Fnv32();
                                    if (defines.Contains(id))
                                    {
                                        errors.AddFormatted(ErrorMessages.MacroRedefinition, file, Carret, Current);
                                        goto default;
                                    }
                                    else
                                    {
                                        discardNewLineToken = false;
                                        discardWhitespaceToken = false;
                                        defines.Add(id);
                                    }
                                }
                                break;

                            default:
                                {
                                    errors.AddFormatted(ErrorMessages.InvalidMacroName, file, Carret);
                                }
                                return ProductionState.Failure;
                        }
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
                            default:
                                {
                                    return ProductionState.Shift | ProductionState.Preserve;
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
        protected virtual ProductionState Undef(ref int state, Token token)
        {
            switch (token)
            {
                case Token.Identifier:
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
        bool Undef(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Undef(ref state, token), state);
        }
        #endregion

        #region #line
        protected virtual ProductionState Line(ref int state, Token token)
        {
            if (origFile == string.Empty)
            {
                origFile = file;
            }
            switch (state)
            {
                case 0:
                    {
                        switch (token)
                        {
                            #region identifier
                            case Token.Identifier:
                                {
                                    #region default
                                    if (Current == "default")
                                    {
                                        if (file != origFile)
                                            file = origFile;

                                        line = tokenizer.Carret.Line + lineOffset;
                                        tokenizer.Carret = new TextPointer(decimal.ToUInt32(line), Carret.Column);

                                        discardNewLineToken = false;
                                        return ProductionState.Shift;
                                    }
                                    #endregion

                                    #region hidden
                                    else if (Current == "hidden")
                                    {
                                        discardNewLineToken = false;
                                        return ProductionState.Shift;
                                    }
                                    #endregion

                                    #region invalid
                                    else
                                        goto default;
                                    #endregion
                                }
                            #endregion

                            #region Numeric
                            case Token.Numeric:
                                {
                                    decimal actLine = tokenizer.Carret.Line + lineOffset;
                                    if (!decimal.TryParse(Current, NumberStyles.Integer | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out line) || line < 0)
                                    {
                                        goto default;
                                    }
                                    tokenizer.Carret = new TextPointer(decimal.ToUInt32(line), Carret.Column);
                                    lineOffset = actLine - line;
                                    if (file != origFile)
                                    {
                                        file = origFile;
                                    }
                                    discardNewLineToken = false;
                                    return ProductionState.Shift;
                                }
                            #endregion

                            default:
                                {
                                    errors.AddFormatted(ErrorMessages.InvalidLineNumber, file, Carret);
                                }
                                return ProductionState.Failure;
                        }
                    }

                case 1:
                    {
                        if (token == Token.StringLiteral)
                        {
                            file = Current;
                        }
                        else if (token == Token.BogusStringLiteral)
                        {
                            errors.AddFormatted(ErrorMessages.UnterminatedFileName, file, Carret, "#line");
                            return ProductionState.Failure;
                        }
                        else if (token == Token.NewLine)
                            break;
                        else
                        {
                            errors.AddFormatted(ErrorMessages.InvalidFileName, file, Carret, "#line");
                            return ProductionState.Failure;
                        }
                    }
                    break;
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

        #region #region
        protected virtual ProductionState Region(ref int state, Token token)
        {
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Region(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Region(ref state, token), state);
        }
        #endregion

        #region endregion
        protected virtual ProductionState Endregion(ref int state, Token token)
        {
            while (!tokenizer.EndOfStream && token != Token.NewLine)
            {
                token = MoveNext();
            }
            return ProductionState.Success;
        }
        bool Endregion(Token token)
        {
            int state = BeginExecuteRule();
            return EndExecuteRule(Endregion(ref state, token), state);
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
                decimal tmp;
                if (LogicalOr(out tmp))
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
        /// A function to be overridden in any inherited class. Will be executed
        /// for any token ready to be processed by the compiler
        /// </summary>
        /// <param name="token"></param>
        protected virtual void OnNextCompilerToken(CompilerToken token)
        { }

        protected override bool Finalize(bool result, object context)
        {
            foreach (Tuple<Token, TextPointer, bool> scope in scopeStack)
            {
                errors.AddFormatted(ErrorMessages.UnterminatedDirective, file, new TextPointer(scope.Item2.Line, 0));
            }
            return result;
        }
    }
}

