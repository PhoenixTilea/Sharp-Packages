// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Behavior
{
    /// <summary>
    /// An interface to implement in classes and structs to support dynamic attribution
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The object ID to identify components
        /// </summary>
        InstanceId Id { get; }
    }
}
