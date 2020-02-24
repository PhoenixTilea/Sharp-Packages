// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace SE
{
    /// <summary>
    /// Provides access to embedded resources in the assembly
    /// </summary>
    public static class ResourceManager
    {
        private readonly static HashSet<ResourceDescriptor> resources;
        private readonly static ReadWriteLock resourceLock;

        /// <summary>
        /// Provides an iterator to all embedded resources available
        /// </summary>
        public static IEnumerable<ResourceDescriptor> Resources
        {
            get{ return resources; }
        }

        static ResourceManager()
        {
            resources = new HashSet<ResourceDescriptor>();
            resourceLock = new ReadWriteLock();

            LoadResources(Assembly.GetEntryAssembly());
        }

        public static int LoadResources(Assembly assembly)
        {
            string[] names = assembly.GetManifestResourceNames();
            int count = 0;

            foreach (string name in names)
            {
                int index = name.IndexOf('@');
                string component; if (index != -1)
                    component = name.Substring(0, index);
                else
                    component = string.Empty;

                string path; if (!string.IsNullOrWhiteSpace(component))
                    path = name.Substring(component.Length + 1);
                else
                    path = name;

                ResourceDescriptor descriptor = new ResourceDescriptor(assembly, component, path);
                using (ThreadContext.WriteLock(resourceLock))
                    resources.Add(descriptor);

                count++;
            }
            return count;
        }

        /// <summary>
        /// Tries to get the named resource from the collection of embedded resources
        /// </summary>
        /// <param name="descriptor">The resource descriptor</param>
        /// <param name="resource">The result value if successfull</param>
        /// <returns>True if the embedded resource has been found, false otherwise</returns>
        public static bool TryGet(string descriptor, out ResourceDescriptor result)
        {
            using (ThreadContext.ReadLock(resourceLock))
            {
                foreach (ResourceDescriptor resource in resources)
                    if (resource.Name.Equals(descriptor, StringComparison.InvariantCulture) || resource.GetAbsolutePath().EndsWith(descriptor, StringComparison.InvariantCulture))
                    {
                        result = resource;
                        return true;
                    }
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Gets the named resource from the collection of embedded resources
        /// </summary>
        /// <param name="descriptor">The resource descriptor</param>
        /// <returns>The embedded resource if successful</returns>
        public static ResourceDescriptor Get(string descriptor)
        {
            ResourceDescriptor result; if (!TryGet(descriptor, out result))
                throw new IndexOutOfRangeException();

            return result;
        }

        /// <summary>
        /// Gets the named resource data stream from the collection of embedded resources
        /// </summary>
        /// <param name="descriptor">The resource descriptor</param>
        /// <returns>The embedded resource data stream if successful, null otherwise</returns>
        public static Stream GetStream(Assembly assembly, string descriptor)
        {
            return assembly.GetManifestResourceStream(descriptor);
        }
    }
}
