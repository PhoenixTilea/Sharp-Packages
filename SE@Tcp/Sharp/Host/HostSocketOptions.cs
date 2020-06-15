// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Remoting.Tcp
{
    /// <summary>
    /// Provides a set of configuration parameters
    /// </summary>
    public class HostSocketOptions : SocketOptions
    {
        public int AcceptBacklog;
    }
}
