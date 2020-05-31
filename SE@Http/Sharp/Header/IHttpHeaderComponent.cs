// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using SE;

namespace SE.Storage.Http
{
    /// <summary>
    /// An HTTP Header component to be attached to every request of a single resource
    /// </summary>
    public interface IHttpHeaderComponent
    {
        /// <summary>
        /// Appends this component's data to the provided plain request
        /// </summary>
        void AppendValue(HttpWebRequest request);
    }
}
