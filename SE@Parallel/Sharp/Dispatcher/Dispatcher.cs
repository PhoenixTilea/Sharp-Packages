// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Dispatches incoming payload to registered subscribers
    /// </summary>
    /// <typeparam name="ListType">A container type to determine the dispatching order</typeparam>
    public abstract class Dispatcher<ListType> : IDispatcher, IDispatcherInternal where ListType : ICollection<Tuple<Adapter, object>>, new()
    {
        protected ReadWriteLock subscriptionLock;
        protected ListType subscriptions;

        protected StreamBase owner;
        /// <summary>
        /// The channel this Dispatcher belongs to
        /// </summary>
        public StreamBase Owner
        {
            get { return owner; }
        }
        StreamBase IDispatcherInternal.Owner
        {
            set { owner = value; }
        }

        public int Count
        {
            get
            {
                using (ThreadContext.ReadLock(subscriptionLock))
                {
                    if (subscriptions != null)
                        return subscriptions.Count;
                }
                return 0;
            }
        }

        /// <summary>
        /// Initializes this Dispatcher
        /// </summary>
        public Dispatcher()
        {
            subscriptionLock = new ReadWriteLock();
            this.subscriptions = new ListType();
        }

        public virtual void Register(Adapter adapter, object action, params object[] options)
        {
            using (ThreadContext.WriteLock(subscriptionLock))
                if (subscriptions != null)
                {
                    subscriptions.Add(new Tuple<Adapter, object>(adapter, action));
                    adapter.Register(owner);
                }
        }

        public virtual void Remove(Adapter adapter, object action)
        {
            using (ThreadContext.WriteLock(subscriptionLock))
                if (subscriptions != null)
                {
                    foreach (Tuple<Adapter, object> target in subscriptions)
                        if (target.Item1 == adapter && (action == null || target.Item2 == action))
                        {
                            subscriptions.Remove(target);
                            adapter.Remove(owner);
                            break;
                        }
                }
        }
        public void Remove(Adapter adapter)
        {
            Remove(adapter, null);
        }

        public abstract bool Dispatch(IPromiseNotifier<object> sender, object[] args);

        public virtual void Clear()
        {
            using (ThreadContext.WriteLock(subscriptionLock))
                if (subscriptions != null)
                {
                    foreach (Tuple<Adapter, object> target in subscriptions)
                        target.Item1.Remove(owner);

                    subscriptions.Clear();
                }
        }
        public virtual void Dispose()
        {
            using (ThreadContext.WriteLock(subscriptionLock))
                if (subscriptions != null)
                {
                    ICollection<Tuple<Adapter, object>> subs = subscriptions.Clone();
                    foreach (Tuple<Adapter, object> target in subs)
                        target.Item1.Remove(owner);

                    subscriptions = default(ListType);
                }
        }
    }
}