// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using SE;

namespace SE.Storage.Http
{
    public partial class HttpRemoteResource : FileSystemDescriptor, ICollection<IHttpHeaderComponent>
    {
        public override UInt32 Id
        {
            get { return GetAbsolutePath().Fnv32(); }
        }

        protected Uri baseUri;
        /// <summary>
        /// The remote location this resource is located to
        /// </summary>
        public Uri Location
        {
            get { return baseUri; }
        }

        public override string Name
        {
            get { return Path.GetFileNameWithoutExtension(GetAbsolutePath()); }
        }
        public override string FullName
        {
            get { return Path.GetFileName(GetAbsolutePath()); }
        }

        List<IHttpHeaderComponent> header;
        /// <summary>
        /// A collection of components attached to this resource
        /// </summary>
        public IEnumerable<IHttpHeaderComponent> Header
        {
            get { return header; }
        }

        public int Count
        {
            get { return header.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public override FileSystemEntryType Type
        {
            get { return FileSystemEntryType.Remote; }
        }

        /// <summary>
        /// Creates a new remote file system entry at given location
        /// </summary>
        /// <param name="baseUri">The location to expect the element at</param>
        public HttpRemoteResource(Uri baseUri)
        {
            switch (baseUri.Scheme.ToLowerInvariant())
            {
                case "http":
                case "https": break;
                default: throw new ArgumentOutOfRangeException("baseUri.Scheme");
            }
            this.header = new List<IHttpHeaderComponent>();
            this.header.Add(new UserAgentComponent());
            this.baseUri = baseUri;
        }
        /// <summary>
        /// Creates a new remote file system entry at given location
        /// </summary>
        /// <param name="baseUriString">The location to expect the element at</param>
        public HttpRemoteResource(string baseUriString)
            :this(new Uri(baseUriString))
        { }

        HttpWebRequest Initialize(string method, string relativePath)
        {
            HttpWebRequest request; if (!string.IsNullOrWhiteSpace(relativePath))
            {
                request = HttpWebRequest.Create(new Uri(baseUri, relativePath)) as HttpWebRequest;
            }
            else request = HttpWebRequest.Create(baseUri) as HttpWebRequest;
            request.Method = method;

            foreach (IHttpHeaderComponent component in header)
                component.AppendValue(request);

            return request;
        }

        public void Add(IHttpHeaderComponent item)
        {
            header.Add(item);
        }

        public void Clear()
        {
            header.Clear();
        }

        public bool Contains(IHttpHeaderComponent item)
        {
            return header.Contains(item);
        }

        public void CopyTo(IHttpHeaderComponent[] array, int arrayIndex)
        {
            header.CopyTo(array, arrayIndex);
        }

        public bool Remove(IHttpHeaderComponent item)
        {
            return header.Remove(item);
        }

        public override string GetAbsolutePath()
        {
            return baseUri.AbsoluteUri;
        }

        public override string GetRelativePath(FileSystemDescriptor root)
        {
            return PathDescriptor.GetRelativePath(root, baseUri);
        }

        public override void Create()
        {
            Post(string.Empty);
        }

        /// <summary>
        /// Determines if this file system entry points to a valid ressource
        /// </summary>
        /// <returns>True if the located ressource exists, false otherwise</returns>
        public virtual bool Exists(string relativePath)
        {
            HttpWebRequest request = Initialize(Methods.Head, relativePath);
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    if (statusCode >= HttpStatusCode.Continue && statusCode < HttpStatusCode.BadRequest)
                    {
                        return true;
                    }
                    else return false;
                }
            }
            catch(WebException er)
            {
                if (er.Status == WebExceptionStatus.Success)
                {
                    return true;
                }
                else return false;
            }
        }
        public override bool Exists()
        {
            return Exists(string.Empty);
        }

        public override void Delete()
        {
            Delete(string.Empty);
        }

        public override void Equalize()
        { }
        
        public IEnumerator<IHttpHeaderComponent> GetEnumerator()
        {
            return header.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Enables SSL on all instances of the remote client
        /// </summary>
        /// <param name="callback">A callback used to check for certificate validity</param>
        public static void EnableSsl(RemoteCertificateValidationCallback callback)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2
            ServicePointManager.ServerCertificateValidationCallback += callback;
        }
        /// <summary>
        /// Enables SSL on all instances of the remote client
        /// </summary>
        public static void EnableSsl()
        {
            EnableSsl(new RemoteCertificateValidationCallback(AcceptAny));
        }

        private static bool AcceptAny(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
