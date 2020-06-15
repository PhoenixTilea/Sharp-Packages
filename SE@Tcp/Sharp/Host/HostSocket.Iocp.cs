// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SE;

namespace SE.Remoting.Tcp
{
    public partial class HostSocket<T> where T : HostSocketOptions, new()
    {
        void HandleIocpEvent(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    {
                        OnAsyncAcceptCompleted(e);
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
                        OnError(null, error);
                    }
                    break;
            }
        }

        /// <summary>
        /// Occures whenever an asynchronous accept operation has returned
        /// </summary>
        protected virtual void OnAsyncAcceptCompleted(SocketAsyncEventArgs e)
        {
            accepting.Exchange(false);
            if (!initialized)
                return;

            if (e.SocketError == SocketError.Success)
            {
                if (!OnAccept(e.AcceptSocket))
                     OnReject(e.AcceptSocket);
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
        /// Occures whenever an accept operation successfully returnes
        /// </summary>
        /// <param name="remoteTarget">The remote end point requesting a connection</param>
        protected virtual bool OnAccept(System.Net.Sockets.Socket remoteTarget)
        {
            OnSessionCreated(new Session<T>(this, remoteTarget));
            return true;
        }

        /// <summary>
        /// Occures whenever a new session is created
        /// </summary>
        /// <param name="session">A session object hosted by this socket</param>
        protected virtual void OnSessionCreated(Session<T> session)
        { }

        /// <summary>
        /// Occures whenever an accept operation was rejected
        /// </summary>
        /// <param name="remoteTarget">The remote end point requesting a connection</param>
        protected virtual void OnReject(System.Net.Sockets.Socket remoteTarget)
        { }

        /// <summary>
        /// Occures whenever a send operation successfully returnes
        /// </summary>
        protected internal virtual void OnError(Session<T> session, SocketError error)
        { }
    }
}
