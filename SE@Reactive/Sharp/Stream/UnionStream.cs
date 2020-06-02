// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a collection into a push-based notification stream
    /// </summary>
    public struct UnionStream<T> : IReactiveStream<T>
    {
        readonly IReactiveStream<T> first;
        readonly IReactiveStream<T> second;

        /// <summary>
        /// Creates a wrapper instance around the passed collection
        /// </summary>
        public UnionStream(IReactiveStream<T> first, IReactiveStream<T> second)
        {
            this.first = first;
            this.second = second;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            IDisposable[] result = new IDisposable[2];
            result[0] = first.Subscribe(observer);
            result[1] = second.Subscribe(observer);

            return new BatchDisposer(result);
        }
    }
}