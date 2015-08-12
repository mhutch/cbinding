using ClangSharp;

namespace CBinding.Parser
{
	public class Namespace : Symbol
	{
		public Namespace (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}
	}
}
