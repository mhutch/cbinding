using System;
using MonoDevelop.Components.Commands;
using ClangSharp;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Ide;
using CBinding.Parser;
using MonoDevelop.Core;

namespace CBinding.Refactoring
{
	public class GotoDefinitionHandler : CommandHandler
	{
				protected override void Run ()
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			CProject project = doc.Project as CProject;
			CXCursor cursor = project.cLangManager.getCursor (doc.FileName, doc.Editor.CaretLocation);
			CXCursor definingCursor = project.db.getDefinition (clang.getCursorReferenced (cursor));
			if (clang.Cursor_isNull (definingCursor) == 0) {
				SourceLocation loc = project.cLangManager.getCursorLocation (definingCursor);
				IdeApp.Workbench.OpenDocument ((FilePath)loc.FileName, project, (int)loc.Line, (int)loc.Column);
			}
		}

		protected override void Update (CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			CProject project = doc.Project as CProject;
			CXCursor cursor = project.cLangManager.getCursor (doc.FileName, doc.Editor.CaretLocation);
			info.Visible = (clang.Cursor_isNull (cursor) == 0);
			info.Bypass = !info.Visible;
		}

	}
}

