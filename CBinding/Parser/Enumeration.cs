using ClangSharp;

namespace CBinding.Parser
{
	public class Enumeration : Symbol
	{
		public Enumeration (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}
	}
}