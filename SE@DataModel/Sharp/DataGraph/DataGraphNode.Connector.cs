// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.DataModel
{
    public abstract partial class DataGraphNode<InputConnectorType, OutputConnectorType> where InputConnectorType : DataGraphNode<InputConnectorType, OutputConnectorType>.Connector
                                                                                         where OutputConnectorType : DataGraphNode<InputConnectorType, OutputConnectorType>.Connector
    {
        /// <summary>
        /// A connection anchor on a node
        /// </summary>
        public abstract class Connector
        {
            DataGraphNode<InputConnectorType, OutputConnectorType> parent;
            /// <summary>
            /// This connectors parent node
            /// </summary>
            public DataGraphNode<InputConnectorType, OutputConnectorType> Parent
            {
                get { return parent; }
            }

            protected int count;
            /// <summary>
            /// Gets the amount of connections added to this anchor
            /// </summary>
            public int Count
            {
                get { return count; }
                internal set { count = Math.Max(0, value); }
            }

            /// <summary>
            /// Initializes a new anchor instance
            /// </summary>
            public Connector(DataGraphNode<InputConnectorType, OutputConnectorType> parent)
            {
                this.parent = parent;
            }
        }
    }
}
