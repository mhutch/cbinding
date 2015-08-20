using MonoDevelop.Components.Commands;
using ClangSharp;
using MonoDevelop.Ide;
using CBinding.Parser;
using MonoDevelop.Core;
using MonoDevelop.Ide.FindInFiles;
using System;

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
			var monitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor (true, true);
			try {
				var doc = IdeApp.Workbench.ActiveDocument;
				var project = (CProject)doc.Project;
				CXCursor cursor = project.ClangManager.GetCursor (doc.FileName, doc.Editor.CaretLocation);
				CXCursor referredCursor = project.ClangManager.GetCursorReferenced (cursor);
				bool leastOne = false;
				foreach (var decl in project.DB.GetDefinitionLocation (referredCursor)) {
					leastOne = true;
					var sr = new SearchResult (
						new FileProvider (decl.FileName),
						decl.Offset,
						1
					);
					monitor.ReportResult (sr);
				}
				if (!leastOne) {
					CXCursor defCursor = project.ClangManager.GetCursorDefinition (referredCursor);
					var loc = project.ClangManager.GetCursorLocation (defCursor);
					IdeApp.Workbench.OpenDocument (loc.FileName, project, loc.Line, loc.Column);
				}
			} catch (Exception ex) {
				if (monitor != null)
					monitor.ReportError ("Error finding definition", ex);
				else
					LoggingService.LogError ("Error finding definition", ex);
			} finally {
				if (monitor != null)
					monitor.Dispose ();
			}
			
		}

		/// <summary>
		/// Update the specified info.
		/// </summary>
		/// <param name="info">Info.</param>
		protected override void Update (CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			CProject project;
			if (doc == null || (project = doc.Project as CProject) == null || !project.HasLibClang) {
				info.Enabled = info.Visible = false;
				return;
			}

			CXCursor cursor = project.ClangManager.GetCursor (doc.FileName, doc.Editor.CaretLocation);
			CXCursor referredCursor = project.ClangManager.GetCursorReferenced (cursor);
			CXCursor defCursor = project.ClangManager.GetCursorDefinition (referredCursor);
			var loc = project.ClangManager.GetCursorLocation (defCursor);
			info.Enabled = info.Visible = (clang.Cursor_isNull (referredCursor) == 0 && loc.FileName != null);
		}

	}
}

