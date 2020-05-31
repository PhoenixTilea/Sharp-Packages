// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Security;
using SE;

namespace SE.Storage.Http
{
	public static partial class StringExtension
    {
		/// <summary>
		/// Creates a secure representation of the string content
		/// </summary>
		public static SecureString ToSecureString(this string str)
		{
			SecureString secureString = new SecureString();
			if (!string.IsNullOrEmpty(str))
			{
				foreach (char c in str)
					secureString.AppendChar(c);
			}
			return secureString;
		}
	}
}
