// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a collection into a push-based notification stream
    /// </summary>
    public struct AnonymousStream<T> : IReactiveStream<T>
    {
        Func<IObserver<T>, IDisposable> action;

        /// <summary>
        /// Creates a wrapper instance around the passed collection
        /// </summary>
        public AnonymousStream(Func<IObserver<T>, IDisposable> action)
        {
            this.action = action;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            try
            {
                return action(observer);
            }
            catch (Exception er)
            {
                observer.OnError(er);
            }
            return VoidDisposer.Instance;
        }
    }
}