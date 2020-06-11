// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using SE;

namespace SE.Remoting.Udp
{
    public partial class SocketBase
    {
        /// <summary>
        /// Passes this thread to the underlaying network layer until data was received
        /// </summary>
        /// <param name="endPoint">The sender that has sent binary data</param>
        /// <param name="buffer">An array to copy binary data to</param>
        /// <param name="offset">An offset of which the array should be shifted</param>
        /// <param name="size">The capacity of bytes to be read</param>
        /// <returns>The amount of bytes that have been read into the array</returns>
        public virtual int Receive(ref EndPoint endPoint, byte[] buffer, int offset, int size)
        {
            if (!initialized || size == 0)
            {
                return 0;
            }
            try
            {
                int count = socket.ReceiveFrom(buffer, offset, size, SocketFlags.None, ref endPoint);
                if (count > 0)
                {
                    stats.Received++;
                    stats.BytesReceived += count;

                    OnReceive(endPoint as IPEndPoint, buffer, offset, count);
                }
                return count;
            }
            catch (SocketException er)
            {
                HandleIocpError(endPoint, er.SocketErrorCode);
                return 0;
            }
            catch (ObjectDisposedException) 
            { 
                return 0; 
            }
        }
        /// <summary>
        /// Passes this thread to the underlaying network layer until data was received
        /// </summary>
        /// <param name="endPoint">The sender that has sent binary data</param>
        /// <param name="buffer">An array to copy binary data to</param>
        /// <returns>The amount of bytes that have been read into the array</returns>
        public int Receive(ref EndPoint endPoint, byte[] buffer)
        {
            return Receive(ref endPoint, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Starts an asynchronous receive operation to the udnerlaying network layer
        /// </summary>
        /// <returns>True if the operation was scheduled successfully, false otherwise</returns>
        public virtual bool ReceiveAsync()
        {
            if (!initialized || receiving.CompareExchange(true, false))
            {
                return false;
            }
            try
            {
                asyncReceiveContext.RemoteEndPoint = endPoint;
                asyncReceiveContext.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
                if (!socket.ReceiveFromAsync(asyncReceiveContext))
                {
                    OnAsyncReceiveCompleted(asyncReceiveContext);
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
