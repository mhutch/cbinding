using System;
using ClangSharp;
using ICSharpCode.NRefactory6.CSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.MonoCSharp;

namespace CBinding
{
	public class Function : Symbol
	{
		string simpleName;
		public Function (CXCursor cursor) : base (cursor){
			CXString cxstring = clang.getCursorSpelling (cursor);
			simpleName = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
			clang.disposeString (cxstring);
		}
		public string SimpleName {
			get {
				return simpleName;
			}
		}

		public int ParameterCount {
			get { return clang.Cursor_getNumArguments (represented); }
		}

		public bool IsConst {
			get {
				return Convert.ToBoolean (clang.isConstQualifiedType(clang.getCursorType (represented)));
			}
		}

		public string[] Parameters{
			get{
				string[] parameters	= new string[ParameterCount];
				for(uint i = 0; i < ParameterCount; i++){
					CXString cxstring = clang.getCursorDisplayName (clang.Cursor_getArgument (represented, i));
					parameters [i] = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
					clang.disposeString (cxstring);
				}
				return parameters;
			}
		}
	}
}