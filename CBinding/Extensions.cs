namespace ClangSharp
{
	using System.Runtime.InteropServices;

	public partial struct CXString
	{
		public override string ToString()
		{
			string retval = Marshal.PtrToStringAnsi(clang.getCString (this));
			clang.disposeString(this);
			return retval;
		}
	}

	public partial struct CXType
	{
		public override string ToString()
		{
			return clang.getTypeSpelling(this).ToString();
		}
	}

	public partial struct CXCursor
	{
		public override string ToString()
		{
			return clang.getCursorSpelling(this).ToString();
		}
	}

	public partial struct CXDiagnostic
	{
		public override string ToString()
		{
			return clang.getDiagnosticSpelling(this).ToString();
		}
	}

	public partial struct CXUnsavedFile
	{
		public void Initialize (string fileName, string content, bool bomPresent)
		{
			Filename = fileName;
			Contents = content;
			Length = content.Length;
			if (bomPresent) {
				Length += 3;
				Contents = "   " + Contents;
			}
		}
	}
}