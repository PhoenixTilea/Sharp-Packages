using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Html
{
	public partial class HtmlParser
	{
		List<ElementNode> openElements;
		List<ElementNode> formatting;
		ElementNode head;
		ElementNode form;
		bool foster;
		
		public ElementNode Current
		{
			get
			{
				if (openElements.Count > 0)
				{
					return openElements[openElements.Count - 1];
				}
			}
		}
		
		void InsertNode(IDomNode node, ElementNode? overrideTarget)
		{
			ElementNode target;
			if (overrideTarget != null)
				target = overrideTarget;
			else
				target = Current;
			
			ElementNode adjustedInsert;
			int adjustedInsertAt;
			HtmlElement tElement = target.Element;
			if (foster && (tElement is Table || tElement is Tbody || tElement is Tfoot || tElement is Thead || tElement is Tr))
			{
				ElementNode lastTemplate = LastInStack(Template);
				ElementNode lastTable = LastInStack(Table);
				if (lastTemplate != null && 
					(lastTable == null || openElements.IndexOf(lastTemplate) > openElements.IndexOf(lastTable))
				{
					adjustedInsert = lastTemplate;
					adjustedInsertAt = lastTemplate.Length;
				}
				else if (lastTable == null)
				{
					adjustedInsert = openElements[0];
					adjustedInsertAt = adjustedInsert.Length;
				}
				else if (lastTable.Parent != null)
				{
					adjustedInsert = lastTable.Parent;
					adjustedInsertAt = adjustedInsert.Children.IndexOf(lastTable);
				}
				else
				{
					ElementNode previous = openElements[openElements.IndexOf(lastTable) - 1];
					adjustedInsert = previous;
					adjustedInsertAt = previous.Length;
				}
			}
			else
			{
				adjustedInsert = target;
				adjustedInsertAt = target.Length;
			}
			InsertNode(node, adjustedInsert, adjustedInsertAt);
		}
		void InsertNode(IDomNode node, ElementNode parent, int at)
		{
			if (node is TextNode && at > 0)
			{
				IDomNode previous = parent.Children[at - 1];
				if (previous is TextNode)
				{
					previous.AppendData(node.data);
					return;
				}
			}
			parent.AddChild(node, at);
			if (node is ElementNode && !ElementCache.IsVoid(node.Element.Name))
				OpenElement(node);
		}
		
		DoctypeNode CreateDoctypeNode()
		{
			HtmlMetaData data = GetMetaToken();
			string publicId; if (!data.Properties.TryGetValue("PUBLIC", out publicId)
			{
				publicId = null;
			}
			string systemId; if (!data.Properties.TryGetValue("SYSTEM", out systemId))
			{
				systemId = null;
			}
			
			DoctypeNode node = new DoctypeNode(data.Name, publicId, systemId);
			
			if (node.Name != "html" || publicId != null || 
				!(systemId == null || systemId == "about:legacy-compat))
			{
				// error
			}
			#region quirks mode
			if (node.name != "html" || 
				systemId == "http://www.ibm.com/data/dtd/v11/ibmxhtml1-transitional.dtd") 
			{
				document.Mode = Quirks.Quirks;
			}
			else if (!string.IsNullOrEmpty(publicId) && (
				publicId == "-//W3O//DTD W3 HTML Strict 3.0//EN//") ||
				publicId == "-/W3C/DTD HTML 4.0 Transitional/EN") ||
				publicId == "HTML") ||
				
				publicId.StartsWith("+//Silmaril//dtd html Pro v0r11 19970101//") ||
				publicId.StartsWith("-//AS//DTD HTML 3.0 asWedit + extensions//") ||
				publicId.StartsWith("-//AdvaSoft Ltd//DTD HTML 3.0 asWedit + extensions//") ||
				publicId.StartsWith("-//IETF//DTD HTML 2.0 Level 1//") ||
				publicId.StartsWith("-//IETF//DTD HTML 2.0 Level 2//") ||
				publicId.StartsWith("-//IETF//DTD HTML 2.0 Strict Level 1//") ||
				publicId.StartsWith("-//IETF//DTD HTML 2.0 Strict Level 2//") ||
				publicId.StartsWith(""-//IETF//DTD HTML 2.0 Strict//") ||
				publicId.StartsWith("-//IETF//DTD HTML 2.0//") ||
				publicId.StartsWith("-//IETF//DTD HTML 2.1E//") ||
				publicId.StartsWith("-//IETF//DTD HTML 3.0//") ||
				publicId.StartsWith("-//IETF//DTD HTML 3.2 Final//") ||
				publicId.StartsWith("-//IETF//DTD HTML 3.2//") ||
				publicId.StartsWith("-//IETF//DTD HTML 3//") ||
				publicId.StartsWith("-//IETF//DTD HTML Level 0//") ||
				publicId.StartsWith("-//IETF//DTD HTML Level 1//") ||
				publicId.StartsWith("-//IETF//DTD HTML Level 2//") ||
				publicId.StartsWith("-//IETF//DTD HTML Level 3//") ||
				publicId.StartsWith("-//IETF//DTD HTML Strict Level 0//") ||
				publicId.StartsWith("-//IETF//DTD HTML Strict Level 1//") ||
				publicId.StartsWith("-//IETF//DTD HTML Strict Level 2//") ||
				publicId.StartsWith("-//IETF//DTD HTML Strict Level 3//") ||
				publicId.StartsWith("-//IETF//DTD HTML Strict//") ||
				publicId.StartsWith("-//IETF//DTD HTML//") ||
				publicId.StartsWith("-//Metrius//DTD Metrius Presentational//") ||
				publicId.StartsWith("-//Microsoft//DTD Internet Explorer 2.0 HTML Strict//") ||
				publicId.StartsWith("-//Microsoft//DTD Internet Explorer 2.0 HTML//") ||
				publicId.StartsWith("-//Microsoft//DTD Internet Explorer 2.0 Tables//") ||
				publicId.StartsWith("-//Microsoft//DTD Internet Explorer 3.0 HTML Strict//") ||
				publicId.StartsWith("-//Microsoft//DTD Internet Explorer 3.0 HTML//") ||
				publicId.StartsWith("-//Microsoft//DTD Internet Explorer 3.0 Tables//") ||
				publicId.StartsWith("-//Netscape Comm. Corp.//DTD HTML//") ||
				publicId.StartsWith("-//Netscape Comm. Corp.//DTD Strict HTML//") ||
				publicId.StartsWith("-//O'Reilly and Associates//DTD HTML 2.0//") ||
				publicId.StartsWith("-//O'Reilly and Associates//DTD HTML Extended 1.0//") ||
				publicId.StartsWith("-//O'Reilly and Associates//DTD HTML Extended Relaxed 1.0//") ||
				publicId.StartsWith("-//SQ//DTD HTML 2.0 HoTMetaL + extensions//") ||
				publicId.StartsWith("-//SoftQuad Software//DTD HoTMetaL PRO 6.0::19990601::extensions to HTML 4.0//") ||
				publicId.StartsWith("-//SoftQuad//DTD HoTMetaL PRO 4.0::19971010::extensions to HTML 4.0//") ||
				publicId.StartsWith("-//Spyglass//DTD HTML 2.0 Extended//") ||
				publicId.StartsWith("-//Sun Microsystems Corp.//DTD HotJava HTML//") ||
				publicId.StartsWith("-//Sun Microsystems Corp.//DTD HotJava Strict HTML//") ||
				publicId.StartsWith("-//W3C//DTD HTML 3 1995-03-24//") ||
				publicId.StartsWith("-//W3C//DTD HTML 3.2 Draft//") ||
				publicId.StartsWith("-//W3C//DTD HTML 3.2 Final//") ||
				publicId.StartsWith("-//W3C//DTD HTML 3.2//") ||
				publicId.StartsWith("-//W3C//DTD HTML 3.2S Draft//") ||
				publicId.StartsWith("-//W3C//DTD HTML 4.0 Frameset//") ||
				publicId.StartsWith("-//W3C//DTD HTML 4.0 Transitional//") ||
				publicId.StartsWith("-//W3C//DTD HTML Experimental 19960712//") ||
				publicId.StartsWith("-//W3C//DTD HTML Experimental 970421//") ||
				publicId.StartsWith("-//W3C//DTD W3 HTML//") ||
				publicId.StartsWith("-//W3O//DTD W3 HTML 3.0//") ||
				publicId.StartsWith("-//WebTechs//DTD Mozilla HTML 2.0//") ||
				publicId.StartsWith("-//WebTechs//DTD Mozilla HTML//")
			))
			{
				document.Mode = Quirks.Quirks;
			}
			else if (systemId == null &&
				(publicId.StartsWith("-//W3C//DTD HTML 4.01 Frameset//") || publicId.StartsWith("-//W3C//DTD HTML 4.01 Transitional//"))
			{
				document.Mode = Quirks.Quirks;
			}
			#endregion
			
			#region limited quirks mode
			else if (publicId.StartsWith("-//W3C//DTD XHTML 1.0 Frameset//") ||
				publicId.StartsWith("-//W3C//DTD XHTML 1.0 Transitional//") ||
				(systemId != null && (
					publicId.StartsWith("-//W3C//DTD HTML 4.01 Frameset//") ||
					publicId.StartsWith("-//W3C//DTD HTML 4.01 Transitional//")
				))
			)
			{
				document.Mode = Quirks.LimitedQuirks;
			}
			#endregion
			
			return node;
		}
		
		CommentNode CreateCommentNode()
		{
			HtmlMetaData data = GetMetaToken();
			CommentNode node = new CommentNode(data.Name);
			return node;
		}
		
		TextNode CreateTextNode(string? data)
		{
			if (data == null)
				data = GetMetaToken().Name;
			TextNode node = new TextNode(data);
			return node;
		}
		
		ElementNode CreateElementNode(string? name)
		{
			HtmlElement element;
			if (!string.IsNullOrEmpty(name))
			{
				element = ElementCache.CreateElement(name);
			}
			else
			{
				HtmlMetaData data = GetMetaToken();
				name = data.Name;
				element = ElementCache.CreateElement(data);
			}
			if (element != null)
			{
				ElementNode node = new ElementNode(element);
				return node;
			}
			else
			{
				// error
				return null;
			}
		}
		
		
		HtmlMetaData GetMetaToken()
		{
			return ((HtmlTokenizer) tokenizer).MetaToken;
		}
		
		void OpenElement(ElementNode node)
		{
			openElements.Add(node);
			if (IsFormatting(node.Element) && !formatting.Contains(node))
				AddFormattingElement(node);
		}
		
		void CloseElement(ElementNode node)
		{
			openElements.Remove(node);
			if (formatting.Contains(node)) 
				formatting.Remove(node);
		}
		
		bool HasOpenElement(Type elementType, out ElementNode match)
		{
			foreach (ElementNode node in openelements)
			{
				if (node.Element is elementType)
				{
					match = node;
					return true;
				}
			}
			return false;
		}
		
	}
}