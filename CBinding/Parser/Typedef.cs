using ClangSharp;

namespace CBinding.Parser
{
	public class Typedef : Symbol
	{
		public Typedef (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}

	}

}
