using ClangSharp;

namespace CBinding.Parser
{
	public class Variable : Symbol
	{
		public Variable (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}

	}

}
