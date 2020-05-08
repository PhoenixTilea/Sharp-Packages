// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    /// <summary>
    /// Represents the eventual completion or failure of an asynchronous operation, and its resulting value
    /// </summary>
    public interface IPromise<T> : IPromiseNotifier<T>
    {
        /// <summary>
        /// The operation result if existing
        /// </summary>
        T Result { get; }

        /// <summary>
        /// The last error occurred while operating
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// A value indicating the state of the provider
        /// </summary>
        PromiseState State { get; }

        /// <summary>
        /// Attaches notifiers to the provider
        /// </summary>
        /// <param name="resolve">Notifies the observer that the provider has finished processing</param>
        /// <param name="reject">Notifies the observer that the provider has experienced an error condition</param>
        /// <returns>A promise object to bind to</returns>
        IPromise<T> Then(Action<T> resolve, Action<Exception> reject);
        /// <summary>
        /// Attaches notifiers to the provider
        /// </summary>
        /// <param name="action">Notifies the observer about the provider result</param>
        /// <returns>A promise object to bind to</returns>
        IPromise<T> Then(Action<IPromise<T>> action);

        /// <summary>
        /// Attaches a notifier to the provider on finished operating
        /// </summary>
        /// <param name="action">A callback to be executed</param>
        /// <returns>A promise object to bind to</returns>
        IPromise<T> Resolve(Action<T> action);

        /// <summary>
        /// Attaches a notifier to the provider on experiencing an error condition
        /// </summary>
        /// <param name="action">A callback to be executed</param>
        /// <returns>A promise object to bind to</returns>
        IPromise<T> Reject(Action<Exception> action);
    }
}
