using System;
using System.Collections.Generic;
using System.Reflection;
using SE;

namespace SE.Text.Html
{
	public class HtmlElement
	{
		public string Name
		{
			get
			{
				Attribute[] ats = Attribute.GetCustomAttributes(typeof(this));
				foreach (Attribute a in ats)
				{
					if (a is ElementAttribute) return ((ElementAttribute)a).Name;
				}
				return "element";
			};
		}
		
		public bool IsVoid
		{
			get
			{
				Attribute[] ats = Attribute.GetCustomAttributes(typeof(this));
				foreach (Attribute a in ats)
				{
					if (a is ElementAttribute) return ((ElementAttribute)a).IsVoid;
				}
				return true;
			};
		}
		
		Dictionary<string, string> properties;
		/// <summary>
		/// Html attributes defined on this element
		/// </summary>
		public Dictionary<string, string> Properties
		{
			get { return properties; }
		}
		
		public HtmlElement()
		{
			this.properties = new Dictionary<string, string>();
		}
		
		/// <summary>
		/// Attempts to retrieve the value of the named attribute
		/// </summary>
		/// <param name="name">The name of the attribute to get</param>
		/// <returns>The value of the named attribute or an empty string</returns>
		public string GetProperty(string name)
		{
			string value; if (properties.TryGetValue(name.ToLowerInvarient(), out value))
				return value;
			else return string.Empty;
		}
		
		/// <summary>
		/// Define an attribute on this element
		/// If the attribute already exists, its value will be changed to the given value
		/// </summary>
		/// <param name="name">The name of the attribute to define</param>
		/// <param name="value">The value to assign to the attribute</param>
		public void SetProperty(string name, string value)
		{
			properties[name.ToLowerInvarient()] = value;
		}
		
		public bool HasProperty(string name)
		{
			return properties.ContainsKey(name.ToLowerInvarient());
		}
		
		public override string ToString()
		{
			return this.Name;
		}
		
	}
}