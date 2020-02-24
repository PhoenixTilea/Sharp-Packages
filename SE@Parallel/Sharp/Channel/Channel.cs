// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Parallel.Processing
{
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext(action as Action, sender);
        }

        public bool Dispatch()
        {
            return Dispatch(AdapterContext.NullReceiver);
        }
        public virtual bool Dispatch(IReceiver sender)
        {
            return dispatcher.Dispatch(sender, null);
        }
    }
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel<T0> : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action<T0> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext<T0>(action as Action<T0>, args, sender);
        }

        public bool Dispatch(T0 a0)
        {
            return Dispatch(AdapterContext.NullReceiver, new object[]
                {
                    a0
                });
        }
        public bool Dispatch(IReceiver sender, T0 a0)
        {
            return Dispatch(sender, new object[]
                {
                    a0
                });
        }
        public bool Dispatch(object[] args)
        {
            return Dispatch(AdapterContext.NullReceiver, args);
        }
        public virtual bool Dispatch(IReceiver sender, object[] args)
        {
            if (args.Length < 1)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel<T0, T1> : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action<T0, T1> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext<T0, T1>(action as Action<T0, T1>, args, sender);
        }

        public bool Dispatch(T0 a0, T1 a1)
        {
            return Dispatch(AdapterContext.NullReceiver, new object[]
                {
                    a0,
                    a1
                });
        }
        public bool Dispatch(IReceiver sender, T0 a0, T1 a1)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1
                });
        }
        public bool Dispatch(object[] args)
        {
            return Dispatch(AdapterContext.NullReceiver, args);
        }
        public virtual bool Dispatch(IReceiver sender, object[] args)
        {
            if (args.Length < 2)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel<T0, T1, T2> : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action<T0, T1, T2> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext<T0, T1, T2>(action as Action<T0, T1, T2>, args, sender);
        }

        public bool Dispatch(T0 a0, T1 a1, T2 a2)
        {
            return Dispatch(AdapterContext.NullReceiver, new object[]
                {
                    a0,
                    a1,
                    a2
                });
        }
        public bool Dispatch(IReceiver sender, T0 a0, T1 a1, T2 a2)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1,
                    a2
                });
        }
        public bool Dispatch(object[] args)
        {
            return Dispatch(AdapterContext.NullReceiver, args);
        }
        public virtual bool Dispatch(IReceiver sender, object[] args)
        {
            if (args.Length < 3)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel<T0, T1, T2, T3> : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action<T0, T1, T2, T3> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext<T0, T1, T2, T3>(action as Action<T0, T1, T2, T3>, args, sender);
        }

        public bool Dispatch(T0 a0, T1 a1, T2 a2, T3 a3)
        {
            return Dispatch(AdapterContext.NullReceiver, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3
                });
        }
        public bool Dispatch(IReceiver sender, T0 a0, T1 a1, T2 a2, T3 a3)
        {
            return Dispatch(sender, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3
                });
        }
        public bool Dispatch(object[] args)
        {
            return Dispatch(AdapterContext.NullReceiver, args);
        }
        public virtual bool Dispatch(IReceiver sender, object[] args)
        {
            if (args.Length < 4)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel<T0, T1, T2, T3, T4> : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action<T0, T1, T2, T3, T4> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext<T0, T1, T2, T3, T4>(action as Action<T0, T1, T2, T3, T4>, args, sender);
        }

        public bool Dispatch(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)
        {
            return Dispatch(AdapterContext.NullReceiver, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3,
                    a4
                });
        }
        public bool Dispatch(IReceiver sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)
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
        public bool Dispatch(object[] args)
        {
            return Dispatch(AdapterContext.NullReceiver, args);
        }
        public virtual bool Dispatch(IReceiver sender, object[] args)
        {
            if (args.Length < 5)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel<T0, T1, T2, T3, T4, T5> : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action<T0, T1, T2, T3, T4, T5> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext<T0, T1, T2, T3, T4, T5>(action as Action<T0, T1, T2, T3, T4, T5>, args, sender);
        }

        public bool Dispatch(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
        {
            return Dispatch(AdapterContext.NullReceiver, new object[]
                {
                    a0,
                    a1,
                    a2,
                    a3,
                    a4,
                    a5
                });
        }
        public bool Dispatch(IReceiver sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
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
        public bool Dispatch(object[] args)
        {
            return Dispatch(AdapterContext.NullReceiver, args);
        }
        public virtual bool Dispatch(IReceiver sender, object[] args)
        {
            if (args.Length < 6)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel<T0, T1, T2, T3, T4, T5, T6> : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action<T0, T1, T2, T3, T4, T5, T6> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext<T0, T1, T2, T3, T4, T5, T6>(action as Action<T0, T1, T2, T3, T4, T5, T6>, args, sender);
        }

        public bool Dispatch(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
        {
            return Dispatch(AdapterContext.NullReceiver, new object[]
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
        public bool Dispatch(IReceiver sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
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
        public bool Dispatch(object[] args)
        {
            return Dispatch(AdapterContext.NullReceiver, args);
        }
        public virtual bool Dispatch(IReceiver sender, object[] args)
        {
            if (args.Length < 7)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel<T0, T1, T2, T3, T4, T5, T6, T7> : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action<T0, T1, T2, T3, T4, T5, T6, T7> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext<T0, T1, T2, T3, T4, T5, T6, T7>(action as Action<T0, T1, T2, T3, T4, T5, T6, T7>, args, sender);
        }

        public bool Dispatch(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
        {
            return Dispatch(AdapterContext.NullReceiver, new object[]
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
        public bool Dispatch(IReceiver sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
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
        public bool Dispatch(object[] args)
        {
            return Dispatch(AdapterContext.NullReceiver, args);
        }
        public virtual bool Dispatch(IReceiver sender, object[] args)
        {
            if (args.Length < 8)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel<T0, T1, T2, T3, T4, T5, T6, T7, T8> : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext<T0, T1, T2, T3, T4, T5, T6, T7, T8>(action as Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>, args, sender);
        }

        public bool Dispatch(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
        {
            return Dispatch(AdapterContext.NullReceiver, new object[]
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
        public bool Dispatch(IReceiver sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
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
        public bool Dispatch(object[] args)
        {
            return Dispatch(AdapterContext.NullReceiver, args);
        }
        public virtual bool Dispatch(IReceiver sender, object[] args)
        {
            if (args.Length < 9)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
    /// <summary>
    /// Routes incomming tasks to coresponding Adapter
    /// </summary>
    public class Channel<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : ChannelBase
    {
        public Channel(IDispatcher dispatcher)
            : base(dispatcher)
        { }

        public virtual void Register(Adapter adapter, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> action, params object[] options)
        {
            dispatcher.Register(adapter, action, options);
        }

        public override AdapterContext GetContext(object action, object[] args, IReceiver sender)
        {
            return new AdapterCallContext<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(action as Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>, args, sender);
        }

        public bool Dispatch(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
        {
            return Dispatch(AdapterContext.NullReceiver, new object[]
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
        public bool Dispatch(IReceiver sender, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
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
        public bool Dispatch(object[] args)
        {
            return Dispatch(AdapterContext.NullReceiver, args);
        }
        public virtual bool Dispatch(IReceiver sender, object[] args)
        {
            if (args.Length < 10)
                throw new ArgumentOutOfRangeException("args.Length");

            return dispatcher.Dispatch(sender, args);
        }
    }
}
