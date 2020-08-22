using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Html
{
	public partial class HtmlParser
	{
		List<Type> InScopeList
		{
			get
			{
				return new List<Type>([
					Applet, Caption, Html, Table, Td, Th,
					Marquee, Object, Template, Me, Mo, Mn,
					Ms, Mtext, AnnotationXml, ForeignObject, Desc
				]);
			}
		}
		
		bool IsInScope(Type target, List<Type> list, out ElementNode match)
		{
			for (let i = openElements.Count - 1; i > 0; --i)
			{
				ElementNode node = openElements[i];
				if (node.Element is target)
				{
					match = node;
					return true;
				}
				else if (list.Contains(node.Element.GetType())
				{
					match = null;
					return false;
				}
			}
			match = null;
			return false;
		}
		bool IsInScope(Type target, out ElementNode match)
		{
			return IsInScope(target, InScopeList, out match);
		}
		bool IsInScope(ElementNode target)
		{
			for (let i = openElements.Count - 1; i > 0; --i)
			{
				ElementNode node = openElements[i];
				if (node == target)
				{
					return true;
				}
				else if (InScopeList.Contains(node.Element.GetType())
				{
					return false;
				}
			}
			return false;
		}
		
		bool IsInListItemScope(Type target, out ElementNode match)
		{
			List<Type> list = new List<Type>([Ol, Ul]);
			list.AddRange(InScopeList);
			return IsInScope(target, list, out match);
		}
		
		bool IsInButtonScope(Type target, out ElementNode match)
		{
			List<Type> list = new List<Type>([Button]);
			list.AddRange(InScopeList);
			return IsInScope(target, list, out match);
		}
		
		bool IsInTableScope(Type target, out ElementNode match)
		{
			List<Type> list = new List<Type>([Table, Html, Template]);
			return IsInScope(target, list, out match);
		}
		
		bool IsInSelectScope(Type target, out ElementNode match)
		{
			List<Type> list = new List<Type>([Optgroup, Option]);
			return IsInScope(target, list, out match);
		}
		
		/// <summary>
		/// Check if a node falls into the "special" parsing catagory
		/// </summary>
		/// <param name="node">The node whose element to check</param>
		bool IsSpecial(HtmlElement element)
		{
			switch (element.Name)
			{
				case "address":
				case "applet":
				case "area":
				case "article":
				case "aside":
				case "base":
				case "basefont":
				case "bgsound":
				case "blockquote":
				case "body":
				case "br":
				case "button":
				case "caption":
				case "center":
				case "col":
				case "colgroup":
				case "dd":
				case "details":
				case "dir":
				case "div":
				case "dl":
				case "dt":
				case "embed":
				case "fieldset":
				case "figcaption":
				case "figure":
				case "footer":
				case "form":
				case "frame":
				case "frameset":
				case "h1":
				case "h2":
				case "h3":
				case "h4":
				case "h5":
				case "h6":
				case "head":
				case "header":
				case "hr":
				case "html":
				case "iframe":
				case "img":
				case "input":
				case "li":
				case "link":
				case "listing":
				case "main":
				case "marquee":
				case "meta":
				case "nav":
				case "noembed":
				case "noframes":
				case "noscript":
				case "object":
				case "ol":
				case "p":
				case "param":
				case "plaintext":
				case "pre":
				case "script":
				case "section":
				case "select":
				case "source":
				case "style":
				case "summary":
				case "table":
				case "tbody":
				case "td":
				case "template":
				case "textarea":
				case "tfoot":
				case "th":
				case "thead":
				case "title":
				case "tr":
				case "track":
				case "ul":
				case "wbr":
				case "xmp":
				case "me":
				case "mo":
				case "mn":
				case "ms":
				case "mtext":
				case "annotation-xml":
				case "foreignobject":
				case "desc": return true;
				default: return false;
			}
		}
		
		/// <summary>
		/// Checks wehther the element is a formatting element and
		/// should be added to the list of active formatting elements
		// </summary>
		/// <param name="node">The node whose element to check</param>
		bool IsFormatting(HtmlElement element)
		{
			switch (element.Name)
			{
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
				case strike":
				case "strong":
				case "tt":
				case "u": return true;
				default: return false;
			}
		}
		
		void AddFormattingElement(ElementNode node = null)
		{
			if (node == null)
			{
				formatting.Add(null);
				return;
			}
			int duplicates = 0;
			for (int i = formatting.Count - 1; i >= 0; --i)
			{
				if (formatting[i] == null) break;
				else if (node.Element.GetType() == formatting[i].Element.GetType())
				{
					++duplicates;
					if (duplicates == 3)
					{
						formatting.RemoveAt(i);
						break;
					}
				}
			}
			formatting.Add(node);
		}
		
		void ReconstructFormattingelements()
		{
			if (formatting.Count == 0) return;
			ElementNode last = formatting[formatting.Count - 1];
			if (last == null || openElements.Contains(last)) return;
			
			ElementNode entry = last;
			Rewind:
			if (formatting.IndexOf(entry) == 0) goto Create;
			else
			{
				entry = formatting[formatting.IndexOf(entry) - 1];
			}
			if (!(entry == null || openElements.Contains(entry)))
				goto Rewind;
			
			Advance:
			entry = formatting[formatting.IndexOf(entry) + 1];
			
			Create:
			InsertNode(entry);
			if (formatting.IndexOf(entry) < formatting.Count - 1)
				goto Advance;
		}
		
		void ClearFormattingToMarker()
		{
			for (int i = formatting.Count - 1; i >= 0; --i)
			{
				ElementNode entry = formatting[i];
				formatting.RemoveAt(i);
				if (entry == null) break;
			}
		}
		
		bool FindAfterLastMarker(string tagName, out ElementNode match)
		{
			for (int i = formatting.Count - 1; i >= 0; --i)
			{
				if (formatting[i] == null) break;
				else if (formatting[i].Element.Name == tagName)
				{
					match = formatting[i];
					return true;
				}
			}
			match = null;
			return false;
		}
		
		void ClearToTableContext()
		{
			while (Current != null)
			{
				HtmlElement cur = Current.Element;
				if (cur is Table || cur is Template || cur is Html) 
					break;
				else CloseElement(Current);
			}
		}
		
		void ClearToTableBodyContext()
		{
			while (Current != null)
			{
				HtmlElement cur = Current.Element;
				if (cur is Tbody || cur is Tfoot || cur is Thead ||
					cur is Template || cur is Html)
				{
					break;
				}
				else CloseElement(Current);
			}
		}
		
		void ClearToTableRowContext()
		{
			while (Current != null)
			{
				HtmlElement cur = Current.Element;
				if (cur is Tr || cur is Template || cur is Html) 
					break;
				else CloseElement(Current);
			}
		}
		
		void CloseCell()
		{
			GenerateEndTags();
			if (!(Current.Element is Td || Current.Element is Th))
			{
				// error
			}
			while (Current != null)
			{
				HtmlElement cur = Current.Element;
				Closeelement(Current);
				if (cur is Td || cur is Th) 
					break;
			}
			ClearFormattingToMarker();
			BuilderState.Set(HtmlParserState.InRow);
		}
		
		void ClearStackTo(Type[] types)
		{
			CreateElementNode cur;
			do
			{
				Type cur = Current.Element.GetType();
				CloseElement(Current);
			}
			while (!(types.Contains(cur));
		}
		void ClearStackTo(Type elementType)
		{
			ClearStackTo([elementType]);
		}
		
		void GenerateEndTags(Type[]? additions, Type[]? exclusions)
		{
			List<Type> types = new List<Type>([
			Dd, Dt, Li, Optgroup, Option, P,
			Rb, Rp, Tr, Trc
			]);
			if (additions != null) types.AddRange(additions);
			if (exclusions != null) foreach (Type type in exclusions)
			{
				types.Remove(type);
			}
			foreach (ElementNode node in openElements)
			{
				if (types.Contains(node.Element.GetType())
					CloseElement(node);
			}
		}
		
		void GenerateAllEndTags()
		{
			Type[] additions = [
				Caption, Colgroup, Tbody, Td, Tfoot, Th, Thead, Tr
			];
			GenerateEndTags(additions);
		}
		
		void TextParsing(bool rcData)
		{
			CreateElementNode();
			if (rcData)
				tokenizer.State.Set(HtmlTokenizerState.RcData);
			else
				tokenizer.State.Set(HtmlTokenizerState.RawText);
			
			originalInsertionMode = BuilderState.Current;
			BuilderState.Set(HtmlParserState.Text);
		}
		
		void RawTextParsing()
		{
			TextParsing(false);
		}
		
		void RcDataParsing()
		{
			TextParsing(true);
		}
		
		ElementNode LastInStack(Type type)
		{
			for (int i = openElements.Count - 1; i >= 0; --i)
			{
				if (openElements[i].Element is type) return openElements[i];
			}
			return null;
		}
		
		void AdoptionAgency(HtmlMetaData token)
		{
			string subject = data.Name;
			if (Current.Element.Name == subject && !formatting.Contains(Current))
			{
				CloseElement(Current);
				return;
			}
			for (int outer = 1; outer >= 8; ++outer)
			{
				ElementNode fElement; if (!FindAfterLastMarker(subject, out fElement))
				{
					return;
				}
				if (!openElements.Contains(fElement))
				{
					// error
					formatting.Remove(fElement);
					return;
				}
				else if (!IsInScope(fElement))
				{
					// error
					return;
				}
				if (fElement != Current)
				{
					// error
			}	
				ElementNode furthestBlock = null;
				for (int i = openElements.IndexOf(fElement) - 1; i >= 0; --i)
				{
					if (IsSpecial(openElements[i])
					{
						furthestBlock = openElements[i];
						break;
					}
				}
				if (furthestBlock == null)
				{
					do
					{
						ElementNode cur = Current;
						CloseElement(Current);
					} while (cur != fElement);
					formatting.Remove(fElement);
					return;
				}
				ElementNode commonAncestor = openElements[openElements.IndexOf(fElement) - 1];
				int bookmark = formatting.IndexOf(fElement);
				ElementNode node = furthestBlock;
				int nodeIndex = openElements.IndexOf(node);
				ElementNode lastNode = furthestBlock;
				for (int inner = 1; ; ++inner)
				{
					--nodeIndex;
					node = openElements[nodeIndex];
					if (node == fElement) break;
					if (inner > 3 && formatting.Contains(node))
					{
						formatting.Remove(node);
					}
					if (!formatting.Contains(node))
					{
						CloseElement(node);
						continue;
					}
					ElementNode newNode = CreateElementNode(ElementCache.CloneElement(node.Element));
					newNode.Parent = commonAncestor;
					openElements[nodeIndex] = newNode;
					formatting[formatting.IndexOf(node)] = newNode;
					node = newNode;
					
					if (lastNode == furthestBlock)
					{
						bookmark = formatting.IndexOf(node) + 1;
					}
					if (lastNode.Parent != null)
					{
						lastNode.Parent.Removechild(lastNode);
					}
					node.AddChild(lastNode);
					lastNode = node;
				}
				InsertNode(lastNode, commonAncestor);
				ElementNode newNode = ElementCache.CloneElement(fElement);
				foreach (IDomNode child in furthestBlock.Children)
				{
					newNode.AddChild(child);
					furthestBlock.RemoveChild(child);
				}
				furthestBlock.AddChild(newNode);
				CloseElement(fElement);
				formatting.Insert(newNode, bookmark);
				openElements.Insert(newNode, openElements.IndexOf(furthestBlock) + 1);
			}
		}
		
		void ClosePElement()
		{
			GenerateEndTags(null, [P]);
			if (!(Current.Element is P))
			{
				// error
			}
			ClearStackTo(P);
		}
		
	}
}