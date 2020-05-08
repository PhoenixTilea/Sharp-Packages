// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// A queue based round-robin dispatching strategy
    /// </summary>
    public class RoundRobinDispatcher : Dispatcher<DispatcherQueue<Tuple<Adapter, object>>>
    {
        protected Dictionary<object, Func<AdapterContext, bool>> filter;

        /// <summary>
        /// Creates a new dispatcher instance
        /// </summary>
        public RoundRobinDispatcher()
        {
            filter = new Dictionary<object, Func<AdapterContext, bool>>();
        }

        public override void Register(Adapter adapter, object action, params object[] options)
        {
            using (ThreadContext.WriteLock(subscriptionLock))
            {
                base.Register(adapter, action, options);

                if (options != null && options.Length > 0 && options[0] is Func<AdapterContext, bool>)
                    if (!filter.ContainsKey(action))
                        filter.Add(action, options[0] as Func<AdapterContext, bool>);
            }
        }
        public override void Remove(Adapter adapter, object action)
        {
            using (ThreadContext.WriteLock(subscriptionLock))
            {
                base.Remove(adapter, action);

                if (action != null)
                    foreach (Tuple<Adapter, object> target in subscriptions)
                        if (target.Item2 == action)
                            return;

                filter.Remove(action);
            }
        }

        public override bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            Tuple<Adapter, object> first = null;
            Tuple<Adapter, object> target = null;
            AdapterContext ctx = null;
            using (ThreadContext.ReadLock(subscriptionLock))
            {
                if (subscriptions.Count == 0)
                    return false;

                for (; first == null || subscriptions.Comparer.Equals(subscriptions.Peek(), first);)
                {
                    target = subscriptions.Dequeue();
                    if (first == null)
                        first = target;

                    if (target != null)
                    {
                        subscriptions.Enqueue(target);

                        Func<AdapterContext, bool> flt; if (filter.TryGetValue(target.Item2, out flt))
                        {
                            ctx = owner.GetContext(target.Item2, args, sender);
                            if (!flt(ctx))
                                continue;
                        }
                        else ctx = owner.GetContext(target.Item2, args, sender);
                        break;
                    }
                }
            }

            if (target != null)
            {
                if (!target.Item1.Enabled) target.Item1.Initialize();
                if (target.Item1.AcceptsMessage)
                    return target.Item1.Enqueue(ctx);
            }
            return false;
        }
    }
}