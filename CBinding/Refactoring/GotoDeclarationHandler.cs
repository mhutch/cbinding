using System;
using MonoDevelop.Components.Commands;
using ClangSharp;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Ide;
using CBinding.Parser;
using MonoDevelop.Core;

namespace CBinding.Refactoring
{
	/// <summary>
	/// Goto declaration handler.
	/// </summary>
	public class GotoDeclarationHandler
	{
		/// <summary>
		/// Run this instance and jump to declaration of the cursor at the caret's position.
		/// </summary>
		public void Run ()
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			CProject project = doc.Project as CProject;
			CXCursor cursor = project.cLangManager.getCursor (doc.FileName, doc.Editor.CaretLocation);
			CXCursor referredCursor = project.cLangManager.getCursorReferenced (cursor);
			CXCursor declCursor = project.db.getDeclaration (referredCursor);
			referredCursor = declCursor;
			if (clang.Cursor_isNull (referredCursor) == 0) {
				SourceLocation loc = project.cLangManager.getCursorLocation (referredCursor);
				IdeApp.Workbench.OpenDocument ((FilePath)loc.FileName, project, (int)loc.Line, (int)loc.Column);
			}
		}

		/// <summary>
		/// Update the specified info.
		/// </summary>
		/// <param name="info">Info.</param>
		public void Update (CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			CProject project = doc.Project as CProject;
			CXCursor cursor = project.cLangManager.getCursor (doc.FileName, doc.Editor.CaretLocation);
			CXCursor referredCursor = project.cLangManager.getCursorReferenced (cursor);
			CXCursor declCursor = project.db.getDeclaration (referredCursor);
			referredCursor = declCursor;
			info.Visible = (clang.Cursor_isNull (referredCursor) == 0);
			info.Bypass = !info.Visible;
		}

	}
}

