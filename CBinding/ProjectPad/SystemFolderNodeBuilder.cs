using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Gtk;

using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide.Projects;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Ide;

namespace CBinding.ProjectPad
{
	class SystemFolderNodeBuilder : FolderNodeBuilder
	{
		public override Type NodeDataType {
			get {
				return typeof (SystemFolder);
			}
		}

		public override Type CommandHandlerType {
			get {
				return typeof (SystemFolderCommandHandler);
			}
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return string.Empty;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var item = (SystemFolder)dataObject;
			nodeInfo.Label = GLib.Markup.EscapeText (item.Path.FileName);
			nodeInfo.Icon = Context.GetIcon (MonoDevelop.Ide.Gui.Stock.OpenFolder);
			nodeInfo.ClosedIcon = Context.GetIcon (MonoDevelop.Ide.Gui.Stock.ClosedFolder);
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			var folder = (SystemFolder)dataObject;
			return Directory.GetFiles (folder.Path).Length > 0 || Directory.GetDirectories (folder.Path).Length > 0;
		}

		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			var folder = (SystemFolder)dataObject;
			if (!Directory.Exists (folder.Path)) return;
			var item = (SolutionItem)builder.GetParentDataItem (typeof (SolutionItem), true);

			foreach (string path in Directory.GetFiles (folder.Path)) {
				builder.AddChild (new SystemFile (new FilePath (path), item, false));
			}

			foreach (string path in Directory.GetDirectories (folder.Path)) {
				builder.AddChild (new SystemFolder (new FilePath (path), item, false));
			}
		}

		public override object GetParentObject (object dataObject)
		{
			var folder = (SystemFolder)dataObject;
			return new SystemFolder (folder.Path.ParentDirectory, folder.ParentWorkspaceObject, false);
		}

		public override string GetFolderPath (object dataObject)
		{
			return ((SystemFolder)dataObject).Path;
		}
	}

	class SystemFolderCommandHandler : NodeCommandHandler
	{
		class TempProject : Project
		{
			FolderBasedProject project;

			public TempProject (FolderBasedProject project)
			{
				Initialize (this);
				FileName = Path.GetTempPath ();
				this.project = project;
			}

			protected override string [] OnGetSupportedLanguages ()
			{
				return project.OnGetSupportedLanguages ();
			}

			protected override void OnGetTypeTags (HashSet<string> types)
			{
				types.Add (""); //Throws an exception otherwise.
			}
		}

		static FilePath PreviousFolderPath {
			get; set;
		}

		public string GetPath (object dataObject)
		{
			if (dataObject is SystemFolder) {
				return ((SystemFolder)dataObject).Path;
			}
			return ((SystemFile)dataObject).Path;
		}

		void CopyFile (FilePath sourcePath, FilePath targetPath, bool move = false)
		{
			if (targetPath.IsDirectory)
				targetPath += Path.DirectorySeparatorChar.ToString () + sourcePath.FileName;

			if (targetPath.Equals (sourcePath)) {
				MessageService.ShowWarning (GettextCatalog.GetString (
					"There is already a file with the name '{0}' in the target directory.", targetPath.FileName));
				return;
			}

			if (!Directory.Exists (targetPath.ParentDirectory)) {
				FileService.CreateDirectory (targetPath.ParentDirectory);
			}

			if (File.Exists (targetPath)) {
				if (MessageService.Confirm (GettextCatalog.GetString ("The file '{0}' already exists. Do you want to overwrite it?", targetPath.FileName), AlertButton.OverwriteFile)) {
					FileService.DeleteFile (targetPath);
				} else {
					return;
				}
			}

			if (move) {
				using (var monitor = IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor ("Moving file...", MonoDevelop.Ide.Gui.Stock.StatusSolutionOperation, false))
					FileService.MoveFile (sourcePath, targetPath);
			} else {
				using (var monitor = IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor ("Copying file...", MonoDevelop.Ide.Gui.Stock.StatusSolutionOperation, false))
					FileService.CopyFile (sourcePath, targetPath);
			}
		}

		void CopyDirectory (FilePath sourcePath, FilePath targetPath, bool move = false)
		{
			targetPath += Path.DirectorySeparatorChar + sourcePath.FileName + Path.DirectorySeparatorChar;
			sourcePath += Path.DirectorySeparatorChar;

			if (targetPath.Equals (sourcePath)) {
				MessageService.ShowWarning (GettextCatalog.GetString (
					"There is already a directory with the name '{0}' in the target directory.", targetPath.FileName));
				return;
			}

			if (Directory.Exists (targetPath)) {
				if (MessageService.Confirm (string.Format ("The directory '{0}' already exists. Do you want to delete it?\nThis will remove all its contents as well.", targetPath.FileName), AlertButton.Delete)) {
					//TODO: Iterate over all files and overwrite them individually.
					//var foundFiles = Directory.GetFiles (sourcePath, "*", SearchOption.AllDirectories);
					//pass files relative to sourcePath combine it with targetPath
					FileService.DeleteDirectory (targetPath);
				} else {
					return;
				}
			}

			if (move) {
				using (var monitor = IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor ("Moving directory...", MonoDevelop.Ide.Gui.Stock.StatusSolutionOperation, false))
					FileService.MoveDirectory (sourcePath, targetPath);
			} else {
				using (var monitor = IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor ("Copying directory...", MonoDevelop.Ide.Gui.Stock.StatusSolutionOperation, false))
					FileService.CopyDirectory (sourcePath, targetPath);
			}
		}

		public override bool CanDropNode (object dataObject, DragOperation operation, DropPosition position)
		{
			string targetDirectory = GetPath (CurrentNode.DataItem);

			var systemFile = dataObject as SystemFile;
			if (systemFile != null) {
				switch (operation) {
				case DragOperation.Move:
					return targetDirectory != systemFile.Path.ParentDirectory;
				case DragOperation.Copy:
					return true;
				default:
					return false;
				}
			}

			if (dataObject is SystemFolder) {
				return ((SystemFolder)dataObject).Path != targetDirectory || operation == DragOperation.Copy;
			}

			var selectionData = dataObject as SelectionData;
			if (selectionData != null) {
				var data = selectionData;
				if (data.Type == "text/uri-list")
					return true;
			}

			return false;
		}

		public override void OnMultipleNodeDrop (object [] dataObjects, DragOperation operation, DropPosition position)
		{
			OnMultipleNodeDrop (dataObjects, operation);
		}

		public override void OnNodeDrop (object dataObjects, DragOperation operation, DropPosition position)
		{
			OnNodeDrop (dataObjects, operation);
		}

		public override void OnMultipleNodeDrop (object [] dataObjects, DragOperation operation)
		{
			var project = CurrentNode.GetParentDataItem (typeof (FolderBasedProject), true) as FolderBasedProject;
			var srcFiles = new List<FilePath> ();
			var dstFiles = new List<FilePath> ();

			foreach (object obj in dataObjects) {
				var builder = Tree.BuilderContext.GetTreeBuilder (obj);
				builder.MoveToParent ();
				Tuple<FilePath, FilePath> t = NodeDrop (obj, operation);
				srcFiles.Add (t.Item1);
				dstFiles.Add (t.Item2);
				builder.UpdateChildren ();
				Tree.BuilderContext.GetTreeBuilder (CurrentNode).UpdateChildren ();
			}

			if (operation == DragOperation.Move) {
				project.OnFilesMoved (srcFiles, dstFiles);
			} else if (operation == DragOperation.Copy) {
				project.OnFilesCopied (srcFiles, dstFiles);
			}
		}

		public override void OnNodeDrop (object dataObjects, DragOperation operation)
		{
			var project = CurrentNode.GetParentDataItem (typeof (FolderBasedProject), true) as FolderBasedProject;
			var builder = Tree.BuilderContext.GetTreeBuilder (dataObjects);
			builder.MoveToParent ();
			Tuple<FilePath, FilePath> t = NodeDrop (dataObjects, operation);
			Tree.BuilderContext.GetTreeBuilder (CurrentNode).UpdateChildren ();
			builder.UpdateChildren ();

			if (t != null) {
				if (operation == DragOperation.Move) {
					project.OnFileMoved (t.Item1, t.Item2);
				} else {
					project.OnFileCopied (t.Item1, t.Item2);
				}
			}
		}

		Tuple<FilePath, FilePath> NodeDrop (object dataObjects, DragOperation operation)
		{
			FilePath sourcePath = GetPath (dataObjects);
			FilePath targetPath = GetPath (CurrentNode.DataItem);

			var filesToSave = new List<Document> ();
			foreach (Document doc in IdeApp.Workbench.Documents) {
				if (doc.IsDirty && doc.IsFile) {
					if (doc.Name == sourcePath || doc.Name.StartsWith (sourcePath + Path.DirectorySeparatorChar, StringComparison.Ordinal)) {
						filesToSave.Add (doc);
					}
				}
			}

			if (filesToSave.Count > 0) {
				var sb = new StringBuilder ();
				foreach (Document doc in filesToSave) {
					if (sb.Length > 0) sb.Append (",\n");
					sb.Append (Path.GetFileName (doc.Name));
				}

				string question;
				question = operation == DragOperation.Move ?
													 GettextCatalog.GetString ("Do you want to save the file '{0}' before the move operation?", sb.ToString ()) :
													 GettextCatalog.GetString ("Do you want to save the file '{0}' before the copy operation?", sb.ToString ());

				var noSave = new AlertButton (GettextCatalog.GetString ("Don't Save"));
				AlertButton res = MessageService.AskQuestion (question, AlertButton.Cancel, noSave, AlertButton.Save);
				if (res == AlertButton.Cancel)
					return null;
				if (res == AlertButton.Save) {
					try {
						foreach (Document doc in filesToSave) {
							doc.Save ();
						}
					} catch (Exception ex) {
						MessageService.ShowError (GettextCatalog.GetString ("Save operation failed."), ex);
						return null;
					}
				}
			}

			if (dataObjects is SystemFile) {
				CopyFile (sourcePath, targetPath, operation == DragOperation.Move);
				return Tuple.Create (sourcePath, targetPath);
			} else if (dataObjects is SystemFolder) {
				CopyDirectory (sourcePath, targetPath, operation == DragOperation.Move);
				return Tuple.Create (sourcePath, targetPath);
			} else if (dataObjects is SelectionData) {
				SelectionData data = (SelectionData)dataObjects;
				if (data.Type != "text/uri-list")
					return null;
				string sources = Encoding.UTF8.GetString (data.Data);
				Console.WriteLine ("text/uri-list:\n{0}", sources);
				string [] files = sources.Split (new string [] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
				for (int n = 0; n < files.Length; n++) {
					var uri = new Uri (files [n]);
					if (uri.Scheme != "file")
						return null;
					if (Directory.Exists (uri.LocalPath))
						return null;
					CopyFile (uri.LocalPath, targetPath, operation == DragOperation.Move);
				}
			}
			return null;
		}

		[CommandUpdateHandler (ProjectCommands.AddFiles)]
		[CommandUpdateHandler (ProjectCommands.AddNewFiles)]
		[CommandUpdateHandler (ProjectCommands.AddFilesFromFolder)]
		[CommandUpdateHandler (ProjectCommands.AddExistingFolder)]
		[CommandUpdateHandler (ProjectCommands.NewFolder)]
		public void UpdateHandlers (CommandInfo info)
		{
			info.Enabled = Directory.Exists (((SystemFolder)CurrentNode.DataItem).Path);
		}

		[CommandHandler (ProjectCommands.AddFiles)]
		public void AddFilesToProject ()
		{
			var project = (FolderBasedProject)CurrentNode.GetParentDataItem (typeof (FolderBasedProject), true);
			var targetRoot = ((FilePath)GetPath (CurrentNode.DataItem)).CanonicalPath;

			var fdiag = new AddFileDialog (GettextCatalog.GetString ("Add files"));
			fdiag.CurrentFolder = !PreviousFolderPath.IsNullOrEmpty ? PreviousFolderPath : targetRoot;
			fdiag.SelectMultiple = true;
			fdiag.TransientFor = IdeApp.Workbench.RootWindow;
			fdiag.BuildActions = project.GetBuildActions ();

			string overrideAction = null;

			if (!fdiag.Run ())
				return;
			PreviousFolderPath = fdiag.SelectedFiles.Select (f => f.FullPath.ParentDirectory).FirstOrDefault ();

			var files = fdiag.SelectedFiles;
			overrideAction = fdiag.OverrideAction;

			var folder = CurrentNode.GetParentDataItem (typeof (SystemFolder), true) as SystemFolder;
			FilePath baseDirectory = folder != null ? folder.Path : project.BaseDirectory;

			foreach (var file in files) {
				CopyFile (file, baseDirectory);
			}

			project.OnFilesAdded (files.ToList ());
			Tree.BuilderContext.GetTreeBuilder (CurrentNode).UpdateChildren ();
		}

		[CommandHandler (ProjectCommands.AddNewFiles)]
		public void AddNewFiles ()
		{
			var project = (FolderBasedProject)CurrentNode.GetParentDataItem (typeof (FolderBasedProject), true);

			var p = new TempProject (project);
			IdeApp.ProjectOperations.CreateProjectFile (p, ((SystemFolder)CurrentNode.DataItem).Path);

			if (p.Files.FirstOrDefault () != null)
				project.OnFileAdded (p.Files.FirstOrDefault ().FilePath);
		}

		[CommandHandler (ProjectCommands.AddFilesFromFolder)]
		public void AddFilesFromFolder ()
		{
			var project = (FolderBasedProject)CurrentNode.GetParentDataItem (typeof (FolderBasedProject), true);
			var targetRoot = ((FilePath)GetPath (CurrentNode.DataItem)).CanonicalPath;

			var ofdlg = new SelectFolderDialog (GettextCatalog.GetString ("Import From Folder"))
			{
				CurrentFolder = !PreviousFolderPath.IsNullOrEmpty ? PreviousFolderPath : targetRoot
			};
			if (!ofdlg.Run ())
				return;
			PreviousFolderPath = ofdlg.SelectedFile.CanonicalPath;
			if (!PreviousFolderPath.ParentDirectory.IsNullOrEmpty)
				PreviousFolderPath = PreviousFolderPath.ParentDirectory;

			var srcRoot = ofdlg.SelectedFile.CanonicalPath;
			var foundFiles = Directory.GetFiles (srcRoot, "*", SearchOption.AllDirectories);

			if (foundFiles.Length == 0) {
				MessageService.GenericAlert (MonoDevelop.Ide.Gui.Stock.Information,
											 GettextCatalog.GetString ("Empty directory."),
											 GettextCatalog.GetString ("Directory {0} is empty, no files have been added.", srcRoot.FileName),
											 AlertButton.Close);
				return;
			}

			using (var impdlg = new IncludeNewFilesDialog (GettextCatalog.GetString ("Select files to add from {0}", srcRoot.FileName), srcRoot)) {
				impdlg.AddFiles (foundFiles);
				if (MessageService.ShowCustomDialog (impdlg) != (int)ResponseType.Ok)
					return;

				var srcFiles = impdlg.SelectedFiles;
				var targetFiles = srcFiles.Select (f => targetRoot.Combine (f.ToRelative (srcRoot)));

				foreach (var file in srcFiles.ToArray ()) {
					CopyFile (file, targetRoot);
				}
				project.OnFilesAdded (targetFiles.ToList ());
			}

			Tree.BuilderContext.GetTreeBuilder (CurrentNode).UpdateChildren ();
		}

		[CommandHandler (ProjectCommands.AddExistingFolder)]
		public void AddExistingFolder ()
		{
			var project = (FolderBasedProject)CurrentNode.GetParentDataItem (typeof (FolderBasedProject), true);
			var selectedFolder = ((FilePath)GetPath (CurrentNode.DataItem)).CanonicalPath;

			var ofdlg = new SelectFolderDialog (GettextCatalog.GetString ("Add Existing Folder"))
			{
				CurrentFolder = !PreviousFolderPath.IsNullOrEmpty ? PreviousFolderPath : selectedFolder
			};
			if (!ofdlg.Run ())
				return;

			// We store the parent directory of the folder the user chooses as they will not need to add the same
			// directory twice. We can save them navigating up one directory by doing it for them
			PreviousFolderPath = ofdlg.SelectedFile.CanonicalPath;
			if (!PreviousFolderPath.ParentDirectory.IsNullOrEmpty)
				PreviousFolderPath = PreviousFolderPath.ParentDirectory;

			var srcRoot = ofdlg.SelectedFile.CanonicalPath;
			var targetRoot = selectedFolder.Combine (srcRoot.FileName);

			if (File.Exists (targetRoot)) {
				MessageService.ShowWarning (GettextCatalog.GetString (
					"There is already a file with the name '{0}' in the target directory", srcRoot.FileName));
				return;
			}

			var foundFiles = Directory.GetFiles (srcRoot, "*", SearchOption.AllDirectories);

			using (var impdlg = new IncludeNewFilesDialog (GettextCatalog.GetString ("Select files to add from {0}", srcRoot.FileName), srcRoot.ParentDirectory)) {
				impdlg.AddFiles (foundFiles);
				if (MessageService.ShowCustomDialog (impdlg) == (int)ResponseType.Ok) {
					var srcFiles = impdlg.SelectedFiles;
					var targetFiles = srcFiles.Select (f => targetRoot.Combine (f.ToRelative (srcRoot)));
					foreach (var srcFile in srcFiles) {
						foreach (var targetFile in targetFiles) {
							if (srcFile.ToRelative (srcRoot).Equals (targetFile.ToRelative (targetRoot)))
								CopyFile (srcFile, targetFile);
						}
					}
					project.OnFilesAdded (targetFiles.ToList ());
				}
			}
			Tree.BuilderContext.GetTreeBuilder (CurrentNode).UpdateChildren ();
		}

		[CommandHandler (ProjectCommands.NewFolder)]
		public void AddNewFolder ()
		{
			var project = CurrentNode.GetParentDataItem (typeof (SolutionItem), true) as SolutionItem;

			string baseFolderPath = GetPath (CurrentNode.DataItem);
			string directoryName = Path.Combine (baseFolderPath, GettextCatalog.GetString ("New Folder"));
			int index = -1;

			if (Directory.Exists (directoryName)) {
				while (Directory.Exists (directoryName + (++index + 1))) {
				}
			}

			if (index >= 0) {
				directoryName += index + 1;
			}

			Directory.CreateDirectory (directoryName);

			CurrentNode.Expanded = true;
			Tree.BuilderContext.GetTreeBuilder (CurrentNode).UpdateChildren ();

			//FIXME: use `Tree.StartLabelEdit ()`
		}

		public override void RenameItem (string newName)
		{
			var file = CurrentNode.DataItem as SystemFolder;

			string oldname = file.Path;
			string newname = Path.Combine (Path.GetDirectoryName (oldname), newName);

			if (newname != oldname) {
				try {
					if (!FileService.IsValidPath (newname)) {
						MessageService.ShowWarning (GettextCatalog.GetString ("The name you have chosen contains illegal characters. Please choose a different name."));
					} else if (File.Exists (newname) || Directory.Exists (newname)) {
						MessageService.ShowWarning (GettextCatalog.GetString ("File or directory name is already in use. Please choose a different one."));
					} else {
						var oldFiles = Array.ConvertAll (
							Directory.GetFiles (oldname, "*", SearchOption.AllDirectories), (input) => (FilePath)input).ToList ();
						FileService.RenameDirectory (oldname, newname);
						var newFiles = Array.ConvertAll (
							Directory.GetFiles (newname, "*", SearchOption.AllDirectories), (input) => (FilePath)input).ToList ();

						var tb = Tree.BuilderContext.GetTreeBuilder (CurrentNode);
						tb.MoveToParent ();
						tb.UpdateChildren ();

						var project = CurrentNode.GetParentDataItem (typeof (FolderBasedProject), true) as FolderBasedProject;
						if (project == null) return;
						project.OnFilesRenamed (oldFiles, newFiles);
					}
				} catch (ArgumentException) { // new file name with wildcard (*, ?) characters in it
					MessageService.ShowWarning (GettextCatalog.GetString ("The name you have chosen contains illegal characters. Please choose a different name."));
				} catch (IOException ex) {
					MessageService.ShowError (GettextCatalog.GetString ("There was an error renaming the file."), ex);
				}
			}
		}

		public override void ActivateItem ()
		{
			CurrentNode.Expanded = !CurrentNode.Expanded;
		}

		public override void DeleteMultipleItems ()
		{
			var project = CurrentNode.GetParentDataItem (typeof (FolderBasedProject), true) as FolderBasedProject;

			if (CurrentNodes.Length == 1) {
				var file = (SystemFolder)CurrentNodes [0].DataItem;
				if (!MessageService.Confirm (GettextCatalog.GetString ("Are you sure you want to permanently delete the directory {0}?", file.Path), AlertButton.Delete))
					return;
			} else {
				if (!MessageService.Confirm (GettextCatalog.GetString ("Are you sure you want to permanently delete all selected directories?"), AlertButton.Delete))
					return;
			}

			var foundFiles = new List<FilePath> ();

			foreach (SystemFolder folder in CurrentNodes.Select (n => (SystemFolder)n.DataItem)) {
				try {
					foundFiles = foundFiles.Concat (Array.ConvertAll (
						Directory.GetFiles (folder.Path, "*", SearchOption.AllDirectories), (input) => (FilePath)input)).ToList ();
					FileService.DeleteDirectory (folder.Path);
				} catch (Exception ex) {
					MessageService.ShowError (GettextCatalog.GetString ("The directory {0} could not be deleted", folder.Path), ex);
				}
			}

			var tb = Tree.BuilderContext.GetTreeBuilder (CurrentNode);
			tb.MoveToParent ();
			tb.UpdateChildren ();

			if (project == null) return;
			project.OnFilesRemoved (foundFiles);
		}

		public override bool CanDeleteItem ()
		{
			var project = CurrentNode.GetParentDataItem (typeof (SolutionItem), true) as SolutionItem;
			var folder = (SystemFolder)CurrentNode.DataItem;

			if (project != null)
				return folder.Path != project.BaseDirectory;

			return true;
		}
	}
}
