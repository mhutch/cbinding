using System;
using ClangSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;
using System.IO;

namespace CBinding.Parser
{

	public class Symbol
	{
		protected CProject project;
		protected string fileName;
		protected string signature;
		protected string spelling;
		protected CX_CXXAccessSpecifier access;
		protected CXCursor represented;
		protected CXCursor parent;
		protected CXCursor referenced;
		protected bool isDefinition;
		protected bool isConst;
		protected SourceLocation begin;
		protected SourceLocation end;
		protected int spellingLength;
		protected string uSR;

		public Symbol (CProject proj, string fileN, CXCursor cursor)
		{
			project = proj;
			fileName = fileN;
			represented = cursor;
			parent = clang.getCursorSemanticParent (cursor);
			access = clang.getCXXAccessSpecifier (cursor);
			referenced = clang.getCursorReferenced (cursor);
			uSR = project.cLangManager.getCursorUSRString (cursor);
			signature = project.cLangManager.getCursorDisplayName (cursor);
			begin = project.cLangManager.getSourceLocation (
				clang.getRangeStart (clang.Cursor_getSpellingNameRange (cursor,0,0))
			);
			end = project.cLangManager.getSourceLocation (
				clang.getRangeEnd (clang.Cursor_getSpellingNameRange (cursor,0,0))
			);
			isDefinition = clang.isCursorDefinition (cursor) != 0;
			isConst = clang.isConstQualifiedType (clang.getCursorType (cursor)) != 0;
			spellingLength = Convert.ToInt32 (end.Offset - begin.Offset);
			spelling = clang.getCursorSpelling (cursor).ToString ();
		}

		public CProject Project {
			get {
				return project;
			}
		}

		public string FileName {
			get {
				return fileName;
			}
		}

		public string Signature {
			get {
				return signature;
			}
		}

		public string Spelling {
			get {
				return spelling;
			}
		}

		public CX_CXXAccessSpecifier Access {
			get {
				return access;
			}
		}

		public CXCursor Represented {
			get {
				return represented;
			}
		}

		public CXCursor Parent {
			get {
				return parent;
			}
		}

		public CXCursor Referenced {
			get {
				return referenced;
			}
		}

		public bool IsDefinition {
			get {
				return isDefinition;
			}
		}

		public bool IsConst {
			get {
				return isConst;
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

		public int SpellingLength {
			get {
				return spellingLength;
			}
		}

		public string USR {
			get {
				return uSR;
			}
		}

		public override bool Equals (object obj)
		{
			Symbol other = obj as Symbol;
			return represented.Equals (other.represented);
		}
	}
}