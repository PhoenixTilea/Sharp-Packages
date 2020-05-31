// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using SE;

namespace SE.Storage.Http
{
    /// <summary>
    /// A header component that sets the default user agent to a mozilla mobile browser
    /// </summary>
    public class UserAgentComponent : IHttpHeaderComponent
    {
        const string userAgent = @"Mozilla/5.0 (Mobile; rv:26.0) Gecko/26.0 Firefox/26.0";

        /// <summary>
        /// Creates a new default user agent component instance
        /// </summary>
        public UserAgentComponent()
        { }

        public void AppendValue(HttpWebRequest request)
        {
            request.UserAgent = userAgent;
        }
    }
}
