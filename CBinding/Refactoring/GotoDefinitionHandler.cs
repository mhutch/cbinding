using MonoDevelop.Components.Commands;
using ClangSharp;
using MonoDevelop.Ide;
using CBinding.Parser;
using MonoDevelop.Core;

namespace CBinding.Refactoring
{
	/// <summary>
	/// Goto definition handler.
	/// </summary>
	public class GotoDefinitionHandler : CommandHandler
	{
		/// <summary>
		/// Run this instance and jump to definition of the cursor at the caret's position.
		/// </summary>
		protected override void Run ()
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			var project = (CProject)doc.Project;
			CXCursor cursor = project.ClangManager.GetCursor (doc.FileName, doc.Editor.CaretLocation);
			CXCursor definingCursor = project.DB.getDefinition (clang.getCursorReferenced (cursor));
			if (clang.Cursor_isNull (definingCursor) == 0) {
				SourceLocation loc = project.ClangManager.GetCursorLocation (definingCursor);
				IdeApp.Workbench.OpenDocument ((FilePath)loc.FileName, project, (int)loc.Line, (int)loc.Column);
			}
		}

		/// <summary>
		/// Update the specified info.
		/// </summary>
		/// <param name="info">Info.</param>
		protected override void Update (CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			var project = (CProject)doc.Project;
			CXCursor cursor = project.ClangManager.GetCursor (doc.FileName, doc.Editor.CaretLocation);
			info.Visible = (clang.Cursor_isNull (cursor) == 0);
			info.Bypass = !info.Visible;
		}

	}
}

