// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Html
{
	/// <summary>
	/// HTML5 8.2.5. Tree construction compliant DOM builder
	/// https://www.w3.org/TR/html53/
	/// </summary>
	public partial class HtmlParser : TreeBuilder<HtmlToken, HtmlTokenizerState, HtmlParserState>
	{
		HtmlDocument document;
		
		bool allowScripts;

		List<string> errors;
		/// <summary>
		/// A collection of error messages occurred during build
		/// </summary>
		public List<string> Errors
		{
			get { return errors; }
		}

		public HtmlParser(HtmlDocument document)
		{
			this.document = document;
			this.openElements = new List<HtmlDomNode>();
			this.formatting = new List<HtmlDomNode>();
			this.head = null;
			this.form = null;
			this.errors = new List<string>();
			this.allowScripts = document.AllowScripts;
			this.framesetOkay = true;
			this.foster = false;
			this.originalInsertionMode = null;
			this.templateInsertionModes = new Stack<HtmlParserState>();
		}

		protected override StreamTokenizer<HtmlToken, HtmlTokenizerState> Begin(Stream stream, bool isUtf8)
		{
			// reset fields
			return new HtmlTokenizer(stream, isUtf8, allowScripts);
		}

		protected override bool DiscardToken(HtmlToken token, object context)
		{
			switch (token)
			{
				case HtmlToken.Whitespace: return true;
				default: return false;
			}
		}

		protected override bool ProcessToken(HtmlToken token, object context)
		{
		Head:
			switch (BuilderState.Current)
			{
				#region 8.2.5.4.1. The "initial" insertion mode
				case HtmlParserState.Initial:
					{
						if (!InitialMode(token))
							goto Head;
					}
					break;
				#endregion
				
				#region 8.2.5.4.2. The "before html" insertion mode
				case HtmlParserState.BeforeHtml:
				{
					if (!BeforeHtmlMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.3. The "before head" insertion mode
				case HtmlParserState.BeforeHead:
				{
					if (!BeforeHeadMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.4. The "in head" insertion mode
				case HtmlParserState.InHead:
				{
					if (!InHeadMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.5. The "in head noscript" insertion mode
				case HtmlParserState.InHeadNoScript:
				{
					if (!InHeadNoScriptMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.6. The "after head" insertion mode
				case HtmlParserState.AfterHead:
				{
					if (!AfterHeadMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.7. The "in body" insertion mode
				case HtmlParserState.InBody:
				{
					if (!InBodyMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.8. The "text" insertion mode
				case HtmlParserState.Text:
				{
					if (!TextMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.9. The "in table" insertion mode
				case HtmlParserState.InTable:
				{
					if (!InTableMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.10. The "in table text" insertion mode
				case HtmlParserState.InTableText:
				{
					if (!InTableTextMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.11. The "in caption" insertion mode
				case HtmlParserState.InCaption:
				{
					if (!InCaptionMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.12. The "in column group" insertion mode
				case HtmlParserState.InColumnGroup:
				{
					if (!InColumnGroupMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.13. The "in table body" insertion mode
				case HtmlParserState.InTableBody:
				{
					if (!InTableBodyMode(token))
							goto Head;
				}
				break;
				
				
				#region 8.2.5.4.14. The "in row" insertion mode
				case HtmlParserState.InRow:
				{
					if (!InRowMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.15. The "in cell" insertion mode
				case HtmlParserState.InCell:
				{
					if (!InCellMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.16. The "in select" insertion mode
				case HtmlParserState.InSelect:
				{
					if (!InSelectMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.17. The "in select in table" insertion mode
				case HtmlParserState.InSelectInTable:
				{
					if (!InSelectInTableMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.18. The "in template" insertion mode
				case HtmlParserState.InTemplate:
				{
					if (!InTemplateMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.19. The "after body" insertion mode
				case HtmlParserState.AfterBody:
				{
					if (!AfterBodyMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.20. The "in frameset" insertion mode
				case HtmlParserState.InFrameset:
				{
					if (!InFramesetMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.21. The "after frameset" insertion mode
				case HtmlParserState.AfterFrameset:
				{
					if (!AfterFramesetMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.22. The "after after body" insertion mode
				case HtmlParserState.AfterAfterBody:
				{
					if (!AfterAfterBodyMode(token))
							goto Head;
				}
				break;
				
				#region 8.2.5.4.23. The "after after frameset" insertion mode
				case HtmlParserState.AfterAfterFrameset:
				{
					if (!AfterAfterFramesetMode(token))
							goto Head;
				}
				break;

				#region Failure
				default:
				case HtmlParserState.Failure:
					{
						MoveToEnd();
					}
					return false;
				#endregion
			}
			return true;
		}

		protected override bool Finalize(bool result, object context)
		{
			return result;
		}

				
		
		
		
		
		
	}
}
