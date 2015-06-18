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
	class Reference
	{
		CProject project;
		CXCursor cursor;
		CXSourceRange sourceRange;
		SourceLocation begin, end;
		uint offset;

		public Reference(CProject proj, CXCursor cursor, CXSourceRange sourceRange) {
			project = proj;
			this.cursor = cursor;
			this.sourceRange = sourceRange;
			begin = project.cLangManager.getSourceLocation (clang.getRangeStart (sourceRange));
			end = project.cLangManager.getSourceLocation (clang.getRangeEnd (sourceRange));
			offset = Convert.ToUInt32 (begin.Offset);
		}

		public CXSourceRange SourceRange {
			get {
				return sourceRange;
			}
			set {
				sourceRange = value;
			}
		}

		public CXCursor Cursor {
			get {
				return cursor;
			}
			set {
				cursor = value;
			}
		}

		public SourceLocation Begin {
			get {
				return begin;
			}
		}
			
		public SourceLocation End {
			get {
				return end;
			}
		}

		public uint Length{
			get{
				return end.Offset - begin.Offset;
			}
		}

		public uint Offset {
			get {
				return offset;
			}
		}

		public string FileName {
			get{
				return begin.FileName;
			}
		}

		public override bool Equals (object obj)
		{
			Reference other = obj as Reference;
			return 
				other.begin.FileName.Equals (begin.FileName) 
				&& other.offset.Equals (offset) 
				&& Length.Equals (other.Length);
		}
	}
}

