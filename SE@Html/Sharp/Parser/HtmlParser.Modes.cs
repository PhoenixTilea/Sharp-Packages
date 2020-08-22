using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Html
{
	public partial class HtmlParser
	{
		HtmlParserState originalInsertionMode;
		Stack<HtmlParserState> templateInsertionModes;
		
		private HtmlParserState CurrentTemplateMode
		{
			get
			{
				int modes = templateInsertionModes.Count;
				if (modes > 0) return templateInsertionModes[modes - 1];
				else return null;
			}
		}
		
		#region HTML5 8.2.5.4.1. The "initial" insertion mode
		/// <summary>
		/// https://www.w3.org/TR/html53/syntax.html#the-initial-insertion-mode
		/// </summary>
		ProductionState ProcessInitialMode(HtmlToken token)
		{
			switch (token)
			{
				#region Comment
				case HtmlToken.Comment:
					{
						document.AddChild(CreateCommentNode());
					}
					break;
				#endregion

				#region Doctype
				case HtmlToken.Doctype:
					{
						DoctypeNode node = CreateDoctypeNode();
						document.AddChild(node);
						document.Doctype = node;
						BuilderState.Set(HtmlParserState.BeforeHtml);
					}
					break;
				#endregion

				#region anything else
				default:
					{
						// error
						document.Mode = Quirks.Quirks;
						BuilderState.Set(HtmlParserState.BeforeHtml);
					}
					return ProductionState.Preserve;
					#endregion
			}
			return ProductionState.Success;
		}
		bool InitialMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInitialMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.2. The "before html" insertion mode
		/// <summary>
		/// https://www.w3.org/TR/html53/syntax.html#the-before-html-insertion-mode
		/// </summary>
		ProductionState ProcessBeforeHtmlMode(HtmlToken token)
		{
			switch (token)
			{
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region comment
				case HtmlToken.Comment:
				{
					document.AddChild(CreateCommentNode());
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					if (name == "html")
					{
						ElementNode node = CreateElementNode();
						document.AddChild(node);
						OpenElement(node);
						BuilderState.Set(HtmlParserState.BeforeHead);
					}
					else goto AnythingElse;
				}
				break;
				#endregion
				
				#region close tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						case "head":
						case "body":
						case "html":
						case "br": goto AnythingElse;
						default:
						{
							// error
						}
						break;
					}
				}
				break;
				#endregion
				
				#region anything else
				default:
				AnythingElse:
				{
					ElementNode node = CreateElementNode("html");
					document.AddChild(node);
					OpenElement(node);
					BuilderState.Set(HtmlParserState.BeforeHead);
				}
				return ProductionState.Preserve;
				#endregion
			}
			return ProductionState.Success;
		}
		bool BeforeHtmlMode(HtmlToken token)
		{
			return FinalizeRule(ProcessBeforeHtmlMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.3. The "before head" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessBeforeHeadMode(HtmlToken token)
		{
			switch (token)
			{
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html": return ProcessInBodyMode(token);
						case "head":
						{
							ElementNode node = CreateElementNode();
							head = node;
							BuilderState.Set(HtmlParserState.InHead);
						}
						break;
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						case "head":
						case "body":
						case "html":
						case "br": goto AnythingElse;
						default:
						{
							// error
						}
						break;
					}
				}
				break;
				#endregion
				
				#region anything else
				AnythingElse:
				default:
				{
					ElementNode node = CreateElementNode("head");
					head = node;
					BuilderState.Set("in head");
				}
				return ProductionState.Preserve;
			}
			return ProductionState.Success;
		}
		bool BeforeHeadMode(HtmlToken token)
		{
			return FinalizeRule(ProcessBeforeHeadMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.4. The "in head" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInHeadMode(HtmlToken token)
		{
			switch (token)
			{
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html": return ProcessInBodyMode(token);
						#endregion
						
						#region self-closing tags
						case "base":
						case "basefont":
						case "bgsound":
						case "link":
						case "meta":
						{
							InsertNode(CreateElementNode(null, true));
						}
						break;
						#endregion
						
						#region title
						case "title":
						{
							RcDataParsing();
						}
						break;
						#endregion
						
						#region noscript
						case "noscript":
						case "noframes":
						case "style":
						{
							if (name != "noscript" || allowScrips)
							{
								RawTextParsing();
							}
							else
							{
								InsertNode(CreateElementNode());
								BuilderState.Set(HtmlParserState.InHeadNoScript);
							}
						}
						break;
						#endregion
						
						#region script
						case "script":
						{
							
						}
						break;
						#endregion
						
						#region "template"
						case "template":
						{
							InsertNode(CreateelementNode());
							AddFormattingElement();
							framesetOkay = false;
							BuilderState.Set(HtmlParserState.InTemplate);
							templateInsertionModes.Push(HtmlParserState.InTemplate);
						}
						break;
						#endregion
						
						#region "head":
						case "head":
						{
							// error
						}
						break;
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region head
						case "head":
						{
							CloseElement(Current);
							BuilderState.Set(HtmlParserState.AfterHead);
						}
						break;
						#endregion
						
						#region body, html, br
						case "body":
						case "html":
						case "br": goto AnythingElse;
						#endregion
						
						#region "template"
						case "template":
						{
							if (!HasOpenElement(Template))
							{
								// error
								break;
							}
							
							GenerateAllEndTags();
							if (!(Current.Element is Template))
							{
								// error
							}
							ClearStackTo(Template);
							ClearFormattingToMarker();
							templateInsertionModes.Pop();
							ResetInsertionMode();
						}
						break;
						#endregion
						
						#region any other end tag
						default:
						{
							// error
						}
						break;
					}
				}
				break;
				#endregion
				
				#region anything els3
				default:
				AnythingElse:
				{
					CloseElement(Current);
					BuilderState.Set(HtmlParserState.AfterHead);
				}
				return ProductionState.Preserve;
				#endregion
			}
			return ProductionState.Success;
		}
		bool InHeadMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInHeadMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.5. The "in head noscript" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInHeadNoScriptMode(HtmlToken token)
		{
			switch (token)
			{
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html": return ProcessInBodyMode(token);
						#endregion
						
						#region process in head
						case "basefont":
						case "bgsound":
						case "link":
						case "meta":
						case "noframes":
						case "style": return ProcessInHeadMode(token);
						#endregion
						
						#region head, noscript
						case "head":
						case "noscript":
						{
							// error
						}
						break;
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region noscript
						case "noscript":
						{
							CloseElement(Current);
							BuilderState.Set(HtmlParserState.InHead);
						}
						break;
						#endregion
						
						#region br
						case "br": goto AnythingElse;
						#endregion
						
						#region any other end tag
						default:
						{
							// error
						}
						break;
						#endregion
					}
				}
				break;
				#endregion
				
				#region comment
				case HtmlToken.Comment: return ProcessInHeadMode(token);
				#endregion
				
				#region anything else
				default:
			AnythingElse:
				{
					// error
					CloseElement(Current);
					BuilderState.Set(HtmlParserState.InHead);
				}
				return ProductionState.Preserve;
			}
			return ProductionState.Success;
		}
		bool InHeadNoScriptMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInHeadNoScriptMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.6. The "after head" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessAfterHeadMode(HtmlToken token)
		{
			switch (token)
			{
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html": return ProcessInBodyMode(token);
						#endregion
						
						#region body
						case "body":
						{
							InsertNode(CreateElementNode());
							BuilderState.Set(HtmlParserState.InBody);
							framesetOkay = false;
						}
						break;
						#endregion
						
						#region frameset
						case "frameset":
						{
							InsertNode(CreateElementNode());
							BuilderState.Set(HtmlParserState.InFrameset);
						}
						break;
						#endregion
						
						#region head only tags
						case "base":
						case "basefont":
						case "bgsound":
						case "meta":
						case "link":
						case "noframes":
						case "script":
						case "style":
						case "template":
						case "title":
						{
							// error
							OpenElement(head);
							ProcessInHeadMode(token);
							CloseElement(head);
						}
						break;
						#endregion
						
						#region head
						case "head":
						{
							// error
						}
						break;
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetatoken().Name;
					switch (name)
					{
						#region "template"
						case "template": return ProcessInHeadMode(token);
						#endregion
						
						#region jump to AnythingElse
							case "body":
						case "html":
						case "br": goto AnythingElse;
						#endregion
						
						#region any other end tag
						default:
						{
							// error
						}
						break;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
				default:
			AnythingElse:
				{
					InsertNode(CreateElementNode("body"));
					BuilderState.Set(HtmlParserState.InBody);
				}
				return ProductionState.Preserve;
				#endregion
			}
			return ProductionState.Success;
		}
		bool AfterHeadMode(HtmlToken token)
		{
			return FinalizeRule(ProcessAfterHeadMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.7. The "in body" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInBodyMode(HtmlToken token)
		{
			switch (token)
			{
				#region text
				case HtmlToken.Data:
				{
					ReconstructFormattingElements();
					InsertNode(CreateTextNode());
					framesetOkay = false;
				}
				break;
				#endregion
				
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html":
						{
							// error
							if (HasOpenElement(Template)) break;
							Diction<string, string> props = GetMetaToken().Properties;
							HtmlElement html = openElements[0].Element;
							foreach (string prop in props.Keys())
							{
								html.SetProperty(prop, props[prop]);
							}
						}
						break;
						#endregion
						
						#region process in head
						case "base":
						case "basefont":
						case "bgsound":
						case "link":
						case "meta":
						case "noframes":
						case "script":
						case "style":
						case "title":
						case "template": return ProcessInHeadMode(token);
						#endregion
						
						#region body
						case "body":
						{
							// error
							if (openelements.Count < 2 || !(openElements[1].Element is Body) || HasOpenElement(Template))
								break;
							
							framesetOkay = false;
							Dictionary<string, string> props = GetMetatoken().Properties;
							HtmlElement body = openelements[1].Element;
							foreach (string prop in props.Keys())
							{
								body.SetProperty(prop, props[prop]);
							}
						}
						break;
						#endregion
						
						#region frameset
						case "frameset":
						{
							// error
							if (!framesetOkay || openElements.Count < 2 || !(openElements[1].Element is Body))
								break;
							
							if (openElements[1].Parent != null)
								openElements[1].Parent.RemoveChild(openElements[1]);
							ClearStackTo(Html);
							InsertNode(CreateElementNode());
							BuilderState.Set(HtmlParserState.InFrameset);
						}
						break;
						#endregion
						
						#region sections
						case "address":
						case "article":
						case "aside":
						case "blockquote":
						case "center":
						case "details":
						case "dialog":
						case "dir":
						case "div":
						case "dl":
						case "fieldset":
						case "figcaption":
						case "figure":
						case "footer":
						case "header":
						case "main":
						case "nav":
						case "ol":
						case "p":
						case "section":
						case "summary":
						case "ul":
						{
							if (IsInButtonScope(P))
							{
								ClosePElement();
							}
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region heading
						case "h1":
						case "h2":
						case "h3":
						case "h4":
						case "h5":
						case "h6":
						{
							if (IsInButtonScope(P))
							{
								ClosePElement();
							}
							switch (Current.Element.Name)
							{
								case "h1":
								case "h2":
								case "h3":
								case "h4":
								case "h5":
								case "h6":
								{
									// error
									CloseElement(Current);
								}
								break;
							}
							InsertNode(CreateelementNode());
						}
						break;
						#endregion
						
						#region pre, listing
						case "pre":
						case "listing":
						{
							if (IsInButtonScope(P))
							{
								ClosePElement();
							}
							InsertNode(CreateElementNode());
							framesetOkay = false;
						}
						break;
						#endregion
						
						#region form
						case "form":
						{
							ElementNode t;
							if (form != null || !(HasOpenElement(Template, out t))
							{
								// error
								break;
							}
							if (IsInButtonScope(P))
							{
								ClosePElement();
							}
							ElementNode f = CreateElementNode();
							InsertNode(f);
							if (t == null) form = f;
						}
						break;
						#endregion
						
						#region li
						case "li":
						{
							framesetOkay = false;
							ElementNode node = Current;
							
							for ( ; ; )
							{
								if (node.Element is Li)
								{
									GenerateEndTags(null, [Li]);
									if (!(Current.Element is Li))
									{
										// error
									}
									ClearStackTo(Li);
									break;
								}
								if (IsSpecial(node) && !(node.Element is Address || node.Element is Div || node.Element is P))
									break;
								else
									node = openElements[openelements.IndexOf(node) - 1];
							}
							
							if (IsInButtonScope(P))
							{
								ClosePElement();
							}
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region dd, dt
						case "dd":
						case "dt":
						{
							framesetOkay = false;
							ElementNode node = Current;
							
							for ( ; ; )
							{
								if (node.Element is Dd)
								{
									GenerateEndTags(null, [Dd]);
									if (!(Current.Element is Dd))
									{
										// error
									}
									ClearStackTo(Dd);
									break;
								}
								if (node.Element is Dt)
								{
									GenerateEndTags(null, [Dt]);
									if (!(Current.Element is Dt))
									{
										// error
									}
									ClearStackTo(Dt);
									break;
								}
								if (IsSpecial(node) && !(node.Element is Address || node.Element is Div || node.Element is P))
									break;
								else
									node = openElements[openelements.IndexOf(node) - 1];
							}
							
							if (IsInButtonScope(P))
							{
								ClosePElement();
							}
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region plaintext
						case "plaintext":
						{
							if (IsInButtonScope(P))
							{
								ClosePElement();
							}
							InsertNode(CreateelementNode());
							tokenizer.State.Set(HtmlTokenizerState.PlainText);
						}
						break;
						#endregion
						
						#region button
						case "button":
						{
							ElementNode button; if (IsInScope(Button, out button))
							{
								// error
								GenerateEndTags();
								ClearStackTo(Button);
							}
							ReconstructFormattingelements();
							InsertNode(CreateElementNode());
							framesetOkay = false;
						}
						break;
						#endregion
						
						#region a
						case "a":
						{
							ElementNode a; if (FindAfterLastMarker("a", out a))
							{
								// error
								AdoptionAgency(GetMetaToken());
								CloseElement(a);
							}
							reconstructFormattingElements();
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region formatting elements
						case "b":
						case "big":
						case "code":
						case "em":
						case "font":
						case "i":
						case "s":
						case "small":
						case "strike":
						case "strong":
						case "tt":
						case "u":
						{
							ReconstructFormattingElements();
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region nobr
						case "nobr":
						{
							ReconstructFormattingElements();
							if (IsInScope(Nobr))
							{
								// error
								AdoptionAgency(GetMetaToken());
								ReconstructFormattingElements();
							}
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region applet, marquee, object
						case "applet":
						case "marquee":
						case "object":
						{
							ReconstructFormattingElements();
							InsertNode(CreateElementNode());
							AddFormattingElement(null);
							framesetOkay = false;
						}
						break;
						#endregion
						
						#region table
						case "table":
						{
							if (document.Mode != Quirks.Quirks && IsInButtonScope(P))
							{
								ClosePElement();
							}
							InsertNode(CreateElementNode());
							framesetOkay = false;
							BuilderState.Set(HtmlParserState.InTable);
						}
						break;
						#endregion
						
						#region area, br, embed, img, wbr
						case "area":
						case "br":
						case "embed":
						case "img":
						case "wbr":
						{
							reconstructFormattingElements();
							InsertNode(CreateElementNode());
							framesetOkay = false;
						}
						break;
						#endregion
						
						#region input
						case "input":
						{
							ReconstructFormattingElements();
							ElementNode input = CreateElementNode();
							InsertNode(input);
							string typeProp = input.Element.GetProperty("type");
							if (string.IsNullOrEmpty(typeProp) || typeProp != "hidden")
								framesetOkay = false;
						}
						break;
						#endregion
						
						#region param, source, track
						case "param":
						case "source":
						case "track":
						{
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region hr
						case "hr":
						{
							if (IsInButtonScope(P))
							{
								ClosePElement();
							}
							InsertNode(CreateElementNode());
							framesetOkay = false;
						}
						break;
						#endregion
						
						#region image
						case "image":
						{
							GetMetaToken().Name = "img";
						}
						return ProductionState.Preserve;
						#endregion
						
						#region textarea
						case "textarea":
						{
							InsertNode(CreateelementNode());
							tokenizer.State.Set(HtmlTokenizerState.RcData);
							originalInsertionMode = BuilderState.Current;
							framesetOkay = false;
							BuilderState.Set(HtmlParserState.Text);
						}
						break;
						#endregion
						
						#region xmp
						case "xmp":
						{
							if (IsInButtonScope(P))
							{
								ClosePElement();
							}
							reconstructFormattingElements();
							framesetOkay = false;
							RawTextParsing();
						}
						break;
						#endregion
						
						#region iframe
						case "iframe":
						{
							framesetOkay = false;
							RawTextParsing();
						}
						break;
						#endregion
						
						#region noembed
						case "noembed":
						case "noscript":
						{
							if (name == "noscript" && !allowScript) break;
							RawTextParsing();
						}
						break;
						#endregion
						
						#region select
						case "select":
						{
							ReconstructFormattingElements();
							InsertNode(CreateElementNode());
							framesetOkay = false;
							switch (BuilderState.Current)
							{
								case HtmlParserState.InTable:
								case HtmlParserState.InCaption:
								case HtmlParserState.InTableBody:
								case HtmlParserState.InRow:
								case HtmlParserState.InCell:
								{
									BuilderState.Set(HtmlParserState.InSelectInTable);
								}
								break;
								
								default:
								{
									BuilderState.Set(HtmlParserState.InSelect);
								}
								break;
							}
						}
						break;
						#endregion
						
						#region optgropu, option
						case "optgroup":
						case "option":
						{
							if (Current.Element is Option)
							{
								CloseElement(Current);
							}
							ReconstructFormattingelements();
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region rb
						case "rb":
						{
							if (IsInScope(Rb))
							{
								GenerateEndTags();
								if (!(Current.Element is Rb || (Current.Parent != null && Current.Parent.Element is Rb))
								{
									// error
								}
							}
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region rp, rt
						case "rp":
						case "rt":
						{
							if (IsInScope(Rb))
							{
								GenerateEndTags(null, [Rtc]);
								if (!(Current.Element is Rtc || Current.Element is Rb))
								{
									// error
								}
							}
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region rtc
						case "rtc":
						{
							if (IsInScope(Rb))
							{
								GenerateEndTags();
								if (!(Current.Element is Rb))
								{
									// error
								}
							}
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region math
						case "math":
						{
							ReconstructFormattingElements();
							// handle mathML
						}
						break;
						#endregion
						
						#region svg
						case "svg":
						{
							ReconstructFormattingelements();
							// handle svg
						}
						break;
						#endregion
						
						#region invalid start tag
						case "caption":
						case "col":
						case "colgroup":
						case "frame":
						case "head":
						case "tbody":
						case "td":
						case "tfoot":
						case "th":
						case "thead":
						case "tr":
						{
							// error
						}
						break;
						#endregion
						
						#region any other start tag
						default:
						{
							ReconstructFormattingElements();
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
					}
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region template
						case "template": return ProcessInHeadMode(token);
						#endregion
						
						#region body or html
						case "body":
						case "html":
						{
							ElementNode body; if (IsInScope(Body, out body))
							{
								// error
								break;
							}
							foreach (ElementNode node in openElements)
							{
								string tagName = node.Element.Name;
								switch (tagName)
								{
									case "dd":
									case "dt":
									case "li":
									case "optgroup":
									case "option":
									case "p":
									case "rb":
									case "rp":
									case "rt":
									case "rtc":
									case "tbody":
									case "td":
									case "tfoot":
									case "th":
									case "thead":
									case "tr":
									case "body":
									case "html": continue;
									default:
									{
										// error
									}
								}
							}
							BuilderState.Set(HtmlParserState.AfterBody);
							if (name == "html") return ProductionState.Preserve;
						}
						break;
						#endregion
						
						#region section
						case "address":
						case "article":
						case "aside":
						case "blockquote":
						case "button":
						case "center":
						case "details":
						case "dialog":
						case "dir":
						case "div":
						case "dl":
						case "fieldset":
						case "figcaptions":
						case "figure":
						case "footer":
						case "header":
						case "listing":
						case "main":
						case "nav":
						case "ol":
						case "pre":
						case "section":
						case "summary":
						case "ul":
						{
							Type type; if (TryGetElementType(name, out type))
							{
								if (!IsInScope(type))
								{
									// error
									break;
								}
							}
							else break;
							
							GenerateEndTags();
							if (!(Current.Element is type))
							{
								// error
							}
							ClearStackTo(type);
						}
						break;
						#endregion
						
						#region form
						case "form":
						{
							ElementNode t; if (!HasOpenElement(Template, out t))
							{
								ElementNode node = form;
								form = null;
								if (node == null || !openElements.Contains(node))
								{
									// error
									break;
								}
								GenerateEndTags();
								if (Current != node)
								{
									// error
								}
								CloseElement(node);
							}
							else
							{
								if (!IsInScope(Form))
								{
									// error
									break;
								}
								GenerateEndTags();
								if (!(Current.Element is Form))
								{
									// error
								}
								ClearStackTo(Form);
							}
						}
						break;
						#endregion
						
						#region p
						case "p":
						{
							if (!IsInButtonScope(P))
							{
								// error
								InsertNode(CreateElementNode("p"));
							}
							ClosePElement();
						}
						break;
						#endregion
						
						#region li
						case "li":
						{
							if (IsInListItemScope(Li))
							{
								// error
								break;
							}
							GenerateEndTags(null, [Li]);
							if (!(Current.Element is Li))
							{
								// error
							}
							ClearStackTo(Li);
						}
						break;
						#endregion
						
						#region dd, dt
						case "dd":
						case "dt":
						{
							Type type; if (ElementCache.TryGetElementType(name, out type))
							{
								if (!IsInScope(type))
								{
									// error
									break;
								}
							}
							else break;
							GenerateEndTags(null, type);
							if (!(Current.Element is type))
							{
								// error
							}
							ClearStackTo(type);
						}
						break;
						#endregion
						
						#region headings
						case "h1":
						case "h2":
						case "h3":
						case "h4":
						case "h5":
						case "h6":
						{
							if (!(HasInScope(H1) || HasInScope(H2) || HasInScope(H3) ||
								HasInScope(H4) || HasInScope(H5) || HasInScope(H6))
							{
								// error
								break;
							}
							GenerateEndTags();
							if (Current.Element.Name != name)
							{
								// error
							}
							ClearStackTo([H1, H2, H3, H4, H5, H6]);
						}
						break;
						#endregion
						
						#region formatting
						case "a":
						case "b":
						case "big":
						case "code":
						case "em":
						case "font":
						case "i":
						case "nobr":
						case "s":
						case "small":
						case "strike":
						case "strong":
						case "tt":
						case "u":
						{
							AdoptionAgency(GetMetaToken());
						}
						break;
						#endregion
						
						#region applet, marquee, object
						case "applet":
						case "marquee":
						case "object":
						{
							Type type; if (ElementCache.TryGetElementType(name, out type))
							{
								if (!IsInScope(type))
								{
									// error
									break;
								}
							}
							else break;
							
							GenerateEndTags();
							if (!(Current.Element is type))
							{
								// error
							}
							ClearStackTo(type);
							ClearToLastMarker();
						}
						break;
						#endregion
						
						#region br
						case "br":
						{
							ReconstructFormattingElements();
							InsertNode(CreateElementNode("br"));
							framesetOkay = false;
						}
						break;
						#endregion
						
						#region any other end tag
						default:
						{
							Type type; if (!ElementCache.TryGetElementType(name, out type))
								break;
							ElementNode node = Current;
							for ( ; ; )
							{
								if (node.Element.Name == name)
								{
									GenerateEndTags(null, [type]);
									if (node != Current)
									{
										// error
									}
									do
									{
										ElementNode cur = Current;
										CloseElement(Current);
									}
									while (cur != node);
									break;
								}
								else if (IsSpecial(node))
								{
									// error
									return ProductionState.Success;
								}
								node = openElements[openElements.IndexOf(node) - 1];
							}
						}
						break;
						#endregion
					}
				}
				break;
				#endregion
				
			}
			return ProductionState.Success;
		}
		bool InBodyMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInBodyMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.8. The "text" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessTextMode(HtmlToken token)
		{
			switch (token)
			{
				#region data
				case HtmlToken.Data:
				{
					InsertNode(CreateTextNode());
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.CloseTag:
				{
					CloseElement(Current);
					BuilderState.Set(originalInsertionMode);
				}
				break;
				#endregion
			}
			return ProductionState.Success;
		}
		bool TextMode(HtmlToken token)
		{
			return FinalizeRule(ProcessTextMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.9. The "in table" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInTableMode(HtmlToken token)
		{
			switch (token)
			{
				#region data
				case HtmlToken.Data:
				{
					switch (Current.Element.Name)
					{
						case "table":
						case "tbody":
						case "tfoot":
						case "thead":
						case "tr":
						{
							// pending tokens?
							originalInsertionMode = BuilderState.Current;
							BuilderState.Set(HtmlParserState.InTableText);
						}
						return ProductionState.Preserve;
						#endregion
					}
				}
				break;
				#endregion
				
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region caption
						case "caption":
						{
							ClearToTableContext();
							AddFormattingElement();
							InsertNode(CreateElementNode());
							BuilderState.Set(HtmlParserState.InCaption);
						}
						break;
						#endregion
						
						#region colgroup
						case "colgroup":
						{
							ClearToTableContext();
							InsertNode(CreateElementNode());
							BuilderState.Set(HtmlParserState.InColumnGroup);
						}
						break;
						#endregion
						
						#region col
						case "col":
						{
							ClearToTableContext();
							InsertNode(CreateElementNode("colgroup"));
							BuilderState.Set(HtmlParserState.InColumnGroup);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region table regions
						case "tbody":
						case "tfoot":
						case "thead":
						{
							ClearToTableContext();
							InsertNode(CreateElementNode());
							BuilderState.Set(HtmlParserState.InTableBody);
						}
						break;
						#endregion
						
						#region cell or row
						case "td":
						case "th":
						case "tr":
						{
							ClearToTableContext();
							InsertNode(CreateElementNode("tbody"));
							BuilderState.Set(HtmlParserState.InTableBody);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region table
						case "table":
						{
							// error
							if (!IsInTableScope(Table))
							{
								break;
							}
							ClearStackTo(Table);
							ResetInsertionMode();
						}
						return ProductionState.Preserve;
						#endregion
						
						#region style, script, template
						case "style":
						case "script":
						case "template": return ProcessInHeadMode(token);
						#endregion
						
						#region input
						case "input":
						{
							HtmlMetaData data = GetMetaToken();
							string type; if (!data.Properties.TryGetValue("TYPE", out type) || type.ToLowerInvarient() != "hidden")
							{
								goto AnythingElse;
							}
							else
							{
								// error
								InsertNode(CreateElementNode());
							}
						}
						break;
						#endregion
						
						#region form
						case "form":
						{
							// error
							if (HasOpenElement(Template) || form != null) break;
							else
							{
								ElementNode f = CreateelementNode();
								form = f;
								CloseElement(f);
							}
						}
						break;
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region table
						case "table":
						{
							if (!IsInTableScope(Table))
							{
								// error
								break;
							}
							ClearStackTo(Table);
							ResetInsertionMode();
						}
						break;
						#endregion
						
						#region invalid end tag
						case "body":
						case "caption":
						case "col":
						case "colgroup":
						case "html":
						case "tbody":
						case "td":
						case "tfoot":
						case "th":
						case "thead":
						case "tr":
						{
							// error
						}
						break;
						#endregion
						
						#region template
						case "template": return ProcessInHeadMode(token);
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
				default:
			AnythingElse:
				{
					// error
					foster = true;
					ProcessInBodyMode(token);
					foster = false;
				}
				break;
				#endregion
			}
			return ProductionState.Success;
		}
		bool InTableMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInTableMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.10. The "in table text" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInTableTextMode(HtmlToken token)
		{
			switch (token)
			{
				// 
			}
			return ProductionState.Success;
		}
		bool InTableTextMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInTableTextMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.11. The "in caption" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInCaptionMode(HtmlToken token)
		{
			switch (token)
			{
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region invalid start tag
						case "caption":
						case "col":
						case "colgroup":
						case "tbody":
						case "td":
						case "tfoot":
						case "th":
						case "thead":
						case "tr":
						{
							if (!IsInTableScope(Caption))
							{
								// error
								break;
							}
							GenerateEndTags();
							if (!(Current.Element is Caption))
							{
								// error
							}
							ClearStackTo(Caption);
							ClearToLastMarker();
							BuilderState.Set(HtmlParserState.InTable);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region caption
						case "caption":
						{
							if (!IsInTableScope(Caption))
							{
								// error
								break;
							}
							GenerateEndTags();
							if (!(Current.Element is Caption))
							{
								// error
							}
							ClearStackTo(Caption);
							ClearToLastMarker();
							BuilderState.Set(HtmlParserState.InTable);
						}
						break;
						#endregion
						
						#region table
						case "table":
						{
							if (!IsInTableScope(Caption))
							{
								// error
								break;
							}
							GenerateEndTags();
							if (!(Current.Element is Caption))
							{
								// error
							}
							ClearStackTo(Caption);
							ClearToLastMarker();
							BuilderState.Set(HtmlParserState.InTable);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region invalid end tag
						case "body":
						case "col":
						case "colgroup":
						case "html":
						case "tbody":
						case "td":
						case "tfoot":
						case "th":
						case "thead":
						case "tr":
						{
							// error
						}
						break;
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
				default: 
			AnythingElse: return ProcessInBodyMode(token);
				#endregion
			}
			return ProductionState.Success;
		}
		bool InCaptionMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInCaptionMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.12. The "in column group" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInColumnGroupMode(HtmlToken token)
		{
			switch (token)
			{
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html": return ProcessInBodyMode(token);
						#endregion
						
						#region col
						case "col":
						{
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region template
						case "template": return ProcessInHeadMode(token);
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region colgroup
						case "colgroup":
						{
							if (!(Current.Element is Colgroup))
							{
								// error
								break;
							}
							closeElement(Current);
							BuilderState.Set(HtmlParserState.InTable);
						}
						break;
						#endregion
						
						#region col
						case "col":
						{
							// error
						}
						break;
						#endregion
						
						#region template
						case "template": return ProcessInHeadMode(token);
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
				default:
				AnythingElse:
				{
					if (!(Current.Element is Colgroup))
					{
						// error
						break;
					}
					CloseElement(Current);
					BuilderState.Set(HtmlParserState.InTable);
				}
				return ProductionState.Preserve;
				#endregion
			}
			return ProductionState.Success;
		}
		bool InColumnGroupMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInColumnGroupMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.13. The "in table body" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInTableBodyMode(HtmlToken token)
		{
			switch (token)
			{
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region tr
						case "tr":
						{
							ClearToTableBodyContext();
							InsertNode(CreateElementNode());
							BuilderState.Set(HtmlParserState.InRow);
						}
						break;
						#endregion
						
						#region cell
						case "td":
						case "th":
						{
							// error
							ClearToTableBodyContext();
							InsertNode(CreateElementNode("tr"));
							BuilderState.Set(HtmlParserState.InRow);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region return to in body mode
						case "caption":
						case "col":
						case "colgroup":
						case "tbody":
						case "tfoot":
						case "thead":
						{
							if (!(IsInTableScope(Tbody) || IsInTableScope(Tfoot) || IsInTableScope(Thead))
							{
								// error
								break;
							}
							ClearToTableBodyContext();
							CloseElement(Current);
							BuilderState.Set(HtmlParserState.InTable);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region closing table body
						case "tbody":
						case "tfoot":
						case "thead":
						{
							Type type = ElementCache.GetElementType(name);
							if (!IsInTableScope(type))
							{
								// error
								break;
							}
							ClearToTableBodycontext();
							CloseElement(Current);
							BuilderState.Set(HtmlParserState.InTable);
						}
						break;
						#endregion
						
						#region table
						case "table":
						{
							if (!(IsInTableScope(Tbody) || IsInTableScope(Tfoot) || IsInTableScope(Thead))
							{
								// error
								break;
							}
							ClearToTableBodyContext();
							CloseElement(Current);
							BuilderState.Set(HtmlParserState.InTable);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region invalid end tag
						case "body":
						case "caption":
						case "col":
						case "colgroup":
						case "html":
						case "td":
						case "td":
						case "tr":
						{
							// error
						}
						break;
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
				default: 
			AnythingElse: return ProcessInTableMode(token);
				#endregion
			}
			return ProductionState.Success;
		}
		bool InTableBodyMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInTableBodyMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.14. The "in row" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInRowMode(HtmlToken token)
		{
			switch (token)
			{
				#region start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						case "td":
						case "th":
						{
							ClearToTableRowContext();
							InsertNode(CreateElementNode());
							BuilderState.Set(HtmlParserState.InCell);
							AddFormattingElement();
						}
						break;
						#endregion
						
						#region reprocess in table body
						case "caption":
						case "col":
						case "colgroup":
						case "tbody":
						case "tfoot":
						case "thead":
						case "tr":
						{
							if (!IsInTableScope(Tr))
							{
								// error
								break;
							}
							ClearToTableRowContext();
							Closeelement(Current);
							BuilderState.Set(HtmlParserState.InTableBody);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region end tag
				case HtmlToken.ClseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region end row
						case "tr":
						{
							if (!IsInTableScope(Tr))
							{
								// error
								break;
							}
							ClearToTableRowContext();
							Closeelement(Current);
							BuilderState.Set(HtmlParserState.InTableBody);
						}
						break;
						#endregion
						
						#region table
						case "table":
						{
							if (!IsInTableScope(Tr))
							{
								// error
								break;
							}
							ClearToTableRowContext();
							Closeelement(Current);
							BuilderState.Set(HtmlParserState.InTableBody);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region end table body
						case "tbody":
						case "tfoot":
						case "thead":
						{
							Type type = ElementCache.TryGetElementType(name);
							if (!IsInTableScope(type))
							{
								// error
								break;
							}
							if (!IsInTableScope(Tr)) break;
							
							ClearToTableRowContext();
							CloseElement(Current);
							BuilderState.Set(HtmlParserState.InTableBody);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region invalid end tag
						case "body":
						case "caption":
						case "col":
						case "colgroup":
						case "html":
						case "td":
						case "th":
						{
							// error
						}
						break;
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
				default: 
			AnythingElse: return ProcessInTableMode(token);
				#endregion
			}
			return ProductionState.Success;
		}
		bool InRowMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInRowMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.15. The "in cell" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInCellMode(HtmlToken token)
		{
			switch (token)
			{
				#region start tag
				case HtmlToken.StartTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region process in table
						case "caption":
						case "col":
						case "colgroup":
						case "tbody":
						case "td":
						case "tfoot":
						case "th":
						case "thead":
						case "tr":
						{
							if (!(IsInTableScope(Td) || IsInTableScope(Th))
							{
								// error
								break;
							}
							CloseCell();
						}
						return ProductionState.Preserve;
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region close tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region end cell
						case "td":
						case "th":
						{
							Tyep type = ElementCache.TryGetElementType(name, out type);
							if (!IsInTableScope(type))
							{
								// error
								break;
							}
							GenerateEndTags();
							if (Current.Element.Name != name)
							{
								// error
							}
							ClearStackTo(type);
							ClearToLastMarker();
							BuilderState.Set(HtmlParserState.InRow);
						}
						break;
						#endregion
						
						#region invalid end tag
						case "body":
						case "caption":
						case "col":
						case "colgroup":
						case "html":
						{
							// error
						}
						break;
						#endregion
						
						#region reprocess after closing cell
						case "table":
						case "tbody":
						case "tfoot":
						case "thead":
						case "tr":
						{
							Type type; if (ElementCache.TryGetElementType(name, out type))
							{
								if (!IsInTableScope(type))
								{
									// error
									break;
								}
							}
							else break;
							CloseCell();
						}
						return ProductionState.Preserve;
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
			default:
			AnythingElse:
			return ProcessInBodyMode(token);
			#endregion
			}
			return ProductionState.Success;
		}
		bool InCellMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInCellMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.16. The "in select" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInSelectMode(HtmlToken token)
		{
			switch (token)
			{
				#region data
				case HtmlToken.Data:
				{
					InsertNode(CreateTextNode());
				}
				break;
				#endregion
				
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.StartTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html": return ProcessInBodyMode(token);
						#endregion
						
						#region option
						case "option":
						{
							if (Current.Element is Option)
								CloseElement(Current);
							InsertNode(CreateelementNode());
						}
						break;
						#endregion
						
						#region optgroup
						case "optgroup":
						{
							if (Current.Element is Option)
								CloseElement(Current);
							if (Current.Element is Optgroup)
								CloseElement(Current);
							InsertNode(CreateelementNode());
						}
						break;
						#endregion
						
						#region select
						case "select":
						{
							// error
							if (!IsInSelectScope(Select)) break;
							ClearStackTo(Select);
							ResetInsertionMode();
						}
						break;
						#endregion
						
						#region input or textarea
						case "input":
						case "textarea":
						{
							// error
							if (!IsInSelectScope(Select)) break;
							ClearStackTo(Select);
							ResetInsertionMode();
						}
						return ProductionState.Preserve;
						#endregion
						
						#region script or template
						case "script":
						case "template": return ProcessInHeadMode(token);
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region close tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region optgroup
						case "optgroup":
						{
							if (Current.Element is Option)
							{
								if (openElements[openElements.Count - 2].Element is Optgroup)
								{
									closeElement(Current);
								}
							}
							if (Current.Element is Optgroup)
							{
								CloseElement(Current);
							}
							else
							{
								// error
							}
						}
						break;
						#endregion
						
						#region option
						case "option":
						{
							if (Current.Element is Option)
								CloseElement(Current);
							else
							{
								// error
							}
						}
						break;
						#endregion
						
						#region select
						case "select":
						{
							if (!IsInSelectScope(Select))
							{
								// error
								break;
							}
							ClearStackTo(Select);
							ResetInsertionMode();
						}
						break;
						#endregion
						
						#region template
						case "template": return ProcessInHeadMode(token);
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
			default:
			AnythingElse:
			// error
			#endregion
			}
			return ProductionState.Success;
		}
		bool InSelectMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInSelectMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.17. The "in select in table" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInSelectInTableMode(HtmlToken token)
		{
			switch (token)
			{
				#region start tag
				case HtmlToken.StartTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region in table start tags
						case "caption":
						case "tbody":
						case "tfoot":
						case "thead":
						case "td":
						case "th":
						case "tr":
						{
							// error
							ClearStackTo(Select);
							ResetInsertionMode();
						}
						return ProductionState.Preserve;
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region close tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						case "caption":
						case "table":
						case "tbody":
						case "tfoot":
						case "thead":
						case "tr":
						case "td":
						case "th":
						{
							// error
							Type type; if (ElementCache.TryGetElementType(name, out type))
							{
								if (!IsInTableScope(type)) break;
							}
							else break;
							ClearStackTo(Select);
							ResetInsertionMode();
						}
						return ProductionState.Preserve;
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
			default:
			AnythingElse:
			return ProcessInSelectMode(token);
			#endregion
			}
			return ProductionState.Success;
		}
		bool InSelectInTableMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInSelectInTableMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.18. The "in template" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInTemplateMode(HtmlToken token)
		{
			switch (token)
			{
				#region process in body
				case HtmlToken.Data:
				case HtmlToken.Comment:
				case HtmlToken.Doctype: return ProcessInBodyMode(token);
				#endregion
				
				#region start tag
				case HtmlToken.StartTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region process in head
						case "base":
						case "basefont":
						case "bgsound":
						case "link":
						case "meta":
						case "noframes":
						case "script":
						case "style":
						case "template":
						case "title": return ProcessInHeadMode(token);
						#endregion
						
						#region table sections
						case "caption":
						case "colgroup":
						case "tbody":
						case "tfoot":
						case "thead":
						{
							templateInsertionModes.Pop();
							templateInsertionModes.Push(HtmlParserState.InTable);
							BuilderState.Set(HtmlParserState.InTable);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region col
						case "col":
						{
							templateInsertionModes.Pop();
							templateInsertionModes.Push(HtmlParserState.InColumnGroup);
							BuilderState.Set(HtmlParserState.InColumnGroup);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region tr
						case "tr":
						{
							templateInsertionModes.Pop();
							templateInsertionModes.Push(HtmlParserState.InTableBody);
							BuilderState.Set(HtmlParserState.InTableBody);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region table cell
						case "td":
						case "th":
						{
							templateInsertionModes.Pop();
							templateInsertionModes.Push(HtmlParserState.InRow);
							BuilderState.Set(HtmlParserState.InRow);
						}
						return ProductionState.Preserve;
						#endregion
						
						#region any other start tag
						default:
						{
							templateInsertionModes.Pop();
							templateInsertionModes.Push(HtmlParserState.InBody);
							BuilderState.Set(HtmlParserState.InBody);
						}
						return ProductionState.Preserve;
						#endregion
					}
				}
				break;
				#endregion
				
				#region close tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region template
						case "template": return ProcessInHeadMode(token);
						#endregion
						
						#region any other end tag
						default:
						{
							// error
						}
						break;
						#endregion
					}
				}
				break;
				#endregion
			}
			return ProductionState.Success;
		}
		bool InTemplateMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInTemplateMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.19. The "after body" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessAfterBodyMode(HtmlToken token)
		{
			switch (token)
			{
				#region comment
				case HtmlToken.Comment:
				{
					openElements[0].AddChild(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.StartTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html": return ProcessInBodyMode(token);
						#endregion						
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region close tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html":
						{
							BuilderState.Set(HtmlParserState.AfterAfterBody);
						}
						break;
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
			default:
			AnythingElse:
			{
				// error
				BuilderState.Set(HtmlParserState.InBody);
			}
			return ProductionState.Preserve;
			#endregion
			}
			return ProductionState.Success;
		}
		bool AfterBodyMode(HtmlToken token)
		{
			return FinalizeRule(ProcessAfterBodyMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.20. The "in frameset" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessInFramesetMode(HtmlToken token)
		{
			switch (token)
			{
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.StartTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html": return ProcessInBodyMode(token);
						#endregion
						
						#region frameset, frame
						case "frameset":
						case "frame":
						{
							InsertNode(CreateElementNode());
						}
						break;
						#endregion
						
						#region noframes
						case "noframes": return ProcessInHeadMode(token);
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region close tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region frameset
						case "frameset":
						{
							if (Current == document.Root)
							{
								// error
								break;
							}
							CloseElement(Current);
							if (!(Current.Element is Frameset))
							{
								BuilderState.Set(HtmlParserState.AfterFrameset);
							}
						}
						break;
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
			default:
			AnythingElse:
			{
			
			// error
			}
			break;
			#endregion
			}
			return ProductionState.Success;
		}
		bool InFramesetMode(HtmlToken token)
		{
			return FinalizeRule(ProcessInFramesetMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.21. The "after frameset" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessAfterFramesetMode(HtmlToken token)
		{
			switch (token)
			{
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype:
				{
					// error
				}
				break;
				#endregion
				
				#region start tag
				case HtmlToken.StartTag:
				case HtmlToken.SelfCloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html": return ProcessInBodyMode(token);
						#endregion
						
						#region noframes
						case "noframes": return ProcessInheadMode(token);
						#endregion
						
						#region any other start tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region close tag
				case HtmlToken.CloseTag:
				{
					string name = GetMetaToken().Name;
					switch (name)
					{
						#region html
						case "html":
						{
							BuilderState.Set(HtmlParserState.AfterAfterFrameset);
						}
						break;
						#endregion
						
						#region any other end tag
						default: goto AnythingElse;
						#endregion
					}
				}
				break;
				#endregion
				
				#region anything else
			default:
			AnythingElse:
			{
				// error
			}
			break;
			#endregion
			}
			return ProductionState.Success;
		}
		bool AfterFramesetMode(HtmlToken token)
		{
			return FinalizeRule(ProcessAfterFramesetMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.22. The "after after body" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessAfterAfterBodyMode(HtmlToken token)
		{
			switch (token)
			{
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype: return ProcessInBodyMode(token);
				#endregion
				
				#region html start tag
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					if (GetMetaToken().Name == "html") 
						return ProcessInBodyMode(token);
					else goto default;
				}
				#endregion
				
				default:
				{
					// error
					BuilderState.Set(HtmlParserState.InBody);
				}
				return ProductionState.Preserve;
			}
			return ProductionState.Success;
		}
		bool AfterAfterBodyMode(HtmlToken token)
		{
			return FinalizeRule(ProcessAfterAfterBodyMode(token));
		}
		#endregion
		
		#region HTML5 8.2.5.4.23. The "after after frameset" insertion mode
		/// <summary>
		/// 
		/// </summary>
		ProductionState ProcessAfterAfterFramesetMode(HtmlToken token)
		{
			switch (token)
			{
				#region comment
				case HtmlToken.Comment:
				{
					InsertNode(CreateCommentNode());
				}
				break;
				#endregion
				
				#region doctype
				case HtmlToken.Doctype: return ProcessInBodyMode(token);
				#endregion
				
				#region start tags
				case HtmlToken.OpenTag:
				case HtmlToken.SelfCloseTag:
				{
					string name= GetMetaToken.Name;
					if (name == "html") 
						return ProcessInBodyMode(token);
					else if (name == "noframes") 
						return ProcessInHeadMode(token);
					else goto default;
				}
				#endregion
				
				#region anything else
					default:
					{
						// error
					}
					break;
					#endregion
			}
			return ProductionState.Success;
		}
		bool AfterAfterFramesetMode(HtmlToken token)
		{
			return FinalizeRule(ProcessAfterAfterFramesetMode(token));
		}
		#endregion
		
		void SwitchToTextMode()
		{
			originalInsertionMode = BuilderState.Current;
			BuilderState.Set(HtmlParserState.Text);
		}
		
		void SwitchToInTableTextMode()
		{
			originalInsertionMode = BuilderState.Current);
			BuilderState.Set(HtmlParserState.InTableText);
		}
		
		void ResetInsertionMode()
		{
			bool last = false;
			ElementNode node = Current;
			do
			{
				HtmlElement element = node.Element;
				if (openElements.IndexOf(node) == 0)
					last = true;
				if (element is Select)
				{
					ElementNode ancestor = node;
					while (!last)
					{
						if (openElements.IndexOf(ancestor) == 0)
							break;
						else
							ancestor = openElements[openElements.IndexOf(ancestor) - 1];
						if (ancestor.Element is Template)
							break;
						if (ancestor.Element is Table)
						{
							BuilderState.Set(HtmlParserState.InSelectInTable);
							return;
						}
					}
					BuilderState.Set(HtmlParserState.InSelect);
				}
				else if (!last && (element is Td || element is Th))
				{
					BuilderState.Set(HtmlParserState.InCell);
				}
				else if (element is Tr)
				{
					BuilderState.Set(HtmlParserState.InRow);
				}
				else if (element is Tbody || element is Tfoot || element is Thead)
				{
					BuilderState.Set(HtmlParserState.InTableBody);
				}
				else if (element is Caption)
				{
					BuilderState.Set(HtmlParserState.InCaption);
				}
				else if (element is Colgroup)
				{
					BuilderState.Set(HtmlParserState.InColumnGroup);
				}
				else if (element is Table)
				{
					BuilderState.Set(HtmlParserState.InTable);
				}
				else if (element is Template)
				{
					BuilderState.Set(CurrentTemplateMode);
				}
				else if (!last && element is Head)
				{
					BuilderState.Set(HtmlParserState.InHead);
				}
				else if (element is Body)
				{
					BuilderState.Set(HtmlParserState.InBody);
				}
				else if (element is Frameset)
				{
					BuilderState.Set(HtmlParserState.InFrameset);
				}
				else if (element is Html)
				{
					if (head == null)
					{
						BuilderState.Set(HtmlParserState.BeforeHead);
					}
					else
					{
						BuilderState.Set(HtmlParserState.AfterHead);
					}
				}
				else if (last)
				{
					BuilderState.Set(HtmlParserState.InBody);
				}
				else
				{
					node = openElements[openElements.IndexOf(node) - 1];
				}
			}
		}

	}
}