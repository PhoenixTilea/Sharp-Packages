// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using SE;
using SE.Config;
using SE.Json;

namespace SE.App
{
    /// <summary>
    /// Provides static methods and properties to handle tool applications with
    /// auto configuring from JSON manifest and program arguments
    /// </summary>
    public partial class Application
    {
        enum ApplicationFlags : byte
        {
            None = 0,

            NoCache = 0x1,
            DisplayManual = 0x2
        }

        public const string CacheDirectoryName = ".cache";
        public const string ConfigDirectoryName = "Config";

        private static int[] errorCodes = null;

        private static string name;
        /// <summary>
        /// Returns this Application's file system name
        /// </summary>
        public static string Name
        {
            get { return name; }
        }

        private static string rootFile;
        /// <summary>
        /// Returns the full qualified file system location this assembly is
        /// based on
        /// </summary>
        public static string Self
        {
            get { return rootFile; }
        }

        private static string rootPath;
        /// <summary>
        /// The absolute path to the location this Application is 
        /// grouped by
        /// </summary>
        public static string RootPath
        {
            get { return rootPath; }
        }

        private static string workerPath;
        /// <summary>
        /// The absolute path to the location this Application should
        /// work at
        /// </summary>
        public static string WorkerPath
        {
            get { return workerPath; }
        }

        private static string manifestFile;

        [AutoConfig("?", Text = "Displays this manual pages", FlagIndex = 1)]
        [AutoConfig("no-cache", Text = "Tells the application to purge its local cache", FlagIndex = 0)]
        private static ApplicationFlags flags = ApplicationFlags.None;
        
        /// <summary>
        /// Determines if the application should purge its local cache
        /// </summary>
        public static bool NoCache
        {
            get { return ((flags & ApplicationFlags.NoCache) == ApplicationFlags.NoCache); }
        }

        /// <summary>
        /// Determines if the application manual should be displayed
        /// </summary>
        public static bool DisplayManual
        {
            get { return ((flags & ApplicationFlags.DisplayManual) == ApplicationFlags.DisplayManual); }
        }

        [AutoConfig(SeverityFlags.Minimal, "log", Text = "Sets log level output to any of [silent|minimal|full]")]
        private static SeverityFlags logSeverity = SeverityFlags.Minimal;
        /// <summary>
        /// A user defined log level
        /// </summary>
        public static SeverityFlags LogSeverity
        {
            get { return logSeverity; }
        }

        /// <summary>
        /// Obtains the compiler related file version
        /// </summary>
        public static Version Version
        {
            get { return AssemblyName.GetAssemblyName(Assembly.GetEntryAssembly().Location).Version; }
        }

        /// <summary>
        /// Gets last modification timestamp of this Application
        /// </summary>
        public static DateTime Timestamp
        {
            get { return new FileInfo(rootFile).LastWriteTimeUtc; }
        }

        /// <summary>
        /// An event called on application unload
        /// </summary>
        public static event EventHandler Shutdown;

        static Application()
        {
            rootFile = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            name = Path.GetFileNameWithoutExtension(rootFile);

            rootPath = new Uri(Path.GetDirectoryName(rootFile)).LocalPath;
            workerPath = Path.GetFullPath(Environment.CurrentDirectory);

            OnLoad();
        }
        static partial void OnLoad();

        /// <summary>
        /// Initializes this Application by it's manifest file on disk
        /// </summary>
        public static AutoConfigResult LoadManifest(IEnumerable<string> args)
        {
            OnConfigRequested();

            if (string.IsNullOrWhiteSpace(manifestFile) || !File.Exists(manifestFile))
                manifestFile = Path.Combine(rootPath, name + ".json");
            if (File.Exists(manifestFile))
                using (FileStream fs = new FileStream(manifestFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    JsonDocument doc = new JsonDocument();
                    if (doc.Load(fs))
                        JsonConfig.GetOptions(doc, true, CommandLineOptions.Options);
                }

            foreach (KeyValuePair<string, List<string>> @override in ArgumentConverter.Translate(args, true))
            {
                if (CommandLineOptions.Options.ContainsKey(@override.Key)) CommandLineOptions.Options[@override.Key] = @override.Value;
                else CommandLineOptions.Options.Add(@override.Key, @override.Value);
            }

            AutoConfigResult result = AutoConfigResult.Create();
            AutoConfig.MapToObject<Application>(true, ref result);

            OnInitialize();
            return result;
        }
        static partial void OnConfigRequested();
        /// <summary>
        /// Initializes this Application by a given set of arguments passed 
        /// at start
        /// </summary>
        /// <param name="args">A set of arguments to apply</param>
        /// <returns>The result of the configuration process</returns>
        public static AutoConfigResult Initialize(IEnumerable<string> args)
        {
            JsonConfig.LoadManifest<Application>(true);
            CommandLineOptions.Load(args, true);

            AutoConfigResult result = AutoConfigResult.Create();
            AutoConfig.MapToObject<Application>(true, ref result);

            OnInitialize();
            return result;
        }
        static partial void OnInitialize();

        /// <summary>
        /// Combines a set of values into certain return code
        /// </summary>
        /// <param name="category">The category of the return code</param>
        /// <param name="subCategory">The sub category of the return code</param>
        /// <param name="code">The ID value describing the result</param>
        /// <returns>The final return code</returns>
        public static int GetResultFromValues(int category, int subCategory, byte code)
        {
            return (((byte)category << 16) | ((byte)subCategory << 8) | code);
        }
        /// <summary>
        /// Tries to map a given error or return code into a known final return code
        /// </summary>
        /// <param name="code">The code to map</param>
        /// <returns>The final return code</returns>
        public static int GetResultFromKnownError(int code)
        {
            if (code < 0 || code > errorCodes.Length)
                throw new InvalidCastException();

            return errorCodes[(int)code];
        }

        /// <summary>
        /// Obtains a final success return code
        /// </summary>
        public static int SuccessReturnCode
        {
            get { return 0; }
        }
        /// <summary>
        /// Obtains a final general failure return code
        /// </summary>
        public static int FailureReturnCode
        {
            get { return 1; }
        }

        private static string LinearFindDirectory(string path, string directory)
        {
            string current = Path.Combine(path, directory);
            if (Directory.Exists(current))
                return current;

            path = Path.GetDirectoryName(path);
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            return LinearFindDirectory(path, directory);
        }

        /// <summary>
        /// Detects the logical top level location grouping this Application and a subset
        /// of necessary directories
        /// </summary>
        /// <param name="basePath">The basic location to start searching</param>
        /// <returns>The logical absolute top level location</returns>
        public static string GetTopLevelPath(string basePath = null)
        {
            if (string.IsNullOrWhiteSpace(basePath))
                basePath = workerPath;

            string tmp = null;
            if (!string.IsNullOrWhiteSpace(tmp = LinearFindDirectory(basePath, CacheDirectoryName)))
            {
                return Path.GetDirectoryName(tmp);
            }
            else if (!string.IsNullOrWhiteSpace(tmp = LinearFindDirectory(basePath, ConfigDirectoryName)))
            {
                return Path.GetDirectoryName(tmp);
            }
            else if (!string.IsNullOrWhiteSpace(tmp = LinearFindDirectory(rootPath, CacheDirectoryName)))
            {
                return Path.GetDirectoryName(tmp);
            }
            else if (!string.IsNullOrWhiteSpace(tmp = LinearFindDirectory(rootPath, ConfigDirectoryName)))
            {
                return Path.GetDirectoryName(tmp);
            }
            else return rootPath;
        }

        /// <summary>
        /// Calls the Shutdown event to deinitialize additionally before shutdown
        /// </summary>
        public static void Unload()
        {
            if (Shutdown != null)
                Shutdown.Invoke(null, EventArgs.Empty);
        }
    }
}
