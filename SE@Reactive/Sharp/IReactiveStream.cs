// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Defines a provider for push-based notification
    /// </summary>
    public interface IReactiveStream<T> : IObservable<T>
    { }
}
