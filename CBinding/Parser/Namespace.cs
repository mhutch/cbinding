using ClangSharp;

namespace CBinding.Parser
{
	public class Namespace : Symbol
	{
		public Namespace (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}
	}
}
