// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using SE;

namespace SE.Text.Html
{
    public struct HtmlMetaData
    {
        HtmlToken type;

        public HtmlToken Type
        {
            get { return type; }
            set { type = value; }
        }

        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        Dictionary<string, string> properties;

        public Dictionary<string, string> Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = new Dictionary<string, string>();
                }
                return properties;
            }
        }

        bool quirksFlag;

        public bool QuirksFlag
        {
            get { return quirksFlag; }
            set { quirksFlag = value; }
        }

        public override string ToString()
        {
            switch (type)
            {
                case HtmlToken.Whitespace:
                case HtmlToken.Data:
                case HtmlToken.Comment:
                    return name;

                case HtmlToken.OpenTag:
                case HtmlToken.CloseTag:
                case HtmlToken.SelfCloseTag:
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<");
                        if (type == HtmlToken.CloseTag)
                        {
                            sb.Append("/");
                        }
                        sb.Append(name);
                        if (properties != null)
                        {
                            foreach (KeyValuePair<string, string> propertie in properties)
                            {
                                sb.Append(" ");
                                sb.AppendFormat("{0}=\"{1}\"", propertie.Key, propertie.Value);
                            }
                        }
                        if (type == HtmlToken.SelfCloseTag)
                        {
                            sb.Append("/");
                        }
                        sb.Append(">");
                        return sb.ToString();
                    }

                default:
                    return base.ToString();
            }
        }
    }
}
