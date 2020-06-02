// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Provides a mechanism for receiving push-based notifications
    /// </summary>
    public interface IReceiver<T> : IObserver<T>
    { }

    /// <summary>
    /// Provides a mechanism for receiving push-based notifications and returning a response
    /// </summary>
    public interface IReceiver<T, Result>
    {
        /// <summary>
        /// Provides the observer with new data
        /// </summary>
        /// <param name="value">The new element in the sequence</param>
        /// <returns>Result returned upon observation of a new element</returns>
        Result OnNext(T value);

        /// <summary>
        /// Notifies the observer that the provider has experienced an error condition
        /// </summary>
        /// <param name="error">The exception that occurred</param>
        /// <returns>Result returned upon observation of an error</returns>
        Result OnError(Exception error);

        /// <summary>
        /// Notifies the observer that the provider has finished sending push-based notifications
        /// </summary>
        /// <returns>Result returned upon observation of the sequence completion</returns>
        Result OnCompleted();
    }
}
