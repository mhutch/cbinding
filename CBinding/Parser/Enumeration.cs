using ClangSharp;

namespace CBinding.Parser
{
	public class Enumeration : Symbol
	{
		public Enumeration (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}
	}
}