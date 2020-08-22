using System;
using SE;

namespace SE.Text.Html
{
	public partial class HtmlParser
	{
		public static class ErrorMessages
		{
			public static string InvalidToken = "Invalid token {0} in {1} insertion mode";
			public static string InvalidDoctype = "Invalid doctype token - Name: {0}, Public: {1}, System{2}";
			public static string InvalidTagName = "Unrecognized tag name {0}";
			public static string InvalidStartTag = "Invalid start tag {0} in {1}";
			public static string InvalidEndTag = "Invalid end tag {0} in {1} insertion mode";
			
		}
	}
}