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
    public class SocketOptions
    {
        public bool DualMode;

        public bool ReuseAddress;
        public bool ExclusiveAddressUse;
        public bool KeepAlive;
        public bool NoDelay;

        public int ReceiveBufferSize;
        public int SendBufferSize;
    }
}
