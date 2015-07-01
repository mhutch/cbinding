using System;
using ClangSharp;
using System.Threading;


namespace CBinding.Parser
{
	/// <summary>
	/// Translation unit parser. Can traverse the translation unit's AST in clang_visitChildren with it's Visit method.
	/// Builds the symbol database associated with a project.
	/// </summary>
	public class TranslationUnitParser{
		ClangProjectSymbolDatabase db;
		string file;
		CancellationToken token;

		public TranslationUnitParser(ClangProjectSymbolDatabase db, string file, CancellationToken cancelToken){
			this.db = db;
			this.file = file;
			token = cancelToken;
		}

		public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data){
			if (token.IsCancellationRequested) {
				return CXChildVisitResult.CXChildVisit_Break;
			}
			db.AddToDatabase (file, cursor);
			return CXChildVisitResult.CXChildVisit_Recurse;
		}

	}
	
}
