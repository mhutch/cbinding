using System;
using ClangSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;

namespace CBinding.Parser
{
	public class Variable : Symbol
	{
		public Variable (CProject proj, string fileN, CXCursor cursor) : base (proj, fileN, cursor) 
		{
		}
	}
}