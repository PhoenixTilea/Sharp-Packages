// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using SE;

namespace SE.Storage.Http
{
    /// <summary>
    /// A basic http result container
    /// </summary>
    public abstract class HttpResult<T> : IHttpResult, IHttpPromise
    {
        private static Func<HttpWebRequest, HttpResult<T>> creator;

        HttpWebRequest request;
        HttpWebResponse response;

        ContentType contentType;
        /// <summary>
        /// The content MIME type of the result data
        /// </summary>
        public ContentType ContentType
        {
            get
            {
                if (contentType == null)
                {
                    contentType = new ContentType(response.ContentType);
                }
                return contentType;
            }
        }

        Encoding encoding;
        /// <summary>
        /// A character encoding the content written in
        /// </summary>
        public Encoding Encoding
        {
            get 
            {
                if (encoding == null)
                {
                    if (!string.IsNullOrWhiteSpace(ContentType.CharSet))
                    {
                        encoding = Encoding.GetEncoding(response.CharacterSet);
                    }
                    else if (!string.IsNullOrWhiteSpace(ContentType.CharSet))
                    {
                        encoding = Encoding.GetEncoding(ContentType.CharSet);
                    }
                    else encoding = Encoding.UTF8;
                }
                return encoding;
            }
        }

        static HttpResult()
        {
            creator = typeof(T).GetCreator<Func<HttpWebRequest, HttpResult<T>>>();
        }
        /// <summary>
        /// Creates a new http result instance from the provided request
        /// </summary>
        /// <param name="request"></param>
        public HttpResult(HttpWebRequest request)
        {
            this.request = request;
        }

        public bool Resolve()
        {
            try
            {
                response = (request.GetResponse() as HttpWebResponse);
                OnResolve(response.GetResponseStream());

                return true;
            }
            catch (WebException er)
            {
                if (er.Status == WebExceptionStatus.Success)
                {
                    response = (er.Response as HttpWebResponse);
                    OnResolve(response.GetResponseStream());

                    return true;
                }
                else
                {
                    OnReject(er);
                    return false;
                }
            }
        }

        public abstract void OnResolve(Stream value);
        public abstract void OnReject(Exception error);

        public virtual void Dispose()
        {
            if (response != null)
            {
                response.Close();
                response = null;
            }
        }

        /// <summary>
        /// Tries to create a new http result instance from the provided request
        /// </summary>
        public static HttpResult<T> Create(HttpWebRequest request)
        {
            return creator(request);
        }
    }
}
