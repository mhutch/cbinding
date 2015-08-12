using ClangSharp;

namespace CBinding.Parser
{

	public class MemberFunction : Function
	{
		public MemberFunction (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}
	}

}
