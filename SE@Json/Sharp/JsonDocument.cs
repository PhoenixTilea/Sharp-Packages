// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE;

namespace SE.Json
{
    /// <summary>
    /// A DOM document in JSON format
    /// </summary>
    public class JsonDocument
    {
        protected List<JsonNode> nodes;
        /// <summary>
        /// The first node in this Document
        /// </summary>
        public JsonNode Root
        {
            get
            {
                if (nodes.Count > 0) return nodes[0];
                else return null;
            }
        }

        protected JsonParser parser;
        /// <summary>
        /// A collection of error messages since last document parsing
        /// </summary>
        public IEnumerable<string> Errors
        {
            get
            {
                if (parser == null) return null;
                else return parser.Errors;
            }
        }

        /// <summary>
        /// Determines if the document has parser errors
        /// </summary>
        public bool HasErrors
        {
            get
            {
                if (parser != null) return (parser.Errors.Count > 0);
                else return false;
            }
        }

        /// <summary>
        /// Creates a new JSON DOM document
        /// </summary>
        public JsonDocument()
        {
            this.nodes = new List<JsonNode>();
        }

        /// <summary>
        /// Adds a new root node to this Document
        /// </summary>
        /// <param name="type">Defines the kind of node that should be added</param>
        /// <returns>The newly created node if successfull, null otherwise</returns>
        public JsonNode AddNode(JsonNodeType type)
        {
            if (nodes.Count > 0 || type > JsonNodeType.Array)
                return null;

            JsonNode node = new JsonNode();
            node.Type = type;

            nodes.Add(node);
            return node;
        }
        /// <summary>
        /// Adds a new child node to an element in this Document
        /// </summary>
        /// <param name="root">The newly created childs parent</param>
        /// <param name="type">Defines the kind of node that should be added</param>
        /// <returns>The newly created node if successfull, null otherwise</returns>
        public JsonNode AddNode(JsonNode root, JsonNodeType type)
        {
            if (root == null)
                return null;

            JsonNode node = new JsonNode();
            node.Type = type;

            AddChild(root, node);

            nodes.Add(node);
            return node;
        }
        /// <summary>
        /// Appends an existing node as child node to an element in this Document
        /// </summary>
        /// <param name="root">The appended childs parent</param>
        /// <param name="node">An existing node to append</param>
        public void AddAppend(JsonNode root, JsonNode node)
        {
            if (root == null)
            {
                if(Root == null) nodes.Add(node);
                return;
            }

            AddChild(root, node);
            nodes.Add(node);
        }

        void AddChild(JsonNode root, JsonNode child)
        {
            if (root.Child == null) root.Child = child;
            else
            {
                JsonNode prevChild = root.Child;
                for (; ; )
                {
                    if (prevChild.Next != null) prevChild = prevChild.Next;
                    else
                    {
                        prevChild.Next = child;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes an existing node and all child nodes attached to it from
        /// this Document
        /// </summary>
        /// <param name="node">An existing node to be removed</param>
        /// <returns>True if the node was removed successfully, false otherwise</returns>
        public bool RemoveNode(JsonNode node)
        {
            if (node == null) return false;

            /**
             Unlink node from DOM layer
            */
            for (int i = 0; i < nodes.Count; i++)
                if (nodes[i].Next == node)
                {
                    nodes[i].Next = node.Next;
                    break;
                }

            if (node.Type <= JsonNodeType.Array)
            {
                while (node.Child != null)
                {
                    RemoveNode(node.Child);
                    node.Child = null;
                }
            }

            nodes.Remove(node);
            return true;
        }
        /// <summary>
        /// Removes all nodes from this Document at once
        /// </summary>
        public void Clear()
        {
            nodes.Clear();
        }

        /// <summary>
        /// Tries to load this Document's content from a given stream
        /// </summary>
        /// <param name="stream">The stream to load content from</param>
        /// <returns>True if content was successfully parsed, false otherwise</returns>
        public virtual bool Load(Stream stream)
        {
            Clear();

            if(parser == null) parser = new JsonParser(this);
            return parser.Parse(stream, true);
        }
        /// <summary>
        /// Tries to save this Document's content to a given stream
        /// </summary>
        /// <param name="stream">The stream to save content to</param>
        /// <returns>True if content was successfully saved, false otherwise</returns>
        public virtual bool Save(Stream stream)
        {
            if (nodes.Count <= 0)
                return false;

            JsonNode root = nodes[0];
            Serialize(stream, root);
            stream.Flush();

            return true;
        }

        void Serialize(Stream stream, JsonNode node)
        {
            if (node.Name != null && node.Name.Length > 0)
            {
                stream.WriteByte((byte)'"');

                byte[] tmp = new byte[node.Name.Length];
                for (int i = 0; i < tmp.Length; i++)
                    tmp[i] = (byte)node.Name[i];

                stream.Write(tmp, 0, tmp.Length);

                stream.WriteByte((byte)'\"');
                stream.WriteByte((byte)':');
            }

            switch (node.Type)
            {
                case JsonNodeType.Object:
                    {
                        stream.WriteByte((byte)'{');
                        if (node.Child != null)
                            Serialize(stream, node.Child);
                        stream.WriteByte((byte)'}');
                    }
                    break;
                case JsonNodeType.Array:
                    {
                        stream.WriteByte((byte)'[');
                        if (node.Child != null)
                            Serialize(stream, node.Child);
                        stream.WriteByte((byte)']');
                    }
                    break;
                case JsonNodeType.Bool:
                    {
                        string str = node.ToBoolean().ToString().ToLowerInvariant();
                        byte[] tmp = new byte[str.Length];
                        for (int i = 0; i < tmp.Length; i++)
                            tmp[i] = (byte)str[i];

                        stream.Write(tmp, 0, tmp.Length);
                    }
                    break;
                case JsonNodeType.Integer:
                    {
                        string str = node.ToInteger().ToString(System.Globalization.CultureInfo.InvariantCulture);
                        byte[] tmp = new byte[str.Length];
                        for (int i = 0; i < tmp.Length; i++)
                            tmp[i] = (byte)str[i];

                        stream.Write(tmp, 0, tmp.Length);
                    }
                    break;
                case JsonNodeType.Decimal:
                    {
                        string str = node.ToDecimal().ToString(System.Globalization.CultureInfo.InvariantCulture);
                        byte[] tmp = new byte[str.Length];
                        for (int i = 0; i < tmp.Length; i++)
                            tmp[i] = (byte)str[i];

                        stream.Write(tmp, 0, tmp.Length);
                    }
                    break;
                case JsonNodeType.String:
                    {
                        stream.WriteByte((byte)'"');
                        string str = node.ToString();
                        byte[] tmp = new byte[str.Length];
                        for (int i = 0; i < tmp.Length; i++)
                            tmp[i] = (byte)str[i];

                        stream.Write(tmp, 0, tmp.Length);
                        stream.WriteByte((byte)'"');
                    }
                    break;
            }

            if (node.Next != null)
            {
                stream.WriteByte((byte)',');
                Serialize(stream, node.Next);
            }
        }
    }
}
