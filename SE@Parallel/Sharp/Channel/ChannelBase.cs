// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Routes incomming tasks to attached Adapter
    /// </summary>
    public abstract class ChannelBase : IDisposable
    {
        protected IDispatcher dispatcher;

        /// <summary>
        /// Ammount of attached Adapter instances
        /// </summary>
        public int Count
        {
            get { return dispatcher.Count; }
        }

        /// <summary>
        /// Creates a new channel with defined dispatching strategy
        /// </summary>
        public ChannelBase(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            (dispatcher as IDispatcherInternal).Owner = this;
        }

        /// <summary>
        /// Creates a channel specific adapter execution context
        /// </summary>
        /// <param name="action">A function pointer for execution</param>
        /// <param name="args">An array of function arguments</param>
        /// <param name="sender">The recipient of execution results</param>
        /// <returns>A channel specific context</returns>
        public abstract AdapterContext GetContext(object action, object[] args, IReceiver sender);

        /// <summary>
        /// Stops an Adapter from listening to this Channel
        /// </summary>
        /// <param name="adapter">The Adapter to stop listen to this Channel</param>
        public void Remove(Adapter adapter)
        {
            dispatcher.Remove(adapter);
        }

        /// <summary>
        /// Stops all Adapters from listening to this Channel
        /// </summary>
        public void Clear()
        {
            dispatcher.Clear();
        }

        public virtual void Dispose()
        {
            dispatcher.Dispose();
        }
    }
}
