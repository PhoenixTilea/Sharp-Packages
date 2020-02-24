// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// Provides stackable sate transitions for token processing
    /// </summary>
    public class ProcessingState<StateId> : IEnumerable<StateId> where StateId : struct, IConvertible, IComparable, IFormattable
    {
        Stack<StateId> stack;
        public int Count
        {
            get { return stack.Count; }
        }

        /// <summary>
        /// Gets or sets current state in exclusive mode
        /// </summary>
        public StateId Current
        {
            get
            {
                if (stack.Count == 0) return default(StateId);
                else return stack.Peek();
            }
            set { Set(value); }
        }

        /// <summary>
        /// Creates a new stackable state
        /// </summary>
        public ProcessingState()
        {
            this.stack = new Stack<StateId>();
        }

        public static implicit operator StateId(ProcessingState<StateId> state)
        {
            return state.Current;
        }

        /// <summary>
        /// Sets the provided state in exclusive mode
        /// </summary>
        public void Set(StateId value)
        {
            if (stack.Count > 0)
                stack.Clear();

            stack.Push(value);
        }
        /// <summary>
        /// Adds the provided state in inclusive mode and makes it the
        /// current active state. Anything else is preserved
        /// </summary>
        public void Add(StateId value)
        {
            stack.Push(value);
        }

        /// <summary>
        /// Modifies current top most state to provided one
        /// </summary>
        public void Change(StateId value)
        {
            if (stack.Count > 0)
                stack.Pop();

            stack.Push(value);
        }

        /// <summary>
        /// Resets current states to default
        /// </summary>
        public void Reset()
        {
            stack.Clear();
        }

        /// <summary>
        /// Removes the top stacked state up to one sate that is preserved
        /// </summary>
        /// <returns></returns>
        public StateId Remove()
        {
            if (stack.Count <= 1) return default(StateId);
            else return stack.Pop();
        }

        public IEnumerator<StateId> GetEnumerator()
        {
            return stack.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return stack.GetEnumerator();
        }

        public override string ToString()
        {
            return Current.ToString();
        }
    }
}
