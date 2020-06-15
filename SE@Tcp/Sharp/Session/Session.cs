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
    /// A connector class between the provided TCP host socket and a remote end point
    /// </summary>
    public partial class Session<T> : IDisposable where T : HostSocketOptions, new()
    {
        byte[] receiveBuffer;
        byte[] sendBuffer;

        SocketAsyncEventArgs asyncReceiveContext;
        SocketAsyncEventArgs asyncSendContext;

        HostSocket<T> host;
        /// <summary>
        /// The host socket this session is associated with
        /// </summary>
        public HostSocket<T> Host
        {
            get { return host; }
        }

        System.Net.Sockets.Socket socket;
        /// <summary>
        /// Provides access to the udnerlaying raw socket
        /// </summary>
        protected System.Net.Sockets.Socket RawSocket
        {
            get { return socket; }
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

        protected SocketBase<T>.SocketInfo stats;
        /// <summary>
        /// Provides statistics information about this socket
        /// </summary>
        public SocketBase<T>.SocketInfo Stats
        {
            get { return stats; }
        }

        /// <summary>
        /// Initializes a new session between the provided sockets
        /// </summary>
        public Session(HostSocket<T> host, System.Net.Sockets.Socket remoteTarget)
        {
            this.host = host;
            this.socket = remoteTarget;

            if (receiveBuffer == null)
            {
                this.receiveBuffer = new byte[host.Options.ReceiveBufferSize];
            }
            else Array.Resize(ref receiveBuffer, host.Options.ReceiveBufferSize);
            if (sendBuffer == null)
            {
                this.sendBuffer = new byte[host.Options.SendBufferSize];
            }
            else Array.Resize(ref sendBuffer, host.Options.SendBufferSize);

            this.asyncReceiveContext = new SocketAsyncEventArgs();
            this.asyncReceiveContext.Completed += HandleIocpEvent;
            this.asyncSendContext = new SocketAsyncEventArgs();
            this.asyncSendContext.Completed += HandleIocpEvent;

            this.initialized = true;
        }
        ~Session()
        {
            Dispose(false);
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
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException er) 
                {
                    host.OnError(this, er.SocketErrorCode);
                }

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
