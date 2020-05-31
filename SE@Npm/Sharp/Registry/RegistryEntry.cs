// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Json;
using SE;
using System.Collections;
using System.IO;
using System.Text;

namespace SE.Npm
{
    /// <summary>
    /// Defines an entry in the NPM package registry
    /// </summary>
    public class RegistryEntry : JsonDocument, IEnumerable<PackageInfo>
    {
        const string PackageName = "name";
        const string PackageDescription = "description";
        const string PackageReleases = "versions";
        const string PackageReleaseDates = "time";

        PackageId id;
        /// <summary>
        /// This package's three-component ID
        /// </summary>
        public PackageId Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// The package owner
        /// </summary>
        public string Owner
        {
            get { return id.Owner.ToUpperInvariant(); }
        }
        /// <summary>
        /// The package namespace
        /// </summary>
        public string Namespace
        {
            get { return id.Namespace.ToTitleCase(); }
        }
        /// <summary>
        /// The package names
        /// </summary>
        public string Name
        {
            get { return id.Name.ToTitleCase(); }
        }

        string description;
        /// <summary>
        /// A descriptive text displayed in some package browser
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        HashSet<PackageInfo> versions;
        /// <summary>
        /// A list of releases from this package 
        /// </summary>
        public HashSet<PackageInfo> Versions
        {
            get { return versions; }
        }

        /// <summary>
        /// Creates a new package meta instance
        /// </summary>
        public RegistryEntry()
        {
            this.versions = new HashSet<PackageInfo>();
        }

        public override bool Load(Stream stream, Encoding encoding)
        {
            if (base.Load(stream, encoding))
            {
                if (Root == null)
                    return false;

                return Process(Root.Child);
            }
            else return false;
        }

        /// <summary>
        /// Tries to parse a registry entry from provided json
        /// </summary>
        /// <returns>True if successfully parsed, false otherwise</returns>
        internal bool Process(JsonNode rootNode)
        {
            while (rootNode != null)
            {
                switch (rootNode.Name.ToLowerInvariant())
                {
                    #region PackageName = 'name';
                    case PackageName: if(rootNode.Type == JsonNodeType.String)
                        {
                            object pid; if (PackageId.TryParse(rootNode.ToString(), out pid))
                            {
                                id = pid as PackageId;
                                break;
                            }
                            else parser.Errors.Add(string.Format(ResponseCodes.PackageInfo.InvalidPackageName, rootNode.ToString()));
                        }
                        return false;
                    #endregion

                    #region PackageDescription = 'description';
                    case PackageDescription: if (rootNode.Type == JsonNodeType.String)
                        {
                            description = rootNode.ToString();
                        }
                        break;
                    #endregion

                    #region PackageReleases = 'versions';
                    case PackageReleases: if(rootNode.Type == JsonNodeType.Object)
                        {
                            if (ReadPackageReleases(rootNode))
                                break;
                        }
                        return false;
                    #endregion

                    #region PackageReleaseDates = 'time';
                    case PackageReleaseDates: if(rootNode.Type == JsonNodeType.Object)
                        {
                            ReadReleaseDates(rootNode);
                        }
                        break;
                    #endregion
                }
                rootNode = rootNode.Next;
            }
            return true;
        }

        bool ReadPackageReleases(JsonNode property)
        {
            property = property.Child;
            while (property != null)
            {
                if (property.Type == JsonNodeType.Object)
                {
                    PackageVersion ver = PackageVersion.Create(property.Name.ToString());
                    if (!ver.IsValid)
                        return false;

                    PackageInfo release = null;
                    foreach (PackageInfo version in versions)
                        if (version.Version == ver)
                        {
                            release = version;
                            break;
                        }
                    if (release == null)
                    {
                        release = new PackageInfo();
                    }
                    if (release.Process(property.Child))
                    {
                        versions.Add(release);
                    }
                }

                property = property.Next;
            }
            return true;
        }
        void ReadReleaseDates(JsonNode property)
        {
            property = property.Child;
            while (property != null)
            {
                if (property.Type == JsonNodeType.String)
                {
                    PackageVersion ver = PackageVersion.Create(property.Name.ToString());
                    if (!ver.IsValid)
                        continue;

                    PackageInfo release = null;
                    foreach (PackageInfo version in versions)
                        if (version.Version == ver)
                        {
                            release = version;
                            break;
                        }

                    if (release != null)
                    {
                        DateTime date; if (DateTime.TryParse(property.ToString(), out date))
                            release.ReleaseDate = date;
                    }
                }

                property = property.Next;
            }
        }

        public IEnumerator<PackageInfo> GetEnumerator()
        {
            return versions.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
