using ClangSharp;

namespace CBinding.Parser
{
	public class ClassTemplate : Class
	{
		public ClassTemplate (CProject proj, CXCursor cursor ) : base (proj , cursor)
		{
		}
	}
}
