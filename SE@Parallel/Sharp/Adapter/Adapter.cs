// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// An execution end-point receiving tasks to execute a defined way
    /// </summary>
    public class Adapter : IDisposable
    {
        protected List<ChannelBase> subscriptions;
        protected Behavior behavior;

        /// <summary>
        /// Indicates if this Adapter is enabled and ready for processing
        /// </summary>
        public bool Enabled
        {
            get { return behavior.Enabled; }
        }
        /// <summary>
        /// Indicates if this Adapter is enabled and ready to receive tasks
        /// </summary>
        public bool AcceptsMessage
        {
            get
            {
                AdapterState tmp = behavior.State;
                return (tmp == AdapterState.Ready ||
                        tmp == AdapterState.Suspended);
            }
        }
        /// <summary>
        /// Indicates an error state that wasn't handled by a result receiver
        /// </summary>
        public bool HasError
        {
            get { return (behavior.State == AdapterState.Error); }
        }

        /// <summary>
        /// Creates a new Adapter under defined behavior
        /// </summary>
        public Adapter(Behavior behavior)
        {
            this.subscriptions = new List<ChannelBase>();
            this.behavior = behavior;
        }

        /// <summary>
        /// Starts this Adapter listening to certain channel
        /// </summary>
        /// <param name="channel">A channel to receive tasks from</param>
        public void Register(ChannelBase channel)
        {
            lock (subscriptions)
                if(subscriptions != null)
                    subscriptions.Add(channel);
        }
        /// <summary>
        /// Stops this Adapter listening to certain channel
        /// </summary>
        /// <param name="channel">A channel to stop receive tasks from</param>
        public void Remove(ChannelBase channel)
        {
            lock (subscriptions)
                if (subscriptions != null)
                    subscriptions.Remove(channel);
        }

        /// <summary>
        /// Initializes the Adapter and it's behavior
        /// </summary>
        public virtual void Initialize()
        {
            behavior.Initialize();
        }
        /// <summary>
        /// Swaps an already initialized Adapter behavior
        /// </summary>
        /// <param name="exchange">The behavior to store in this Adapter</param>
        /// <returns>The behaior taken from this Adapter</returns>
        public Behavior Swap(Behavior exchange)
        {
            Behavior tmp = behavior;
            behavior = exchange;

            return tmp;
        }

        /// <summary>
        /// Stores a task in this Adapter to be processed
        /// </summary>
        /// <param name="context">The execution context to store</param>
        /// <returns>True if the task was successfully received, false otherwise</returns>
        public virtual bool Enqueue(AdapterContext context)
        {
            return behavior.Enqueue(context);
        }

        public void Dispose()
        {
            if(subscriptions != null)
                lock (subscriptions)
                {
                    ICollection<ChannelBase> subs = subscriptions.Clone();
                    foreach (ChannelBase channel in subs)
                        channel.Remove(this);

                    subscriptions = null;
                }
            behavior.Dispose();
        }
    }
}
