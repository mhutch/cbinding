using MonoDevelop.Components.Commands;
using ClangSharp;
using MonoDevelop.Ide;
using CBinding.Parser;
using MonoDevelop.Core;
using System;
using MonoDevelop.Ide.FindInFiles;

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
			var monitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor (true, true);
			try {
				var doc = IdeApp.Workbench.ActiveDocument;
				var project = (CProject)doc.Project;
				CXCursor cursor = project.ClangManager.GetCursor (doc.FileName, doc.Editor.CaretLocation);
				CXCursor referredCursor = project.ClangManager.GetCursorReferenced (cursor);
				bool leastOne = false;
				foreach (var decl in project.DB.getDeclarations (referredCursor)) {
					leastOne = true;
					var sr = new SearchResult (
						new FileProvider (decl.FileName),
						decl.Offset,
						1
					);
					monitor.ReportResult (sr);
				}
				if (!leastOne) {
					var loc = project.ClangManager.GetCursorLocation (referredCursor);
					IdeApp.Workbench.OpenDocument (loc.FileName, project, loc.Line, loc.Column);
				}
			} catch (Exception ex) {
				if (monitor != null)
					monitor.ReportError ("Error finding declarations", ex);
				else
					LoggingService.LogError ("Error finding declarations", ex);
			} finally {
				if (monitor != null)
					monitor.Dispose ();
			}
		}

		/// <summary>
		/// Update the specified info.
		/// </summary>
		/// <param name="info">Info.</param>
		public void Update (CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			CProject project;
			if (doc == null || (project = doc.Project as CProject) == null || !project.HasLibClang) {
				info.Enabled = info.Visible = false;
				return;
			}

			CXCursor cursor = project.ClangManager.GetCursor (doc.FileName, doc.Editor.CaretLocation);
			CXCursor referredCursor = project.ClangManager.GetCursorReferenced (cursor);
			info.Enabled = info.Visible = (clang.Cursor_isNull (referredCursor) == 0);
		}

	}
}

