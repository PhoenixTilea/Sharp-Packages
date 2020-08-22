// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Html
{
    public enum HtmlToken
    {
        Doctype = 30,

        Comment = 27,

        OpenTag = 20,
        SelfCloseTag = 19,
        CloseTag = 18,

        Data = 2,

        Whitespace = 1,
        Invalid = 0
    }
}
