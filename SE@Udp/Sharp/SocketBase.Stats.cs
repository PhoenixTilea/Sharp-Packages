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
        /// Provides a collection of information about IOCP operations
        /// </summary>
        public struct SocketInfo
        {
            /// <summary>
            /// The total amount of bytes received
            /// </summary>
            public long BytesReceived;

            /// <summary>
            /// The amount of bytes included in currend send operation
            /// </summary>
            public long BytesSending;
            /// <summary>
            /// The total amount of bytes sent
            /// </summary>
            public long BytesSent;

            /// <summary>
            /// A timestamp determining last occurance of a transmission
            /// </summary>
            public long Timestamp;

            /// <summary>
            /// The total amount of transmissions received
            /// </summary>
            public long Received;
            /// <summary>
            /// The total amount of transmissions sent
            /// </summary>
            public long Sent;
        }

        SocketInfo stats;
        /// <summary>
        /// Provides statistics information about this socket
        /// </summary>
        public SocketInfo Stats
        {
            get { return stats; }
        }
    }
}
