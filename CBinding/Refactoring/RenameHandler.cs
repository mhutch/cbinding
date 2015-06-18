using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using ClangSharp;
using MonoDevelop.Core;

namespace CBinding.Refactoring
{/*
	//Based on code from CSharpBinding
	public class RenameHandler
	{
		CProject project;

		public RenameHandler(CProject proj) {
			project = proj;
		}

		public void Update (CommandInfo commandInfo) {
			var doc = IdeApp.Workbench.ActiveDocument;
			if (doc == null || doc.FileName == FilePath.Null || doc.ParsedDocument == null) {
				commandInfo.Enabled = false;
				return;
			}
			CXCursor cursor = project.cLangManager.getCursor (doc.FileName, doc.Editor.CaretLocation);
			commandInfo.Bypass = !IsRenameableCursor(cursor);			
		}

		public void Run(TextEditor textEditor, DocumentContext documentContext) {
			
		}

		private bool IsRenameableCursor(CXCursor cursor) {
			switch (cursor.kind) {
			case CXCursorKind.CXCursor_VarDecl:
			case CXCursorKind.CXCursor_VariableRef:
			case CXCursorKind.CXCursor_ClassDecl:
			case CXCursorKind.CXCursor_ClassTemplate:
			case CXCursorKind.CXCursor_ClassTemplatePartialSpecialization:
			case CXCursorKind.CXCursor_FunctionDecl:
			case CXCursorKind.CXCursor_FunctionTemplate:
			case CXCursorKind.CXCursor_FieldDecl:
			case CXCursorKind.CXCursor_MemberRef:
			case CXCursorKind.CXCursor_CXXMethod:
			case CXCursorKind.CXCursor_Namespace:
			case CXCursorKind.CXCursor_NamespaceRef:
			case CXCursorKind.CXCursor_NamespaceAlias:
			case CXCursorKind.CXCursor_EnumDecl:
			case CXCursorKind.CXCursor_EnumConstantDecl:
			case CXCursorKind.CXCursor_StructDecl:
			case CXCursorKind.CXCursor_TypedefDecl:
			case CXCursorKind.CXCursor_TypeRef:
			case CXCursorKind.CXCursor_DeclRefExpr:
			case CXCursorKind.CXCursor_ParmDecl:
			case CXCursorKind.CXCursor_TemplateTypeParameter:
			case CXCursorKind.CXCursor_TemplateTemplateParameter:
			case CXCursorKind.CXCursor_NonTypeTemplateParameter:
				return true;
			default:
				return false;
			}		}
	}
	*/
}
