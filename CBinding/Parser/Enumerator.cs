using ClangSharp;

namespace CBinding.Parser
{
	public class Enumerator : Symbol
	{
		public Enumerator (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}
	}
}
