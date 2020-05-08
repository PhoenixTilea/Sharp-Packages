// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.IO;
using SE;

namespace SE.Config
{
    /// <summary>
    /// Command Line Argument Converter
    /// </summary>
    public static class ArgumentConverter
    {
        /// <summary>
        /// Processes a single string into a list of string pairs
        /// </summary>
        /// <param name="args">The string to be processed</param>
        /// <returns>A list of string pair arguments</returns>
        public static string[] MakeArgsList(string args)
        {
            if (string.IsNullOrWhiteSpace(args)) return new string[0];
            List<string> result = new List<string>();

            int first = 0;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case '\'':
                    case '"':
                        {
                            char coresponding = args[i];
                            bool ignore = false;
                            do
                            {
                                i++;

                                if (args[i] == '/') ignore = true;
                                else if (ignore) ignore = false;
                            }
                            while (i < args.Length && (args[i] != coresponding || ignore));
                        }
                        break;
                    case ' ':
                        {
                            result.Add(args.Substring(first, i - first).Replace("\"", "").Replace("'", ""));
                            first = (i + 1);
                        }
                        break;
                }
            }
            if (first < args.Length)
                result.Add(args.Substring(first, args.Length - first).Replace("\"", "").Replace("'", ""));

            return result.ToArray();
        }

        /// <summary>
        /// Converts a set of command line parameter options into a string
        /// </summary>
        /// <param name="options">The list of options to covnert</param>
        /// <returns>An argument string to be passed to the command line</returns>
        public static string MakeArgsString(IEnumerable<KeyValuePair<string, List<string>>> options)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, List<string>> option in options)
            {
                if (option.Value[0] == null)
                {
                    sb.Append('"');
                    sb.Append(option.Key);
                    sb.Append("\" ");
                }
                else
                {
                    sb.Append('-');
                    sb.Append(option.Key);
                    sb.Append(' ');
                    foreach (string value in option.Value)
                    {
                        sb.Append('"');
                        sb.Append(value);
                        sb.Append("\" ");
                    }
                }
            }
            return sb.ToString().Trim();
        }
        /// <summary>
        /// Converts a set of command line parameters into a string
        /// </summary>
        /// <param name="parameters">The list of command line parameters to covnert</param>
        /// <returns>An argument string to be passed to the command line</returns>
        public static string MakeArgsString(IEnumerable<KeyValuePair<string, object[]>> parameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, object[]> parameter in parameters)
            {
                if (parameter.Value[0] == null)
                {
                    sb.Append('"');
                    sb.Append(parameter.Key);
                    sb.Append("\" ");
                }
                else
                {
                    if (!parameter.Key.StartsWith("%"))
                    {
                        sb.Append('-');
                        sb.Append(parameter.Key);
                        sb.Append(' ');
                    }
                    foreach (object value in parameter.Value)
                        if (value != null && !value.Equals("true"))
                        {
                            sb.Append('"');
                            sb.Append(value);
                            sb.Append("\" ");
                        }
                }
            }
            return sb.ToString().Trim();
        }
        /// <summary>
        /// Converts a set of command line parameters into a string
        /// </summary>
        /// <param name="args">The list of command line parameters to covnert</param>
        /// <returns>An argument string to be passed to the command line</returns>
        public static string MakeArgsString(IEnumerable<string> args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string arg in args)
            {
                sb.Append('"');
                sb.Append(arg);
                sb.Append("\" ");
            }
            return sb.ToString().Trim();
        }

        private static void AddOrCreateOption(string option, string value, bool ignoreCase, Dictionary<string, List<string>> result)
        {
            if (ignoreCase && option.Length > 1)
                option = option.ToLowerInvariant();

            List<string> argsList; if (result.TryGetValue(option, out argsList))
                argsList.Add(value);
            else
            {
                argsList = new List<string>();
                argsList.Add(value);
                result.Add(option, argsList);
            }
        }
        private static bool CreateOption(string option, string value, bool ignoreCase, Dictionary<string, List<string>> result)
        {
            if (ignoreCase && option.Length > 1)
                option = option.ToLowerInvariant();

            List<string> argsList; if (!result.TryGetValue(option, out argsList))
            {
                argsList = new List<string>();
                argsList.Add(value);
                result.Add(option, argsList);

                return true;
            }
            else return false;
        }

        private static void AddOptions(IEnumerable<string> args, bool ignoreCase, ref string possibleOption, Dictionary<string, List<string>> result)
        {
            IEnumerator<string> arguments = args.GetEnumerator();
            while (arguments.MoveNext())
            {
                string arg = arguments.Current;
                if (!string.IsNullOrEmpty(arg))
                {
                    if (arg.StartsWith("@"))
                    {
                        if (!string.IsNullOrEmpty(possibleOption))
                            AddOrCreateOption(possibleOption, "true", ignoreCase, result);

                        arg = arg.TrimStart('@');
                        possibleOption = "@";
                    }
                    if (arg.StartsWith("-"))
                    {
                        if (string.IsNullOrEmpty(possibleOption)) possibleOption = arg.TrimStart(1, '-');
                        else
                        {
                            CreateOption(possibleOption, "true", ignoreCase, result);
                            possibleOption = arg.TrimStart(1, '-');
                        }

                        if (possibleOption == "@")
                            possibleOption = string.Empty;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(possibleOption))
                        {
                            AddOrCreateOption(possibleOption, arg, ignoreCase, result);
                            possibleOption = string.Empty;
                        }
                        else
                        {
                            if (ignoreCase && arg.Length > 1)
                                arg = arg.ToLowerInvariant();

                            if(!CreateOption(arg, null, ignoreCase, result))
                                result[arg].Add(null);
                        }
                    }
                }
            }
        }
        private static void FinishOptionList(bool ignoreCase, string possibleOption, Dictionary<string, List<string>> result)
        {
            if (!string.IsNullOrEmpty(possibleOption))
            {
                List<string> argsList; if (!result.TryGetValue(possibleOption, out argsList))
                {
                    argsList = new List<string>();
                    argsList.Add("true");

                    if (ignoreCase && possibleOption.Length > 1)
                        possibleOption = possibleOption.ToLowerInvariant();

                    result.Add(possibleOption, argsList);
                }
            }
        }

        /// <summary>
        /// Parses the input enumerator into a collection of options
        /// </summary>
        /// <param name="args">The input enumerator to parse</param>
        /// <param name="ignoreCase">True if string case should be ignored, false otherwise</param>
        /// <returns>The preparsed list of options</returns>
        public static Dictionary<string, List<string>> GetOptions(IEnumerable<string> args, bool ignoreCase)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
            string possibleOption = string.Empty;

            AddOptions(args, ignoreCase, ref possibleOption, result);
            FinishOptionList(ignoreCase, possibleOption, result);
            
            return result;
        }
        /// <summary>
        /// Parses the input enumerator into a collection of options
        /// </summary>
        /// <param name="args">The input enumerator to parse</param>
        /// <param name="ignoreCase">True if string case should be ignored, false otherwise</param>
        /// <returns>The preparsed list of options</returns>
        public static void GetOptions(IEnumerable<string> args, bool ignoreCase, Dictionary<string, List<string>> result)
        {
            string possibleOption = string.Empty;

            AddOptions(args, ignoreCase, ref possibleOption, result);
            FinishOptionList(ignoreCase, possibleOption, result);
        }
        /// <summary>
        /// Tries to parse a response file into a collection of options
        /// </summary>
        /// <param name="responseFile">A file system path to a response file</param>
        /// <param name="ignoreCase">True if string case should be ignored, false otherwise</param>
        /// <returns>The preparsed list of options</returns>
        public static bool GetOptions(string responseFile, bool ignoreCase, Dictionary<string, List<string>> result)
        {
            if (File.Exists(responseFile))
                try
                {
                    string possibleOption = string.Empty;

                    using (FileStream fs = new FileStream(responseFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (StreamReader sr = new StreamReader(fs, true))
                        while (sr.Peek() != -1)
                        {
                            string line = sr.ReadLine().Trim();
                            if (line.Length == 0 || line.StartsWith("#"))
                                continue;

                            AddOptions(line.Split(null), ignoreCase, ref possibleOption, result);
                        }

                    FinishOptionList(ignoreCase, possibleOption, result);
                    return true;
                }
                catch (FieldAccessException)
                { }
                catch (FileNotFoundException)
                { }
            return false;
        }

        /// <summary>
        /// Translates an enumeration into command line options
        /// </summary>
        /// <param name="args">The input enumerator to parse</param>
        /// <param name="ignoreCase">True if string case should be ignored, false otherwise</param>
        /// <returns>A collection of command line options</returns>
        public static void Translate(IEnumerable<string> args, bool ignoreCase, Dictionary<string, List<string>> result)
        {
            GetOptions(args, ignoreCase, result);
            List<string> argsList; if (result.TryGetValue("@", out argsList))
            {
                for (int i = 0; i < argsList.Count; i++)
                {
                    if (GetOptions(argsList[i], ignoreCase, result))
                    {
                        argsList.RemoveAt(i);
                        i--;
                    }
                }
                if (argsList.Count == 0)
                    result.Remove("@");
            }
        }
        /// <summary>
        /// Translates an enumeration into command line options
        /// </summary>
        /// <param name="args">The input enumerator to parse</param>
        /// <param name="ignoreCase">True if string case should be ignored, false otherwise</param>
        /// <returns>A collection of command line options</returns>
        public static Dictionary<string, List<string>> Translate(IEnumerable<string> args, bool ignoreCase)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
            Translate(args, ignoreCase, result);

            return result;
        }

        /// <summary>
        /// Converts the given value string into a value or instance of type
        /// </summary>
        /// <param name="type">The type to process</param>
        /// <param name="value">A value string to convert</param>
        /// <param name="result">The default value</param>
        /// <returns>True if conversion was successfull, false otherwise</returns>
        public static bool TryConvert(Type type, string value, out object result)
        {
            try
            {
                if (type.IsEnum)
                {
                    object tmp; if ((tmp = Enum.Parse(type, value, true)) != null) result = tmp;
                    else
                    {
                        int integer; if (int.TryParse(value, out integer)) result = Enum.ToObject(type, integer);
                        else
                        {
                            result = null;
                            return false;
                        }
                    }
                }
                else if (type != typeof(string))
                {
                    MethodInfo parseCultureVariant = type.GetMethod("Parse", new[] { typeof(string), typeof(CultureInfo) });
                    MethodInfo parse = type.GetMethod("Parse", new[] { typeof(string) });
                    if (parseCultureVariant == null && parse == null)
                    {
                        if (type == typeof(object))
                        {
                            result = value;
                            return true;
                        }
                        else
                        {
                            result = GetDefaultValue(type);
                            return false;
                        }
                    }
                    else result = ((parseCultureVariant != null) ? parseCultureVariant.Invoke(null, new object[] { value, CultureInfo.InvariantCulture }) : parse.Invoke(null, new object[] { value }));
                }
                else result = value;
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
        /// <summary>
        /// Converts the given value string into a value or instance of type
        /// </summary>
        /// <param name="value">A value string to convert</param>
        /// <param name="result">The default value</param>
        /// <returns>True if conversion was successfull, false otherwise</returns>
        public static bool TryConvert<T>(string value, out object result)
        {
            return TryConvert(typeof(T), value, out result);
        }
        /// <summary>
        /// Converts the given value string into a value or instance of type
        /// </summary>
        /// <param name="type">The type to process</param>
        /// <param name="value">A value string to convert</param>
        /// <returns>The default value</returns>
        public static object Convert(Type type, string value)
        {
            object result; if(!TryConvert(type, value, out result))
                result = GetDefaultValue(type);

            return result;
        }
        /// <summary>
        /// Converts the given value string into a value or instance of type
        /// </summary>
        /// <param name="value">A value string to convert</param>
        /// <returns>The default value</returns>
        public static object Convert<T>(string value)
        {
            return Convert(typeof(T), value);
        }

        /// <summary>
        /// Determines a types default value or null
        /// </summary>
        /// <param name="type">The type to process</param>
        /// <returns>The default value</returns>
        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type);
            return null;
        }
        /// <summary>
        /// Determines a types default value or null
        /// </summary>
        /// <returns>The default value</returns>
        public static object GetDefaultValue<T>()
        {
            return GetDefaultValue(typeof(T));
        }
    }
}
