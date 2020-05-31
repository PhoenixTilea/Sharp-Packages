// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Npm
{
    /// <summary>
    /// Provides distribution information inside of a package info instance
    /// </summary>
    public class DistributionInfo
    {
        string checksum;
        /// <summary>
        /// The checksum of the downloaded file
        /// </summary>
        public string Checksum
        {
            get { return checksum; }
        }

        Uri url;
        /// <summary>
        /// The address of the file to download
        /// </summary>
        public Uri Url
        {
            get { return url; }
        }

        /// <summary>
        /// Creates a new distribution info instance
        /// </summary>
        public DistributionInfo(string checksum, Uri url)
        {
            this.checksum = checksum;
            this.url = url;
        }
    }
}