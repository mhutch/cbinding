using System;
using ClangSharp;
using ICSharpCode.NRefactory6.CSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;
using CBinding;

namespace CBinding
{
	public class Field : Symbol
	{
		public Field(CXCursor cursor) : base(cursor)
		{
		}

	}

}