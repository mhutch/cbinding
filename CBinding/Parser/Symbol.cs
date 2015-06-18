using System;
using ClangSharp;
using ICSharpCode.NRefactory6.CSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;
using System.IO;

namespace CBinding.Parser
{

	public class Symbol
	{
		protected string signature;
		protected CX_CXXAccessSpecifier access;
		protected CXCursor represented;

		public Symbol (CXCursor cursor)
		{
			represented = cursor;
			access = clang.getCXXAccessSpecifier (cursor);
			CXString cxstring = clang.getCursorDisplayName (cursor);
			signature = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
			clang.disposeString (cxstring);
		}

		public string Signature {
			get {
				return signature;
			}
		}
	}
}