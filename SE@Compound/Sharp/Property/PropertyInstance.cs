// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// A property instance container used on compound objects
    /// </summary>
    public class PropertyInstance
    {
        Type propertyType;
        /// <summary>
        /// The property type stored in this container object
        /// </summary>
        public Type PropertyType
        {
            get { return propertyType; }
        }

        object value;
        /// <summary>
        /// The property value currently set
        /// </summary>
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Creates a new property instance of the given type
        /// </summary>
        public PropertyInstance(Type propertyType)
        {
            this.propertyType = propertyType;
            if (propertyType.IsValueType && Nullable.GetUnderlyingType(propertyType) == null)
                value = Activator.CreateInstance(propertyType);
        }
    }
}
