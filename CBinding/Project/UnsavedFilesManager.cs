using System;
using MonoDevelop.Ide;
using MonoDevelop.Core.Text;
using MonoDevelop.Ide.Gui;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ClangSharp;

namespace CBinding
{

	public class UnsavedFilesManager : IDisposable
	{
		Document current;
		CProject Project { get; }
		ConcurrentDictionary<string, UnsavedFile> UnsavedFileCollection { get; }

		public UnsavedFilesManager (CProject proj)
		{
			Project = proj;
			IdeApp.Workbench.ActiveDocumentChanged += HandleChange;
			IdeApp.Workbench.DocumentOpened += HandleOpen;
			UnsavedFileCollection = new ConcurrentDictionary<string, UnsavedFile> ();
		}

		void HandleOpen (object sender, DocumentEventArgs e)
		{
			e.Document.Saved += HandleSave;
			e.Document.Closed += HandleClose;
			UnsavedFileCollection.TryAdd (e.Document.Name, new UnsavedFile (false, e.Document.Editor.Text));
		}

		void HandleChange (object sender, EventArgs e)
		{
			if (current != null) {
				current.Editor.TextChanged -= HandleTextChange;
			}

			current = IdeApp.Workbench.ActiveDocument;

			if (current != null) {
				current.Editor.TextChanged += HandleTextChange;
			}
		}

		void HandleSave (object sender, EventArgs e)
		{
			var document = (Document) sender;
			UnsavedFileCollection [document.Name].IsDirty = false;
		}

		void HandleClose (object sender, EventArgs e)
		{
			var document = (Document) sender;
			document.Closed -= HandleClose;
			document.Saved -= HandleSave;
			UnsavedFile dummy = null;
			UnsavedFileCollection.TryRemove (document.Name, out dummy);
		}

		void HandleTextChange (object sender, TextChangeEventArgs e)
		{
			UnsavedFileCollection [current.Name].IsDirty = true;
			UnsavedFileCollection [current.Name].Text = 
				e.RemovalLength != 0 ?
					UnsavedFileCollection [current.Name].Text.Remove (e.Offset, e.RemovalLength)
					:
					UnsavedFileCollection [current.Name].Text.Insert (e.Offset, e.InsertedText.Text);
		}

		public List<CXUnsavedFile> Get ()
		{
			var unsavedFiles = new List<CXUnsavedFile> ();
			foreach (var unsaved in UnsavedFileCollection) {
				if (unsaved.Value.IsDirty) {
					CXUnsavedFile unsavedFile = new CXUnsavedFile ();
					unsavedFile.Initialize (unsaved.Key, unsaved.Value.Text, Project.ClangManager.IsBomPresentInFile (unsaved.Key));
					unsavedFiles.Add (unsavedFile);
				}
			}
			return unsavedFiles;
		}

		protected virtual void OnDispose(bool disposing)
		{
			if (disposing) {
				IdeApp.Workbench.ActiveDocumentChanged -= HandleChange;
				IdeApp.Workbench.DocumentOpened -= HandleOpen;
			}
		}

		~UnsavedFilesManager()
		{
			OnDispose(false);
		}

		#region IDisposable implementation

		void IDisposable.Dispose ()
		{
			OnDispose(true); 
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}

