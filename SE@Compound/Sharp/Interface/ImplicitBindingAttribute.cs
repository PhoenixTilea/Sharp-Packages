// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Reflection.Compound
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    /// <summary>
    /// An attribute to indicate implicit binding behavior
    /// </summary>
    public class ImplicitBindingAttribute : Attribute
    { }
}
