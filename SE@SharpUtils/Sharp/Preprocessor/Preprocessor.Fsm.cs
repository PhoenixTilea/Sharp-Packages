using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Sharp
{
    public partial class Preprocessor
    {
        Stack<int> productionStates = new Stack<int>();
        Stack<Tuple<Token, TextPointer, bool>> scopeStack = new Stack<Tuple<Token, TextPointer, bool>>();
        
        bool discardNonControlTokens;
        bool discardWhitespaceToken;
        bool discardNewLineToken;
        
        decimal line;
        UInt32 id;
        
        bool EndExecuteRule(ProductionState fsmCommand, int state)
        {
            switch (fsmCommand)
            {
                case ProductionState.Failure: BuilderState.Add(SharpPreprocessorState.Failure); return false;
                case ProductionState.Revert:
                case ProductionState.Success:
                {
                    if (BuilderState.Remove() != default(SharpPreprocessorState))
                    {
                        
                    }
                    else BuilderState.Reset();
                }
                break;
                default:
                {
                    if ((fsmCommand & ProductionState.Shift) == ProductionState.Shift)
                    {
                        productionStates.Push(state + 1);
                    }
                    else
                    {
                        productionStates.Push(state);
                    }
                    
                    if ((fsmCommand & ProductionState.Reduce) == ProductionState.Reduce)
                    {
                        productionStates.Push(0);
                    }
                    
                    if ((fsmCommand & ProductionState.Revert) == ProductionState.Revert)
                    {
                        goto case ProductionState.Revert;
                    }
                }
                break;
            }
            if ((fsmCommand & ProductionState.Preserve) == ProductionState.Preserve) return false;
            else return true;
        }
        
        int BeginExecuteRule()
        {
            if (productionStates.Count == 0)
                productionStates.Push(0);
            
            return productionStates.Pop();
        }
        
        void BeginConditional(Token token, bool state)
        {
            scopeStack.Push(Tuple.Create(token, Carret, state));
            EvaluateConditionalScope();
        }
        
        bool GetConditionalScope(bool skipCurrent)
        {
            bool result = true;
            foreach (Tuple<Token, TextPointer, bool> scope in scopeStack)
            {
                if (!skipCurrent)
                {
                    result &= scope.Item3;
                    switch (scope.Item1)
                    {
                        case Token.ElifDirective:
                        case Token.ElseDirective:
                        {
                            skipCurrent = true;
                        }
                        break;
                    }
                }
                else
                {
                    switch (scope.Item1)
                    {
                        case Token.IfDirective:
                        {
                            skipCurrent = false;
                        }
                        break;
                    }
                }
            }
            return result;
        }
        
        bool GetConditionalState()
        {
            bool result = false;
            foreach (Tuple<Token, TextPointer, bool> scope in scopeStack)
            {
                switch (scope.Item1)
                {
                    case Token.IfDirective:
                    {
                        result |= scope.Item3;
                    }
                    return result;
                    default:
                    {
                        result |= scope.Item3;
                    }
                    break;
                }
            }
            return result;
        }
        
        void EvaluateConditionalScope()
        {
            discardNonControlTokens = !GetConditionalScope(false);
        }
        
        void EndConditional()
        {
            for ( ; ; )
            {
                if (scopeStack.Count > 0)
                {
                    switch (scopeStack.Pop().Item1)
                    {
                        case Token.IfDirective:
                        {
                            EvaluateConditionalScope();
                        }
                        return;
                    }
                }
                else errors.AddFormatted(ErrorMessages.UnexpectedEndConditional, file, Carret);
            }
        }
    }
}