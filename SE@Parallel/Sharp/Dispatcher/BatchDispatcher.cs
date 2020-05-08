// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// A list based endpoint dispatching strategy
    /// </summary>
    public class BatchDispatcher : Dispatcher<List<Tuple<Adapter, object>>>
    {
        protected Dictionary<object, Func<AdapterContext, bool>> filter;

        /// <summary>
        /// Creates a new dispatcher instance
        /// </summary>
        public BatchDispatcher()
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
            int adapterCount = 0;
            bool result = false;
            using (ThreadContext.ReadLock(subscriptionLock))
            {
                result = subscriptions.Count > 0;
                foreach (Tuple<Adapter, object> target in subscriptions)
                {
                    AdapterContext ctx;
                    Func<AdapterContext, bool> flt; if (filter.TryGetValue(target.Item2, out flt))
                    {
                        ctx = owner.GetContext(target.Item2, args, sender);
                        if (!flt(ctx))
                            continue;
                    }
                    else ctx = owner.GetContext(target.Item2, args, sender);

                    if (!target.Item1.Enabled) target.Item1.Initialize();
                    if (target.Item1.AcceptsMessage)
                    {
                        result &= target.Item1.Enqueue(ctx);
                        adapterCount++;
                    }
                }
            }
            return (result && adapterCount > 0);
        }
    }
}