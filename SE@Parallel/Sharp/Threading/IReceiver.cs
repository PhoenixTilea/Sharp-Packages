// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    /// <summary>
    /// Parallel Processing Result Receiver Interface
    /// </summary>
    public interface IReceiver
    {
        /// <summary>
        /// Sets the completion state after successfully execution
        /// </summary>
        /// <param name="host">Host object to set the result to</param>
        /// <param name="result">Result object as state of success</param>
        void SetResult(object host, object result);
        /// <summary>
        /// Sets the failure state after an exception occurred
        /// </summary>
        /// <param name="host">Host object to set the result to</param>
        /// <param name="error">An exception to provide further failure information</param>
        void SetError(object host, Exception error);
    }
}
