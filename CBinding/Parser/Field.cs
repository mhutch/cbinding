using ClangSharp;
using CBinding;

namespace CBinding.Parser
{
	public class Field : Symbol
	{
		public Field (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
		}

	}

}