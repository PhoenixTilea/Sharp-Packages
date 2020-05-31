// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Storage.Http
{
    /// <summary>
    /// Encapsulates the result of an http request
    /// </summary>
    public interface IHttpResult : IDisposable
    {
        /// <summary>
        /// Resolves the request into response data
        /// </summary>
        bool Resolve();
    }
}
