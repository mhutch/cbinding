using System;
using ClangSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;
using CBinding;

namespace CBinding.Parser
{
	public class Field : Symbol
	{
		public Field (CProject proj, string fileN, CXCursor cursor) : base (proj, fileN, cursor) 
		{
		}

	}

}