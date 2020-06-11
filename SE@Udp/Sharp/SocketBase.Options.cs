// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Remoting.Udp
{
    public partial class SocketBase
    {
        /// <summary>
        /// Provides a set of configuration parameters
        /// </summary>
        public class SocketOptions
        {
            public bool DualMode;

            public bool ReuseAddress;
            public bool ExclusiveAddressUse;
            public bool FixedAddressUse;
            public bool FixedPortUse;

            public int ReceiveBufferSize;
            public int SendBufferSize;
        }

        SocketOptions options;
        /// <summary>
        /// A collection of configuration parameters used when initializing
        /// this socket
        /// </summary>
        public SocketOptions Options
        {
            get { return options; }
        }
    }
}
