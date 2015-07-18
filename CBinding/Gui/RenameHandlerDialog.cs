using System;
using MonoDevelop.Ide.Gui;
using ClangSharp;
using System.Collections.Generic;
using CBinding.Refactoring;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace CBinding
{
	public partial class RenameHandlerDialog : Gtk.Dialog
	{
		protected CProject project;
		protected CXCursor cursorReferenced;
		protected string UsrOfReferenced;
		protected string spelling;
		protected string newSpelling;
		protected Document document;

		public RenameHandlerDialog (CProject proj, Document doc)
		{
			project = proj;
			cursorReferenced = project.ClangManager.GetCursorReferenced(
				project.ClangManager.GetCursor (
					doc.FileName,
					doc.Editor.CaretLocation
				)
			);
			UsrOfReferenced = project.ClangManager.GetCursorUsrString (cursorReferenced);
			spelling = project.ClangManager.GetCursorSpelling(cursorReferenced);
			document = doc;
		}

		/// <summary>
		/// Invoked when OK button is clicked. Runs the rename
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			newSpelling = renameEntry.Text;
			Regex identifier = new Regex ("^[a-zA-Z_][a-zA-Z0-9_]*$");
			if(identifier.IsMatch (newSpelling)) {
				FindRefsAndRename (project, cursorReferenced);
				this.Destroy ();
				return;
			}
			if(unsafeCheckBox.Active) {
				FindRefsAndRename (project, cursorReferenced);
				this.Destroy ();
				return;
			}
			unsafeLabel.Show ();
		}

		List<Reference> references = new List<Reference>();

		public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data){
			CXCursor referenced = project.ClangManager.GetCursorReferenced (cursor);
			string USR = project.ClangManager.GetCursorUsrString (referenced);
			if (UsrOfReferenced.Equals (USR)) {
				CXSourceRange range = clang.Cursor_getSpellingNameRange (cursor, 0, 0);
				Reference reference = new Reference (project, cursor, range);
				var file = project.Files.GetFile (reference.FileName);
				if (file != null) {
					Document doc = IdeApp.Workbench.OpenDocument (reference.FileName, project, false);
					if (!references.Contains (reference)
						//this check is needed because explicit namespace qualifiers, eg: "std" from std::toupper
						//are also found when finding eg:toupper references, but has the same cursorkind as eg:"toupper"
						&& doc.Editor.GetTextAt (reference.Begin.Offset, reference.Length).Equals (cursor.ToString ())) {
						references.Add (reference);
					}			
				}
			}
			return CXChildVisitResult.Recurse;
		}

		/// <summary>
		/// Finds references and renames them.
		/// </summary>
		/// <param name="project">Project.</param>
		/// <param name="cursor">Cursor.</param>
		public void FindRefsAndRename (CProject project, CXCursor cursor)
		{
			try {
				project.ClangManager.FindReferences (this);
				references.Sort ();
				int diff = newSpelling.Length - spelling.Length;
				Dictionary<string, int> offsets = new Dictionary<string, int>();
				Dictionary<string, StringBuilder> tmp = new Dictionary<string, StringBuilder>();
				foreach (var reference in references) {
					try {
						var doc = IdeApp.Workbench.OpenDocument (reference.FileName, project, false);
						if(!offsets.ContainsKey (reference.FileName)) {
							offsets.Add (reference.FileName, 0);
							tmp.Add(reference.FileName, new StringBuilder(doc.Editor.Text));
						}
						int i = offsets[reference.FileName];
						tmp[reference.FileName].Remove (reference.Offset + i*diff, reference.Length);
						tmp[reference.FileName].Insert (reference.Offset + i*diff, newSpelling);
						offsets[reference.FileName] = ++i;
					} catch (Exception){
					}
				}
				foreach(var content in tmp)
					IdeApp.Workbench.GetDocument (content.Key).Editor.ReplaceText (
						new TextSegment (
							0, 
							IdeApp.Workbench.GetDocument (content.Key).Editor.Text.Length
						),
						content.Value.ToString ()
					);
			} catch (Exception ex) {
				LoggingService.LogError ("Error renaming references", ex);
			} 
		}

		/// <summary>
		/// Update the specified info.
		/// </summary>
		/// <param name="info">Info.</param>
		public void Update (CommandInfo info)
		{
			if (clang.Cursor_isNull (cursorReferenced) == 0) {
				info.Enabled = info.Visible = IsReferenceOrDeclaration (cursorReferenced);
				info.Bypass = !info.Visible;
			}
		}

		/// <summary>
		/// Initialize rename widget
		/// </summary>
		public void RunRename ()
		{
			Build ();
		}

		/// <summary>
		/// Determines whether the specified cursor is a declaration or reference by its kind.
		/// </summary>
		/// <returns><c>true</c> if cursor is reference or declaration; otherwise, <c>false</c>.</returns>
		/// <param name="cursor">Cursor.</param>
		bool IsReferenceOrDeclaration (CXCursor cursor)
		{
			return clang.isReference (cursor.kind) != 0|| clang.isDeclaration (cursor.kind) != 0;
		}

		/// <summary>
		/// Invoked on clicking Cancel
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			Destroy ();
		}
	}
}