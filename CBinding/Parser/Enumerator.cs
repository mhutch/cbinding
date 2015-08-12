using ClangSharp;

namespace CBinding.Parser
{
	public class Enumerator : Symbol
	{
		public Enumerator (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}
	}
}
