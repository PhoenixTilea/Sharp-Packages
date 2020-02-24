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
        /// Returns a type from this assembly with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A type instance or null</returns>
        public static Type GetType<T>(this Assembly assembly, bool inherit = false)
        {
            Type reference = typeof(T);

            if (reference.IsAttribute())
            {
                foreach (Type type in assembly.GetTypes())
                    if (type.IsDefined(reference, inherit))
                        return type;
            }
            else
            {
                foreach (Type type in assembly.GetTypes())
                    if (reference.IsAssignableFrom(type))
                        return type;
            }

            return null;
        }
        /// <summary>
        /// Returns a type list from this assembly with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A type list instance or null</returns>
        public static IEnumerable<Type> GetTypes<T>(this Assembly assembly, bool inherit = false)
        {
            List<Type> types = new List<Type>();
            Type reference = typeof(T);

            if (reference.IsAttribute())
            {
                foreach (Type type in assembly.GetTypes())
                    if (type.IsDefined(reference, inherit))
                        types.Add(type);
            }
            else
            {
                foreach (Type type in assembly.GetTypes())
                    if (reference.IsAssignableFrom(type))
                        types.Add(type);
            }

            return types;
        }
    }
}
