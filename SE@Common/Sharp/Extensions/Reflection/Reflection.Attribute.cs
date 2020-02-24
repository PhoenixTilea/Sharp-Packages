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
        /// Gets an attribute of certain type from this MemberInfo if possible
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>The attribute instance for this member or null</returns>
        public static T GetAttribute<T>(this MemberInfo nfo) where T : Attribute
        {
            object[] attribs = nfo.GetCustomAttributes(typeof(T), true);
            return ((attribs == null || attribs.Length == 0) ? null : (T)attribs[0]);
        }
        /// <summary>
        /// Gets an attribute list of certain type from this MemberInfo if possible
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>The attribute list instance for this member or null</returns>
        public static T[] GetAttributes<T>(this MemberInfo nfo) where T : Attribute
        {
            object[] attribs = nfo.GetCustomAttributes(typeof(T), true);
            return ((attribs == null || attribs.Length == 0) ? null : (T[])attribs);
        }
        /// <summary>
        /// Gets an attribute of certain type from this MemberInfo if possible
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <param name="attrib">The attribute instance for this member</param>
        /// <returns>True if the member type contains an attribute of the given type, false otherwise</returns>
        public static bool TryGetAttribute<T>(this MemberInfo nfo, out T attrib) where T : Attribute
        {
            object[] attribs = nfo.GetCustomAttributes(typeof(T), true);
            attrib = ((attribs == null || attribs.Length == 0) ? null : (T)attribs[0]);
            return (attrib != null);
        }
        /// <summary>
        /// Gets an attribute list of certain type from this MemberInfo if possible
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <param name="attrib">The attribute list instance for this member</param>
        /// <returns>True if the member type contains an attribute of the given type, false otherwise</returns>
        public static bool TryGetAttributes<T>(this MemberInfo nfo, out T[] attrib) where T : Attribute
        {
            object[] attribs = nfo.GetCustomAttributes(typeof(T), true);
            attrib = ((attribs == null || attribs.Length == 0) ? null : (T[])attribs);
            return (attrib != null);
        }
        /// <summary>
        /// Determines if this member has certain attribute set
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>True if the member type contains an attribute of the given type, false otherwise</returns>
        public static bool HasAttribute<T>(this MemberInfo nfo) where T : Attribute
        {
            object[] attribs = nfo.GetCustomAttributes(typeof(T), true);
            return (attribs != null && attribs.Length != 0);
        }

        /// <summary>
        /// Determines if this type is an attribute
        /// </summary>
        /// <returns>True if this type inherits from System.Attribute, false otherwise</returns>
        public static bool IsAttribute(this Type type)
        {
            Type attributeType = typeof(Attribute);
            while (type.BaseType != null)
            {
                type = type.BaseType;
                if (type == attributeType)
                    return true;
            }
            return false;
        }
    }
}
