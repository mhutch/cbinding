using ClangSharp;

namespace CBinding.Parser
{

	public class MemberFunction : Function
	{
		public MemberFunction (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}
	}

}
