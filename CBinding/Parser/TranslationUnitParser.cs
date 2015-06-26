using System;
using ClangSharp;


namespace CBinding.Parser
{
	/// <summary>
	/// Translation unit parser. Can traverse the translation unit's AST in clang_visitChildren with it's Visit method.
	/// Builds the symbol database associated with a project.
	/// </summary>
	public class TranslationUnitParser{
		private ClangProjectSymbolDatabase db;
		private string file;

		public TranslationUnitParser(ClangProjectSymbolDatabase db, string file){
			this.db = db;
			this.file = file;
		}

		public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data){
			db.AddToDatabase (file, cursor);
			return CXChildVisitResult.CXChildVisit_Recurse;
		}

	}
	
}
