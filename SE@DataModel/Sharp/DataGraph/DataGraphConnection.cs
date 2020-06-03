// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.DataModel
{
    /// <summary>
    /// Defines a link from an input to an output connector
    /// </summary>
    public abstract class DataGraphConnection<InputConnectorType, OutputConnectorType> where InputConnectorType : DataGraphNode<InputConnectorType, OutputConnectorType>.Connector
                                                                                       where OutputConnectorType : DataGraphNode<InputConnectorType, OutputConnectorType>.Connector
    {
        protected InputConnectorType inputConnector;
        /// <summary>
        /// The input connector
        /// </summary>
        public InputConnectorType InputConnector
        {
            get { return inputConnector; }
        }

        protected OutputConnectorType outputConnector;
        /// <summary>
        /// The output connector
        /// </summary>
        public OutputConnectorType OutputConnector
        {
            get { return outputConnector; }
        }

        /// <summary>
        /// Initializes a new connection from the passed input to the output connector.
        /// Increments connector counts
        /// </summary>
        public DataGraphConnection(OutputConnectorType outputConnector, InputConnectorType inputConnector)
        {
            this.outputConnector = outputConnector;
            this.outputConnector.Count++;
            this.inputConnector = inputConnector;
            this.inputConnector.Count++;
        }
        /// <summary>
        /// Disposes the connection and decrements connector counts
        /// </summary>
        public void Dispose()
        {
            if (outputConnector != null)
            {
                outputConnector.Count--;
            }
            if (inputConnector != null)
            {
                inputConnector.Count--;
            }
        }
    }
}
