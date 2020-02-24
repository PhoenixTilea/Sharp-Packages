// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using SE;

namespace SE.Reflection.Compound
{
    public static partial class ExpressionExtension
    {
        /// <summary>
        /// Extracts the expressions from a list of meta objects
        /// </summary>
        /// <param name="objects">A list of meta objects to handle</param>
        /// <returns>The list of expressions contained</returns>
        public static Expression[] ToExpressionList(this DynamicMetaObject[] objects)
        {
            return Array.ConvertAll<DynamicMetaObject, Expression>(objects, (input) =>
            {
                return input.Expression;

            });
        }
        /// <summary>
        /// Extracts the expressions from a list of meta objects
        /// </summary>
        /// <param name="objects">A list of meta objects to handle</param>
        /// <returns>The list of expressions contained</returns>
        public static Expression[] ToExpressionList(this DynamicMetaObject[] objects, Expression headElement)
        {
            Expression[] result = new Expression[objects.Length + 1];
            result[0] = headElement;

            for (int i = 0; i < objects.Length; i++)
                result[i + 1] = objects[i].Expression;

            return result;
        }
        /// <summary>
        /// Extracts the expressions from a list of meta objects
        /// </summary>
        /// <param name="objects">A list of meta objects to handle</param>
        /// <returns>The list of expressions contained</returns>
        public static Expression[] ToExpressionList<T>(this DynamicMetaObject[] objects)
        {
            Type listType = typeof(T);
            return Array.ConvertAll<DynamicMetaObject, Expression>(objects, (input) =>
            {
                return Expression.Convert(input.Expression, listType);

            });
        }
        /// <summary>
        /// Extracts the expressions from a list of meta objects
        /// </summary>
        /// <param name="objects">A list of meta objects to handle</param>
        /// <returns>The list of expressions contained</returns>
        public static Expression[] ToExpressionList<T>(this DynamicMetaObject[] objects, Expression headElement)
        {
            Type listType = typeof(T);

            Expression[] result = new Expression[objects.Length + 1];
            result[0] = Expression.Convert(headElement, listType);

            for (int i = 0; i < objects.Length; i++)
                result[i + 1] = Expression.Convert(objects[i].Expression, listType);

            return result;
        }
    }
}
