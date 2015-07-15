using System;
using MonoDevelop.Ide;
using MonoDevelop.Core.Text;
using MonoDevelop.Ide.Gui;
using System.Collections.Generic;

namespace CBinding
{

	public class UnsavedFilesManager : IDisposable
	{
		Document current;
		CProject Project { get; }
		public Dictionary<string, UnsavedFile> UnsavedFileCollection { get; }

		public UnsavedFilesManager (CProject proj)
		{
			Project = proj;
			IdeApp.Workbench.ActiveDocumentChanged += HandleChange;
			IdeApp.Workbench.DocumentOpened += HandleOpen;
			UnsavedFileCollection = new Dictionary<string, UnsavedFile> ();
		}

		void HandleOpen (object sender, DocumentEventArgs e)
		{
			e.Document.Saved += HandleSave;
			e.Document.Closed += HandleClose;
			UnsavedFileCollection.Add (e.Document.Name, new UnsavedFile (false, e.Document.Editor.Text));
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
			var realSender = (Document) sender;
			UnsavedFileCollection [realSender.Name].IsDirty = false;
		}

		void HandleClose (object sender, EventArgs e)
		{
			var realSender = (Document) sender;
			realSender.Closed -= HandleClose;
			realSender.Saved -= HandleSave;
			UnsavedFileCollection.Remove (realSender.Name);
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

