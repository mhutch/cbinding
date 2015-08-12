using ClangSharp;

namespace CBinding.Parser
{
	public class Class : Symbol
	{
		public Class (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}
	}
}
