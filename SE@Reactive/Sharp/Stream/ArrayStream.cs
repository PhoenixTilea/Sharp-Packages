// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a collection into a push-based notification stream
    /// </summary>
    public struct ArrayStream<T> : IReactiveStream<T>
    {
        T[] data;

        /// <summary>
        /// Creates a wrapper instance around the passed collection
        /// </summary>
        public ArrayStream(T[] data)
        {
            this.data = data;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            try
            {
                for (int i = 0; i < data.Length; i++)
                    observer.OnNext(data[i]);

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
