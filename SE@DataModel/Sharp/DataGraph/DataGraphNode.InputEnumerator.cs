// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using SE;

namespace SE.DataModel
{
    public abstract partial class DataGraphNode<InputConnectorType, OutputConnectorType> where InputConnectorType : DataGraphNode<InputConnectorType, OutputConnectorType>.Connector
                                                                                         where OutputConnectorType : DataGraphNode<InputConnectorType, OutputConnectorType>.Connector
    {
        /// <summary>
        /// A class specific allocation free enumerator
        /// </summary>
        public struct InputEnumerator : IEnumerable<InputConnectorType>, IEnumerator<InputConnectorType>
        {
            DataGraphNode<InputConnectorType, OutputConnectorType> instance;
            int id;

            public InputConnectorType Current
            {
                get { return instance.inputPins.GetValue(id); }
            }
            object IEnumerator.Current
            {
                get { return Current; }
            }

            public InputEnumerator(DataGraphNode<InputConnectorType, OutputConnectorType> instance)
            {
                this.instance = instance;
                this.id = -1;
            }
            public void Dispose()
            {
                instance = null;
                id = -1;
            }

            public bool MoveNext()
            {
                if (id + 1 < instance.inputPins.Length)
                {
                    id++;
                    return true;
                }
                else return false;
            }
            public void Reset()
            {
                id = -1;
            }

            public IEnumerator<InputConnectorType> GetEnumerator()
            {
                return this;
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
