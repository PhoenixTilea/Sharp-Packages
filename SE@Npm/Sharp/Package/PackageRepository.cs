// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Npm
{
    /// <summary>
    /// Provides information about a repository inside a package info
    /// </summary>
    public struct PackageRepository
    {
        string type;
        /// <summary>
        /// The protocol type used to access the repository
        /// </summary>
        public string Type
        {
            get { return type; }
        }

        Uri url;
        /// <summary>
        /// The address of the repository
        /// </summary>
        public Uri Url
        {
            get { return url; }
        }

        string directory;
        /// <summary>
        /// An optional path offset to the package.json file
        /// </summary>
        public string Directory
        {
            get { return directory; }
        }

        /// <summary>
        /// Creates a new repository info instance
        /// </summary>
        public PackageRepository(string type, Uri url, string directory)
        {
            this.type = type;
            this.url = url;
            this.directory = directory;
        }
    }
}
