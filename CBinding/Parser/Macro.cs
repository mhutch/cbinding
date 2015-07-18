using ClangSharp;

namespace CBinding.Parser
{
	public class Macro : Symbol
	{
		public Macro (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}
	}
}
