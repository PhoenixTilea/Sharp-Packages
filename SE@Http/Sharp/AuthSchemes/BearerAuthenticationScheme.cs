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
    /// Defines authentication via a bearer token (OAuth2)
    /// </summary>
    public class BearerAuthenticationScheme : IHttpHeaderComponent
    {
        SecureString token;

        /// <summary>
        /// Creates a new authentication scheme from provided token data
        /// </summary>
        public BearerAuthenticationScheme(SecureString token)
        {
            this.token = token;
        }
        /// <summary>
        /// Creates a new authentication scheme from provided token data
        /// </summary>
        public BearerAuthenticationScheme(string token)
            : this(token.ToSecureString())
        { }

        public void AppendValue(HttpWebRequest request)
        {
            request.Headers.Add(HttpRequestHeader.Authorization, string.Concat("Bearer ", token.GetValue()));
            request.PreAuthenticate = true;
        }
    }
}
