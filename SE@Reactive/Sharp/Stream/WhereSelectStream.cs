// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a collection into a push-based notification stream
    /// </summary>
    public struct WhereSelectStream<TSource, TResult> : IReactiveStream<TResult>
    {
        struct WhereSelectConnector : IReceiver<TSource>
        {
            readonly Func<TSource, TResult> selector;
            readonly IObserver<TResult> receiver;

            public WhereSelectConnector(IObserver<TResult> receiver, Func<TSource, TResult> selector)
            {
                this.selector = selector;
                this.receiver = receiver;
            }

            public void OnNext(TSource value)
            {
                try
                {
                    receiver.OnNext(selector(value));
                }
                catch (Exception er)
                {
                    receiver.OnError(er);
                }
            }
            public void OnError(Exception error)
            {
                receiver.OnError(error);
            }
            public void OnCompleted()
            {
                receiver.OnCompleted();
            }
        }

        readonly IReactiveStream<TSource> stream;
        readonly Func<TSource, TResult> selector;

        /// <summary>
        /// Creates a wrapper instance around the passed collection
        /// </summary>
        public WhereSelectStream(IReactiveStream<TSource> stream, Func<TSource, TResult> selector)
        {
            this.stream = stream;
            this.selector = selector;
        }

        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            return stream.Subscribe(new WhereSelectConnector(observer, selector));
        }
    }
}