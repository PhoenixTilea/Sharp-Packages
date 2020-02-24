// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Storage
{
    /// <summary>
    /// Defines a filtering statement
    /// </summary>
    public class FilterToken
    {
        protected string pattern;
        /// <summary>
        /// The pattern to validate match
        /// </summary>
        public string Pattern
        {
            get { return pattern; }
        }

        protected FilterToken child;
        /// <summary>
        /// A connection to the first child node
        /// </summary>
        public FilterToken Child
        {
            get
            {
                return child;
            }
            internal set
            {
                child = value;
            }
        }

        protected FilterToken next;
        /// <summary>
        /// A connection to the next sibling node
        /// </summary>
        public FilterToken Next
        {
            get
            {
                return next;
            }
            internal set
            {
                next = value;
            }
        }

        protected bool exclude;
        /// <summary>
        /// Gets or sets if matches should count as failure
        /// </summary>
        public bool Exclude
        {
            get
            {
                return exclude;
            }
            set
            {
                exclude = value;
            }
        }

        protected FilterType type;
        /// <summary>
        /// The concatenation type of this filter statement
        /// </summary>
        public FilterType Type
        {
            get 
            { 
                return type;
            }
            set 
            {
                type = value;
            }
        }

        /// <summary>
        /// Creates a new filter stamenet token
        /// </summary>
        /// <param name="pattern">A pattern to validate matches</param>
        public FilterToken(string pattern)
        {
            this.pattern = pattern;
        }

        /// <summary>
        /// Validates the pattern of this statement against certain input
        /// </summary>
        /// <param name="str">An input string to validate</param>
        /// <returns>True if input matches the pattern, false otherwise</returns>
        public bool IsMatch(string str)
        {
            return IsMatch(str, 0, 0);
        }
        /// <summary>
        /// Validates the pattern of this statement against certain input
        /// </summary>
        /// <param name="str">An input string to validate</param>
        /// <param name="index">The start index to begin verification of the input</param>
        /// <param name="offset">An offset to the pattern at which validation should start</param>
        /// <returns>True if input matches the pattern, false otherwise</returns>
        private bool IsMatch(string str, int index, int offset)
        {
            for (; ; )
            {
                if (offset == pattern.Length) return (index == str.Length);
                else if (pattern[offset] == '*')
                {
                    for (int i = str.Length; i >= index; i--)
                    {
                        if (IsMatch(str, i, offset + 1))
                            return true;
                    }
                    return false;
                }
                else if (index < str.Length && (Char.ToLower(str[index]) == Char.ToLower(pattern[offset]) || pattern[offset] == '?'))
                {
                    index++;
                    offset++;
                }
                else return false;
            }
        }

        /// <summary>
        /// Locates the first child who's statement is of the given pattern or
        /// first child if none has the pattern
        /// </summary>
        /// <param name="pattern">The pattern to match</param>
        /// <returns>The child node located</returns>
        public FilterToken GetChild(string pattern)
        {
            FilterToken result = child;
            while (result != null)
            {
                if (result.pattern == pattern)
                    return result;

                result = result.next;
            }
            return result;
        }

        public override string ToString()
        {
            return string.Format("[{2}] {0}{1}", exclude ? "not " : string.Empty, pattern, type);
        }
    }
}
