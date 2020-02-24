// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SE
{
    public static partial class ReflectionExtension
    {
        /// <summary>
        /// Gets an anonymous creator function from a given type
        /// </summary>
        /// <param name="delegateType">The type of creator function to search for</param>
        /// <param name="instanceType">The type to get a creator function for</param>
        /// <returns>The creator function delegate</returns>
        public static Delegate GetCreator(this Type delegateType, Type instanceType)
        {
            if (!typeof(Delegate).IsAssignableFrom(delegateType))
                throw new ArgumentNullException("delegateType");

            MethodInfo invoke = delegateType.GetMethod("Invoke");
            Type[] parameterTypes = invoke.GetParameters().Select(pi => pi.ParameterType).ToArray();
            Type resultType = invoke.ReturnType;
            if (!resultType.IsAssignableFrom(instanceType))
                throw new ArgumentNullException("instanceType");

            ConstructorInfo ctor = instanceType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null);
            if (ctor == null)
                throw new ArgumentNullException("instanceType");

            System.Linq.Expressions.ParameterExpression[] parapeters = parameterTypes.Select(System.Linq.Expressions.Expression.Parameter).ToArray();
            System.Linq.Expressions.LambdaExpression newExpression = System.Linq.Expressions.Expression.Lambda(delegateType, System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.New(ctor, parapeters), resultType), parapeters);
            Delegate result = newExpression.Compile();
            return result;
        }
        /// <summary>
        /// Gets an anonymous creator function from a given type
        /// </summary>
        /// <param name="delegateType">The type of creator function to search for</param>
        /// <typeparam name="T">The type to get a creator function for</typeparam>
        /// <returns>The creator function delegate</returns>
        public static T GetCreator<T>(this Type instanceType)
        {
            return (T)(object)GetCreator(typeof(T), instanceType);
        }

        /// <summary>
        /// Creates an object instance directly from a type if possible
        /// </summary>
        /// <typeparam name="T">The type of instance to be returned</typeparam>
        /// <returns>A newly created object instance or null</returns>
        public static T CreateInstance<T>(this Type type)
        {
            Func<T> creator = GetCreator<Func<T>>(type);
            if (creator != null) return creator();
            else return default(T);
        }
    }
}
