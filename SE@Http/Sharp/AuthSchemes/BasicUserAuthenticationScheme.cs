// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using SE;

namespace SE.Storage.Http
{
    /// <summary>
    /// Defines authentication via username and password of the current OS user
    /// </summary>
    public class BasicUserAuthenticationScheme : IHttpHeaderComponent
    {
        /// <summary>
        /// Creates a new authentication scheme from OS user credentials
        /// </summary>
        public BasicUserAuthenticationScheme()
        { }

        public void AppendValue(HttpWebRequest request)
        {
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UseDefaultCredentials = true;
        }
    }
}
