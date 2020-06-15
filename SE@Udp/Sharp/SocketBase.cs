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
    /// A base class for UDP driven operations on the udnerlaying network layer
    /// </summary>
    public abstract partial class SocketBase<T> : IDisposable where T : SocketOptions, new()
    {
        private readonly static IPEndPoint IPv4Any = new IPEndPoint(IPAddress.Any, 0);
        private readonly static IPEndPoint IPv6Any = new IPEndPoint(IPAddress.IPv6Any, 0);

        byte[] receiveBuffer;
        byte[] sendBuffer;

        SocketAsyncEventArgs asyncReceiveContext;
        SocketAsyncEventArgs asyncSendContext;
        System.Net.Sockets.Socket socket;

        /// <summary>
        /// Provides access to the udnerlaying raw socket
        /// </summary>
        protected System.Net.Sockets.Socket RawSocket
        {
            get { return socket; }
        }

        IPEndPoint endPoint;
        /// <summary>
        /// The target this socket was bound to
        /// </summary>
        public IPEndPoint EndPoint
        {
            get { return endPoint; }
        }

        bool initialized;
        /// <summary>
        /// Determines if this socket has been initialized
        /// </summary>
        public bool Initialized
        {
            get { return initialized; }
        }

        atomic_bool receiving;
        /// <summary>
        /// Determines if an asynchronous receive operation is in progress
        /// </summary>
        public bool Receiving
        {
            get { return receiving; }
        }

        atomic_bool sending;
        /// <summary>
        /// Determines if an asynchronous send operation is in progress
        /// </summary>
        public bool Sending
        {
            get { return sending; }
        }

        bool disposed;
        /// <summary>
        /// An indicator set when this socket got disposed
        /// </summary>
        public bool Disposed
        {
            get { return disposed; }
        }

        T options;
        /// <summary>
        /// A collection of configuration parameters used when initializing
        /// this socket
        /// </summary>
        public T Options
        {
            get { return options; }
        }

        /// <summary>
        /// Initializes this socket with the given options
        /// </summary>
        protected SocketBase(IPEndPoint endPoint, T options)
        {
            this.endPoint = endPoint;
            this.options = options;
            this.options.ReceiveBufferSize = 1024;
            this.options.SendBufferSize = 1024;
        }
        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="endPoint">The target this socket should be bound to</param>
        public SocketBase(IPEndPoint endPoint)
            : this(endPoint, new T())
        { }
        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="address">The target IP this socket should be bound to</param>
        /// <param name="port">The target port number this socket should be bound to</param>
        public SocketBase(IPAddress address, int port)
            : this(new IPEndPoint(address, port), new T())
        { }
        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="address">The target IP this socket should be bound to</param>
        /// <param name="port">The target port number this socket should be bound to</param>
        public SocketBase(string address, int port) 
            : this(IPAddress.Parse(address), port) 
        { }
        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="port">A system port number this socket should be created on</param>
        public SocketBase(int port)
            : this(IPAddress.IPv6Any, port)
        {
            options.FixedPortUse = true;
        }
        ~SocketBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Opens a new peer on the underlaying network layer
        /// </summary>
        /// <returns>True if the peer was opened successfully, false otherwise</returns>
        public virtual bool Open()
        {
            if (initialized)
                return false;

            if (receiveBuffer == null)
            {
                receiveBuffer = new byte[options.ReceiveBufferSize];
            }
            else Array.Resize(ref receiveBuffer, options.ReceiveBufferSize);
            if (sendBuffer == null)
            {
                sendBuffer = new byte[options.SendBufferSize];
            }
            else Array.Resize(ref sendBuffer, options.SendBufferSize);

            asyncReceiveContext = new SocketAsyncEventArgs();
            asyncReceiveContext.Completed += HandleIocpEvent;
            asyncSendContext = new SocketAsyncEventArgs();
            asyncSendContext.Completed += HandleIocpEvent;

            socket = new System.Net.Sockets.Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            OnCreated();

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, options.ReuseAddress);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, options.ExclusiveAddressUse);
            socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, !options.DualMode);
            if (!options.FixedAddressUse)
            {
                if (options.FixedPortUse)
                {
                    socket.Bind(new IPEndPoint(endPoint.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddress.IPv6Any : IPAddress.Any, endPoint.Port));
                }
                else socket.Bind(endPoint.AddressFamily == AddressFamily.InterNetworkV6 ? IPv6Any : IPv4Any);
            }
            else socket.Bind(endPoint);

            initialized = true;

            OnInitialize();
            return true;
        }

        /// <summary>
        /// Closes the existing peer on the underlaying network layer
        /// </summary>
        /// <returns>True if the peer was closed successfully, false otherwise</returns>
        public virtual bool Close()
        {
            if (!initialized)
                return false;

            OnClosing();

            initialized = false;
            
            asyncReceiveContext.Completed -= HandleIocpEvent;
            asyncSendContext.Completed -= HandleIocpEvent;
            try
            {
                socket.Close();
                socket.Dispose();
                asyncReceiveContext.Dispose();
                asyncSendContext.Dispose();
            }
            catch (ObjectDisposedException) { }

            receiving = false;
            sending = false;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Close();
                }
                disposed = true;
            }
        }
    }
}
