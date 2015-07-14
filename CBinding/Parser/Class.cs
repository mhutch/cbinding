using ClangSharp;

namespace CBinding.Parser
{
	public class Class : Symbol
	{
		public Class (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}
	}
}
