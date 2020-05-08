// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    /// <summary>
    /// A simple asynchronous operation awaitable
    /// </summary>
    public class CompletionPromise<T> : Promise<T>
    {
        T result;
        public override T Result
        {
            get { return default(T); }
        }

        public CompletionPromise()
        {
            this.procedure = null;
            this.lastError = null;
            this.state = 1;
        }

        public override void OnResolve(T value)
        {
            result = value;
            if (state.Exchange(0) != 0)
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