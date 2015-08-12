using System;
using ClangSharp;
using System.Collections.Generic;

namespace CBinding.Parser
{
	public class Function : Symbol
	{
		public Function (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}
	}
}
