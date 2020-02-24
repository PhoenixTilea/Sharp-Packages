// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Dynamic;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// An interface to implement in classes to support runtime composition
    /// </summary>
    public interface ICompoundObject : IDynamicMetaObjectProvider
    {
        /// <summary>
        /// The object ID to identify components
        /// </summary>
        InstanceId Id { get; }
    }
}
