using ClangSharp;

namespace CBinding.Parser
{
	public class Macro : Symbol
	{
		public Macro (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}
	}
}
