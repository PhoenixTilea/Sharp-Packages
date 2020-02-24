// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// Enables certain behavior in a CompoundPolicy
    /// </summary>
    public enum CompoundPolicyFlags
    {
        AllowDynamicProperties = 0x1,
        AllowPropertyDefaults = 0x2,

        AllowExtensionMethods = 0x4,
        AllowImplicitInterfaceMapping = 0x8,
    }
}
