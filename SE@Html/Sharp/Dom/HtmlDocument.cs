using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Html
{
	/// <summary>
	/// The DOM representation of a parsed HTML 5 document
	/// </summary>
	public class HtmlDocument : IDomNode
	{
		bool allowScripts;
		/// <summary>
		/// Whether or not scripts will be processed on this document
		/// </summary>
		public bool AllowScripts
		{
			get { return allowScripts; }
			set { allowScripts = value; }
		}
		
		List<IDomNode> children;
		/// <summary>
		/// List of root children for this document
		/// </summary>
		public List<IDomNode> Children
		{
			get { return children; }
		}
		
		/// <summary>
		/// The number of children in the document
		/// </summary>
		public int Length
		{
			get { return children.Count; }
		}
		
		Quirks mode;
		/// <summary>
		/// Get the status of the quirks flag
		/// </summary>
		public Quirks Mode
		{
			get { return mode; }
			set { mode = value; }
		}
		
		DoctypeNode doctype;
		/// <summary>
		/// Pointer to the document's Doctype
		/// </summary>
		public DoctypeNode Doctype
		{
			get { return doctype; }
			set { doctype = value; }
		}
		
		protected HtmlParser parser;
		
		/// <summary>
		/// Get the list of parser errors
		/// </summary>
		public IEnumerable<string> Errors
		{
			get
			{
				if (parser != null) return parser.Errors;
				else return null;
			}
		}
		
		/// <summary>
		/// Determine whether the document's parser has errors
		/// </summary>
		public bool HasErrors
		{
			get
			{
				if (parser != null) return (parser.Errors.Count > 0);
				else return false;
			}
		}
		
		/// <summary>
		/// Creates a new Html Dom Document
		/// </summary>
		public HtmlDocument()
		{
			this.doctype = null;
			this.children = new List<IDomNode>();
			this.mode = Quirks.NoQuirks;
		}
		
		/// <summary>
		/// Add a new root node to this document
		/// </summary>
		/// <param name="node">The DOM node to add</param>
		public void AddChild(IDomNode node, int? at)
		{
			if (at == null || at >= children.Length || at < 0)
				children.Add(node);
			else
				children.Insert(node, at);
		}
		
	}
}