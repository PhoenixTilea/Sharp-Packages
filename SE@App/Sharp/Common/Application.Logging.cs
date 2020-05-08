// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.App
{
    public partial class Application
    {
        /// <summary>
        /// Writes a message to local declared log output. The default level
        /// is Full locking
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Log(string message, params object[] args)
        {
            Log(SeverityFlags.Full, message, args);
        }
        /// <summary>
        /// Writes a message to local declared log output and level
        /// </summary>
        /// <param name="severity">The log level to use</param>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Log(SeverityFlags severity, string message, params object[] args)
        {
            if (severity > logSeverity)
                return;

            LogInternal(message, args);
        }
        static partial void LogInternal(string message, params object[] args);

        /// <summary>
        /// Writes a message to local declared warn output. The default level
        /// is Full locking
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Warning(string message, params object[] args)
        {
            Warning(SeverityFlags.Full, message, args);
        }
        /// <summary>
        /// Writes a message to local declared warn output and level
        /// </summary>
        /// <param name="severity">The log level to use</param>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Warning(SeverityFlags severity, string message, params object[] args)
        {
            if (severity > logSeverity)
                return;

            LogWarningInternal(message, args);
        }
        static partial void LogWarningInternal(string message, params object[] args);

        /// <summary>
        /// Writes a message to local declared error output. The default level
        /// is Full locking
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Error(string message, params object[] args)
        {
            Error(SeverityFlags.Full, message, args);
        }
        /// <summary>
        /// Writes a message to local declared error output and level
        /// </summary>
        /// <param name="severity">The log level to use</param>
        /// <param name="message">The message to write</param>
        /// <param name="args">Optional arguments to parse into the message</param>
        public static void Error(SeverityFlags severity, string message, params object[] args)
        {
            if (severity > logSeverity)
                return;

            LogErrorInternal(message, args);
        }
        static partial void LogErrorInternal(string message, params object[] args);
    }
}
