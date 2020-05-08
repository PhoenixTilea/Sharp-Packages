
// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using SE;
using SE.Json;

namespace SE.Config
{
    /// <summary>
    /// An auto configure provider processing JSON files
    /// </summary>
    public static class JsonConfig
    {
        /// <summary>
        /// Tries to find and apply a JSON file in this assemblies base directory to
        /// certain object instance
        /// </summary>
        /// <param name="target">An object instance to do field and property lookup</param>
        /// <returns>A collection of result information</returns>
        public static AutoConfigResult LoadManifest(object target, bool ignoreCase = false)
        {
            string startupPath;
            Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                assembly = System.Reflection.Assembly.GetCallingAssembly();
                startupPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            }
            else startupPath = System.IO.Path.GetDirectoryName(assembly.Location);
            string manifest = Path.Combine(startupPath, assembly.GetName().Name + ".json");

            if (File.Exists(manifest))
                using (FileStream fs = new FileStream(manifest, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    JsonDocument doc = new JsonDocument();
                    if (doc.Load(fs)) return JsonConfig.Parse(target, doc, ignoreCase);
                }

            return new AutoConfigResult();
        }
        /// <summary>
        /// Tries to find and apply a JSON file in this assemblies base directory to
        /// certain object instance
        /// </summary>
        /// <param name="target">An object instance to do field and property lookup</param>
        /// <param name="assembly">The assembly to seek for the JSON file at</param>
        /// <returns>A collection of result information</returns>
        public static AutoConfigResult LoadManifest(object target, Assembly assembly, bool ignoreCase = false)
        {
            string startupPath = System.IO.Path.GetDirectoryName(assembly.Location);
            string manifest = Path.Combine(startupPath, assembly.GetName().Name + ".json");

            if (File.Exists(manifest))
                using (FileStream fs = new FileStream(manifest, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    JsonDocument doc = new JsonDocument();
                    if (doc.Load(fs)) return JsonConfig.Parse(target, doc, ignoreCase);
                }

            return new AutoConfigResult();
        }
        /// <summary>
        /// Tries to find and apply a JSON file in this assemblies base directory to
        /// any static fields and properties of Type T
        /// </summary>
        /// <returns>A collection of result information</returns>
        public static AutoConfigResult LoadManifest<T>(bool ignoreCase = false)
        {
            string startupPath;
            Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                assembly = System.Reflection.Assembly.GetCallingAssembly();
                startupPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            }
            else startupPath = System.IO.Path.GetDirectoryName(assembly.Location);
            string manifest = Path.Combine(startupPath, assembly.GetName().Name + ".json");

            if (File.Exists(manifest))
                using (FileStream fs = new FileStream(manifest, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    JsonDocument doc = new JsonDocument();
                    if (doc.Load(fs)) return JsonConfig.Parse<T>(doc, ignoreCase);
                }

            return new AutoConfigResult();
        }
        /// <summary>
        /// Tries to find and apply a JSON file in this assemblies base directory to
        /// any static fields and properties of Type T
        /// </summary>
        /// <param name="assembly">The assembly to seek for the JSON file at</param>
        /// <returns>A collection of result information</returns>
        public static AutoConfigResult LoadManifest<T>(Assembly assembly, bool ignoreCase = false)
        {
            string startupPath = System.IO.Path.GetDirectoryName(assembly.Location);
            string manifest = Path.Combine(startupPath, assembly.GetName().Name + ".json");

            if (File.Exists(manifest))
                using (FileStream fs = new FileStream(manifest, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    JsonDocument doc = new JsonDocument();
                    if (doc.Load(fs)) return JsonConfig.Parse<T>(doc, ignoreCase);
                }

            return new AutoConfigResult();
        }

        /// <summary>
        /// Applies an already processed JSON document to certain object instance
        /// </summary>
        /// <param name="target">An object instance to do field and property lookup</param>
        /// <param name="file">The processed JSON document to apply</param>
        /// <returns>A collection of result information</returns>
        public static AutoConfigResult Parse(object target, JsonDocument file, bool ignoreCase = false)
        {
            AutoConfigResult result = AutoConfigResult.Create();
            try
            {
                Dictionary<string, List<string>> preparsed = GetOptions(file, ignoreCase);
                AutoConfig.MapToObject(target, preparsed, ignoreCase, ref result);
            }
            catch (Exception er)
            {
                result.Errors.Add(er.Message);
            }
            return result;
        }
        /// <summary>
        /// Applies an already processed JSON document to any static fields and properties of Type T
        /// </summary>
        /// <param name="file">The processed JSON document to apply</param>
        /// <returns>A collection of result information</returns>
        public static AutoConfigResult Parse<T>(JsonDocument file, bool ignoreCase = false)
        {
            AutoConfigResult result = AutoConfigResult.Create();
            try
            {
                Dictionary<string, List<string>> preparsed = GetOptions(file, ignoreCase);
                AutoConfig.MapToObject<T>(preparsed, ignoreCase, ref result);
            }
            catch (Exception er)
            {
                result.Errors.Add(er.Message);
            }
            return result;
        }

        /// <summary>
        /// Parses an already processed JSON document to obtain the included attributes
        /// </summary>
        /// <param name="file">The processed JSON document to parse</param>
        /// <returns>A list of attributes and their coresponding values</returns>
        public static void GetOptions(JsonDocument file, bool ignoreCase, Dictionary<string, List<string>> result)
        {
            Json.JsonNode node = file.Root.Child;
            while (node != null)
            {
                if (!string.IsNullOrWhiteSpace(node.Name))
                {
                    string tmp = node.Name;
                    if (ignoreCase && tmp.Length > 1)
                        tmp = tmp.ToLowerInvariant();

                    List<string> values; if (!result.TryGetValue(tmp, out values))
                    {
                        values = new List<string>();
                        result.Add(tmp, values);
                    }

                    switch (node.Type)
                    {
                        case JsonNodeType.Bool:
                        case JsonNodeType.Decimal:
                        case JsonNodeType.Integer:
                        case JsonNodeType.String: values.Add(node.RawValue.ToString()); break;
                        case JsonNodeType.Array:
                            {
                                Json.JsonNode entries = node.Child;
                                while (entries != null)
                                {
                                    switch (entries.Type)
                                    {
                                        case JsonNodeType.Bool:
                                        case JsonNodeType.Decimal:
                                        case JsonNodeType.Integer:
                                        case JsonNodeType.String: values.Add(entries.RawValue.ToString()); break;
                                    }
                                    entries = entries.Next;
                                }
                            }
                            break;
                    }
                }
                node = node.Next;
            }
        }
        /// <summary>
        /// Parses an already processed JSON document to obtain the included attributes
        /// </summary>
        /// <param name="file">The processed JSON document to parse</param>
        /// <returns>A list of attributes and their corresponding values</returns>
        public static Dictionary<string, List<string>> GetOptions(JsonDocument file, bool ignoreCase = false)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            GetOptions(file, ignoreCase, result);

            return result;
        }
    }
}
