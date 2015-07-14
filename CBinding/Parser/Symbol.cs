using System;
using ClangSharp;

namespace CBinding.Parser
{
	/// <summary>
	/// Represents clang abstract symbols from the built AST
	/// </summary>
	public class Symbol
	{
		
		public bool IsDeclaration { get; protected set; }

		public bool IsGlobal { get; protected set; }

		public CXCursor Referenced { get; protected set; }

		public bool IsDefinition { get; protected set; }

		public bool IsConst { get; protected set; }

		public SourceLocation Begin { get; protected set; }

		public SourceLocation End { get; protected set; }

		public int SpellingLength { get; protected set; }

		public string Usr { get; protected set; }

		public CProject Project { get; protected set; }

		public string FileName { get; protected set; }

		public string Signature { get; protected set; }

		public string Spelling { get; protected set; }

		public string Name { get; protected set; }

		public CX_CXXAccessSpecifier Access { get; protected set; }

		public CXCursor Represented { get; protected set; }

		public CXCursor ParentCursor { get; protected set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="CBinding.Parser.Symbol"/> is ours, meaning its not a cursor from an included file.
		/// </summary>
		/// <value><c>true</c> if ours; otherwise, <c>false</c>.</value>
		public bool Ours { get; protected set; }

		public Symbol (CProject proj, string fileN, CXCursor cursor, bool global)
		{
			Project = proj;
			FileName = fileN;
			Represented = cursor;
			ParentCursor = clang.getCursorSemanticParent (cursor);
			Access = clang.getCXXAccessSpecifier (cursor);
			Referenced = clang.getCursorReferenced (cursor);
			Usr = Project.ClangManager.GetCursorUsrString (cursor);
			Signature = Project.ClangManager.GetCursorDisplayName (cursor);
			Begin = Project.ClangManager.GetSourceLocation (
				clang.getRangeStart (clang.Cursor_getSpellingNameRange (cursor,0,0))
			);
			End = Project.ClangManager.GetSourceLocation (
				clang.getRangeEnd (clang.Cursor_getSpellingNameRange (cursor,0,0))
			);
			IsDefinition = clang.isCursorDefinition (cursor) != 0;
			IsConst = clang.isConstQualifiedType (clang.getCursorType (cursor)) != 0;
			SpellingLength = Convert.ToInt32 (End.Offset - Begin.Offset);
			Spelling = clang.getCursorSpelling (cursor).ToString ();
			IsDeclaration = clang.isDeclaration (cursor.kind) != 0;
			IsGlobal = global;
			Ours = fileN.Equals (Begin.FileName);
		}
			
		public Symbol Parent {
			get {
				try 
				{
					switch (ParentCursor.kind) {
					case CXCursorKind.TranslationUnit:
						return null;
					case CXCursorKind.Namespace:
						return new Namespace (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.ClassDecl:
						return new Class (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.FieldDecl:
						return new Field (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.ClassTemplate:
						return new ClassTemplate (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.ClassTemplatePartialSpecialization:
						return new ClassTemplatePartial (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.StructDecl:
						return new Struct (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.FunctionDecl:
						return new Function (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.CXXMethod:
						return new MemberFunction (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.FunctionTemplate:
						return new FunctionTemplate (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.EnumDecl:
						return new Enumeration (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.EnumConstantDecl:
						return new Enumerator (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.UnionDecl:
						return new Union (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.TypedefDecl:
						return new Typedef (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.VarDecl:
						return new Variable (Project, FileName, ParentCursor, IsGlobal);
					case CXCursorKind.MacroDefinition:
						return new Macro (Project, FileName, ParentCursor, IsGlobal);
					default:
						return new Symbol (Project, FileName, ParentCursor, IsGlobal);
					}
				} catch (Exception ex) {
					// sometimes, when a cursor is in a "distant" header file symbol initialization can fail (cant open file for read)
					// this is normally not fatal, trying the requested action in the IDE again fixes the problem (99,99%)
					return null;
				}
			}
		}



		public override bool Equals (object obj)
		{
			Symbol other = (Symbol) obj;
			return Represented.Equals (other.Represented);
		}
	}
}