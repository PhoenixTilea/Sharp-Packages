// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Config
{
    /// <summary>
    /// Manages a set of command line arguments
    /// </summary>
    public static class CommandLineOptions
    {
        private static Dictionary<string, List<string>> options;
        /// <summary>
        /// Get the raw list of options
        /// </summary>
        public static Dictionary<string, List<string>> Options
        {
            get { return options; }
        }

        static CommandLineOptions()
        {
            options = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Adds an enumeration into the command line options collection
        /// </summary>
        /// <param name="args">The input enumerator to parse</param>
        public static void Load(IEnumerable<string> args, bool ignoreCase)
        {
            ArgumentConverter.Translate(args, ignoreCase, options);
        }
        /// <summary>
        /// Adds an enumeration into the command line options collection and
        /// replaces previous options
        /// </summary>
        /// <param name="args">The input enumerator to parse</param>
        public static void Replace(IEnumerable<string> args, bool ignoreCase)
        {
            options.Clear();
            ArgumentConverter.Translate(args, ignoreCase, options);
        }

        /// <summary>
        /// Determines if the given option exists in the collection
        /// </summary>
        /// <param name="key">The option parameter to look for</param>
        /// <returns>True if the options is present, false otherwise</returns>
        public static bool ContainsKey(string key)
        {
            return options.ContainsKey(key);
        }
        /// <summary>
        /// Gets an option value from the collection if possible
        /// </summary>
        /// <param name="key">The option parameter to look for</param>
        /// <param name="value">The value of the option if present</param>
        /// <returns>True if the options is present, false otherwise</returns>
        public static bool TryGetValue(string key, out string value)
        {
            List<string> argsList; if (options.TryGetValue(key, out argsList) && argsList.Count > 0)
            {
                value = argsList[0];
                return true;
            }

            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Adds an option into the collection
        /// </summary>
        /// <param name="key">The option parameter to add</param>
        /// <param name="value">The value of the option</param>
        public static void Add(string key, string value)
        {
            List<string> argsList = new List<string>(1);
            argsList.Add(value);

            options.Add(key, argsList);
        }

        /// <summary>
        /// Returns an options value
        /// </summary>
        /// <param name="key">The option parameter to look for</param>
        /// <returns>The value of the given option parameter</returns>
        public static string GetValue(string key)
        {
            List<string> argsList = options[key];
            if (argsList.Count == 0) return string.Empty;
            else return argsList[0];
        }
        /// <summary>
        /// Converts an options value to T. A default value is returned if
        /// conversion failed
        /// </summary>
        /// <param name="key">The option parameter to look for</param>
        /// <returns>An instance T</returns>
        public static T GetValueAs<T>(string key)
        {
            return (T)Convert.ChangeType(ArgumentConverter.Convert<T>(GetValue(key)), typeof(T));
        }

        /// <summary>
        /// Changes an options value
        /// </summary>
        /// <param name="key">The option parameter to look for</param>
        /// <param name="value">The new value to be stored in the option</param>
        public static void SetValue(string key, string value)
        {
            List<string> argsList = options[key];
            argsList.Clear();
            argsList.Add(value);
        }

        /// <summary>
        /// Removes an option if existing
        /// </summary>
        /// <param name="key">The option parameter to look for</param>
        /// <returns>True if the option was successfully removed, false otherwise</returns>
        public static bool Remove(string key)
        {
            return options.Remove(key);
        }

        public new static string ToString()
        {
            return ArgumentConverter.MakeArgsString(options);
        }
    }
}
