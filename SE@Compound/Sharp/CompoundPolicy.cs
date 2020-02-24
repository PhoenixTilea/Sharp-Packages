// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// Defines the behavior of the CompoundBinder on DLR requests
    /// </summary>
    public class CompoundPolicy
    {
        /// <summary>
        /// The default policy
        /// - AllowImplicitInterfaceMapping
        /// </summary>
        public readonly static CompoundPolicy Default = new CompoundPolicy
        (
            CompoundPolicyFlags.AllowDynamicProperties |
            CompoundPolicyFlags.AllowExtensionMethods |
            CompoundPolicyFlags.AllowImplicitInterfaceMapping
        );

        CompoundPolicyFlags flags;
        /// <summary>
        /// Currently enabled behavior
        /// </summary>
        public CompoundPolicyFlags Flags
        {
            get { return flags; }
        }

        /// <summary>
        /// Creates a new policy with the desired behavior
        /// </summary>
        /// <param name="flags">A set of flags to enable certain actions</param>
        public CompoundPolicy(CompoundPolicyFlags flags)
        {
            this.flags = flags;
        }
    }
}
