using ClangSharp;

namespace CBinding.Parser
{
	public class Variable : Symbol
	{
		public Variable (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}

	}

}
