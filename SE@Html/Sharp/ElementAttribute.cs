// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Html
{
    /// <summary>
    /// HTML5 8.1.2. Elements
    /// https://www.w3.org/TR/html53/syntax.html#writing-html-documents-elements
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ElementAttribute : Attribute
    {
        string name;

        public string Name
        {
            get { return name; }
        }

        bool isVoid;

        public bool IsVoid
        {
            get { return isVoid; }
            set { isVoid = value; }
        }

        HtmlTokenizerState tokenization;

        public HtmlTokenizerState Tokenization
        {
            get { return tokenization; }
            set { tokenization = value; }
        }

        public ElementAttribute(string name)
        {
            this.name = name;
            this.isVoid = false;
            this.tokenization = HtmlTokenizerState.Data;
        }
    }
}
