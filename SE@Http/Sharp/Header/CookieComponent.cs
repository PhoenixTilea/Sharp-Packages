// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using SE;

namespace SE.Storage.Http
{
    /// <summary>
    /// A header component that stores cookies from the entire session
    /// </summary>
    public class CookieComponent : IHttpHeaderComponent
    {
        CookieContainer cookieContainer;

        /// <summary>
        /// Creates a new cookie container component instance
        /// </summary>
        public CookieComponent()
        {
            this.cookieContainer = new CookieContainer();
        }

        public void AppendValue(HttpWebRequest request)
        {
            request.CookieContainer = cookieContainer;
        }
    }
}
