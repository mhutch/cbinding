using System;
using ClangSharp;
using ICSharpCode.NRefactory6.CSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.MonoCSharp;

namespace CBinding.Parser
{
	public class Function : Symbol
	{
		string[] parameters;
		int parameterCount;

		public Function (CProject proj, string fileN, CXCursor cursor) : base (proj, fileN, cursor) {
			parameterCount = clang.Cursor_getNumArguments (cursor);
			if (parameterCount != -1) {
				parameters = new string[ParameterCount];
				for (uint i = 0; i < ParameterCount; i++) {
					parameters [i] = project.cLangManager.getCursorDisplayName (clang.Cursor_getArgument (cursor, i));
				}
			}
		}

		public int ParameterCount {
			get { return parameterCount; }
		}
			
		public string[] Parameters{
			get {
				return parameters;
			}
		}
	}
}