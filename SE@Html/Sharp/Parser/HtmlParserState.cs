// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Text.Html
{
	public enum HtmlParserState : byte
	{
		/// <summary>
		/// HTML5 8.2.5.4.1. The "initial" insertion mode
		/// https://www.w3.org/TR/html53/syntax.html#the-initial-insertion-mode
		/// </summary>
		Initial = 0,

		/// <summary>
		/// HTML5 8.2.5.4.2. The "before html" insertion mode
		/// https://www.w3.org/TR/html53/syntax.html#the-before-html-insertion-mode
		/// </summary>
		BeforeHtml = 1,
		
		BeforeHead,
		
		InHead,
		
		InHeadNoScript,
		
		AfterHead,
		
		InBody,
		
		Text,
		
		InTable,
		
		InTabletext,
		
		InCaption,
		
		InColumnGroup,
		
		InTableBody,
		
		InRow,
		
		InCell,
		
		InSelect,
		
		InSelectInTable,
		
		InTemplate,
		
		AfterBody,
		
		InFrameset,
		
		AfterFrameset,
		
		AfterAfterBody,
		
		AfterAfterFrameset,

		Failure
	}
}
