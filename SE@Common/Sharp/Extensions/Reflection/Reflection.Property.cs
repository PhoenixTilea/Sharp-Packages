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
        /// Returns a property from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A property info instance or null</returns>
        public static PropertyInfo GetProperty<T>(this Type type)
        {
            Type attribType = typeof(T);

            foreach (PropertyInfo property in type.GetProperties())
                if (property.GetCustomAttributes(attribType, true).Length > 0)
                    return property;

            return null;
        }
        /// <summary>
        /// Returns a property from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A property info instance or null</returns>
        public static PropertyInfo GetProperty<T>(this Type type, BindingFlags flags)
        {
            Type attribType = typeof(T);

            foreach (PropertyInfo property in type.GetProperties(flags))
                if (property.GetCustomAttributes(attribType, true).Length > 0)
                    return property;

            return null;
        }
        /// <summary>
        /// Returns a property list from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A property info list instance or null</returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>(this Type type)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            Type attribType = typeof(T);

            foreach (PropertyInfo property in type.GetProperties())
                if (property.GetCustomAttributes(attribType, true).Length > 0)
                    properties.Add(property);

            return properties;
        }
        /// <summary>
        /// Returns a property list from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A property info list instance or null</returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>(this Type type, BindingFlags flags)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            Type attribType = typeof(T);

            foreach (PropertyInfo property in type.GetProperties(flags))
                if (property.GetCustomAttributes(attribType, true).Length > 0)
                    properties.Add(property);

            return properties;
        }
    }
}
