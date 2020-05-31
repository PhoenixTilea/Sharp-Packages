// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Npm
{
    /// <summary>
    /// A three-component package identifier
    /// </summary>
    public class PackageId
    {
        string[] components;

        /// <summary>
        /// The package owner
        /// </summary>
        public string Owner
        {
            get
            {
                if (components != null && components.Length >= 0)
                {
                    return components[0];
                }
                else return string.Empty;
            }
        }
        /// <summary>
        /// The package namespace
        /// </summary>
        public string Namespace
        {
            get
            {
                if (components != null && components.Length >= 1)
                {
                    return components[1];
                }
                else return string.Empty;
            }
        }
        /// <summary>
        /// The package name
        /// </summary>
        public string Name
        {
            get
            {
                if (components != null && components.Length >= 2)
                {
                    return components[2];
                }
                else return string.Empty;
            }
        }

        string scope;
        /// <summary>
        /// An optional scope covering this ID
        /// </summary>
        public string Scope
        {
            get { return scope; }
        }

        protected PackageId()
        { }
        /// <summary>
        /// Creates a new id instance from the provided string
        /// </summary>
        /// <param name="value">A string in <owner>.<namespace>.<name> format</param>
        public PackageId(string value)
        {
            this.components = Parse(value, out this.scope);

            if (components.Length != 3)
                throw new ArgumentOutOfRangeException("value");
        }
        /// <summary>
        /// Creates a new id instance from the provided root id and a scope
        /// </summary>
        /// <param name="scope">A string in @<scope> format</param>
        /// <param name="id">The root id to scope</param>
        public PackageId(string scope, PackageId id)
        {
            this.components = id.components;
            this.scope = scope;
        }

        public override bool Equals(object obj)
        {
            return (obj is PackageId && 
                Owner.Equals((obj as PackageId).Owner) &&
                Namespace.Equals((obj as PackageId).Namespace) &&
                Name.Equals((obj as PackageId).Name));
        }

        public override int GetHashCode()
        {
            HashCombiner hash = new HashCombiner();
            hash.Add(Owner);
            hash.Add(Namespace);
            hash.Add(Name);

            return hash.Value;
        }

        public override string ToString()
        {
            string id = string.Format("{0}.{1}.{2}", components[0], components[1], components[2]);
            if (!string.IsNullOrWhiteSpace(scope)) return string.Concat(scope, "/", id);
            else return id;
        }

        /// <summary>
        /// Tries to convert a given string into a valid id instance
        /// </summary>
        /// <param name="value">A string in <owner>.<namespace>.<name> format</param>
        /// <returns>True if parsing the string was successful, false otherwise</returns>
        public static bool TryParse(string value, out object result)
        {
            string scope; string[] components = Parse(value, out scope);
            if (components.Length == 3)
            {
                result = new PackageId();
                (result as PackageId).components = components;
                (result as PackageId).scope = scope;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        private static string[] Parse(string value, out string scope)
        {
            int index = value.IndexOf('/');
            if (index >= 0)
            {
                scope = value.Substring(0, index);
                value = value.Substring(index + 1);
            }
            else scope = string.Empty;
            return value.Trim('.').Split('.');
        }
    }
}
