using System;
using System.Collections.Generic;
using SE;

namespace Se.Text.Html
{
	public class TextNode : DataNode
	{
		/// <summary>
		/// Creates a new Text node
		/// </summary>
		/// <param name="data">Optionally pass in data for this node</param>
		public TextNode(string data = string.Empty)
		: base(data)
		{ }
	
	}
}