// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Json
{
    /// <summary>
    /// A JSON data node
    /// </summary>
    public class JsonNode
    {
        protected JsonNodeType type;
        /// <summary>
        /// The type of data this Node stands for
        /// </summary>
        public JsonNodeType Type
        {
            get 
            {
                return type;
            }
            internal set 
            {
                type = value;
            }
        }

        protected string name;
        /// <summary>
        /// The name of this node
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        protected JsonNode child;
        /// <summary>
        /// A connection to the first child node
        /// </summary>
        public JsonNode Child
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

        protected object rawValue;
        /// <summary>
        /// A data value object this Node contains
        /// </summary>
        public object RawValue
        {
            get 
            {
                return rawValue;
            }
            set 
            {
                rawValue = value;
            }
        }

        protected JsonNode next;
        /// <summary>
        /// A connection to the next sibling node
        /// </summary>
        public JsonNode Next
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

        /// <summary>
        /// Creates a new empty JSON node
        /// </summary>
        internal JsonNode()
        { }

        /// <summary>
        /// Returns the contained raw data as bool value
        /// </summary>
        public bool ToBoolean()
        {
            if (type != JsonNodeType.Bool)
                throw new InvalidCastException();

            return Convert.ToBoolean(rawValue);
        }

        /// <summary>
        /// Returns the contained raw data as 16 bit integer value
        /// </summary>
        public Int16 ToInt16()
        {
            if (type != JsonNodeType.Integer)
                throw new InvalidCastException();

            return Decimal.ToInt16(Convert.ToDecimal(rawValue));
        }
        /// <summary>
        /// Returns the contained raw data as 16 bit unsigned integer value
        /// </summary>
        public UInt16 ToUInt16()
        {
            if (type != JsonNodeType.Integer)
                throw new InvalidCastException();

            return Decimal.ToUInt16(Convert.ToDecimal(rawValue));
        }
        /// <summary>
        /// Returns the contained raw data as 32 bit integer value
        /// </summary>
        public Int32 ToInt32()
        {
            if (type != JsonNodeType.Integer)
                throw new InvalidCastException();

            return Decimal.ToInt32(Convert.ToDecimal(rawValue));
        }
        /// <summary>
        /// Returns the contained raw data as 32 bit unsigned integer value
        /// </summary>
        public UInt32 ToUInt32()
        {
            if (type != JsonNodeType.Integer)
                throw new InvalidCastException();

            return Decimal.ToUInt32(Convert.ToDecimal(rawValue));
        }
        /// <summary>
        /// Returns the contained raw data as 64 bit unsigned integer value
        /// </summary>
        public UInt64 ToUInt64()
        {
            if (type != JsonNodeType.Integer)
                throw new InvalidCastException();

            return Decimal.ToUInt64(Convert.ToDecimal(rawValue));
        }
        /// <summary>
        /// Returns the contained raw data as 32 bit integer value
        /// </summary>
        public Int64 ToInt64()
        {
            if (type != JsonNodeType.Integer)
                throw new InvalidCastException();

            return Decimal.ToInt64(Convert.ToDecimal(rawValue));
        }

        /// <summary>
        /// Returns the contained raw data as 32 bit floating point value
        /// </summary>
        public Single ToSingle()
        {
            if (type != JsonNodeType.Decimal)
                throw new InvalidCastException();

            return Decimal.ToSingle(Convert.ToDecimal(rawValue));
        }
        /// <summary>
        /// Returns the contained raw data as 64 bit floating point value
        /// </summary>
        public Double ToDouble()
        {
            if (type != JsonNodeType.Decimal)
                throw new InvalidCastException();

            return Decimal.ToDouble(Convert.ToDecimal(rawValue));
        }

        /// <summary>
        /// Returns the contained raw data as .NET decimal value
        /// </summary>
        public Decimal ToInteger()
        {
            if (type != JsonNodeType.Integer)
                throw new InvalidCastException();

            return Decimal.Truncate(Convert.ToDecimal(rawValue));
        }
        /// <summary>
        /// Returns the contained raw data as .NET decimal value
        /// </summary>
        public Decimal ToDecimal()
        {
            if (type != JsonNodeType.Decimal)
                throw new InvalidCastException();

            return Convert.ToDecimal(rawValue);
        }

        /// <summary>
        /// Returns the contained raw data as .NET string value
        /// </summary>
        public override string ToString()
        {
            switch(type)
            {
                case JsonNodeType.Bool:
                case JsonNodeType.Integer:
                case JsonNodeType.Decimal:
                case JsonNodeType.String: return rawValue.ToString();
                default: return type.ToString();
            }
        }
    }
}
