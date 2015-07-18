using ClangSharp;

namespace CBinding.Parser
{
	public class Union : Symbol
	{
		public Union (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}
	
	}

}
