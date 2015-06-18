using System;
using ClangSharp;


namespace CBinding.Parser
{
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
