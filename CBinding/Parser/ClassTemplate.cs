using System;
using ClangSharp;
using ICSharpCode.NRefactory6.CSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.CSharp;

namespace CBinding
{
	public class ClassTemplate : Class
	{
		public ClassTemplate (CXCursor cursor) : base (cursor)
		{
		}
	}
}
