// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    public abstract class ReactiveStream<T> : IReactiveStream<T>, IDisposable
    {
        public struct Disposer : IDisposable
        {
            ReactiveStream<T> owner;
            IObserver<T> observer;

            public ReactiveStream<T> Owner
            {
                get { return owner; }
            }

            public Disposer(ReactiveStream<T> owner, IObserver<T> observer)
            {
                this.owner = owner;
                this.observer = observer;
            }
            public void Dispose()
            {
                using (ThreadContext.WriteLock(owner.subscriptionLock))
                    owner.subscriptions.Remove(observer);
            }
        }

        protected HashSet<IObserver<T>> subscriptions;
        protected ReadWriteLock subscriptionLock;

        public ReactiveStream()
        {
            subscriptions = new HashSet<IObserver<T>>();
            subscriptionLock = new ReadWriteLock();
        }
        public void Dispose()
        {
            if (subscriptions.Count > 0)
                using (ThreadContext.WriteLock(subscriptionLock))
                {
                    foreach (IObserver<T> observer in subscriptions)
                        try
                        {
                            observer.OnCompleted();
                        }
                        catch (Exception er)
                        {
                            try
                            {
                                observer.OnError(er);
                            }
                            catch { }
                        }

                    subscriptions.Clear();
                }
        }

        protected void Push(T value)
        {
            using (ThreadContext.ReadLock(subscriptionLock))
            {
                foreach (IObserver<T> observer in subscriptions)
                    try
                    {
                        observer.OnNext(value);
                    }
                    catch (Exception er)
                    {
                        try
                        {
                            observer.OnError(er);
                        }
                        catch { }
                    }
            }
        }
        protected void Push(Exception error)
        {
            using (ThreadContext.ReadLock(subscriptionLock))
            {
                foreach (IObserver<T> observer in subscriptions)
                    try
                    {
                        observer.OnError(error);
                    }
                    catch { }
            }
        }

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            using (ThreadContext.WriteLock(subscriptionLock))
                subscriptions.Add(observer);

            return new Disposer(this, observer);
        }

        /// <summary>
        /// Wraps a subscription action into a push-based notification stream
        /// </summary>
        public static IReactiveStream<T> Create(Func<IObserver<T>, IDisposable> action)
        {
            return new AnonymousStream<T>(action);
        }
    }
}
