using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SE;

namespace SE.Text.Sharp
{
    public partial class Preprocessor
    {
        bool Primary(out decimal result)
        {
            result = 0;
            
            Token current = PreserveToken();
            switch (current)
            {
                #region ( expression )
                case Token.RoundBracketOpen:
                {
                    MoveNext();
                    
                    if (!LogicalOr(out result))
                    {
                        return false;
                    }
                    else if (PreserveToken() != Token.RoundBracketClose) break;
                    else MoveNext();
                }
                return true;
                #endregion
                
                #region Identifier
                case Token.Identifier:
                {
                    MoveNext();
                    
                    switch (Current)
                    {
                        #region false
                        case "false":
                        break;
                        #endregion
                        
                        #region true
                        case "true":
                        {
                            result = 1;
                        }
                        break;
                        #endregion
                        
                        #region defined identifier
                        default:
                        {
                            result = (defines.Contains(Current.Fnv32())) ? 1 : 0;
                            return true;
                        }
                                #endregion
                        }
                }
                return true;
                #endregion
                
                #region empty
                case Token.NewLine:
                {
                    errors.AddFormatted(ErrorMessages.MissingExpressionValue, file, Carret);
                }
                return false;
                #endregion
            }
            errors.AddFormatted(ErrorMessages.InvalidExpressionValue, file, Carret);
            return false;
        }
        
        /// <summary>
        /// ! Expression
        /// C# Operator Precedence #2
        /// </summary>
        bool Unary(out decimal result)
        {
            Token current = PreserveToken();
            switch (current)
            {
                #region !expression
                case Token.LogicalNot:
                {
                    MoveNext();
                    
                    if (Unary(out result))
                    {
                        result = (result == 0) ? 1 : 0;
                    }
                    else return false;
                }
                return true;
                #endregion
                
                default: return Primary(out result);
            }
        }
        
        /// <summary>
        // ==
        /// !=
        /// C# Operator Preceedence #7
        /// </summary>
        bool LogicalEquality(out decimal result)
        {
            if (Unary(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region expression == expression
                    case Token.Equal:
                    {
                        MoveNext();
                            
                        decimal tmp;
                        if (LogicalEquality(out tmp))
                        {
                            result = (result == tmp) ? 1 : 0;
                        }
                        else return false;
                    }
                    return true;
                    #endregion
                
                    #region expression != expression
                    case Token.NotEqual:
                    {
                        MoveNext();
                        
                        decimal tmp;
                        if (LogicalEquality(out tmp))
                        {
                            result = (result == tmp) ? 0 : 1;
                        }
                        else return false;
                    }
                    return true;
                    #endregion
                }
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// &&
        /// C# Operator Preceedence #11
        /// </summary>
        bool LogicalAnd(out decimal result)
        {
            if (LogicalEquality(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region expression && expression
                    case Token.LogicalAnd:
                    {
                        MoveNext();
                        
                        decimal tmp;
                        if (LogicalAnd(out tmp))
                        {
                            result = ((result != 0) && (tmp != 0)) ? 1 : 0;
                        }
                        else return false;
                    }
                    return true;
                    #endregion
                }
                return true;
            }
            return false;
        }
        
        /// <summary>
        // ||
        // C# Operator Preceedence #12
        /// </summary>
        bool LogicalOr(out decimal result)
        {
            if (LogicalAnd(out result))
            {
                Token current = PreserveToken();
                switch (current)
                {
                    #region expression || expresion
                    case Token.LogicalOr:
                    {
                        MoveNext();
                        
                        decimal tmp;
                        if (LogicalOr(out tmp))
                        {
                            result = ((result != 0) || (tmp != 0)) ? 1 : 0;
                        }
                        else return false;
                    }
                    return true;
                    #endregion
                }
                return true;
            }
            return false;
        }
        
    }
}