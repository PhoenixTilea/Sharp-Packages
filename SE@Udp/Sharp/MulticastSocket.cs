// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using SE;

namespace SE.Remoting.Udp
{
    /// <summary>
    /// Defines a multicast socket
    /// </summary>
    public class MulticastSocket : SocketBase<SocketOptions>
    {
        public MulticastSocket(IPEndPoint endPoint)
            : base(endPoint, new SocketOptions())
        { }

        public MulticastSocket(IPAddress address, int port)
            : base(new IPEndPoint(address, port), new SocketOptions())
        { }

        /// <summary>
        /// Creates a specialized socket to the global IPv4 broadcast address
        /// </summary>
        public MulticastSocket(int port)
            : base(IPAddress.Broadcast, port)
        { }

        /// <summary>
        /// Joins the provided multicast group
        /// </summary>
        public virtual void JoinMulticastGroup(IPAddress address)
        {
            if (EndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                RawSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(address));
            }
            else RawSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(address));
            OnJoinedMulticastGroup(address);
        }
        /// <summary>
        /// Joins the provided multicast group
        /// </summary>
        public void JoinMulticastGroup(string address)
        {
            JoinMulticastGroup(IPAddress.Parse(address));
        }

        /// <summary>
        /// Leaves the provided multicast group
        /// </summary>
        public virtual void LeaveMulticastGroup(IPAddress address)
        {
            if (EndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                RawSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, new IPv6MulticastOption(address));
            }
            else RawSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(address));
            OnJoinedMulticastGroup(address);
        }
        /// <summary>
        /// Leaves the provided multicast group
        /// </summary>
        public void LeaveMulticastGroup(string address)
        {
            LeaveMulticastGroup(IPAddress.Parse(address));
        }

        /// <summary>
        /// A callback for whenever a multicast group was joined
        /// </summary>
        protected virtual void OnJoinedMulticastGroup(IPAddress address)
        { }

        /// <summary>
        /// A callback for whenever a multicast group was left
        /// </summary>
        protected virtual void OnLeftMulticastGroup(IPAddress address)
        { }

        protected override void OnCreated()
        {
            if (EndPoint.Address == IPAddress.Broadcast)
            {
                RawSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            }
            Options.FixedAddressUse = false;
            base.OnCreated();
        }
    }
}
