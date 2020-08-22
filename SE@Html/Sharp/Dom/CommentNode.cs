using System;
using System.Collections.Generic;
using SE;

namespace Se.Text.Html
{
	public class CommentNode : DataNode
	{
		/// <summary>
		/// Creates a new comment node
		/// </summary>
		/// <param name="data">Optionally pass in data for this node</param>
		public CommentNode(string data = string.Empty)
		: base(data)
		{ }
		
		
	}
}