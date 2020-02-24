// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// A read-access iterator through a property container
    /// </summary>
    public class PropertyIterator : IEnumerable<KeyValuePair<UInt32, PropertyInstance>>, IDisposable
    {
        PropertyContainer container;

        /// <summary>
        /// The number of items contained in the iterator
        /// </summary>
        public int Count
        {
            get { return container.Count; }
        }

        /// <summary>
        /// Creates a new iterator instance for the given container and reserves read access.
        /// Needs to be disposed to release access to the underlaying container
        /// </summary>
        public PropertyIterator(PropertyContainer container)
        {
            this.container = container;
            ((ReadWriteLock)container).ReadLock();
        }

        public void Dispose()
        {
            ((ReadWriteLock)container).ReadRelease();
        }

        public IEnumerator<KeyValuePair<UInt32, PropertyInstance>> GetEnumerator()
        {
            return container.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return container.GetEnumerator();
        }
    }
}