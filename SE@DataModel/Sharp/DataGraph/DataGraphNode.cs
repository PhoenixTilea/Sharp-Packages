// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using SE;

namespace SE.DataModel
{
    /// <summary>
    /// An abstract node graph item
    /// </summary>
    public abstract partial class DataGraphNode<InputConnectorType, OutputConnectorType> : FlexFieldContainer, IEnumerable<DataGraphNode<InputConnectorType, OutputConnectorType>.Connector>, IEnumerable where InputConnectorType : DataGraphNode<InputConnectorType, OutputConnectorType>.Connector
                                                                                                                                                                                                          where OutputConnectorType : DataGraphNode<InputConnectorType, OutputConnectorType>.Connector
    {
        protected FlexField<InputConnectorType> inputPins;
        /// <summary>
        /// Determines the primary input pin of this node
        /// </summary>
        public InputConnectorType PrimaryInputPin
        {
            get { return inputPins.Value; }
        }
        /// <summary>
        /// Gets a read-only collection of input pins
        /// </summary>
        public IEnumerable<InputConnectorType> InputPins
        {
            get { return new InputEnumerator(this); }
        }

        /// <summary>
        /// The number of available input pins
        /// </summary>
        public int InputCount
        {
            get { return inputPins.Length; }
        }

        protected FlexField<OutputConnectorType> outputPins;
        /// <summary>
        /// Determines the primary output pin of this node
        /// </summary>
        public OutputConnectorType PrimaryOutputPin
        {
            get { return outputPins.Value; }
        }
        /// <summary>
        /// Gets a read-only collection of output pins
        /// </summary>
        public IEnumerable<OutputConnectorType> OutputPins
        {
            get { return new OutputEnumerator(this); }
        }

        /// <summary>
        /// The number of available output pins
        /// </summary>
        public int OutputCount
        {
            get { return outputPins.Length; }
        }

        /// <summary>
        /// Creates a new instance of the data node
        /// </summary>
        public DataGraphNode()
        { }

        protected override void Dispose(bool disposing)
        {
            inputPins.Dispose();
            outputPins.Dispose();
        }

        public IEnumerator<DataGraphNode<InputConnectorType, OutputConnectorType>.Connector> GetEnumerator()
        {
            return new OutputEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
