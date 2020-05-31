// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using SE;

namespace SE.Storage.Http
{
    /// <summary>
    /// An in-place resolved http request
    /// </summary>
    public class HttpStubResult : HttpResult<HttpStubResult>
    {
        Stream stream;
        /// <summary>
        /// The operation result if existing
        /// </summary>
        public Stream Stream
        {
            get { return stream; }
        }

        Exception lastError;
        /// <summary>
        /// The last error occurred while operating
        /// </summary>
        public Exception Error
        {
            get { return lastError; }
        }

        /// <summary>
        /// Creates a new stub http response instance from the provided request
        /// </summary>
        /// <param name="request"></param>
        public HttpStubResult(HttpWebRequest request)
            :base(request)
        { }

        public override void OnResolve(Stream value)
        {
            stream = value;
        }
        public override void OnReject(Exception error)
        {
            lastError = error;
        }
    }
}
