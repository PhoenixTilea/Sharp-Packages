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
    /// An http data request
    /// </summary>
    public class HttpRequest<T> where T : HttpResult<T>
    {
        HttpWebRequest request;

        /// <summary>
        /// Defines the content type of the request data
        /// </summary>
        public string ContentType
        {
            get { return request.ContentType; }
            set { request.ContentType = value; }
        }

        /// <summary>
        /// Creates a new http request from the provided base
        /// </summary>
        public HttpRequest(HttpWebRequest request)
        {
            this.request = request;
        }

        /// <summary>
        /// Returns the data stream instance of this request. The instance has to be
        /// closed after all data has been written
        /// </summary>
        public Stream GetStream()
        {
            return request.GetRequestStream();
        }

        /// <summary>
        /// Completes the request and returns a result instance
        /// </summary>
        public HttpResult<T> Complete()
        {
            return HttpResult<T>.Create(request);
        }
    }
}
