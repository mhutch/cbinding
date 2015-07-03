using System;
using ClangSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;
using System.IO;
using Gdk;

namespace CBinding.Parser
{
	/// <summary>
	/// Represents clang abstract symbols from the built AST
	/// </summary>
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
		public bool IsDeclaration { get; }
		protected bool isConst;
		protected SourceLocation begin;
		protected SourceLocation end;
		protected int spellingLength;
		protected string uSR;
		public bool IsGlobal { get; }
		/// <summary>
		/// Gets a value indicating whether this <see cref="CBinding.Parser.Symbol"/> is ours, meaning its not a cursor from an included file.
		/// </summary>
		/// <value><c>true</c> if ours; otherwise, <c>false</c>.</value>
		public bool Ours { get; private set; }
		public Symbol (CProject proj, string fileN, CXCursor cursor, bool global)
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
			IsDeclaration = clang.isDeclaration (cursor.kind) != 0;
			IsGlobal = global;
			Ours = fileN.Equals (begin.FileName);
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

		public string Name {
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

		public CXCursor ParentCursor {
			get {
				return parent;
			}
		}

		public Symbol Parent {
			get {
				try 
				{
					switch (parent.kind) {
					case CXCursorKind.CXCursor_TranslationUnit:
						return null;
					case CXCursorKind.CXCursor_Namespace:
						return new Namespace (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_ClassDecl:
						return new Class (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_FieldDecl:
						return new Field (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_ClassTemplate:
						return new ClassTemplate (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_ClassTemplatePartialSpecialization:
						return new ClassTemplatePartial (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_StructDecl:
						return new Struct (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_FunctionDecl:
						return new Function (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_CXXMethod:
						return new MemberFunction (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_FunctionTemplate:
						return new FunctionTemplate (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_EnumDecl:
						return new Enumeration (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_EnumConstantDecl:
						return new Enumerator (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_UnionDecl:
						return new Union (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_TypedefDecl:
						return new Typedef (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_VarDecl:
						return new Variable (project, fileName, parent, IsGlobal);
					case CXCursorKind.CXCursor_MacroDefinition:
						return new Macro (project, fileName, parent, IsGlobal);
					default:
						return new Symbol (project, fileName, parent, IsGlobal);
					}
				} catch (Exception ex){
					return null;
				}
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