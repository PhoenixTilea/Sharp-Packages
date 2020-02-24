// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Dispatches incomming payload to registered subscribers
    /// </summary>
    /// <typeparam name="ListType">A container type to determine the dispatching order</typeparam>
    public abstract class Dispatcher<ListType> : IDispatcher, IDispatcherInternal where ListType : ICollection<Tuple<Adapter, object>>, new()
    {
        protected ReadWriteLock subscriberLock;
        protected ListType subscriber;

        protected ChannelBase owner;
        /// <summary>
        /// The channel this Dispatcher belongs to
        /// </summary>
        public ChannelBase Owner
        {
            get { return owner; }
        }
        ChannelBase IDispatcherInternal.Owner
        {
            set { owner = value; }
        }

        public int Count
        {
            get
            {
                using (ThreadContext.ReadLock(subscriberLock))
                    if (subscriber != null)
                        return subscriber.Count;

                return 0;
            }
        }

        /// <summary>
        /// Initializes this Dispatcher
        /// </summary>
        public Dispatcher()
        {
            subscriberLock = new ReadWriteLock();
            this.subscriber = new ListType();
        }

        public virtual void Register(Adapter adapter, object action, params object[] options)
        {
            using (ThreadContext.WriteLock(subscriberLock))
                if (subscriber != null)
                {
                    subscriber.Add(new Tuple<Adapter, object>(adapter, action));
                    adapter.Register(owner);
                }
        }

        public virtual void Remove(Adapter adapter, object action)
        {
            using (ThreadContext.WriteLock(subscriberLock))
                if (subscriber != null)
                {
                    foreach (Tuple<Adapter, object> target in subscriber)
                        if (target.Item1 == adapter && (action == null || target.Item2 == action))
                        {
                            subscriber.Remove(target);
                            adapter.Remove(owner);
                            break;
                        }
                }
        }
        public void Remove(Adapter adapter)
        {
            Remove(adapter, null);
        }

        public abstract bool Dispatch(IReceiver sender, object[] args);

        public virtual void Clear()
        {
            using (ThreadContext.WriteLock(subscriberLock))
                if (subscriber != null)
                {
                    foreach (Tuple<Adapter, object> target in subscriber)
                        target.Item1.Remove(owner);

                    subscriber.Clear();
                }
        }
        public virtual void Dispose()
        {
            using (ThreadContext.WriteLock(subscriberLock))
                if (subscriber != null)
                {
                    ICollection<Tuple<Adapter, object>> subs = subscriber.Clone();
                    foreach (Tuple<Adapter, object> target in subs)
                        target.Item1.Remove(owner);

                    subscriber = default(ListType);
                }
        }
    }
}