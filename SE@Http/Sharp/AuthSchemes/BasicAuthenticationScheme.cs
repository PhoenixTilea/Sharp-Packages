// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using SE;

namespace SE.Storage.Http
{
    /// <summary>
    /// Defines authentication via username and password
    /// </summary>
    public class BasicAuthenticationScheme : IHttpHeaderComponent
    {
        NetworkCredential credentials;

        /// <summary>
        /// Creates a new authentication scheme from provided login data
        /// </summary>
        public BasicAuthenticationScheme(string username, SecureString password)
        {
            string domain = string.Empty;

            int slashIndex = username.IndexOf('\\');
            string login; if (slashIndex >= 0)
            {
                domain = username.Substring(0, slashIndex);
                login = username.Substring(slashIndex + 1);
            }
            else login = username;

            credentials = new NetworkCredential(login, password);
            credentials.Domain = domain;
        }
        /// <summary>
        /// Creates a new authentication scheme from provided login data
        /// </summary>
        public BasicAuthenticationScheme(string username, string password)
            :this(username, password.ToSecureString())
        { }

        public void AppendValue(HttpWebRequest request)
        {
            request.Credentials = credentials;
        }
    }
}
