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
        /// <summary>
        /// Synchronously awaits a remote end to request a connection
        /// </summary>
        /// <returns>True if a connection has been established, false otherwise</returns>
        public virtual bool Accept()
        {
            if (!initialized)
            {
                return false;
            }
            try
            {
                Socket remoteEnd = socket.Accept();
                if (!OnAccept(remoteEnd))
                {
                    OnReject(remoteEnd);
                    return false;
                }
                else return true;
            }
            catch (SocketException er)
            {
                HandleIocpError(er.SocketErrorCode);
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// Synchronously awaits a remote end to request a connection
        /// </summary>
        /// <returns>True if the operation has been scheduled, false otherwise</returns>
        public virtual bool AcceptAsync()
        {
            if (!initialized || accepting.CompareExchange(true, false))
            {
                return false;
            }
            try
            {
                asyncAcceptContext.AcceptSocket = null;
                if (!socket.AcceptAsync(asyncAcceptContext))
                {
                    OnAsyncAcceptCompleted(asyncAcceptContext);
                }
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
    }
}