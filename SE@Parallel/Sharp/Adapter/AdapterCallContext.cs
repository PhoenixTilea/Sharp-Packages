// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext : AdapterContext
    {
        protected Action action;
        public Action Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action action, IReceiver sender)
            : base(sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action();
            sender.SetResult(this, null);

        }
    }
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext<T> : AdapterContext
    {
        protected Action<T> action;
        public Action<T> Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action<T> action, object[] args, IReceiver sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action.DynamicInvoke(args);
            sender.SetResult(this, null);
        }
    }
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext<T0, T1> : AdapterContext
    {
        protected Action<T0, T1> action;
        public Action<T0, T1> Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action<T0, T1> action, object[] args, IReceiver sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action.DynamicInvoke(args);
            sender.SetResult(this, null);
        }
    }
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext<T0, T1, T2> : AdapterContext
    {
        protected Action<T0, T1, T2> action;
        public Action<T0, T1, T2> Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action<T0, T1, T2> action, object[] args, IReceiver sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action.DynamicInvoke(args);
            sender.SetResult(this, null);
        }
    }
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext<T0, T1, T2, T3> : AdapterContext
    {
        protected Action<T0, T1, T2, T3> action;
        public Action<T0, T1, T2, T3> Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action<T0, T1, T2, T3> action, object[] args, IReceiver sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action.DynamicInvoke(args);
            sender.SetResult(this, null);
        }
    }
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext<T0, T1, T2, T3, T4> : AdapterContext
    {
        protected Action<T0, T1, T2, T3, T4> action;
        public Action<T0, T1, T2, T3, T4> Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action<T0, T1, T2, T3, T4> action, object[] args, IReceiver sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action.DynamicInvoke(args);
            sender.SetResult(this, null);
        }
    }
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext<T0, T1, T2, T3, T4, T5> : AdapterContext
    {
        protected Action<T0, T1, T2, T3, T4, T5> action;
        public Action<T0, T1, T2, T3, T4, T5> Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action<T0, T1, T2, T3, T4, T5> action, object[] args, IReceiver sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action.DynamicInvoke(args);
            sender.SetResult(this, null);
        }
    }
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext<T0, T1, T2, T3, T4, T5, T6> : AdapterContext
    {
        protected Action<T0, T1, T2, T3, T4, T5, T6> action;
        public Action<T0, T1, T2, T3, T4, T5, T6> Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action<T0, T1, T2, T3, T4, T5, T6> action, object[] args, IReceiver sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action.DynamicInvoke(args);
            sender.SetResult(this, null);
        }
    }
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext<T0, T1, T2, T3, T4, T5, T6, T7> : AdapterContext
    {
        protected Action<T0, T1, T2, T3, T4, T5, T6, T7> action;
        public Action<T0, T1, T2, T3, T4, T5, T6, T7> Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action<T0, T1, T2, T3, T4, T5, T6, T7> action, object[] args, IReceiver sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action.DynamicInvoke(args);
            sender.SetResult(this, null);
        }
    }
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext<T0, T1, T2, T3, T4, T5, T6, T7, T8> : AdapterContext
    {
        protected Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> action;
        public Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> action, object[] args, IReceiver sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action.DynamicInvoke(args);
            sender.SetResult(this, null);
        }
    }
    /// <summary>
    /// A context class that manages execution
    /// </summary>
    public class AdapterCallContext<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : AdapterContext
    {
        protected Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> action;
        public Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> Action
        {
            get { return action; }
        }

        public AdapterCallContext(Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> action, object[] args, IReceiver sender)
            : base(args, sender)
        {
            this.action = action;
        }

        public override void ExecuteEmbedded()
        {
            action.DynamicInvoke(args);
            sender.SetResult(this, null);
        }
    }
}
