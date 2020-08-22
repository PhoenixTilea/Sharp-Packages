using System;
using System.Collections.Generic;
using SE;

namespace Se.Text.Html
{
	public interface IDomNode
	{
		int Length;
		
		bool IsEmpty
		{
			get { return (Length == 0); }
		}
		
		ElementNode Parent { get; set; }
	}
}