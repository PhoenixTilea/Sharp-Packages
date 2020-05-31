// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using SE;

namespace SE.Storage.Http
{
    public static partial class SecureStringExtension
    {
        /// <summary>
        /// Translates this SecureString into plain C# text
        /// </summary>
        public static string GetValue(this SecureString str)
        {
            return new NetworkCredential(string.Empty, str).Password;
        }
    }
}
