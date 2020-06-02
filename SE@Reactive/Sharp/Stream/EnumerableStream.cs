// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a collection into a push-based notification stream
    /// </summary>
    public struct EnumerableStream<T> : IReactiveStream<T>
    {
        IEnumerable<T> data;

        /// <summary>
        /// Creates a wrapper instance around the passed collection
        /// </summary>
        public EnumerableStream(IEnumerable<T> data)
        {
            this.data = data;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            try
            {
                foreach (T item in data)
                    observer.OnNext(item);

                observer.OnCompleted();
            }
            catch (Exception er)
            {
                observer.OnError(er);
            }
            return VoidDisposer.Instance;
        }
    }
}
