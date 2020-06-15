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
    /// A class for TCP driven operations on the udnerlaying network layer
    /// </summary>
    public partial class Socket<T> : SocketBase<T> where T : SocketOptions, new()
    {
        byte[] receiveBuffer;
        byte[] sendBuffer;

        SocketAsyncEventArgs asyncReceiveContext;
        SocketAsyncEventArgs asyncSendContext;

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

        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="endPoint">The target this socket should be bound to</param>
        public Socket(IPEndPoint endPoint)
            : base(endPoint, new T())
        { }
        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="address">The target IP this socket should be bound to</param>
        /// <param name="port">The target port number this socket should be bound to</param>
        public Socket(IPAddress address, int port)
            : base(new IPEndPoint(address, port))
        { }
        /// <summary>
        /// Creates a new socket instance 
        /// </summary>
        /// <param name="address">The target IP this socket should be bound to</param>
        /// <param name="port">The target port number this socket should be bound to</param>
        public Socket(string address, int port) 
            : base(IPAddress.Parse(address), port) 
        { }

        /// <summary>
        /// Opens a new peer on the underlaying network layer
        /// </summary>
        /// <returns>True if the peer was opened successfully, false otherwise</returns>
        public virtual bool Connect()
        {
            if (initialized)
                return false;

            if (receiveBuffer == null)
            {
                receiveBuffer = new byte[Options.ReceiveBufferSize];
            }
            else Array.Resize(ref receiveBuffer, Options.ReceiveBufferSize);
            if (sendBuffer == null)
            {
                sendBuffer = new byte[Options.SendBufferSize];
            }
            else Array.Resize(ref sendBuffer, Options.SendBufferSize);

            asyncReceiveContext = new SocketAsyncEventArgs();
            asyncReceiveContext.Completed += HandleIocpEvent;
            asyncSendContext = new SocketAsyncEventArgs();
            asyncSendContext.Completed += HandleIocpEvent;

            socket = new System.Net.Sockets.Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            OnCreated();

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, Options.ReuseAddress);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, Options.ExclusiveAddressUse);
            socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, !Options.DualMode);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, Options.KeepAlive);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, Options.NoDelay);

            try
            {
                socket.Connect(endPoint);
            }
            catch (SocketException er)
            {
                OnError(er.SocketErrorCode);
                Close();
                
                return false;
            }

            initialized = true;

            OnInitialize();
            return true;
        }

        /// <summary>
        /// Closes the existing peer on the underlaying network layer
        /// </summary>
        /// <returns>True if the peer was closed successfully, false otherwise</returns>
        public override bool Close()
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
                    OnError(er.SocketErrorCode);
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

        /// <summary>
        /// Resets the peer on the underlaying network layer and tries to reopen it
        /// </summary>
        /// <returns>True if the peer was opened successfully, false otherwise</returns>
        public virtual bool Reset()
        {
            Close();
            return Connect();
        }
    }
}
