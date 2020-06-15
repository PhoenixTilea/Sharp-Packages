// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using SE;

namespace SE.Remoting.Tcp
{
    public partial class Session<T> where T : HostSocketOptions, new()
    {
        /// <summary>
        /// Passes this thread to the underlaying network layer until data was received
        /// </summary>
        /// <param name="buffer">An array to copy binary data to</param>
        /// <param name="offset">An offset of which the array should be shifted</param>
        /// <param name="size">The capacity of bytes to be read</param>
        /// <returns>The amount of bytes that have been read into the array</returns>
        public virtual int Receive(byte[] buffer, int offset, int size)
        {
            if (!initialized || size == 0)
            {
                return 0;
            }
            try
            {
                int count = socket.Receive(buffer, offset, size, SocketFlags.None);
                if (count > 0)
                {
                    stats.Received++;
                    stats.BytesReceived += count;

                    Interlocked.Increment(ref host.stats.Received);
                    Interlocked.Add(ref host.stats.BytesReceived, size);

                    OnReceive(buffer, offset, count);
                }
                return count;
            }
            catch (SocketException er)
            {
                HandleIocpError(er.SocketErrorCode);
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
        /// <param name="buffer">An array to copy binary data to</param>
        /// <returns>The amount of bytes that have been read into the array</returns>
        public int Receive(byte[] buffer)
        {
            return Receive(buffer, 0, buffer.Length);
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
                asyncReceiveContext.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
                if (!socket.ReceiveAsync(asyncReceiveContext))
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
