using System;
using ClangSharp;


namespace CBinding
{
	public class TranslationUnitParser{
		private ClangSymbolDatabase db;

		public TranslationUnitParser(ClangSymbolDatabase db){
			this.db = db;
		}

		public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data){
			db.AddToDatabase (cursor);
			return CXChildVisitResult.CXChildVisit_Recurse;
		}

	}
	
}
