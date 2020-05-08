// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE
{
    /// <summary>
    /// Default generic object policy
    /// </summary>
    public class StackPolicy<TValue> : IPoolingPolicy<Stack<TValue>>
    {
        /// <summary>
        /// A default generic pooling policy instance
        /// </summary>
        public readonly static StackPolicy<TValue> Default = new StackPolicy<TValue>();

        /// <summary>
        /// Creates a new pooling policy instance
        /// </summary>
        public StackPolicy()
        { }

        public Stack<TValue> Create()
        {
            return new Stack<TValue>();
        }
        public bool Return(Stack<TValue> instance)
        {
            instance.Clear();
            return true;
        }
        public bool Delete(Stack<TValue> instance)
        {
            instance.Clear();

            IDisposable disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            return true;
        }
    }
}