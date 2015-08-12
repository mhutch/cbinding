using System;
using ClangSharp;

namespace CBinding.Parser
{
	/// <summary>
	/// Represents clang abstract symbols from the built AST
	/// </summary>
	public class Symbol
	{

		public SourceLocation Begin { get; }

		public CXCursor Represented { get; }

		public string Usr { get; }

		public bool Def { get; }

		public Symbol (CProject project, CXCursor cursor)
		{
			lock (project.ClangManager.SyncRoot) {
				Represented = cursor;
				Usr = clang.getCursorUSR (cursor).ToString ();
				Begin = project.ClangManager.GetSourceLocation (
					clang.getRangeStart (clang.Cursor_getSpellingNameRange (cursor, 0, 0))
				);
				Def = clang.isCursorDefinition (cursor) != 0;
			}
		}
	}
}