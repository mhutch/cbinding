using System;
using ClangSharp;
using System.Threading;


namespace CBinding.Parser
{
	/// <summary>
	/// Translation unit parser. Can traverse the translation unit's AST in clang_visitChildren with it's Visit method.
	/// Builds the symbol database associated with a project.
	/// </summary>
	public class TranslationUnitParser
	{
		ClangProjectSymbolDatabase db;
		string file;
		CancellationToken token;
		CXCursor TUCursor;

		public TranslationUnitParser (ClangProjectSymbolDatabase db, string file, CancellationToken cancelToken, CXCursor TUcur)
		{
			this.db = db;
			this.file = file;
			token = cancelToken;
			TUCursor = TUcur;
		}

		public CXChildVisitResult Visit (CXCursor cursor, CXCursor parent, IntPtr data)
		{
			if (token.IsCancellationRequested)
				return CXChildVisitResult.Break;

			bool global = TUCursor.Equals (parent);
			db.AddToDatabase (file, cursor, global);
			return CXChildVisitResult.Recurse;
		}
	
	}

}
