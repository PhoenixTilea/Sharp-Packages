// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using SE;

namespace SE.Storage.Http
{
    public partial class HttpRemoteResource
    {
        /// <summary>
        /// Performs a POST request to the declared resource
        /// </summary>
        public HttpRequest<T> Post<T>(string relativePath = null) where T : HttpResult<T>
        {
            HttpWebRequest request = Initialize(Methods.Post, relativePath);
            return new HttpRequest<T>(request);
        }
        /// <summary>
        /// Performs a POST request to the declared resource
        /// </summary>
        public HttpRequest<HttpStubResult> Post(string relativePath = null)
        {
            return Delete<HttpStubResult>(relativePath) as HttpRequest<HttpStubResult>;
        }

        /// <summary>
        /// Performs a GET request to the declared resource
        /// </summary>
        public HttpResult<T> Get<T>(string relativePath = null)
        {
            HttpWebRequest request = Initialize(Methods.Get, relativePath);
            return HttpResult<T>.Create(request);
        }
        /// <summary>
        /// Performs a GET request to the declared resource
        /// </summary>
        public HttpStubResult Get(string relativePath = null)
        {
            return Get<HttpStubResult>(relativePath) as HttpStubResult;
        }

        /// <summary>
        /// Performs a PUT request to the declared resource
        /// </summary>
        public HttpRequest<T> Put<T>(string relativePath = null) where T : HttpResult<T>
        {
            HttpWebRequest request = Initialize(Methods.Put, relativePath);
            return new HttpRequest<T>(request);
        }
        /// <summary>
        /// Performs a PUT request to the declared resource
        /// </summary>
        public HttpRequest<HttpStubResult> Put(string relativePath = null)
        {
            return Delete<HttpStubResult>(relativePath) as HttpRequest<HttpStubResult>;
        }

        /// <summary>
        /// Performs a PATCH request to the declared resource
        /// </summary>
        public HttpRequest<T> Patch<T>(string relativePath = null) where T : HttpResult<T>
        {
            HttpWebRequest request = Initialize(Methods.Patch, relativePath);
            return new HttpRequest<T>(request);
        }
        /// <summary>
        /// Performs a PATCH request to the declared resource
        /// </summary>
        public HttpRequest<HttpStubResult> Patch(string relativePath = null)
        {
            return Delete<HttpStubResult>(relativePath) as HttpRequest<HttpStubResult>;
        }

        /// <summary>
        /// Performs a DELETE request to the declared resource
        /// </summary>
        public HttpRequest<T> Delete<T>(string relativePath = null) where T : HttpResult<T>
        {
            HttpWebRequest request = Initialize(Methods.Delete, relativePath);
            return new HttpRequest<T>(request);
        }
        /// <summary>
        /// Performs a DELETE request to the declared resource
        /// </summary>
        public HttpRequest<HttpStubResult> Delete(string relativePath = null)
        {
            return Delete<HttpStubResult>(relativePath) as HttpRequest<HttpStubResult>;
        }
    }
}
