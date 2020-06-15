// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using SE;

namespace SE.Remoting.Tcp
{
    public partial class Socket<T> where T : SocketOptions, new()
    {
        void HandleIocpEvent(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    {
                        OnAsyncReceiveCompleted(e);
                    }
                    break;
                case SocketAsyncOperation.Send:
                    {
                        OnAsyncSendCompleted(e);
                    }
                    break;
                default:
                    {
                        OnAsyncCompleted(e);
                    }
                    break;
            }
        }
        void HandleIocpError(SocketError error)
        {
            switch (error)
            {
                case SocketError.ConnectionAborted:
                case SocketError.ConnectionRefused:
                case SocketError.ConnectionReset:
                case SocketError.OperationAborted:
                case SocketError.Shutdown:
                    {
                        Close();
                    }
                    break;
                default:
                    {
                        OnError(error);
                    }
                    break;
            }
        }

        /// <summary>
        /// Occures whenever an asynchronous receive operation has returned
        /// </summary>
        protected virtual void OnAsyncReceiveCompleted(SocketAsyncEventArgs e)
        {
            receiving.Exchange(false);
            if (!initialized)
                return;

            if (e.SocketError == SocketError.Success)
            {
                int size = e.BytesTransferred;
                if (size > 0)
                {
                    stats.Received++;
                    stats.BytesReceived += size;
                    stats.Timestamp = Stopwatch.GetTimestamp();

                    OnReceive(receiveBuffer, 0, size);
                }
            }
            else HandleIocpError(e.SocketError);
        }
        /// <summary>
        /// Occures whenever an asynchronous send operation has returned
        /// </summary>
        protected virtual void OnAsyncSendCompleted(SocketAsyncEventArgs e)
        {
            sending.Exchange(false);
            if (!initialized)
                return;

            if (e.SocketError == SocketError.Success)
            {
                int size = e.BytesTransferred;
                if (size > 0)
                {
                    stats.BytesSending = 0;
                    stats.BytesSent += size;

                    OnSend(size);
                }
            }
            else HandleIocpError(e.SocketError);
        }
        /// <summary>
        /// Occures whenever an asynchronous operation returns that is neither
        /// receive nor send
        /// </summary>
        protected virtual void OnAsyncCompleted(SocketAsyncEventArgs e)
        { }

        /// <summary>
        /// A callback for whenever a peer has been opened in the udnerlaying
        /// network layer
        /// </summary>
        protected virtual void OnCreated()
        { }

        /// <summary>
        /// A callback for whenever this socket was initialized
        /// </summary>
        protected virtual void OnInitialize()
        { }

        /// <summary>
        /// A callback for whenever this socket is about to be closed
        /// </summary>
        protected virtual void OnClosing()
        { }

        /// <summary>
        /// A callback for whenever this socket has been closed
        /// </summary>
        protected virtual void OnClosed()
        { }

        /// <summary>
        /// Occures whenever a receive operation successfully returnes
        /// </summary>
        /// <param name="buffer">A binary data buffer</param>
        /// <param name="offset">An offset the binary buffer was shifted by</param>
        /// <param name="size">The amount of bytes that have been copied</param>
        protected virtual void OnReceive(byte[] buffer, int offset, int size)
        { }

        /// <summary>
        /// Occures whenever a send operation successfully returnes
        /// </summary>
        /// <param name="size">The amount of bytes that have been copied</param>
        protected virtual void OnSend(int size)
        { }

        /// <summary>
        /// Occures whenever a send operation successfully returnes
        /// </summary>
        protected virtual void OnError(SocketError error)
        { }
    }
}
