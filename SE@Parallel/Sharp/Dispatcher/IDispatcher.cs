// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// The basic interface of channel dispatching behavior
    /// </summary>
    public interface IDispatcher : IDisposable
    {
        /// <summary>
        /// Get the number of subscribers registered
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Registeres a new subscriber based on the channel
        /// </summary>
        /// <param name="adapter">Defines the execution behavior</param>
        /// <param name="action">Defines the execution callback</param>
        /// <param name="options">Additional options that may be processed by the explicite implementation</param>
        void Register(Adapter adapter, object action, params object[] options);

        /// <summary>
        /// Removes a matching subscriber from the list of dispatching targets
        /// </summary>
        /// <param name="adapter">The subscriber to remove</param>
        /// <param name="action">The callback that should be removed only</param>
        void Remove(Adapter adapter, object action);
        /// <summary>
        /// Removes all matching subscribers from the list of dispatching targets
        /// </summary>
        /// <param name="adapter">The subscriber to remove</param>
        void Remove(Adapter adapter);

        /// <summary>
        /// Dispatches the payload based on the implementation dispatching strategy
        /// to registered subscribers
        /// </summary>
        /// <param name="sender">The dispatching issuer that also will receive the result, if any</param>
        /// <param name="args">The payload to dispatch</param>
        /// <returns>True if dispatching was successfull, false otherwise. The sender will potentially receive an error if available</returns>
        bool Dispatch(IReceiver sender, object[] args);

        /// <summary>
        /// Removes any subscriber from the list of dispatching targets
        /// </summary>
        void Clear();
    }
}
