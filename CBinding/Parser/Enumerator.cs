using System;
using ClangSharp;
using ICSharpCode.NRefactory6.CSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;

namespace CBinding
{
	public class Enumerator : Symbol
	{
		public Enumerator (CXCursor cursor) : base (cursor)
		{
		}
	}
}
