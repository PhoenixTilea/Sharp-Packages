// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret> : AdapterContext
    {
        protected Func<Ret> action;
        public Func<Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<Ret> action, IPromiseNotifier<object> sender)
            : base(sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action());
        }
    }
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret, T> : AdapterContext
    {
        protected Func<T, Ret> action;
        public Func<T, Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<T, Ret> action, object[] args, IPromiseNotifier<object> sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action.DynamicInvoke(args));
        }
    }
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret, T0, T1> : AdapterContext
    {
        protected Func<T0, T1, Ret> action;
        public Func<T0, T1, Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<T0, T1, Ret> action, object[] args, IPromiseNotifier<object> sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action.DynamicInvoke(args));
        }
    }
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret, T0, T1, T2> : AdapterContext
    {
        protected Func<T0, T1, T2, Ret> action;
        public Func<T0, T1, T2, Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<T0, T1, T2, Ret> action, object[] args, IPromiseNotifier<object> sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action.DynamicInvoke(args));
        }
    }
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret, T0, T1, T2, T3> : AdapterContext
    {
        protected Func<T0, T1, T2, T3, Ret> action;
        public Func<T0, T1, T2, T3, Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<T0, T1, T2, T3, Ret> action, object[] args, IPromiseNotifier<object> sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action.DynamicInvoke(args));
        }
    }
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret, T0, T1, T2, T3, T4> : AdapterContext
    {
        protected Func<T0, T1, T2, T3, T4, Ret> action;
        public Func<T0, T1, T2, T3, T4, Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<T0, T1, T2, T3, T4, Ret> action, object[] args, IPromiseNotifier<object> sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action.DynamicInvoke(args));
        }
    }
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret, T0, T1, T2, T3, T4, T5> : AdapterContext
    {
        protected Func<T0, T1, T2, T3, T4, T5, Ret> action;
        public Func<T0, T1, T2, T3, T4, T5, Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<T0, T1, T2, T3, T4, T5, Ret> action, object[] args, IPromiseNotifier<object> sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action.DynamicInvoke(args));
        }
    }
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret, T0, T1, T2, T3, T4, T5, T6> : AdapterContext
    {
        protected Func<T0, T1, T2, T3, T4, T5, T6, Ret> action;
        public Func<T0, T1, T2, T3, T4, T5, T6, Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<T0, T1, T2, T3, T4, T5, T6, Ret> action, object[] args, IPromiseNotifier<object> sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action.DynamicInvoke(args));
        }
    }
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret, T0, T1, T2, T3, T4, T5, T6, T7> : AdapterContext
    {
        protected Func<T0, T1, T2, T3, T4, T5, T6, T7, Ret> action;
        public Func<T0, T1, T2, T3, T4, T5, T6, T7, Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<T0, T1, T2, T3, T4, T5, T6, T7, Ret> action, object[] args, IPromiseNotifier<object> sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action.DynamicInvoke(args));
        }
    }
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret, T0, T1, T2, T3, T4, T5, T6, T7, T8> : AdapterContext
    {
        protected Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, Ret> action;
        public Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, Ret> action, object[] args, IPromiseNotifier<object> sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action.DynamicInvoke(args));
        }
    }
    /// <summary>
    /// A context class that manages return value execution
    /// </summary>
    public class AdapterRequestContext<Ret, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : AdapterContext
    {
        protected Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, Ret> action;
        public Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, Ret> Func
        {
            get { return action; }
        }

        public AdapterRequestContext(Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, Ret> action, object[] args, IPromiseNotifier<object> sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            sender.OnResolve(action.DynamicInvoke(args));
        }
    }
}
