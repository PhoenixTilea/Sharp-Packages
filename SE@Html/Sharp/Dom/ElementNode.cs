using System;
using System.Collections.Generic;
using SE;

namespace Se.Text.Html
{
	public class ElementNode : HtmlDomNode
	{
		HtmlElement element;
		/// <summary>
		/// The Html Element this node represents
		/// </summary>
		public HtmlElement Element
		{
			get { return element; }
		}
		
		/// <summary>
		/// The tag name of the element this node represents
		/// </summar>
		public string Name
		{
			get { return element.Name; }
		}
		
		/// <summary>
		/// Gets the list of attributes from this node's element
		/// </summar>
		public Dictionary<string, string> Properties
		{
			get { return element.Properties; }
		}
		
		List<IDomNode> children;
		/// <summary>
		/// A connection to the first child of this node
		/// </summary>
		public List<IDomNode> Children
		{
			get { return children; }
		}
		
		
		/// <summary>
		/// Gets the number of children
		/// </summary>
		public int Length
		{
			get { return children.Count; }
		}
		
		/// <summary>
		/// Creates a new element node
		/// </summary>
		/// <param name="element">The Html Element this node will represent</param>
		public ElementNode(HtmlElement element)
		{
			this.element = element;
			this.children = new List<IDomNode>();
			this.Parent = null;
		}
		
		/// <summary>
		/// Add a new child node to this element
		/// </summary>
		/// <param name="node">The node to be appended to this element</param>
		public void AddChild(IDomNode node, int? at)
		{
			if (at == null || at >= children.Length)
				children.Add(node);
			else
				children.Insert(node, at);
			
			node.Parent = this;
		}
		
	}
}