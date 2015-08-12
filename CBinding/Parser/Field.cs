using ClangSharp;
using CBinding;

namespace CBinding.Parser
{
	public class Field : Symbol
	{
		public Field (CProject proj, CXCursor cursor) : base (proj, cursor)
		{
		}

	}

}