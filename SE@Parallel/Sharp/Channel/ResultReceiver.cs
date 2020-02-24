// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Promise object able to await the result of a parallel processing task
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultReceiver<T> : IReceiver, IDisposable
    {
        protected ConditionVariable signal;

        protected T value;
        /// <summary>
        /// Result value obtained from the Adapter
        /// </summary>
        public T Value
        {
            get { return value; }
        }

        protected Exception error;
        /// <summary>
        /// An object containing failure information
        /// </summary>
        public Exception Error
        {
            get { return error; }
        }

        public ResultReceiver()
        {
            this.signal = new ConditionVariable();
        }

        /// <summary>
        /// Sets calling thread into wait mode and returns when task
        /// result was obtained
        /// </summary>
        /// <param name="result">Result value of successfull</param>
        /// <returns>True if the task was executed successfully, false otherwise</returns>
        public bool GetResult(out T result)
        {
            signal.Await();
            result = value;

            return (error == null);
        }

        /// <summary>
        /// Sets the result value to this promise and wakes waiting threads
        /// if needed
        /// </summary>
        /// <param name="result">The result obtained from execution</param>
        public void SetResult(object host, object result)
        {
            value = (T)result;
            signal.Set();
        }
        /// <summary>
        /// Sets a failure context object to this promise and wakes waiting threads
        /// if needed
        /// </summary>
        /// <param name="result">The failure context object obtained from execution</param>
        public void SetError(object host, Exception error)
        {
            this.error = error;
            signal.Set();
        }

        public void Dispose()
        {
            signal.Set();
        }
    }
}
