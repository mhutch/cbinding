using System;
using ClangSharp;
using ICSharpCode.NRefactory6.CSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;

namespace CBinding.Parser
{
	public class Union : Symbol
	{
		public Union (CXCursor cursor) : base (cursor)
		{
		}
	}
}
