// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using SE;

namespace SE.Remoting.Tcp
{
    /// <summary>
    /// A base class for TCP driven operations on the udnerlaying network layer
    /// </summary>
    public abstract partial class SocketBase<T> : IDisposable where T : SocketOptions, new()
    {
        protected readonly static IPEndPoint IPv4Any = new IPEndPoint(IPAddress.Any, 0);
        protected readonly static IPEndPoint IPv6Any = new IPEndPoint(IPAddress.IPv6Any, 0);

        protected System.Net.Sockets.Socket socket;

        protected IPEndPoint endPoint;
        /// <summary>
        /// The target this socket was bound to
        /// </summary>
        public IPEndPoint EndPoint
        {
            get { return endPoint; }
        }

        protected bool initialized;
        /// <summary>
        /// Determines if this socket has been initialized
        /// </summary>
        public bool Initialized
        {
            get { return initialized; }
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
        ~SocketBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Closes the existing peer on the underlaying network layer
        /// </summary>
        /// <returns>True if the peer was closed successfully, false otherwise</returns>
        public abstract bool Close();

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
