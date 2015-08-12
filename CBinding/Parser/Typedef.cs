using ClangSharp;

namespace CBinding.Parser
{
	public class Typedef : Symbol
	{
		public Typedef (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}

	}

}
