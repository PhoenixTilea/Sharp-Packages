using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Html
{
	public abstract class DataNode : IDomNode
	{
		string data;
		/// <summary>
		/// The raw text data this node contains
		/// </summary>
		public string Data
		{
			get { return data; }
			protected set { data = value; }
		}
		
		/// <summary>
		/// The length of a data node is the length of its data string
		/// </summary>
		public int Length
		{
			get { return data.Length; }
		}
		
		DataNode(string data = string.Empty)
		{
			this.data = data;
		}
		
		public void AppendData(string data)
		{
			this.data += data;
		}
		
	}
}