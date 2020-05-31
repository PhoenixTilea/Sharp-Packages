// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SE.Storage;
using SE.Storage.Http;
using SE;

namespace SE.Npm
{
    public class PackageRegistry
    {
        private static Dictionary<int, RegistryEntry> entries;
        private static ReadWriteLock entryLock;

        HttpRemoteResource client;

        public Uri Location
        {
            get { return client.Location; }
        }

        HashSet<string> scopes;

        public HashSet<string> Scopes
        {
            get { return scopes; }
        }

        static PackageRegistry()
        {
            HttpRemoteResource.EnableSsl();
            entries = new Dictionary<int, RegistryEntry>();
            entryLock = new ReadWriteLock();
        }
        private PackageRegistry()
        {
            this.scopes = new HashSet<string>();
        }
        public PackageRegistry(Uri baseUri)
            : this()
        {
            this.client = new HttpRemoteResource(baseUri);
        }
        public PackageRegistry(string baseUriString)
            : this()
        {
            this.client = new HttpRemoteResource(baseUriString);
        }

        public void ClearToken()
        {
            foreach (IHttpHeaderComponent component in client)
                if (component is BearerAuthenticationScheme)
                {
                    client.Remove(component);
                    break;
                }
        }

        public void SetToken(string token)
        {
            foreach (IHttpHeaderComponent component in client)
                if (component is BearerAuthenticationScheme)
                {
                    client.Remove(component);
                    client.Add(new BearerAuthenticationScheme(token));
                    return;
                }

            client.Add(new BearerAuthenticationScheme(token));
        }

        public bool TryGetPackage(PackageId id, PackageVersion version, out PackageInfo info)
        {
            RegistryEntry entry;
            info = null;

            using (ThreadContext.ReadLock(entryLock))
            {
                entries.TryGetValue(id.GetHashCode(), out entry);
            }
            if(entry == null)
            {
                using (ThreadContext.WriteLock(entryLock))
                {
                    if (!entries.TryGetValue(id.GetHashCode(), out entry))
                    {
                        entry = new RegistryEntry();
                        if (TryGetEntry(id, entry))
                        {
                            entries.Add(id.GetHashCode(), entry);
                        }
                        else return false;
                    }
                }
            }
            if (version.IsValid)
            {
                foreach (PackageInfo release in entry)
                    if (release.Version.Match(version) && (info == null || release.Version > info.Version))
                    {
                        info = release;
                    }
            }
            else
            {
                foreach (PackageInfo release in entry)
                    if (info == null || release.Version > info.Version)
                    {
                        info = release;
                    }
            }
            return (info != null);
        }
        public bool TryGetPackage(PackageInfo info, FileDescriptor target)
        {
            if (info.Distribution != null)
            {
                using (HttpStubResult result = client.Get(info.Distribution.Url.LocalPath))
                {
                    if (result.Resolve())
                    {
                        switch (result.ContentType.MediaType)
                        {
                            case "application/octet-stream":
                                {
                                    using (FileStream fs = target.Open(FileMode.Create, FileAccess.Write))
                                        result.Stream.CopyTo(fs);
                                }
                                return true;
                        }
                    }
                    else { }
                }
            }
            return false;
        }
        public bool TryGetEntry(PackageId id, RegistryEntry entry)
        {
            if (string.IsNullOrWhiteSpace(id.Scope) && scopes.Count > 0)
            {
                foreach (string scope in scopes)
                {
                    if (GetEntry(new PackageId(scope, id), entry))
                        return true;
                }
                return false;
            }
            else return GetEntry(id, entry);
        }

        bool GetEntry(PackageId id, RegistryEntry entry)
        {
            StringBuilder path; if (!string.IsNullOrWhiteSpace(id.Scope))
            {
                path = new StringBuilder(id.Scope.Trim('@'));
                path.Append('/');
                path.Append(id.ToString().Replace("/", "%2F"));
            }
            else path = new StringBuilder(id.ToString());
            using (HttpStubResult result = client.Get(path.ToString()))
            {
                if (result.Resolve())
                {
                    switch (result.ContentType.MediaType)
                    {
                        case "application/json":
                        case "text/json":
                            return entry.Load(result.Stream.GetBuffer(16), result.Encoding);
                    }
                }
                else { }
            }
            return false;
        }
    }
}
