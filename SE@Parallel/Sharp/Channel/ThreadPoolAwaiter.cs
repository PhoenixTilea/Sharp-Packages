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
    public class ThreadPoolAwaiter : IReceiver, IDisposable
    {
        protected atomic_int counter;
        /// <summary>
        /// Determines if all tasks have completed
        /// </summary>
        public bool Finished
        {
            get { return (counter.Value == 0); }
        }

        protected Exception error;
        /// <summary>
        /// An object containing failure information
        /// </summary>
        public Exception Error
        {
            get { return error; }
        }

        public ThreadPoolAwaiter()
        { }

        /// <summary>
        /// Increments the ammount of tasks this promise awaits for completion
        /// </summary>
        public void Increment()
        {
            counter.Increment();
        }

        /// <summary>
        /// Sets calling thread into wait mode and returns when tasks have finished
        /// </summary>
        /// <returns>True if all tasks executed successfully, false otherwise</returns>
        public bool Await()
        {
            while (error == null && counter.Value > 0)
                ThreadScheduler.Join();

            return (error == null);
        }

        /// <summary>
        /// Decrements runtime counter and wakes waiting threads
        /// if needed
        /// </summary>
        public void SetResult(object host, object result)
        {
            counter.Decrement();
        }
        /// <summary>
        /// Sets a failure context object to this promise and wakes waiting threads
        /// if needed
        /// </summary>
        /// <param name="result">The failure context object obtained from execution</param>
        public void SetError(object host, Exception error)
        {
            this.error = error;
            counter.Exchange(0);
        }

        public void Dispose()
        {
            counter.Exchange(0);
        }
    }
}
