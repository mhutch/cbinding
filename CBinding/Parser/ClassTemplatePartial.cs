using ClangSharp;

namespace CBinding.Parser
{
	public class ClassTemplatePartial : ClassTemplate
	{
		public ClassTemplatePartial (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}
	}
}
