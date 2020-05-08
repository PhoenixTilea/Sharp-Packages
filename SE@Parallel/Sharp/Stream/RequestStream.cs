// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret>(action as Func<Ret>, sender);
        }

        public virtual bool Dispatch(IPromiseNotifier<object> sender)
        {
            return dispatcher.Dispatch(sender, null);
        }
    }
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret, T0> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<T0, Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret, T0>(action as Func<T0, Ret>, args, sender);
        }

        public bool Dispatch(IPromiseNotifier<object> sender, T0 a0)
        {
            return Dispatch(sender, new object[]
                {
                    a0
                });
        }
        public virtual bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            if (args.Length < 1)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret, T0, T1> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<T0, T1, Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret, T0, T1>(action as Func<T0, T1, Ret>, args, sender);
        }

        public bool Dispatch(IPromiseNotifier<object> sender, T0 a0, T1 a1)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1
                });
        }
        public virtual bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            if (args.Length < 2)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret, T0, T1, T2> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<T0, T1, T2, Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret, T0, T1, T2>(action as Func<T0, T1, T2, Ret>, args, sender);
        }

        public bool Dispatch(IPromiseNotifier<object> sender, T0 a0, T1 a1, T2 a2)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1,
                    a2
                });
        }
        public virtual bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            if (args.Length < 3)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret, T0, T1, T2, T3> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<T0, T1, T2, T3, Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret, T0, T1, T2, T3>(action as Func<T0, T1, T2, T3, Ret>, args, sender);
        }

        public bool Dispatch(IPromiseNotifier<object> sender, T0 a0, T1 a1, T2 a2, T3 a3)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3
                });
        }
        public virtual bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            if (args.Length < 4)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret, T0, T1, T2, T3, T4> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<T0, T1, T2, T3, T4, Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret, T0, T1, T2, T3, T4>(action as Func<T0, T1, T2, T3, T4, Ret>, args, sender);
        }

        public bool Dispatch(IPromiseNotifier<object> sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3,
                    a4
                });
        }
        public virtual bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            if (args.Length < 5)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret, T0, T1, T2, T3, T4, T5> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<T0, T1, T2, T3, T4, T5, Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret, T0, T1, T2, T3, T4, T5>(action as Func<T0, T1, T2, T3, T4, T5, Ret>, args, sender);
        }

        public bool Dispatch(IPromiseNotifier<object> sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3,
                    a4,
                    a5
                });
        }
        public virtual bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            if (args.Length < 6)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret, T0, T1, T2, T3, T4, T5, T6> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<T0, T1, T2, T3, T4, T5, T6, Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret, T0, T1, T2, T3, T4, T5, T6>(action as Func<T0, T1, T2, T3, T4, T5, T6, Ret>, args, sender);
        }

        public bool Dispatch(IPromiseNotifier<object> sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3,
                    a4,
                    a5,
                    a6
                });
        }
        public virtual bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            if (args.Length < 7)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret, T0, T1, T2, T3, T4, T5, T6, T7> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<T0, T1, T2, T3, T4, T5, T6, T7, Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret, T0, T1, T2, T3, T4, T5, T6, T7>(action as Func<T0, T1, T2, T3, T4, T5, T6, T7, Ret>, args, sender);
        }

        public bool Dispatch(IPromiseNotifier<object> sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3,
                    a4,
                    a5,
                    a6,
                    a7
                });
        }
        public virtual bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            if (args.Length < 8)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret, T0, T1, T2, T3, T4, T5, T6, T7, T8> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret, T0, T1, T2, T3, T4, T5, T6, T7, T8>(action as Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, Ret>, args, sender);
        }

        public bool Dispatch(IPromiseNotifier<object> sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3,
                    a4,
                    a5,
                    a6,
                    a7,
                    a8
                });
        }
        public virtual bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            if (args.Length < 9)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// A channel to obtain promise from pushed tasks
    /// </summary>
    public class RequestStream<Ret, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : StreamBase
    {
        public RequestStream(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, Ret> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IPromiseNotifier<object> sender)
        {
            return new AdapterRequestContext<Ret, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(action as Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, Ret>, args, sender);
        }

        public bool Dispatch(IPromiseNotifier<object> sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3,
                    a4,
                    a5,
                    a6,
                    a7,
                    a8,
                    a9
                });
        }
        public virtual bool Dispatch(IPromiseNotifier<object> sender, object[] args)
        {
            if (args.Length < 10)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
}
