using ClangSharp;

namespace CBinding.Parser
{
	public class ClassTemplate : Class
	{
		public ClassTemplate (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}
	}
}
