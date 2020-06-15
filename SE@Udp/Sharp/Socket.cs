// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using SE;

namespace SE.Remoting.Udp
{
    /// <summary>
    /// Defines a standard UDP socket
    /// </summary>
    public class Socket : SocketBase<SocketOptions>
    {
        public Socket(IPEndPoint endPoint)
            : base(endPoint, new SocketOptions())
        { }

        public Socket(IPAddress address, int port)
            : base(new IPEndPoint(address, port), new SocketOptions())
        { }

        public Socket(string address, int port) 
            : base(IPAddress.Parse(address), port) 
        { }

        public Socket(int port)
            : base(port)
        { }
    }
}
