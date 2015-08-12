using ClangSharp;

namespace CBinding.Parser
{
	public class ClassTemplatePartial : ClassTemplate
	{
		public ClassTemplatePartial (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}
	}
}
