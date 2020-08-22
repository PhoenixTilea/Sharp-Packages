// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using SE;

namespace SE.Text.Html
{
	public static class ElementCache
	{
		private static Dictionary<int, ElementAttribute> elementMeta;

		private static Dictionary<int, Type> elements;

		static ElementCache()
		{
			elementMeta = new Dictionary<int, ElementAttribute>();
			elements = new Dictionary<int, Type>();
			LoadElements(Assembly.GetExecutingAssembly());
		}

		public static void LoadElements(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes<ElementAttribute>())
				foreach (ElementAttribute meta in type.GetAttributes<ElementAttribute>())
				{
					int id = meta.Name.ToLowerInvariant().GetHashCode();
					elementMeta.Add(id, meta);
					elements.Add(id, type);
				}
		}
		
		public static bool TryGetElementType(int id, out Type type)
		{
			return elements.TryGetValue(id, out type);
		}
		
		public static bool TryGetElementType(string name, out Type type)
		{
			return TryGetElementType(name.ToLowerInvarient().GetHashCode(), out type);
		}

		public static bool TryGetMetaData(int id, out ElementAttribute meta)
		{
			return elementMeta.TryGetValue(id, out meta);
		}

		public static bool TryGetMetaData(string name, out ElementAttribute meta)
		{
			return TryGetMetaData(name.ToLowerInvariant().GetHashCode(), out meta);
		}

		public static HtmlTokenizerState GetNextState(int id)
		{
			ElementAttribute meta; if (elementMeta.TryGetValue(id, out meta))
			{
				return meta.Tokenization;
			}
			else return HtmlTokenizerState.Data;
		}

		public static HtmlTokenizerState GetNextState(string name)
		{
			return GetNextState(name.ToLowerInvariant().GetHashCode());
		}

		public static bool IsVoid(int id)
		{
			ElementAttribute meta; if (elementMeta.TryGetValue(id, out meta))
			{
				return meta.IsVoid;
			}
			else return false;
		}

		public static bool IsVoid(string name)
		{
			return IsVoid(name.ToLowerInvariant().GetHashCode());
		}
		
		public static HtmlElement CreateElement(int id)
		{
			Type elementType; if (elements.TryGetValue(id, out elementType))
			{
				return (HtmlElement) new elementType();
			}
			else
			{
				throw new ArgumentException("Invalid element name");
			}
		}
		
		public static HtmlElement Createelement(string name)
		{
			return CreateElement(name.ToLowerInvarient().GetHashCode());
		}
		
		public static Createelement(HtmlMetaData data)
		{
			HtmlElement element = CreateElement(data.Name);
			foreach (string prop in data.Properties.Keys())
			{
				element.SetProperty(prop, data.Properties[prop]);
			}
			return element;
		}
		
		
	}
}
