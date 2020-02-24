// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using SE;

namespace SE.Parallel.Coroutines
{
    /// <summary>
    /// Defines a context used to handle asynchronous execution
    /// </summary>
    public class ExecutionContext : IReceiver
    {
        private static Dictionary<Int32, ExecutionContext> contextScopes;
        private static ReadWriteLock contextLock;

        public static ExecutionContext Current
        {
            get
            {
                ExecutionContext tmp;
                using (ThreadContext.ReadLock(contextLock))
                    contextScopes.TryGetValue(ThreadContext.LocalId, out tmp);

                return tmp;
            }
            private set
            {
                Int32 scope = ThreadContext.LocalId;
                using (ThreadContext.WriteLock(contextLock))
                {
                    if (contextScopes.ContainsKey(scope)) contextScopes[scope] = value;
                    else contextScopes.Add(scope, value);
                }
            }
        }

        Stack<IEnumerator> stack = new Stack<IEnumerator>();
        IReceiver parent;

        IEnumerator active;
        /// <summary>
        /// The current actively executed 'stack pointer'
        /// </summary>
        public IEnumerator Active
        {
            get { return active; }
        }

        ReadWriteLock stateLock = new ReadWriteLock();

        ExecutionState state;
        /// <summary>
        /// Signals the state this context currently is in
        /// </summary>
        public ExecutionState State
        {
            get { return state; }
        }

        /// <summary>
        /// A named flag indicating the current state of this context
        /// </summary>
        public ExecutionFlags Flag
        {
            get { return state.Flag; }
        }
        /// <summary>
        /// True if the context is not awaiting any change, false otherwise
        /// </summary>
        public bool Signaled
        {
            get { return state.Signaled; }
        }

        object result;
        /// <summary>
        /// The returned result if any
        /// </summary>
        public object Result
        {
            get { return result; }
        }

        Exception lastError;
        /// <summary>
        /// Determines the last error occurred if Failed state is set
        /// </summary>
        public Exception LastError
        {
            get { return lastError; }
        }

        static ExecutionContext()
        {
            contextScopes = new Dictionary<Int32, ExecutionContext>(64);
            contextLock = new ReadWriteLock();
        }
        /// <summary>
        /// Creates a new coroutine execution context
        /// </summary>
        /// <param name="handle">The coroutine handle to execute</param>
        /// <param name="parent">A parent to notify on completion</param>
        public ExecutionContext(IEnumerator handle, IReceiver parent)
        {
            this.active = handle;
            this.parent = parent;
        }
        /// <summary>
        /// Creates a new coroutine execution context
        /// </summary>
        /// <param name="handle">The coroutine handle to execute</param>
        public ExecutionContext(IEnumerator handle)
            :this(handle, null)
        { }

        /// <summary>
        /// Processes the last return statement without executing
        /// </summary>
        /// <returns>False if the context has returned, true otherwise</returns>
        public bool Evaluate()
        {
            result = active.Current;
            state = ExecutionState.Create(ExecutionFlags.Active);

            if (result is ExecutionFlags)
                result = ExecutionState.Create((ExecutionFlags)result);

            if (result is IEnumerable)
                result = (result as IEnumerable).GetEnumerator();

            if (result is IEnumerator)
            {
                stack.Push(active);
                active = result as IEnumerator;
                result = null;
            }
            else if (result is ExecutionState)
            {
                state = (ExecutionState)result;
                result = null;

                switch (state.Flag)
                {
                    case ExecutionFlags.Completed:
                        if (stack.Count > 0)
                        {
                            active = stack.Pop();
                            state = ExecutionState.Create(ExecutionFlags.Active);
                        }
                        else return false;
                        break;
                    case ExecutionFlags.Cancel:
                    case ExecutionFlags.Failed: return false;
                    case ExecutionFlags.Reset:
                        active.Reset();
                        break;
                }
            }
            else if (stack.Count > 0) active = stack.Pop();
            else
            {
                state = ExecutionState.Create(ExecutionFlags.Completed);
                return false;
            }
            return true;
        }
        /// <summary>
        /// Executes the context until a context switch has happened
        /// </summary>
        /// <returns>False if the context has returned, true otherwise</returns>
        public bool SwitchContext()
        {
            Current = this;
            using (ThreadContext.WriteLock(stateLock))
                try
                {
                    if (state != null && !state.Signaled)
                        throw new InvalidOperationException();

                    if (active.MoveNext()) return Evaluate();
                    else if (stack.Count > 0)
                    {
                        result = active.Current;
                        state = ExecutionState.Create(ExecutionFlags.Active);
                        active = stack.Pop();

                        return true;
                    }
                    else
                    {
                        result = active.Current;
                        state = ExecutionState.Create(ExecutionFlags.Completed);
                        return false;
                    }
                }
                catch (Exception er)
                {
                    lastError = er;
                    state = ExecutionState.Create(ExecutionFlags.Failed);
                    return false;
                }
                finally
                {
                    Current = null;

                    switch (state.Flag)
                    {
                        case ExecutionFlags.Cancel:
                        case ExecutionFlags.Completed:
                            {
                                if (parent != null)
                                    parent.SetResult(this, result);
                            }
                            break;
                        case ExecutionFlags.Failed:
                            {
                                if (parent != null)
                                    parent.SetError(this, lastError);
                            }
                            break;
                    }
                }
        }

        public void SetResult(object host, object result)
        {
            using (ThreadContext.ReadLock(stateLock))
            {
                if (state.Flag == ExecutionFlags.Pending)
                {
                    this.result = result;
                    state.Signal();
                }
            }
        }
        public void SetError(object host, Exception error)
        {
            using (ThreadContext.ReadLock(stateLock))
            {
                if (state.Flag == ExecutionFlags.Pending)
                {
                    this.lastError = error;
                    state.Signal();
                }
            }
        }

        /// <summary>
        /// Evaluates if current stack is executed as coroutine and returns the active
        /// context if possible
        /// </summary>
        /// <param name="context">An executin context for this coroutine</param>
        /// <returns>True if the stack is executed as coroutine, false otherwise</returns>
        public static bool TryGetContext(out ExecutionContext context)
        {
            context = Current;
            return (context != null);
        }
    }
}
