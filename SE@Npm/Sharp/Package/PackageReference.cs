// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Npm
{
    /// <summary>
    /// A package referenced in the package info
    /// </summary>
    public struct PackageReference
    {
        PackageId id;
        /// <summary>
        /// The referenced package id
        /// </summary>
        public PackageId Id
        {
            get { return id; }
        }

        PackageVersion version;
        /// <summary>
        /// An extended version to match
        /// </summary>
        public PackageVersion Version
        {
            get { return version; }
        }

        /// <summary>
        /// Creates a new reference instance
        /// </summary>
        public PackageReference(PackageId id, PackageVersion version)
        {
            this.id = id;
            this.version = version;
        }

        public override bool Equals(object obj)
        {
            return (obj is PackageReference && 
                id.Equals(((PackageReference)obj).id) &&
                version.Equals(((PackageReference)obj).version));
        }

        public override int GetHashCode()
        {
            HashCombiner hash = HashCombiner.Initialize();
            hash.Add(id);
            hash.Add(version);

            return hash.Value;
        }
    }
}
