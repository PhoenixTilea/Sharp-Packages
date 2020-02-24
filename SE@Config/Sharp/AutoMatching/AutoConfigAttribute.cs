// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Config
{
    /// <summary>
    /// Marks a property or field as configurable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class AutoConfigAttribute : Attribute
    {
        private static Dictionary<Type, Func<ITypeConverter>> converterCache;

        static AutoConfigAttribute()
        {
            converterCache = new Dictionary<Type, Func<ITypeConverter>>();
        }

        protected string id;
        /// <summary>
        /// A string code to indicate this field or property in configuration
        /// </summary>
        public string Id
        {
            get { return id; } 
        }

        protected string category;
        /// <summary>
        /// A string to order further information
        /// </summary>
        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        protected string text;
        /// <summary>
        /// A string to display when further information are required
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        protected int defaultIndex;
        /// <summary>
        /// Indicates the order in which properties are filled from a set of values
        /// that don't have any field or property relation attached
        /// </summary>
        public int DefaultIndex
        {
            get { return defaultIndex; }
            set { defaultIndex = (1 << value); }
        }

        protected int flagIndex;
        /// <summary>
        /// Indicates the index of an enumeration value set additive if this field or
        /// property is of an enumeration type
        /// </summary>
        public int FlagIndex
        {
            get { return flagIndex; }
            set { flagIndex = (1 << value); }
        }

        protected int maskIndex;
        /// <summary>
        /// Indicates the index of an enumeration value set exclusive if this field or
        /// property is of an enumeration type
        /// </summary>
        public int MaskIndex
        {
            get { return maskIndex; }
            set { maskIndex = (1 << value); }
        }

        protected Type typeConverter;
        /// <summary>
        /// Sets a custom converter to apply to field or property assigned values
        /// </summary>
        public Type TypeConverter
        {
            get { return typeConverter; }
            set { typeConverter = value; }
        }

        /// <summary>
        /// Returns true if this field or property could be filled by an unnamed
        /// set of values, false otherwise
        /// </summary>
        public bool IsDefault
        {
            get { return defaultIndex > 0; }
        }
        /// <summary>
        /// Returns true if this field or property is an enumeration type of additive set
        /// index, false otherwise
        /// </summary>
        public bool IsFlag
        {
            get { return (flagIndex > 0); }
        }
        /// <summary>
        /// Returns true if this field or property is an enumeration type of exclusive set
        /// index, false otherwise
        /// </summary>
        public bool IsMaskedValue
        {
            get { return (maskIndex > 0); }
        }

        bool declaredOnly;
        /// <summary>
        /// True if this boolean member should be set even if the option is just declared. The member
        /// has to be a boolean type field or property
        /// </summary>
        public bool DeclaredOnly
        {
            get { return declaredOnly; }
        }

        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        /// <param name="declaredOnly">True if this boolean member should be set even if the option is just declared</param>
        /// <param name="long">A string code to indicate this field or property</param>
        public AutoConfigAttribute(bool declaredOnly, string @long)
        {
            if (@long == null)
                @long = string.Empty;

            this.id = @long;
            this.defaultIndex = 0;
            this.flagIndex = 0;
            this.maskIndex = 0;

            this.declaredOnly = declaredOnly;
        }
        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        /// <param name="long">A string code to indicate this field or property</param>
        public AutoConfigAttribute(string @long)
            :this(false, @long)
        { }
        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        /// <param name="short">The single character code to indicate this field or property</param>
        public AutoConfigAttribute(char @short)
            : this(new string(@short, 1))
        { }
        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        /// <param name="declaredOnly">True if this boolean field should be set even if the option is just declared</param>
        /// <param name="short">The single character code to indicate this field or property</param>
        public AutoConfigAttribute(bool declaredOnly, char @short)
            : this(declaredOnly, new string(@short, 1))
        { }

        /// <summary>
        /// Tries to obtain an instance of this field or property's custom value converter
        /// if attached
        /// </summary>
        /// <param name="converter">The converter instance if successfully created</param>
        /// <returns>True if a custom type converter is attached, false otherwise</returns>
        public bool TryGetConverter(out ITypeConverter converter)
        {
            converter = null;
            if (typeConverter == null) return false;
            else
            {
                Func<ITypeConverter> creator;
                lock(converterCache)
                {
                    if (!converterCache.TryGetValue(typeConverter, out creator))
                    {
                        creator = (typeof(Func<ITypeConverter>).GetCreator(typeConverter) as Func<ITypeConverter>);
                        converterCache.Add(typeConverter, creator);
                    }
                }
                converter = creator();
                return true;
            }
        }
    }
}
