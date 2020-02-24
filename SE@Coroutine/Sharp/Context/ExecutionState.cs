// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;
using SE;

namespace SE.Parallel.Coroutines
{
    /// <summary>
    /// A state object to control the flow of an asynchronous execution
    /// </summary>
    [Serializable]
    public class ExecutionState : IComparable<ExecutionState>
    {
        Int32 rawValue;
        /// <summary>
        /// The compressed state value
        /// </summary>
        public UInt32 RawValue
        {
            get { return (UInt32)rawValue; }
        }

        /// <summary>
        /// A named flag indicating the current state
        /// </summary>
        public ExecutionFlags Flag
        {
            get { return (ExecutionFlags)(rawValue & 0xFF); }
        }
        /// <summary>
        /// True if the state is not awaiting any change, false otherwise
        /// </summary>
        public bool Signaled
        {
            get { return ((Interlocked.CompareExchange(ref rawValue, 0, 0) >> 8) == 0); }
        }

        /// <summary>
        /// Determines if the state value is invalid and should be discarded
        /// </summary>
        public bool IsInvalid
        {
            get { return (rawValue == 0); }
        }

        /// <summary>
        /// Creates a new state from a packed value
        /// </summary>
        /// <param name="value">A compressed state to unpack</param>
        public ExecutionState(UInt32 value)
        {
            rawValue = (Int32)value;
        }

        public static implicit operator UInt32(ExecutionState state)
        {
            return (UInt32)state.rawValue;
        }
        public static implicit operator ExecutionState(UInt32 state)
        {
            return new ExecutionState(state);
        }

        /// <summary>
        /// Decreases the state counter by one if changes are awaited from this state
        /// </summary>
        public void Signal()
        {
            for (; ; )
            {
                Int32 vref = Interlocked.CompareExchange(ref rawValue, 0, 0);
                Int32 value = (vref >> 8) - 1;

                if (value < 0)
                    value = 0;

                value = ((rawValue & 0xFF) | (value << 8));
                if (Interlocked.CompareExchange(ref rawValue, value, vref) == vref)
                    break;
            }
        }

        public int CompareTo(ExecutionState aOther)
        {
            return rawValue.CompareTo(aOther.rawValue);
        }
        public override int GetHashCode()
        {
            return rawValue.GetHashCode();
        }

        /// <summary>
        /// Creates a new state from the given flag and state counter
        /// </summary>
        /// <param name="flag">The state that should be populated</param>
        /// <param name="state">An optional state counter to set</param>
        /// <returns>The newly created state</returns>
        public static ExecutionState Create(ExecutionFlags flag, UInt32 state = 0)
        {
            return (UInt32)(((UInt32)flag & 0xFF) | (state << 8));
        }
    }
}