using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using ClangSharp;
using System.Collections.Generic;
using MonoDevelop.Ide.Editor;
using CBinding.Parser;

namespace CBinding.Refactoring
{
	//Based on code from CSharpBinding
	public class Reference : IComparable
	{
		CProject project;
		public SourceLocation Begin { get; }
		public SourceLocation End {	get; }
		public CXSourceRange SourceRange { get; set; }
		public CXCursor Cursor { get; set; }
		public int Offset {	get; private set; }

		public int Length{
			get{
				return End.Offset - Begin.Offset;
			}
		}
			
		public string FileName {
			get{
				return Begin.FileName;
			}
		}

		public Reference(CProject proj, CXCursor cursor, CXSourceRange sourceRange) {
			project = proj;
			Cursor = cursor;
			SourceRange = sourceRange;
			Begin = project.cLangManager.getSourceLocation (clang.getRangeStart (sourceRange));
			End = project.cLangManager.getSourceLocation (clang.getRangeEnd (sourceRange));
			Offset = Begin.Offset;
		}
			
		public override bool Equals (object obj)
		{
			Reference other = obj as Reference;
			return 
				other.Begin.FileName.Equals (Begin.FileName) 
				&& other.Offset.Equals (Offset) 
				&& Length.Equals (other.Length);
		}

		#region IComparable implementation

		public int CompareTo (object obj)
		{	
			Reference other = obj as Reference;
			return FileName.Equals (other.FileName) ?
				Offset.CompareTo (other.Offset) 
					:
				FileName.CompareTo (other.FileName);
			}

		#endregion
	}
}

