// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using SE;

namespace SE.Config
{
    /// <summary>
    /// An auto configure provider processing string pairs
    /// </summary>
    public static class AutoConfig
    {
        /// <summary>
        /// Applies an already processed list of string pairs to certain object instance
        /// </summary>
        /// <param name="target">An object instance to do field and property lookup</param>
        /// <param name="args">String pairs to match to fields and proeprties</param>
        /// <returns>A collection of result information</returns>
        public static AutoConfigResult Load(object target, IEnumerable<string> args, bool ignoreCase = false)
        {
            AutoConfigResult result = AutoConfigResult.Create();
            try
            {
                Dictionary<string, List<string>> preparsed = ArgumentConverter.GetOptions(args, ignoreCase);
                MapToObject(target, preparsed, ignoreCase, ref result);
            }
            catch (Exception er)
            {
                result.Errors.Add(er.Message);
            }
            return result;
        }
        /// <summary>
        /// Applies an already processed list of string pairs to any 
        /// static fields and properties of Type T
        /// </summary>
        /// <param name="args">String pairs to match to fields and proeprties</param>
        /// <returns>A collection of result information</returns>
        public static AutoConfigResult Load<T>(IEnumerable<string> args, bool ignoreCase = false)
        {
            AutoConfigResult result = AutoConfigResult.Create();
            try
            {
                Dictionary<string, List<string>> preparsed = ArgumentConverter.GetOptions(args, ignoreCase);
                MapToObject<T>(preparsed, ignoreCase, ref result);
            }
            catch (Exception er)
            {
                result.Errors.Add(er.Message);
            }
            return result;
        }

        private static void MapDefaultArgs(object target, FieldInfo[] fields, PropertyInfo[] properties, Dictionary<string, List<string>> options, int minIndex, ref AutoConfigResult result)
        {
            int defaultArgsCounter = 0;
            foreach(KeyValuePair<string, List<string>> args in options.Where(x => x.Value.Count > 0))
            {
                string key = args.Key;
                if (args.Value[0] == null)
                {
                    args.Value[0] = key;
                    key = null;

                    if (SetDefault(target, fields, args.Value, ref minIndex, ref result) || SetDefault(target, properties, args.Value, ref minIndex, ref result))
                        result.ParsedDefault++;
                    else
                        result.Unknown.Add(string.Format("%{0}", defaultArgsCounter++), args.Value.ToArray());
                }
                else result.Unknown.Add(key, args.Value.ToArray());
            }
            options.Clear();
        }
        /// <summary>
        /// Maps a processed list of attributes to certain object instance
        /// </summary>
        /// <param name="target">An object instance to do field and property lookup</param>
        /// <param name="options">A list of attributes and their values</param>
        /// <param name="result">A collection of result information</param>
        public static void MapToObject(object target, Dictionary<string, List<string>> options, bool ignoreCase, ref AutoConfigResult result)
        {
            Type type = target.GetType();
            int minIndex = 0;

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            result.Parsed = options.Count;
            SetMember(target, fields, options, ignoreCase, ref minIndex, ref result);
            SetMember(target, properties, options, ignoreCase, ref minIndex, ref result);
            result.Parsed -= options.Count;

            MapDefaultArgs(target, fields, properties, options, minIndex, ref result);
        }
        /// <summary>
        /// Maps a processed list of CommandLineOptions to certain object instance
        /// </summary>
        /// <param name="target">An object instance to do field and property lookup</param>
        /// <param name="arguments">A list of attributes and their values</param>
        /// <param name="result">A collection of result information</param>
        public static void MapToObject(object target, bool ignoreCase, ref AutoConfigResult result)
        {
            Dictionary<string, List<string>> options = new Dictionary<string, List<string>>(CommandLineOptions.Options.Count);
            foreach (KeyValuePair<string, List<string>> option in CommandLineOptions.Options)
                options.Add(option.Key, new List<string>(option.Value));

            MapToObject(target, options, ignoreCase, ref result);
        }
        /// <summary>
        /// Maps a processed list of attributes to any static fields and properties of Type T
        /// </summary>
        /// <param name="options">A list of attributes and their values</param>
        /// <param name="result">A collection of result information</param>
        public static void MapToObject<T>(Dictionary<string, List<string>> options, bool ignoreCase, ref AutoConfigResult result)
        {
            Type type = typeof(T);
            int minIndex = 0;

            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            result.Parsed = options.Count;
            SetMember(null, fields, options, ignoreCase, ref minIndex, ref result);
            SetMember(null, properties, options, ignoreCase, ref minIndex, ref result);
            result.Parsed -= options.Count;

            MapDefaultArgs(null, fields, properties, options, minIndex, ref result);
        }
        /// <summary>
        /// Maps a processed list of CommandLineOptions to any static fields and properties of Type T
        /// </summary>
        /// <param name="options">A list of attributes and their values</param>
        /// <param name="result">A collection of result information</param>
        public static void MapToObject<T>(bool ignoreCase, ref AutoConfigResult result)
        {
            Dictionary<string, List<string>> options = new Dictionary<string, List<string>>(CommandLineOptions.Options.Count);
            foreach (KeyValuePair<string, List<string>> option in CommandLineOptions.Options)
                options.Add(option.Key, new List<string>(option.Value));

            MapToObject<T>(options, ignoreCase, ref result);
        }

        private static UInt64 GetBitPosition(UInt64 input)
        {
            if (input == 0) return 64;

            UInt64 n = 1;

            if ((input >> 32) == 0) { n = n + 32; input = input << 32; }
            if ((input >> 48) == 0) { n = n + 16; input = input << 16; }
            if ((input >> 56) == 0) { n = n + 8; input = input << 8; }
            if ((input >> 60) == 0) { n = n + 4; input = input << 4; }
            if ((input >> 62) == 0) { n = n + 2; input = input << 2; }
            n = n - (input >> 63);

            return 64 - n;
        }
        private static void GetAttributePage(Type type, BindingFlags flags, PageFormatter formatter)
        {
            foreach (System.Reflection.PropertyInfo property in type.GetProperties<AutoConfigAttribute>(flags | BindingFlags.SetProperty))
            {
                AutoConfigAttribute[] attributes; if(property.TryGetAttributes<AutoConfigAttribute>(out attributes))
                foreach (AutoConfigAttribute attribute in attributes)
                    if (!string.IsNullOrEmpty(attribute.Text))
                    {
                        if (attribute.IsDefault) 
                            formatter.AddRow(string.Format("[Arg {0}]", GetBitPosition((UInt64)attribute.DefaultIndex)), attribute.Text);
                        if(!string.IsNullOrWhiteSpace(attribute.Id)) 
                            formatter.AddRow(string.Format("-{0}", attribute.Id), attribute.Text);
                    }
            }
            foreach (System.Reflection.FieldInfo field in type.GetFields<AutoConfigAttribute>(flags | BindingFlags.SetField))
            {
                AutoConfigAttribute[] attributes; if (field.TryGetAttributes<AutoConfigAttribute>(out attributes))
                    foreach (AutoConfigAttribute attribute in attributes)
                    if (!string.IsNullOrEmpty(attribute.Text))
                    {
                        if (attribute.IsDefault) 
                            formatter.AddRow(string.Format("[Arg {0}]", GetBitPosition((UInt64)attribute.DefaultIndex)), attribute.Text);
                        if (!string.IsNullOrWhiteSpace(attribute.Id)) 
                            formatter.AddRow(string.Format("-{0}", attribute.Id), attribute.Text);
                    }
            }
        }
        /// <summary>
        /// Creates a string from any field and property provided information text and
        /// access code found in certain object instance
        /// </summary>
        /// <param name="target">An object instance to do field and property lookup</param>
        /// <returns>A combined string of access code to text lines</returns>
        public static string GetAttributePage(object target)
        {
            PageFormatter formatter = new PageFormatter();
            GetAttributePage(target.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, formatter);

            formatter.Sort();
            return formatter.ToString();
        }
        /// <summary>
        /// Creates a string from any field and property provided information text and
        /// access code found in certain object instance
        /// </summary>
        /// <param name="target">An object instance to do field and property lookup</param>
        /// <returns>A combined string of access code to text lines</returns>
        public static void GetAttributePage(object target, PageFormatter formatter)
        {
            GetAttributePage(target.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, formatter);
        }
        /// <summary>
        /// Creates a string from any field and property provided information text and
        /// access code found in any static Type T
        /// </summary>
        /// <returns>A combined string of access code to text lines</returns>
        public static string GetAttributePage(Type type)
        {
            PageFormatter formatter = new PageFormatter();
            GetAttributePage(type, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, formatter);

            formatter.Sort();
            return formatter.ToString();
        }
        /// <summary>
        /// Creates a string from any field and property provided information text and
        /// access code found in any static Type T
        /// </summary>
        /// <returns>A combined string of access code to text lines</returns>
        public static void GetAttributePage(Type type, PageFormatter formatter)
        {
            GetAttributePage(type, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, formatter);
        }
        /// <summary>
        /// Creates a string from any field and property provided information text and
        /// access code found in any static Type T
        /// </summary>
        /// <returns>A combined string of access code to text lines</returns>
        public static string GetAttributePage<T>()
        {
            PageFormatter formatter = new PageFormatter();
            GetAttributePage(typeof(T), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, formatter);

            formatter.Sort();
            return formatter.ToString();
        }
        /// <summary>
        /// Creates a string from any field and property provided information text and
        /// access code found in any static Type T
        /// </summary>
        /// <returns>A combined string of access code to text lines</returns>
        public static void GetAttributePage<T>(PageFormatter formatter)
        {
            GetAttributePage(typeof(T), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, formatter);
        }

        private static bool TrySetList(object target, AutoConfigAttribute attrib, MemberInfo member, Type memberType, List<string> values)
        {
            if (!typeof(System.Collections.IList).IsAssignableFrom(memberType))
                return false;

            memberType = memberType.GetGenericArguments()[0];

            System.Collections.IList list;
            if (member.MemberType == MemberTypes.Property)
                list = ((member as PropertyInfo).GetValue(target, null) as System.Collections.IList);
            else
                list = ((member as FieldInfo).GetValue(target) as System.Collections.IList);

            bool defaultSet = false;
            for (int i = 0; i < values.Count; i++)
            {
                if (!defaultSet && attrib.DefaultValue != null && memberType == typeof(string) && values[i] == "true")
                {
                    values[i] = attrib.DefaultValue.ToString();
                    defaultSet = true;
                }
                foreach (string value in values[i].Split(';'))
                {
                    object obj = Parse(attrib, memberType, value.Trim());
                    if (!defaultSet && attrib.DefaultValue != null && memberType != typeof(string) && obj == null)
                    {
                        obj = attrib.DefaultValue.ToString();
                        defaultSet = true;
                    }
                    if (obj != null)
                        list.Add(obj);
                }
                values.RemoveAt(i);
                i--;
            }
            return true;
        }
        private static object Concat(Type type, object value, int index)
        {
            Type enumBase = Enum.GetUnderlyingType(type);
            int val = Convert.ToInt32(value) | index;
            return Convert.ChangeType(val, enumBase);
        }
        private static bool TrySetEnum(object target, AutoConfigAttribute attrib, MemberInfo member, Type memberType, List<string> values)
        {
            if (!memberType.IsEnum)
                return false;

            bool defaultSet = false;
            for (int i = 0; i < values.Count; i++)
                try
                {
                    object v;
                    bool explicite = false;
                    if (attrib.IsFlag && values[i] == "true") v = attrib.FlagIndex;
                    else
                    {
                        if (!defaultSet && attrib.DefaultValue != null && (values[i] == null || values[i].Equals("true")))
                        {
                            v = attrib.DefaultValue;
                            defaultSet = true;
                        }
                        else if (attrib.IsMaskedValue && values[i] == "true")
                        {
                            v = Enum.ToObject(memberType, attrib.MaskIndex);
                        }
                        else v = Parse(attrib, memberType, values[i]);
                        explicite = true;
                    }
                    if (v == null)
                        continue;
                    else if (member.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo nfo = (PropertyInfo)member;
                        if (explicite) nfo.SetValue(target, v, null);
                        else nfo.SetValue(target, Concat(nfo.PropertyType, nfo.GetValue(target, null), (int)v), null);
                    }
                    else
                    {
                        FieldInfo nfo = (FieldInfo)member;
                        if (explicite) nfo.SetValue(target, v);
                        else nfo.SetValue(target, Concat(nfo.FieldType, nfo.GetValue(target), (int)v));
                    }
                    values.RemoveAt(i);
                    i--;
                }
                catch
                { return false; }

            return true;
        }
        private static bool TrySetValue(object target, AutoConfigAttribute attrib, MemberInfo member, Type memberType, List<string> values)
        {
            bool defaultSet = false;
            for (int i = 0; i < values.Count; i++)
            try
            {
                object parsed; if (attrib.DeclaredOnly && memberType == typeof(bool)) parsed = true;
                else parsed = Parse(attrib, memberType, values[i]);

                if (!defaultSet && attrib.DefaultValue != null && (parsed == null || parsed.Equals("true")))
                {
                    parsed = attrib.DefaultValue;
                    defaultSet = true;
                }
                if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo nfo = (PropertyInfo)member;
                    nfo.SetValue(target, parsed, null);
                }
                else
                {
                    FieldInfo nfo = (FieldInfo)member;
                    nfo.SetValue(target, parsed);
                }
                values.RemoveAt(i);
                return true;
            }
            catch { }
            return false;
        }

        private static void SetMember(object target, MemberInfo[] members, Dictionary<string, List<string>> arguments, bool ignoreCase, ref int minIndex, ref AutoConfigResult result)
        {
            foreach (MemberInfo member in members)
            {
                Type memberType;
                if (member.MemberType == MemberTypes.Property)
                    memberType = (member as PropertyInfo).PropertyType;
                else
                    memberType = (member as FieldInfo).FieldType;

                AutoConfigAttribute[] attribs; if (member.TryGetAttributes<AutoConfigAttribute>(out attribs))
                    foreach (AutoConfigAttribute attrib in attribs)
                    {
                        string key;
                        if (ignoreCase) key = attrib.Id.ToLowerInvariant();
                        else key = attrib.Id;

                        List<string> values; if (arguments.TryGetValue(key, out values))
                        {
                            if (TrySetList(target, attrib, member, memberType, values) || TrySetEnum(target, attrib, member, memberType, values) || TrySetValue(target, attrib, member, memberType, values))
                            {
                                if(attrib.IsDefault && attrib.DefaultIndex > minIndex)
                                    minIndex = attrib.DefaultIndex;

                                result.Parsed++;
                            }
                        }
                    }
            }
        }
        private static bool SetDefault(object target, MemberInfo[] members, List<string> values, ref int minIndex, ref AutoConfigResult result)
        {
            foreach (MemberInfo member in members)
            {
                Type memberType;
                if (member.MemberType == MemberTypes.Property)
                    memberType = (member as PropertyInfo).PropertyType;
                else
                    memberType = (member as FieldInfo).FieldType;

                AutoConfigAttribute[] attribs; if (member.TryGetAttributes<AutoConfigAttribute>(out attribs))
                {
                    foreach (AutoConfigAttribute attrib in attribs)
                        if (attrib.IsDefault && attrib.DefaultIndex > minIndex)
                        {
                            if (TrySetList(target, attrib, member, memberType, values) || TrySetEnum(target, attrib, member, memberType, values) || TrySetValue(target, attrib, member, memberType, values))
                            {
                                minIndex = attrib.DefaultIndex;
                                return true;
                            }
                        }
                }
            }
            return false;
        }

        private static object Parse(AutoConfigAttribute attrib, Type type, string value)
        {
            try
            {
                object tmp = null; 
                ITypeConverter converter; if (attrib.TryGetConverter(out converter) && converter.TryParseValue(type, value, out tmp))
                    return tmp;
            }
            catch { }
            return ArgumentConverter.Convert(type, value);
        }
    }
}
