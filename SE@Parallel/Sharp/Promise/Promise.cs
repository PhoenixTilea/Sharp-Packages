// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel
{
    /// <summary>
    /// A single observer asynchronous operation result provider
    /// </summary>
    public abstract class Promise<T> : IPromise<T>
    {
        /// <summary>
        /// A collection of compound action delegates to be executed on result
        /// </summary>
        protected atomic_reference<List<Action<IPromise<T>>>> procedure;
        /// <summary>
        /// A lock object to protect the collection of compound action delegates
        /// </summary>
        protected Spinlock procedureLock;

        /// <summary>
        /// An error object referenced in the result
        /// </summary>
        protected atomic_reference<Exception> lastError;

        /// <summary>
        /// The last error occurred while operating
        /// </summary>
        public Exception Error
        {
            get { return lastError.Value; }
        }
        /// <summary>
        /// The operation result if existing
        /// </summary>
        public abstract T Result { get; }

        /// <summary>
        /// The internal state of the result provider
        /// </summary>
        protected atomic_int state;
        public PromiseState State
        {
            get
            {
                if (lastError.Value != null)
                {
                    return PromiseState.Rejected;
                }
                else if (state.Value == 0)
                {
                    return PromiseState.Resolved;
                }
                else return PromiseState.Pending;
            }
            protected set { state.Value = (int)value; }
        }

        /// <summary>
        /// Creates a new provider instance
        /// </summary>
        public Promise()
        {
            this.procedureLock = new Spinlock();
        }

        public IPromise<T> Then(Action<T> resolve, Action<Exception> reject)
        {
            return Then((promise) =>
            {
                Exception error = promise.Error;
                if (error == null)
                {
                    try
                    {
                        if (resolve != null)
                            resolve(promise.Result);

                        return;
                    }
                    catch (Exception er)
                    {
                        lastError.Exchange(er);
                        error = er;
                    }
                }
                if (reject != null)
                    try
                    {
                        reject(error);
                    }
                    catch (Exception er)
                    {
                        lastError.Exchange(er);
                    }

                });
        }
        public virtual IPromise<T> Then(Action<IPromise<T>> action)
        {
            bool added = false;
            switch (State)
            {
                case PromiseState.Resolved:
                case PromiseState.Rejected:
                    {
                        if (!Propagate() && !added)
                            action(this);
                    }
                    break;
                case PromiseState.Pending:
                    {
                        using (ThreadContext.Lock(procedureLock))
                            if (State == PromiseState.Pending)
                            {
                                if (procedure.UnsafeValue == null)
                                    procedure.Exchange(CollectionPool<List<Action<IPromise<T>>>, Action<IPromise<T>>>.Get());

                                procedure.UnsafeValue.Add(action);
                                added = true;

                                if (State == PromiseState.Pending)
                                    break;
                            }
                    }
                    goto case PromiseState.Resolved;
            }
            return this;
        }

        public abstract void OnResolve(T value);
        public IPromise<T> Resolve(Action<T> action)
        {
            return Then(action, null);
        }

        public abstract void OnReject(Exception error);
        public IPromise<T> Reject(Action<Exception> action)
        {
            return Then(null, action);
        }

        /// <summary>
        /// Propagates the operation result to all observers in order
        /// </summary>
        /// <returns>
        /// True if propagation is called and so executed for the first time, 
        /// false otherwise
        /// </returns>
        protected bool Propagate()
        {
            if (procedure.Value != null)
            {
                using (ThreadContext.Lock(procedureLock))
                    if (procedure.UnsafeValue != null)
                    {
                        foreach (Action<IPromise<T>> storedAction in procedure.UnsafeValue)
                            storedAction(this);

                        CollectionPool<List<Action<IPromise<T>>>, Action<IPromise<T>>>.Return(procedure);
                        procedure.Exchange(null);

                        return true;
                    }
            }
            return false;
        }
    }
}
