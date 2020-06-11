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
        /// Sends an amount of bytes from the provided array synchronously to the 
        /// provided target
        /// </summary>
        /// <param name="endPoint">A target that should receive the binary data</param>
        /// <param name="buffer">An array of binary data to send</param>
        /// <param name="offset">An offset of which the array should be shifted</param>
        /// <param name="size">The amount of bytes to send to the target</param>
        /// <returns>The amount of bytes that haven been sent to the target</returns>
        public virtual int Send(EndPoint endPoint, byte[] buffer, int offset, int size)
        {
            if (!initialized || size == 0)
            {
                return 0;
            }
            try
            {
                int count = socket.SendTo(buffer, (int)offset, (int)size, SocketFlags.None, endPoint);
                if (count > 0)
                {
                    stats.Sent++;
                    stats.BytesSent += count;

                    OnSend(endPoint, count);
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
        /// Sends an amount of bytes from the provided array synchronously to this 
        /// sockets EndPoint
        /// </summary>
        /// <param name="buffer">An array of binary data to send</param>
        /// <param name="offset">An offset of which the array should be shifted</param>
        /// <param name="size">The amount of bytes to send to the target</param>
        /// <returns>The amount of bytes that haven been sent to the target</returns>
        public int Send(byte[] buffer, int offset, int size)
        {
            return Send(endPoint, buffer, offset, size);
        }
        /// <summary>
        /// Sends an amount of bytes from the provided array synchronously to the 
        /// provided target
        /// </summary>
        /// <param name="endPoint">A target that should receive the binary data</param>
        /// <param name="buffer">An array of binary data to send</param>
        /// <returns>The amount of bytes that haven been sent to the target</returns>
        public int Send(EndPoint endPoint, byte[] buffer)
        {
            return Send(endPoint, buffer, 0, buffer.Length);
        }
        /// <summary>
        /// Sends an amount of bytes from the provided array synchronously to this 
        /// sockets EndPoint
        /// </summary>
        /// <param name="buffer">An array of binary data to send</param>
        /// <returns>The amount of bytes that haven been sent to the target</returns>
        public int Send(byte[] buffer)
        {
            return Send(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Sends an amount of bytes from the provided array asynchronously to the 
        /// provided target
        /// </summary>
        /// <param name="endPoint">A target that should receive the binary data</param>
        /// <param name="buffer">An array of binary data to send</param>
        /// <param name="offset">An offset of which the array should be shifted</param>
        /// <param name="size">The amount of bytes to send to the target</param>
        /// <returns>The amount of bytes that haven been sent to the target</returns>
        public virtual int SendAsync(EndPoint endPoint, byte[] buffer, int offset, int size)
        {
            if (!initialized || size == 0 || sending.CompareExchange(true, false))
            {
                return 0;
            }
            size = Math.Min(size, sendBuffer.Length);
            stats.BytesSending = size;
            try
            {
                Buffer.BlockCopy(buffer, offset, sendBuffer, 0, size);

                asyncSendContext.RemoteEndPoint = endPoint;
                asyncSendContext.SetBuffer(sendBuffer, 0, size);
                if (!socket.SendToAsync(asyncSendContext))
                {
                    OnAsyncSendCompleted(asyncSendContext);
                }
                return size;
            }
            catch (ObjectDisposedException) 
            {
                return 0;
            }
        }
        /// <summary>
        /// Sends an amount of bytes from the provided array asynchronously to this 
        /// sockets EndPoint
        /// </summary>
        /// <param name="buffer">An array of binary data to send</param>
        /// <param name="offset">An offset of which the array should be shifted</param>
        /// <param name="size">The amount of bytes to send to the target</param>
        /// <returns>The amount of bytes that haven been sent to the target</returns>
        public int SendAsync(byte[] buffer, int offset, int size)
        {
            return SendAsync(endPoint, buffer, offset, size);
        }
        /// <summary>
        /// Sends an amount of bytes from the provided array asynchronously to the 
        /// provided target
        /// </summary>
        /// <param name="endPoint">A target that should receive the binary data</param>
        /// <param name="buffer">An array of binary data to send</param>
        /// <returns>The amount of bytes that haven been sent to the target</returns>
        public int SendAsync(EndPoint endPoint, byte[] buffer)
        {
            return SendAsync(endPoint, buffer, 0, buffer.Length);
        }
        /// <summary>
        /// Sends an amount of bytes from the provided array asynchronously to this 
        /// sockets EndPoint
        /// </summary>
        /// <param name="buffer">An array of binary data to send</param>
        /// <returns>The amount of bytes that haven been sent to the target</returns>
        public int SendAsync(byte[] buffer)
        {
            return SendAsync(buffer, 0, buffer.Length);
        }
    }
}
