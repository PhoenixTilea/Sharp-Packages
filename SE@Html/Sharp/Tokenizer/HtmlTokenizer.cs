// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Html
{
    /// <summary>
    /// HTML5 8.2.4.Tokenization compliant syntax tokenizer
    /// https://www.w3.org/TR/html53/
    /// </summary>
    public partial class HtmlTokenizer : StreamTokenizer<HtmlToken, HtmlTokenizerState>
    {
        bool allowScripts;

        string currentStartTag;
        string currentAttributeName;

        HtmlMetaData metaToken;

        public HtmlMetaData MetaToken
        {
            get { return metaToken; }
        }

        public HtmlTokenizer(Stream stream, bool isUtf8, bool allowScripts)
            : base(stream, isUtf8)
        {
            //8.2.4. Tokenization
            this.State.Set(HtmlTokenizerState.Data);
            this.textBuffer = new StringBuilder();
            this.allowScripts = allowScripts;
        }

        protected override HtmlToken GetToken(object context)
        {
            metaToken = new HtmlMetaData();
            textBuffer.Clear();

        Head:
            switch (State.Current)
            {
                #region 8.2.4.1. Data state
                case HtmlTokenizerState.Data:
                    {
                        if (ReadData())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.2. RCDATA state
                case HtmlTokenizerState.RcData:
                    {
                        if (ReadRcData())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.3. RAWTEXT state
                case HtmlTokenizerState.RawText:
                    {
                        if (ReadRawText())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.4. Script data state
                case HtmlTokenizerState.ScriptData:
                    {
                        if (ReadScriptData())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.5. PLAINTEXT state
                case HtmlTokenizerState.Plaintext:
                    {
                        if (ReadPlainText())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.6. Tag open state
                case HtmlTokenizerState.TagOpen:
                    {
                        if (ReadTagOpen())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.7. End tag open state
                case HtmlTokenizerState.EndTagOpen:
                    {
                        if (ReadEndTagOpen())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.8. Tag name state
                case HtmlTokenizerState.TagName:
                    {
                        if (ReadTagName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.9. RCDATA less-than sign state
                case HtmlTokenizerState.RcDataLessThan:
                    {
                        if (ReadRcDataLessThan())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.10. RCDATA end tag open state
                case HtmlTokenizerState.RcDataEndTagOpen:
                    {
                        if (ReadRcDataEndTagOpen())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.11. RCDATA end tag name state
                case HtmlTokenizerState.RcDataEndTagName:
                    {
                        if (ReadRcDataEndTagName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.12. RAWTEXT less-than sign state
                case HtmlTokenizerState.RawTextLessThan:
                    {
                        if (ReadRawTextLessThan())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.13. RAWTEXT end tag open state
                case HtmlTokenizerState.RawTextEndTagOpen:
                    {
                        if (ReadRawTextEndTagOpen())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.14. RAWTEXT end tag name state
                case HtmlTokenizerState.RawTextEndTagName:
                    {
                        if (ReadRawTextEndTagName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.15. Script data less-than sign state
                case HtmlTokenizerState.ScriptDataLessThan:
                    {
                        if (ReadScriptDataLessThan())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.16. Script data end tag open state
                case HtmlTokenizerState.ScriptDataEndTagOpen:
                    {
                        if (ReadScriptDataEndTagOpen())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.17. Script data end tag name state
                case HtmlTokenizerState.ScriptDataEndTagName:
                    {
                        if (ReadScriptDataEndTagName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.18. Script data escape start state
                case HtmlTokenizerState.ScriptDataEscapeStart:
                    {
                        if (ReadScriptDataEscapeStart())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.19. Script data escape start dash state
                case HtmlTokenizerState.ScriptDataEscapeStartDash:
                    {
                        if (ReadScriptDataEscapeStartDash())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.20. Script data escaped state
                case HtmlTokenizerState.ScriptDataEscaped:
                    {
                        if (ReadScriptDataEscaped())
                            goto Head;
                    }
                    break;
                #endregion   

                #region 8.2.4.21. Script data escaped dash state
                case HtmlTokenizerState.ScriptDataEscapedDash:
                    {
                        if (ReadScriptDataEscapedDash())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.22. Script data escaped dash dash state
                case HtmlTokenizerState.ScriptDataEscapedDashDash:
                    {
                        if (ReadScriptDataEscapedDashDash())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.23. Script data escaped less-than sign state
                case HtmlTokenizerState.ScriptDataEscapedLessThan:
                    {
                        if (ReadScriptDataEscapedLessThan())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.24. Script data escaped end tag open state
                case HtmlTokenizerState.ScriptDataEscapedEndTagOpen:
                    {
                        if (ReadScriptDataEscapedEndTagOpen())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.25. Script data escaped end tag name state
                case HtmlTokenizerState.ScriptDataEscapedEndTagName:
                    {
                        if (ReadScriptDataEscapedEndTagName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.26. Script data double escape start state
                case HtmlTokenizerState.ScriptDataDoubleEscapeStart:
                    {
                        if (ReadScriptDataDoubleEscapeStart())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.27. Script data double escaped state
                case HtmlTokenizerState.ScriptDataDoubleEscaped:
                    {
                        if (ReadScriptDataDoubleEscaped())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.28. Script data double escaped dash state
                case HtmlTokenizerState.ScriptDataDoubleEscapedDash:
                    {
                        if (ReadScriptDataDoubleEscapedDash())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.29. Script data double escaped dash dash state
                case HtmlTokenizerState.ScriptDataDoubleEscapedDashDash:
                    {
                        if (ReadScriptDataDoubleEscapedDashDash())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.30. Script data double escaped less-than sign state
                case HtmlTokenizerState.ScriptDataDoubleEscapedLessThan:
                    {
                        if (ReadScriptDataDoubleEscapedLessThan())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.31. Script data double escape end state
                case HtmlTokenizerState.ScriptDataDoubleEscapeEnd:
                    {
                        if (ReadScriptDataDoubleEscapeEnd())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.32. Before attribute name state
                case HtmlTokenizerState.BeforeAttributeName:
                    {
                        if (ReadBeforeAttributeName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.33. Attribute name state
                case HtmlTokenizerState.AttributeName:
                    {
                        if (ReadAttributeName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.34. After attribute name state
                case HtmlTokenizerState.AfterAttributeName:
                    {
                        if (ReadAfterAttributeName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.35. Before attribute value state
                case HtmlTokenizerState.BeforeAttributeValue:
                    {
                        if (ReadBeforeAttributeValue())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.36. Attribute value (double-quoted) state
                #endregion

                #region 8.2.4.37. Attribute value (single-quoted) state
                case HtmlTokenizerState.AttributeValueQuoted:
                    {
                        if (ReadAttributeValueQuoted())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.38. Attribute value (unquoted) state
                case HtmlTokenizerState.AttributeValueUnquoted:
                    {
                        if (ReadAttributeValueUnquoted())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.39. After attribute value (quoted) state
                case HtmlTokenizerState.AfterAttributeValueQuoted:
                    {
                        if (ReadAfterAttributeValueQuoted())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.40. Self-closing start tag state
                case HtmlTokenizerState.SelfClosingStartTag:
                    {
                        if (ReadSelfClosingStartTag())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.41. Bogus comment state
                case HtmlTokenizerState.BogusComment:
                    {
                        if (ReadBogusComment())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.42. Markup declaration open state
                case HtmlTokenizerState.MarkupDeclarationOpen:
                    {
                        if (ReadMarkupDeclarationOpen())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.43. Comment start state
                case HtmlTokenizerState.CommentStart:
                    {
                        if (ReadCommentStart())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.44. Comment start dash state
                case HtmlTokenizerState.CommentStartDash:
                    {
                        if (ReadCommentStartDash())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.45. Comment state
                case HtmlTokenizerState.Comment:
                    {
                        if (ReadComment())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.46. Comment less-than sign state
                case HtmlTokenizerState.CommentLessThan:
                    {
                        if (ReadCommentLessThan())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.47. Comment less-than sign bang state
                case HtmlTokenizerState.CommentLessThanBang:
                    {
                        if (ReadCommentLessThanBang())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.48. Comment less-than sign bang dash state
                case HtmlTokenizerState.CommentLessThanBangDash:
                    {
                        if (ReadCommentLessThanBangDash())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.49. Comment less-than sign bang dash dash state
                case HtmlTokenizerState.CommentLessThanBangDashDash:
                    {
                        if (ReadCommentLessThanBangDashDash())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.50. Comment end dash state
                case HtmlTokenizerState.CommentEndDash:
                    {
                        if (ReadCommentEndDash())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.51. Comment end state
                case HtmlTokenizerState.CommentEnd:
                    {
                        if (ReadCommentEnd())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.52. Comment end bang state
                case HtmlTokenizerState.CommentEndBang:
                    {
                        if (ReadCommentEndBang())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.53. DOCTYPE state
                case HtmlTokenizerState.Doctype:
                    {
                        if (ReadDoctype())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.54. Before DOCTYPE name state
                case HtmlTokenizerState.BeforeDoctypeName:
                    {
                        if (ReadBeforeDoctypeName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.55. DOCTYPE name state
                case HtmlTokenizerState.DoctypeName:
                    {
                        if (ReadDoctypeName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.56. After DOCTYPE name state
                case HtmlTokenizerState.AfterDoctypeName:
                    {
                        if (ReadAfterDoctypeName())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.57. After DOCTYPE public keyword state
                case HtmlTokenizerState.AfterDoctypePublicKeyword:
                    {
                        if (ReadAfterDoctypePublicKeyword())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.58. Before DOCTYPE public identifier state
                case HtmlTokenizerState.BeforeDoctypePublicIdentifier:
                    {
                        if (ReadBeforeDoctypePublicIdentifier())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.59. DOCTYPE public identifier (double-quoted) state
                #endregion

                #region 8.2.4.60. DOCTYPE public identifier (single-quoted) state
                case HtmlTokenizerState.DoctypePublicIdentifierQuoted:
                    {
                        if (ReadDoctypePublicIdentifierQuoted())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.61. After DOCTYPE public identifier state
                case HtmlTokenizerState.AfterDoctypePublicIdentifier:
                    {
                        if (ReadAfterDoctypePublicIdentifier())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.62. Between DOCTYPE public and system identifiers state
                case HtmlTokenizerState.BetweenDoctypePublicAndSystemIdentifiers:
                    {
                        if (ReadBetweenDoctypePublicAndSystemIdentifiers())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.63. After DOCTYPE system keyword state
                case HtmlTokenizerState.AfterDoctypeSystemKeyword:
                    {
                        if (ReadAfterDoctypeSystemKeyword())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.64. Before DOCTYPE system identifier state
                case HtmlTokenizerState.BeforeDoctypeSystemIdentifier:
                    {
                        if (ReadBeforeDoctypeSystemIdentifier())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.65. DOCTYPE system identifier (double-quoted) state
                #endregion

                #region 8.2.4.66. DOCTYPE system identifier (single-quoted) state
                case HtmlTokenizerState.DoctypeSystemIdentifierQuoted:
                    {
                        if (ReadDoctypeSystemIdentifierQuoted())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.67. After DOCTYPE system identifier state
                case HtmlTokenizerState.AfterDoctypeSystemIdentifier:
                    {
                        if (ReadAfterDoctypeSystemIdentifier())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.68. Bogus DOCTYPE state
                case HtmlTokenizerState.BogusDoctype:
                    {
                        if (ReadBogusDoctype())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.69. CDATA section state
                case HtmlTokenizerState.CDataSection:
                    {
                        if (ReadCDataSection())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.70. CDATA section bracket state
                case HtmlTokenizerState.CDataSectionBracket:
                    {
                        if (ReadCDataSectionBracket())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.71. CDATA section end state
                case HtmlTokenizerState.CDataSectionEnd:
                    {
                        if (ReadCDataSectionEnd())
                            goto Head;
                    }
                    break;
                #endregion

                #region 8.2.4.72. Character reference state
                case HtmlTokenizerState.CharacterReference:
                    {
                        if (ReadCharacterReference())
                            goto Head;
                    }
                    break;
                #endregion

                default: throw new ArgumentException(State.Current.ToString());
            }

            FinalizeToken();
            return metaToken.Type;
        }

        protected void CreateDataToken(bool useStreamBuffer = true)
        {
            if (!useStreamBuffer)
            {
                metaToken.Name = textBuffer.ToString();
                textBuffer.Clear();
            }
            else metaToken.Name = FlushBufferedData();
            if (string.IsNullOrWhiteSpace(metaToken.Name))
            {
                metaToken.Type = HtmlToken.Whitespace;
            }
            else metaToken.Type = HtmlToken.Data;
        }

        protected void CreateTagToken()
        {
            metaToken.Name = textBuffer.ToString();
            switch (metaToken.Type)
            {
                case HtmlToken.CloseTag:
                case HtmlToken.SelfCloseTag: break;
                default:
                    {
                        metaToken.Type = HtmlToken.OpenTag;
                        currentStartTag = metaToken.Name;
                    }
                    break;
            }
            textBuffer.Clear();
        }

        protected void CreateCommentToken()
        {
            metaToken.Name = textBuffer.ToString();
            if (string.IsNullOrWhiteSpace(metaToken.Name))
            {
                metaToken.Type = HtmlToken.Whitespace;
            }
            else metaToken.Type = HtmlToken.Comment;
            textBuffer.Clear();
        }

        protected void CreateDoctypeToken()
        {
            metaToken.Type = HtmlToken.Doctype;
            textBuffer.Clear();
        }

        protected void AppendAttribute()
        {
            if (!string.IsNullOrWhiteSpace(currentAttributeName) && !metaToken.Properties.ContainsKey(currentAttributeName))
            {
                metaToken.Properties.Add(currentAttributeName, textBuffer.ToString());
            }
            currentAttributeName = string.Empty;
        }

        /// <summary>
        /// HTML5 8.1.2. Elements
        /// https://www.w3.org/TR/html53/syntax.html#writing-html-documents-elements
        /// </summary>
        void FinalizeToken()
        {
            switch (metaToken.Type)
            {
                case HtmlToken.Whitespace:
                case HtmlToken.Data:
                    return;
                case HtmlToken.OpenTag:
                    {
                        switch (metaToken.Name)
                        {
                            #region Void elements
                            case "basefont":
                            case "frame":
                            case "image":
                            case "keygen":
                            case "menuitem":
                                {
                                    metaToken.Type = HtmlToken.SelfCloseTag;
                                }
                                goto DataState;
                            #endregion

                            #region Raw text elements
                            case "xmp":
                            case "noembed":
                            case "noframes":
                                {
                                    State.Set(HtmlTokenizerState.RawText);
                                }
                                break;
                            case "noscript":
                                {
                                    if (!allowScripts) State.Set(HtmlTokenizerState.RawText);
                                    else goto DataState;
                                }
                                break;
                            #endregion

                            #region Plain text elements
                            case "plaintext":
                                {
                                    State.Set(HtmlTokenizerState.Plaintext);
                                }
                                break;
                            #endregion

                            #region Normal elements
                            default:
                                {
                                    ElementAttribute meta; if (ElementCache.TryGetMetaData(metaToken.Name, out meta))
                                    {
                                        State.Set(meta.Tokenization);
                                        if (meta.IsVoid)
                                            metaToken.Type = HtmlToken.SelfCloseTag;
                                    }
                                    else goto DataState;
                                }
                                break;
                            #endregion
                        }
                        return;
                    }
                default:
                DataState:
                    {
                        State.Set(HtmlTokenizerState.Data);
                    }
                    break;
            }
        }
    }
}
