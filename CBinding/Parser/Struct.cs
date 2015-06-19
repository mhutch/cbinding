using System;
using ClangSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;

namespace CBinding.Parser
{
	public class Struct: Symbol
	{
		public Struct (CProject proj, string fileN, CXCursor cursor) : base (proj, fileN, cursor) 
		{
		}
	}
}
