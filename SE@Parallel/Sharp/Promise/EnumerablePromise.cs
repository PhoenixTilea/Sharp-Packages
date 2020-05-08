// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    /// <summary>
    /// An awaitable to an asynchronous vectoring operation
    /// </summary>
    public class EnumerablePromise<T> : Promise<T>
    {
        atomic_int count;

        public int Count
        {
            get { return count.Value; }
        }

        public override T Result
        {
            get { return default(T); }
        }

        public EnumerablePromise()
        {
            this.lastError = null;
            this.state = 1;
        }

        internal void Increment()
        {
            state.Increment();
        }

        public override void OnResolve(T value)
        {
            count.Increment();
            OnResolve();
        }
        internal void OnResolve()
        {
            if (state.Decrement() == 0)
                Propagate();
        }

        public override void OnReject(Exception error)
        {
            lastError.Exchange(error);
            if (state.Exchange(0) != 0)
                Propagate();
        }
    }
}
