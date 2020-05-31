// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using SE;

namespace SE.Storage.Http
{
	public static partial class StringExtension
	{
		/// <summary>
		/// Creates a secure representation of the base64 encoded string content
		/// </summary>
		public static SecureString ToSecureBase64(this string str, Encoding encoding)
		{
			SecureString secureString = new SecureString();
			if (!string.IsNullOrEmpty(str))
			{
				foreach (char c in Convert.ToBase64String(encoding.GetBytes(str)))
					secureString.AppendChar(c);
			}
			return secureString;
		}
		/// <summary>
		/// Creates a secure representation of the base64 encoded string content
		/// </summary>
		public static SecureString ToSecureBase64(this string str)
		{
			return ToSecureBase64(str, Encoding.ASCII);
		}
	}
}
