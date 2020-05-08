// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Parsing
{
    /// <summary>
    /// A strictly directed acyclic graph edge between two nodes
    /// </summary>
    public struct AcyclicGraphEdge<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>
    {
        int id;
        /// <summary>
        /// The unique node ID
        /// </summary>
        public int Id
        {
            get { return id; }
        }

        T incoming;
        /// <summary>
        /// The previous node this edge has come from
        /// </summary>
        public T Incoming
        {
            get { return incoming; }
        }

        T outgoing;
        /// <summary>
        /// The succeeding node this edge is forwarded to
        /// </summary>
        public T Outgoing
        {
            get { return outgoing; }
        }

        int slice;
        /// <summary>
        /// The slice ID
        /// </summary>
        public int Slice
        {
            get { return slice; }
        }

        /// <summary>
        /// Creates a new directed edge from one node to another 
        /// </summary>
        /// <param name="id">The edge ID</param>
        /// <param name="slice">The slice ID this edge belongs to</param>
        /// <param name="incoming">The previous node this edge has come from</param>
        /// <param name="outgoing">The succeeding node this edge is forwarded to</param>
        public AcyclicGraphEdge(int id, int slice, T incoming, T outgoing)
        {
            this.id = id;
            this.slice = slice;
            this.incoming = incoming;
            this.outgoing = outgoing;
        }
        /// <summary>
        /// Creates a new directed edge from one the root state to another node 
        /// </summary>
        /// <param name="id">The edge ID</param>
        /// <param name="slice">The slice ID this edge belongs to</param>
        /// <param name="outgoing">The succeeding node this edge is forwarded to</param>
        public AcyclicGraphEdge(int id, int slice, T outgoing)
            :this(id, slice, default(T), outgoing)
        { }

        public override bool Equals(object obj)
        {
            if (obj is AcyclicGraphEdge<T>)
            {
                AcyclicGraphEdge<T> edge = (AcyclicGraphEdge<T>)obj;
                return
                (
                    incoming.Equals(edge.incoming) &&
                    outgoing.Equals(edge.outgoing) &&
                    id == edge.id
                );
            }
            else return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            HashCombiner hash = HashCombiner.Initialize();
            hash.Add(incoming);
            hash.Add(outgoing);
            hash.Add(id);

            return hash.Value;
        }

        public override string ToString()
        {
            return string.Format("{0} =({2})=> {1}", (incoming.Equals(default(T))) ? string.Empty : incoming.ToString(), outgoing, slice);
        }
    }
}
