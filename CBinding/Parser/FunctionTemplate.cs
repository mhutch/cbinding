using System;
using ClangSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;

namespace CBinding.Parser
{
	public class FunctionTemplate : Function
	{
		public FunctionTemplate (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}

	}
		
}
