// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace SE
{
    /// <summary>
    /// An embedded resource data entry of a defined MIME type
    /// </summary>
    public partial class ResourceDescriptor
    {
        /// <summary>
        /// The unique ID of this data element
        /// </summary>
        public UInt32 Id
        {
            get { return name.ToLowerInvariant().Fnv32(); }
        }

        Assembly assembly;

        string component;
        /// <summary>
        /// The grouping component of this data element
        /// </summary>
        public string Component
        {
            get { return component; }
        }

        string location;
        /// <summary>
        /// The location of this data element on disk
        /// </summary>
        public string Location
        {
            get { return location; }
        }

        string name;
        string extension;

        /// <summary>
        /// The qualified name of the data element
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        /// <summary>
        /// The full qualified name and extension of the data element
        /// </summary>
        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(extension)) return name;
                else return string.Format("{0}.{1}", name, extension);
            }
        }

        /// <summary>
        /// The extension type this data element has
        /// </summary>
        public string Extension
        {
            get { return extension; }
        }

        /// <summary>
        /// Creates a new data element entry
        /// </summary>
        /// <param name="component">The grouping component of this data element</param>
        /// <param name="path">The full path of this data element entry</param>
        public ResourceDescriptor(Assembly assembly, string component, string path)
        {
            this.assembly = assembly;
            this.component = component;
            this.location = Path.GetDirectoryName(path);
            this.name = Path.GetFileNameWithoutExtension(path);
            this.extension = Path.GetExtension(path).Trim('.');
        }
        /// <summary>
        /// Creates a new data element entry from given parent
        /// </summary>
        /// <param name="root">A parent element to embedd</param>
        public ResourceDescriptor(ResourceDescriptor root)
        {
            this.assembly = root.assembly;
            this.component = root.component;
            this.location = root.location;
            this.name = root.name;
            this.extension = root.extension;
        }

        public static bool operator ==(ResourceDescriptor left, ResourceDescriptor right)
        {
            return (left as object == right as object ||
                   (left as object != null && right as object != null &&
                    left.component == right.component && left.FullName == right.FullName));
        }
        public static bool operator !=(ResourceDescriptor left, ResourceDescriptor right)
        {
            return (left as object != right as object &&
                   (left as object == null || right as object == null ||
                    left.component != right.component || left.FullName != right.FullName));
        }

        /// <summary>
        /// Returns the full qualified descriptor of this data element
        /// </summary>
        public string GetAbsolutePath()
        {
            return string.Format("{0}@{1}", component, Path.Combine(location, FullName).Replace('\\', '/'));
        }

        /// <summary>
        /// Checks whether the data element has embedded content
        /// </summary>
        /// <returns>True if content was embedded into the assembly, false otherwise</returns>
        public bool Exists()
        {
            using (Stream stream = Open())
                return (stream != null);
        }

        /// <summary>
        /// Creates a stream object pointing to this element's embedded content
        /// </summary>
        /// <returns>The stream instance</returns>
        public Stream Open()
        {
            return ResourceManager.GetStream(assembly, GetAbsolutePath());
        }

        public override int GetHashCode()
        {
            return GetAbsolutePath().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (this == obj as ResourceDescriptor);
        }

        public override string ToString()
        {
            return GetAbsolutePath();
        }
    }
}