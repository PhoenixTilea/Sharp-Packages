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
        /// Returns a field from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A field info instance or null</returns>
        public static FieldInfo GetField<T>(this Type type)
        {
            Type attribType = typeof(T);

            foreach (FieldInfo field in type.GetFields())
                if (field.GetCustomAttributes(attribType, true).Length > 0)
                    return field;

            return null;
        }
        /// <summary>
        /// Returns a field from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A field info instance or null</returns>
        public static FieldInfo GetField<T>(this Type type, BindingFlags flags)
        {
            Type attribType = typeof(T);

            foreach (FieldInfo field in type.GetFields(flags))
                if (field.GetCustomAttributes(attribType, true).Length > 0)
                    return field;

            return null;
        }
        /// <summary>
        /// Returns a field list from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A field info list instance or null</returns>
        public static IEnumerable<FieldInfo> GetFields<T>(this Type type)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            Type attribType = typeof(T);

            foreach (FieldInfo field in type.GetFields())
                if (field.GetCustomAttributes(attribType, true).Length > 0)
                    fields.Add(field);

            return fields;
        }
        /// <summary>
        /// Returns a field list from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A field info list instance or null</returns>
        public static IEnumerable<FieldInfo> GetFields<T>(this Type type, BindingFlags flags)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            Type attribType = typeof(T);

            foreach (FieldInfo field in type.GetFields(flags))
                if (field.GetCustomAttributes(attribType, true).Length > 0)
                    fields.Add(field);

            return fields;
        }
    }
}
