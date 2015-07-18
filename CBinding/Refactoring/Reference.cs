using System;
using ClangSharp;
using CBinding.Parser;

namespace CBinding.Refactoring
{
	//Based on code from CSharpBinding
	/// <summary>
	/// Reference class containing information about a given cursor and its location in the source files.
	/// </summary>
	public class Reference : IComparable
	{
		CProject project;
		public SourceLocation Begin { get; }
		public SourceLocation End {	get; }
		public CXSourceRange SourceRange { get; set; }
		public CXCursor Cursor { get; set; }
		public int Offset {	get; private set; }
		public int Length{ get; private set; }
		public string FileName { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CBinding.Refactoring.Reference"/> class.
		/// </summary>
		/// <param name="proj">Project.</param>
		/// <param name="cursor">Cursor referenced.</param>
		/// <param name="sourceRange">Source location and range.</param>
		public Reference(CProject proj, CXCursor cursor, CXSourceRange sourceRange) {
			project = proj;
			Cursor = cursor;
			SourceRange = sourceRange;
			Begin = project.ClangManager.GetSourceLocation (clang.getRangeStart (sourceRange));
			End = project.ClangManager.GetSourceLocation (clang.getRangeEnd (sourceRange));
			Offset = Begin.Offset;
			FileName = Begin.FileName;
			Length = End.Offset - Begin.Offset;

		}
			
		public override bool Equals (object obj)
		{
			Reference other = (Reference) obj;
			return 
				other.Begin.FileName.Equals (Begin.FileName) 
				&& other.Offset.Equals (Offset) 
				&& Length.Equals (other.Length);
		}

		#region IComparable implementation

		public int CompareTo (object obj)
		{	
			Reference other = (Reference)obj;
			return FileName.Equals (other.FileName) ?
				Offset.CompareTo (other.Offset) 
					:
				FileName.CompareTo (other.FileName);
			}

		#endregion
	}
}

