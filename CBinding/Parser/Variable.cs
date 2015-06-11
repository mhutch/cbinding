using System;
using ClangSharp;
using ICSharpCode.NRefactory6.CSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;

namespace CBinding
{
	public class Variable : Symbol
	{
		public Variable (CXCursor cursor) : base (cursor)
		{
		}
	}
}