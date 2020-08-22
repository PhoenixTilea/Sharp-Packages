using System;
using System.Collections.Generic;
using SE;

namespace Se.Text.Html
{
	public class DoctypeNode : HtmlDomNode
	{
		string name;
		/// <summary>
		/// The name of the doctype
		/// </summary>
		public string Name
		{
			get { return name; }
		}
		
		string publicId;
		/// <summary>
		/// the doctype's public id, or an empty string
		/// </summary>
		public string PublicId
		{
			get { return publicId; }
		}
		
		string systemId;
		/// <summary>
		/// The doctype's system id, or an empty string
		/// </summary>
		public string SystemId
		{
			get { return systemId; }
		}
		
		public int Length
		{
			get { return 0; }
		}
		
		/// <summary>
		/// Create a new doctype node
		/// </summary>
		/// <param name="name">The name of the document type, which is required</param>
		/// <param name="publicId">The document type's public id - optional</param>
		/// <param name="systemId">The document type's system id - optional</param>
		public DoctypeNode(string name, string? publicId, string? systemId)
		{
			this.name = name;
			this.publicId = publicId;
			this.systemId = systemId;
		}
		
	}
}