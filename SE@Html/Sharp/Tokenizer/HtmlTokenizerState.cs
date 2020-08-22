// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Html
{
    public enum HtmlTokenizerState
    {
        /// <summary>
        /// HTML5 8.2.4.1. Data state
        /// https://www.w3.org/TR/html53/syntax.html#data-state
        /// </summary>
        Data,


        CharacterReference,


        RcData,


        RawText,


        ScriptData,


        Plaintext,

        /// <summary>
        /// HTML5 8.2.4.6. Tag open state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-tag-open-state
        /// </summary>
        TagOpen,


        EndTagOpen,


        TagName,


        RcDataLessThan,


        RcDataEndTagOpen,


        RcDataEndTagName,


        RawTextLessThan,


        RawTextEndTagOpen,


        RawTextEndTagName,


        ScriptDataLessThan,


        ScriptDataEndTagOpen,


        ScriptDataEndTagName,


        ScriptDataEscapeStart,


        ScriptDataEscapeStartDash,


        ScriptDataEscaped,


        ScriptDataEscapedDash,


        ScriptDataEscapedDashDash,


        ScriptDataEscapedLessThan,


        ScriptDataEscapedEndTagOpen,


        ScriptDataEscapedEndTagName,


        ScriptDataDoubleEscapeStart,


        ScriptDataDoubleEscaped,


        ScriptDataDoubleEscapedDash,


        ScriptDataDoubleEscapedDashDash,


        ScriptDataDoubleEscapedLessThan,


        ScriptDataDoubleEscapeEnd,


        BeforeAttributeName,


        AttributeName,


        AfterAttributeName,


        BeforeAttributeValue,


        AttributeValueQuoted,


        AttributeValueUnquoted,


        AfterAttributeValueQuoted,


        SelfClosingStartTag,


        BogusComment,


        MarkupDeclarationOpen,


        CommentStart,


        CommentStartDash,


        Comment,


        CommentEndDash,


        CommentEnd,


        CommentEndBang,

        /// <summary>
        /// HTML5 8.2.4.53. DOCTYPE state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-doctype-state
        /// </summary>
        Doctype,

        /// <summary>
        /// HTML5 8.2.4.54. Before DOCTYPE name state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-before-doctype-name-state
        /// </summary>
        BeforeDoctypeName,

        /// <summary>
        /// HTML5 8.2.4.55. DOCTYPE name state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-doctype-name-state
        /// </summary>
        DoctypeName,

        /// <summary>
        /// HTML5 8.2.4.56. After DOCTYPE name state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-after-doctype-name-state
        /// </summary>
        AfterDoctypeName,

        /// <summary>
        /// HTML5 8.2.4.57. After DOCTYPE public keyword state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-after-doctype-public-keyword-state
        /// </summary>
        AfterDoctypePublicKeyword,

        /// <summary>
        /// HTML5 8.2.4.58. Before DOCTYPE public identifier state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-before-doctype-public-identifier-state
        /// </summary>
        BeforeDoctypePublicIdentifier,

        /// <summary>
        /// HTML5 8.2.4.59. DOCTYPE public identifier (double-quoted) state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-doctype-public-identifier-double-quoted-state
        /// 
        /// HTML5 8.2.4.60. DOCTYPE public identifier (single-quoted) state
        /// https://www.w3.org/TR/html53/syntax.html#doctype-public-identifier-single-quoted-state
        /// </summary>
        DoctypePublicIdentifierQuoted,

        /// <summary>
        /// HTML5 8.2.4.61. After DOCTYPE public identifier state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-after-doctype-public-identifier-state
        /// </summary>
        AfterDoctypePublicIdentifier,

        /// <summary>
        /// HTML5 8.2.4.62. Between DOCTYPE public and system identifiers state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-between-doctype-public-and-system-identifiers-state
        /// </summary>
        BetweenDoctypePublicAndSystemIdentifiers,

        /// <summary>
        /// HTML5 8.2.4.63. After DOCTYPE system keyword state
        /// https://www.w3.org/TR/html53/syntax.html#after-doctype-system-keyword-state
        /// </summary>
        AfterDoctypeSystemKeyword,

        /// <summary>
        /// HTML5 8.2.4.64. Before DOCTYPE system identifier state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-before-doctype-system-identifier-state
        /// </summary>
        BeforeDoctypeSystemIdentifier,

        /// <summary>
        /// HTML5 8.2.4.65. DOCTYPE public identifier (double-quoted) state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-doctype-system-identifier-double-quoted-state
        /// 
        /// HTML5 8.2.4.66. DOCTYPE public identifier (single-quoted) state
        /// https://www.w3.org/TR/html53/syntax.html#doctype-system-identifier-single-quoted-state
        /// </summary>
        DoctypeSystemIdentifierQuoted,

        /// <summary>
        /// HTML5 8.2.4.67. After DOCTYPE system identifier state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-after-doctype-system-identifier-state
        /// </summary>
        AfterDoctypeSystemIdentifier,

        /// <summary>
        /// HTML5 8.2.4.68. Bogus DOCTYPE state
        /// https://www.w3.org/TR/html53/syntax.html#tokenizer-bogus-doctype-state
        /// </summary>
        BogusDoctype,

        CommentLessThan,


        CDataSection,

        CDataSectionBracket,

        CDataSectionEnd,

        CommentLessThanBang,
        CommentLessThanBangDash,
        CommentLessThanBangDashDash
    }
}
