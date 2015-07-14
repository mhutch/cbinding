using ClangSharp;

namespace CBinding.Parser
{
	public class Struct: Symbol
	{
		public Struct (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}
	
	}

}
