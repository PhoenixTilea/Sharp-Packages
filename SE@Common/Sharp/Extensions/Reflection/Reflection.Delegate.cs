// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SE
{
    public static partial class ReflectionExtension
    {
        /// <summary>
        /// Creates a function delegate from this method info object
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate CreateDelegate(this MethodInfo method, object target = null)
        {
            List<Type> types = new List<Type>();
            foreach (ParameterInfo param in method.GetParameters())
                types.Add(param.ParameterType);

            Func<Type[], Type> signatureProxy;
            if (method.ReturnType == (typeof(void))) signatureProxy = System.Linq.Expressions.MemberExpression.GetActionType;
            else
            {
                signatureProxy = System.Linq.Expressions.MemberExpression.GetFuncType;
                types.Add(method.ReturnType);
            }

            if (method.IsStatic) return Delegate.CreateDelegate(signatureProxy(types.ToArray()), method);
            else return Delegate.CreateDelegate(signatureProxy(types.ToArray()), target, method);
        }

        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<T>(this Func<T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A, T>(this Func<A, T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, T>(this Func<A0, A1, T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, T>(this Func<A0, A1, A2, T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, T>(this Func<A0, A1, A2, A3, T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, T>(this Func<A0, A1, A2, A3, A4, T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, A5, T>(this Func<A0, A1, A2, A3, A4, A5, T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, A5, A6, T>(this Func<A0, A1, A2, A3, A4, A5, A6, T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, A5, A6, A7, T>(this Func<A0, A1, A2, A3, A4, A5, A6, A7, T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, A5, A6, A7, A8, T>(this Func<A0, A1, A2, A3, A4, A5, A6, A7, A8, T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, A5, A6, A7, A8, A9, T>(this Func<A0, A1, A2, A3, A4, A5, A6, A7, A8, A9, T> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }

        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate(this Action function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A>(this Action<A> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1>(this Action<A0, A1> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2>(this Action<A0, A1, A2> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3>(this Action<A0, A1, A2, A3> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4>(this Action<A0, A1, A2, A3, A4> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, A5>(this Action<A0, A1, A2, A3, A4, A5> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, A5, A6>(this Action<A0, A1, A2, A3, A4, A5, A6> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, A5, A6, A7>(this Action<A0, A1, A2, A3, A4, A5, A6, A7> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, A5, A6, A7, A8>(this Action<A0, A1, A2, A3, A4, A5, A6, A7, A8> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
        /// <summary>
        /// Creates a function delegate from this function
        /// </summary>
        /// <param name="target">An object instance to bind the delegate to</param>
        /// <returns>A delegate to a function</returns>
        public static Delegate GetDelegate<A0, A1, A2, A3, A4, A5, A6, A7, A8, A9>(this Action<A0, A1, A2, A3, A4, A5, A6, A7, A8, A9> function, object target = null)
        {
            return function.Method.CreateDelegate(target);
        }
    }
}
