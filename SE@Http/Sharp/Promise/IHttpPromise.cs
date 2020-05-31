// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE;

namespace SE.Storage.Http
{
    /// <summary>
    /// Enables http resources to act as notifiable provider
    /// </summary>
    public interface IHttpPromise
    {
        /// <summary>
        /// Notifies the provider that the operation has finished
        /// </summary>
        /// <param name="value">The new element in the sequence</param>
        void OnResolve(Stream value);

        /// <summary>
        /// Notifies the provider that an error condition has occurred
        /// </summary>
        /// <param name="exception">The exception that occurred</param>
        void OnReject(Exception error);
    }
}