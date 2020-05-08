// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// A context class that manages parallel execution
    /// </summary>
    [Serializable]
    public abstract class AdapterContext : IDisposable
    {
        public readonly static EmptyNotifier DefaultNotifier;

        private static Dictionary<Int32, AdapterContext> executionScopes;
        private static ReadWriteLock contextLock;

        public static AdapterContext Current
        {
            get
            {
                AdapterContext tmp;
                using (ThreadContext.ReadLock(contextLock))
                    executionScopes.TryGetValue(ThreadContext.LocalId, out tmp);

                return tmp;
            }
            private set
            {
                Int32 scope = ThreadContext.LocalId;
                using (ThreadContext.WriteLock(contextLock))
                {
                    if (executionScopes.ContainsKey(scope)) executionScopes[scope] = value;
                    else executionScopes.Add(scope, value);
                }
            }
        }

        protected atomic_int executionSignal;
        protected IPromiseNotifier<object> sender;
        /// <summary>
        /// The receiver to response execution result to
        /// </summary>
        public IPromiseNotifier<object> Sender
        {
            get { return sender; }
        }

        protected object[] args;
        /// <summary>
        /// An optional set of arguments to pass into the function
        /// </summary>
        public object[] Args
        {
            get { return args; }
        }

        static AdapterContext()
        {
            DefaultNotifier = new EmptyNotifier();
            executionScopes = new Dictionary<Int32, AdapterContext>(64);
            contextLock = new ReadWriteLock();
        }

        /// <summary>
        /// Wraps a set of arguments along with a result receiver into an executable context
        /// </summary>
        /// <param name="args">An optional set of arguments to pass into the function</param>
        /// <param name="sender">The receiver to response results to</param>
        public AdapterContext(object[] args, IPromiseNotifier<object> sender)
        {
            this.args = args;
            this.sender = sender;
        }
        /// <summary>
        /// Wraps a set of arguments along with a result receiver into an executable context
        /// </summary>
        /// <param name="sender">The receiver to response results to</param>
        public AdapterContext(IPromiseNotifier<object> sender)
            : this(new object[0], sender)
        { }

        /// <summary>
        /// Starts execution of a function context
        /// </summary>
        public void Execute(object _unused = null)
        {
            AdapterContext parentContext = Current;
            if (executionSignal.CompareExchange(1, 0) == 0)
                try
                {
                    Current = this;
                    ExecuteEmbedded();
                }
                catch (Exception er)
                {
                    if (er.InnerException != null)
                    {
                        //er.InnerException.Data.Add(sender, this);
                        sender.OnReject(er.InnerException);
                    }
                    else
                    {
                        //er.Data.Add(sender, this);
                        sender.OnReject(er);
                    }
                }
            Current = parentContext;
        }
        /// <summary>
        /// Executes an embedded function
        /// </summary>
        public abstract void ExecuteEmbedded();

        /// <summary>
        /// Exchanges this contexts sender for another one
        /// </summary>
        /// <returns>The sender previously attached to this context</returns>
        public IPromiseNotifier<object> ExchangeSender(IPromiseNotifier<object> other)
        {
            IPromiseNotifier<object> result = sender;
            sender = other;
            return result;
        }

        public virtual void Dispose()
        {
            if (executionSignal.CompareExchange(2, 0) == 0)
            {
                Exception er = new ObjectDisposedException(GetType().FullName);
                //er.InnerException.Data.Add(sender, this);
                sender.OnReject(er);
            }
        }
    }
}
