// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Html
{
    public partial class HtmlTokenizer
    {
        ///<summary>
        /// HTML5 8.2.4.41. Bogus comment state
        /// https://www.w3.org/TR/html53/syntax.html#bogus-comment-state
        ///</summary>
        bool ReadBogusComment()
        {
            do
            {
                if (EndOfStream)
                    break;

                switch (GetCharacter())
                {
                    #region 8.2.4.1. Data state
                    case '>':
                        {
                            State.Set(HtmlTokenizerState.Data);
                        }
                        break;
                    #endregion

                    #region U+0000 NULL
                    case 0:
                        {
                            RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                            RawDataBuffer.Position++;
                        }
                        goto default;
                    #endregion

                    #region Anything else 
                    default:
                        {
                            textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.BogusComment);

            CreateCommentToken();
            return ProcessingFlags.Complete;
        }

        ///<summary>
        /// HTML5 8.2.4.43. Comment start state
        /// https://www.w3.org/TR/html53/syntax.html#comment-start-state
        ///</summary>
        bool ReadCommentStart()
        {
            metaToken.Type = HtmlToken.Comment;
            switch (PeekCharacter())
            {
                #region 8.2.4.44. Comment start dash state
                case '-':
                    {
                        State.Set(HtmlTokenizerState.CommentStartDash);
                        RawDataBuffer.Position++;

                        textBuffer.Clear();
                    }
                    break;
                #endregion

                #region 8.2.4.1. Data state
                case '>':
                    {
                        CreateDataToken(false);
                    }
                    return ProcessingFlags.Complete;
                #endregion

                #region 8.2.4.45. Comment state
                default:
                    {
                        State.Set(HtmlTokenizerState.Comment);
                        textBuffer.Clear();
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        ///<summary>
        /// HTML5 8.2.4.44. Comment start dash state
        /// https://www.w3.org/TR/html53/syntax.html#comment-start-dash-state
        ///</summary>
        bool ReadCommentStartDash()
        {
            if (EndOfStream)
            {
                State.Set(HtmlTokenizerState.Comment);
            }
            else switch (PeekCharacter())
            {
                #region 8.2.4.51. Comment end state
                case '-':
                    {
                        State.Set(HtmlTokenizerState.CommentEnd);
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion

                #region 8.2.4.1. Data state
                case '>':
                    {
                        State.Set(HtmlTokenizerState.Comment);
                    }
                    break;
                #endregion

                #region 8.2.4.45. Comment state
                default:
                    {
                        State.Set(HtmlTokenizerState.Comment);
                        textBuffer.Append('-');
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        ///<summary>
        /// HTML5 8.2.4.45. Comment state
        /// https://www.w3.org/TR/html53/syntax.html#comment-state
        ///</summary>
        bool ReadComment()
        {
            do
            {
                if (EndOfStream)
                {
                    CreateCommentToken();
                    return ProcessingFlags.Complete;
                }
                else switch(GetCharacter())
                {
                    #region 8.2.4.46. Comment less-than sign state
                    case '<':
                        {
                            State.Set(HtmlTokenizerState.CommentLessThan);
                        }
                        goto default;
                    #endregion

                    #region 8.2.4.50. Comment end dash state
                    case '-':
                        {
                            State.Set(HtmlTokenizerState.CommentEndDash);
                        }
                        break;
                    #endregion

                    #region U+0000 NULL
                    case 0:
                        {
                            RawDataBuffer.Buffer.Add(HtmlEncoding.ReplacementCharacter);
                            RawDataBuffer.Position++;
                        }
                        goto default;
                    #endregion

                    #region Anything else 
                    default:
                        {
                            textBuffer.Append(Char.ConvertFromUtf32((Int32)RawDataBuffer.Head));
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.Comment);
            return ProcessingFlags.Continue;
        }

        ///<summary>
        /// HTML5 8.2.4.46. Comment less-than sign state
        /// https://www.w3.org/TR/html53/syntax.html#comment-less-than-sign-state
        ///</summary>
        bool ReadCommentLessThan()
        {
            do
            {
                switch (GetCharacter())
                {
                    #region 8.2.4.47. Comment less-than sign bang state
                    case '!':
                        {
                            State.Set(HtmlTokenizerState.CommentLessThanBang);
                            textBuffer.Append('!');
                        }
                        break;
                    #endregion

                    #region U+003C LESS-THAN SIGN (<) 
                    case '<':
                        {
                            textBuffer.Append('<');
                        }
                        break;
                    #endregion

                    #region 8.2.4.45. Comment state
                    default:
                        {
                            State.Set(HtmlTokenizerState.Comment);
                            if (!EndOfStream)
                            {
                                RawDataBuffer.Position--;
                            }
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.CommentLessThan);
            return ProcessingFlags.Continue;
        }

        ///<summary>
        /// HTML5 8.2.4.47. Comment less-than sign bang state
        /// https://www.w3.org/TR/html53/syntax.html#comment-less-than-sign-bang-state
        ///</summary>
        bool ReadCommentLessThanBang()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.48. Comment less-than sign bang dash state
                case '-':
                    {
                        State.Set(HtmlTokenizerState.CommentLessThanBangDash);
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion

                #region 8.2.4.45. Comment state
                default:
                    {
                        State.Set(HtmlTokenizerState.Comment);
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        ///<summary>
        /// HTML5 8.2.4.48. Comment less-than sign bang dash state
        /// https://www.w3.org/TR/html53/syntax.html#comment-less-than-sign-bang-dash-state
        ///</summary>
        bool ReadCommentLessThanBangDash()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.49. Comment less-than sign bang dash dash state
                case '-':
                    {
                        State.Set(HtmlTokenizerState.CommentLessThanBangDashDash);
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion

                #region 8.2.4.45. Comment state
                default:
                    {
                        State.Set(HtmlTokenizerState.Comment);
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        ///<summary>
        /// HTML5 8.2.4.49. Comment less-than sign bang dash dash state
        /// https://www.w3.org/TR/html53/syntax.html#comment-less-than-sign-bang-dash-dash-state
        ///</summary>
        bool ReadCommentLessThanBangDashDash()
        {
            switch (PeekCharacter())
            {
                #region 8.2.4.51. Comment end state
                case '>':
                    {
                        State.Set(HtmlTokenizerState.CommentEnd);
                    }
                    break;
                default:
                    {
                        //parser error
                    }
                    goto case '>';
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        ///<summary>
        /// HTML5 8.2.4.50. Comment end dash state
        /// https://www.w3.org/TR/html53/syntax.html#comment-end-dash-state
        ///</summary>
        bool ReadCommentEndDash()
        {
            if (EndOfStream)
            {
                State.Set(HtmlTokenizerState.Comment);
            }
            else switch (PeekCharacter())
            {
                #region 8.2.4.51. Comment end state
                case '-':
                    {
                        State.Set(HtmlTokenizerState.CommentEnd);
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion

                #region 8.2.4.45. Comment state
                default:
                    {
                        State.Set(HtmlTokenizerState.Comment);
                        textBuffer.Append('-');
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }

        ///<summary>
        /// HTML5 8.2.4.51. Comment end state
        /// https://www.w3.org/TR/html53/syntax.html#comment-end-state
        ///</summary>
        bool ReadCommentEnd()
        {
            do
            {
                if (EndOfStream)
                {
                    CreateCommentToken();
                    return ProcessingFlags.Complete;
                }
                else switch(GetCharacter())
                {
                    #region 8.2.4.1. Data state
                    case '>':
                        {
                            CreateCommentToken();
                        }
                        return ProcessingFlags.Complete;
                    #endregion

                    #region 8.2.4.50. Comment end dash state
                    case '!':
                        {
                            State.Set(HtmlTokenizerState.CommentEndBang);
                        }
                        break;
                    #endregion

                    #region U+002D HYPHEN-MINUS (-) 
                    case '-':
                        {
                            textBuffer.Append('-');
                        }
                        break;
                    #endregion

                    #region 8.2.4.45. Comment state
                    default:
                        {
                            State.Set(HtmlTokenizerState.Comment);
                            RawDataBuffer.Position--;

                            textBuffer.Append("--");
                        }
                        break;
                    #endregion
                }
            }
            while (State.Current == HtmlTokenizerState.CommentEnd);
            return ProcessingFlags.Continue;
        }

        ///<summary>
        /// HTML5 8.2.4.52. Comment end bang state
        /// https://www.w3.org/TR/html53/syntax.html#comment-end-bang-state
        ///</summary>
        bool ReadCommentEndBang()
        {
            if (EndOfStream)
            {
                State.Set(HtmlTokenizerState.CommentEnd);
            }
            else switch (PeekCharacter())
            {
                #region 8.2.4.50. Comment end dash state 
                case '-':
                    {
                        textBuffer.Append("--!");
                        RawDataBuffer.Position++;
                    }
                    break;
                #endregion

                #region 8.2.4.1. Data state
                case '>':
                {
                    State.Set(HtmlTokenizerState.CommentEnd);
                }
                break;
                #endregion

                #region 8.2.4.45. Comment state
                default:
                    {
                        State.Set(HtmlTokenizerState.Comment);
                        textBuffer.Append("--!");
                    }
                    break;
                #endregion
            }
            return ProcessingFlags.Continue;
        }
    }
}
