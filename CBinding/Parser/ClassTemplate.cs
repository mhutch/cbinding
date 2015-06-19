using System;
using ClangSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.CSharp;

namespace CBinding.Parser
{
	public class ClassTemplate : Class
	{
		public ClassTemplate (CProject proj, string fileN, CXCursor cursor) : base (proj, fileN, cursor) 
		{
		}
	}
}
