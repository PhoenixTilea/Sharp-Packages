// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using SE;

namespace SE.Remoting.Tcp
{
    /// <summary>
    /// A host class for TCP driven operations on the udnerlaying network layer
    /// </summary>
    public partial class HostSocket<T> : SocketBase<T> where T : HostSocketOptions, new()
    {
        SocketAsyncEventArgs asyncAcceptContext;

        atomic_bool accepting;
        /// <summary>
        /// Determines if an asynchronous accept operation is in progress
        /// </summary>
        public bool Accepting
        {
            get { return accepting; }
        }

        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="endPoint">The target this socket should be bound to</param>
        public HostSocket(IPEndPoint endPoint)
            : base(endPoint, new T())
        {
            Options.AcceptBacklog = 0;
        }
        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="address">The target IP this socket should be bound to</param>
        /// <param name="port">The target port number this socket should be bound to</param>
        public HostSocket(IPAddress address, int port)
            : this(new IPEndPoint(address, port))
        { }
        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="address">The target IP this socket should be bound to</param>
        /// <param name="port">The target port number this socket should be bound to</param>
        public HostSocket(string address, int port) 
            : this(IPAddress.Parse(address), port) 
        { }
        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="port">A system port number this socket should be created on</param>
        public HostSocket(int port)
            : this(IPAddress.IPv6Any, port)
        { }

        /// <summary>
        /// Opens a new peer on the underlaying network layer
        /// </summary>
        /// <returns>True if the peer was opened successfully, false otherwise</returns>
        public virtual bool Open()
        {
            if (initialized)
                return false;

            asyncAcceptContext = new SocketAsyncEventArgs();
            asyncAcceptContext.Completed += HandleIocpEvent;

            socket = new System.Net.Sockets.Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            OnCreated();

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, Options.ReuseAddress);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, Options.ExclusiveAddressUse);
            socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, !Options.DualMode);
            socket.Bind(endPoint);
            socket.Listen(Options.AcceptBacklog);

            initialized = true;

            OnInitialize();
            return true;
        }

        public override bool Close()
        {
            if (!initialized)
                return false;

            OnClosing();

            initialized = false;
            
            asyncAcceptContext.Completed -= HandleIocpEvent;
            try
            {
                socket.Close();
                socket.Dispose();
                asyncAcceptContext.Dispose();
            }
            catch (ObjectDisposedException) { }

            accepting = false;

            OnClosed();
            return true;
        }

        /// <summary>
        /// Resets the peer on the underlaying network layer and tries to reopen it
        /// </summary>
        /// <returns>True if the peer was opened successfully, false otherwise</returns>
        public virtual bool Reset()
        {
            Close();
            return Open();
        }
    }
}
