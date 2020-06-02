// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a value into a push-based notification stream
    /// </summary>
    public class PropertyStream<T> : ReactiveStream<T>
    {
        T propertyValue;
        /// <summary>
        /// Gets or sets this streams value
        /// </summary>
        public T Value
        {
            get { return propertyValue; }
            set
            {
                if (!propertyValue.Equals(value))
                {
                    propertyValue = value;
                    Push(value);
                }
            }
        }

        /// <summary>
        /// Creates a new push-based notification stream with the given value
        /// </summary>
        public PropertyStream(T value)
        {
            this.propertyValue = value;
        }

        public override IDisposable Subscribe(IObserver<T> observer)
        {
            IDisposable result = base.Subscribe(observer);
            try
            {
                observer.OnNext(propertyValue);
            }
            catch (Exception er)
            {
                observer.OnError(er);
            }
            return result;
        }
    }
}
