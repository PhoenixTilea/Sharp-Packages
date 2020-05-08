// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Cpp
{
    /// <summary>
    /// A defined preprocessor macro
    /// </summary>
    public class Macro
    {
        string name;
        /// <summary>
        /// This Macro's name
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        UInt32 id;
        /// <summary>
        /// This Macro's unique ID
        /// </summary>
        public UInt32 Id
        {
            get { return id; }
        }

        bool hasParameter;
        /// <summary>
        /// Determines if this Macro requires input parameters
        /// </summary>
        public bool HasParameter
        {
            get { return hasParameter; }
            protected internal set { hasParameter = value; }
        }

        List<UInt32> parameter;
        /// <summary>
        /// A list IDs referring to named parameters required
        /// </summary>
        public List<UInt32> Parameter
        {
            get
            {
                if (parameter == null)
                {
                    parameter = new List<UInt32>();
                }
                return parameter;
            }
        }

        List<CompilerToken> replacementList;
        /// <summary>
        /// A list of tokens to be inserted into the source stream
        /// </summary>
        public List<CompilerToken> ReplacementList
        {
            get
            {
                if (replacementList == null)
                {
                    replacementList = new List<CompilerToken>();
                }
                return replacementList;
            }
        }

        bool isVariadic;
        /// <summary>
        /// Determines if this Macro has a variadic number of arguments
        /// </summary>
        public bool IsVariadic
        {
            get { return (isVariadic && hasParameter); }
            set { isVariadic = value; }
        }

        /// <summary>
        /// Creates a new Macro instance
        /// </summary>
        /// <param name="id">This Macro's unique ID</param>
        /// <param name="name">This Macro's name</param>
        public Macro(UInt32 id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
