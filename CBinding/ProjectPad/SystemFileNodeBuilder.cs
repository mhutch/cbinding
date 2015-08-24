using System;
using System.Collections.Generic;
using System.IO;

using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace CBinding
{
	public class SystemFile : MonoDevelop.Ide.Gui.Pads.ProjectPad.SystemFile
	{
		public SystemFile (FilePath absolutePath, WorkspaceObject parent) : base (absolutePath, parent)
		{
		}

		public SystemFile (FilePath absolutePath, WorkspaceObject parent, bool showTransparent) : base (absolutePath, parent, showTransparent)
		{
		}
	}
}

namespace CBinding.ProjectPad
{
	class SystemFileNodeBuilder : MonoDevelop.Ide.Gui.Pads.ProjectPad.SystemFileNodeBuilder
	{
		public override Type CommandHandlerType {
			get {
				return typeof (SystemFileNodeCommandHandler);
			}
		}

		public override Type NodeDataType {
			get {
				return typeof (SystemFile);
			}
		}
	}

	class SystemFileNodeCommandHandler : MonoDevelop.Ide.Gui.Pads.ProjectPad.SystemFileNodeCommandHandler
	{
		public override void RenameItem (string newName)
		{
			var file = CurrentNode.DataItem as SystemFile;
			string oldname = file.Path;

			string newname = Path.Combine (Path.GetDirectoryName (oldname), newName);
			if (newname != oldname) {
				try {
					if (!FileService.IsValidPath (newname)) {
						MessageService.ShowWarning (GettextCatalog.GetString ("The name you have chosen contains illegal characters. Please choose a different name."));
					} else if (File.Exists (newname) || Directory.Exists (newname)) {
						MessageService.ShowWarning (GettextCatalog.GetString ("File or directory name is already in use. Please choose a different one."));
					} else {
						FileService.RenameFile (oldname, newname);
					}
				} catch (ArgumentException) { // new file name with wildcard (*, ?) characters in it
					MessageService.ShowWarning (GettextCatalog.GetString ("The name you have chosen contains illegal characters. Please choose a different name."));
				} catch (IOException ex) {
					MessageService.ShowError (GettextCatalog.GetString ("There was an error renaming the file."), ex);
				}
			}

			var project = CurrentNode.GetParentDataItem (typeof (FolderBasedProject), true) as FolderBasedProject;
			if (project == null) return;

			project.OnFileRenamed (oldname, newname);

			var tb = Tree.BuilderContext.GetTreeBuilder (CurrentNode);
			tb.MoveToParent ();
			tb.UpdateChildren ();
		}

		public override void DeleteMultipleItems ()
		{
			var files = new List<FilePath> ();

			if (CurrentNodes.Length == 1) {
				var file = (SystemFile)CurrentNodes [0].DataItem;
				if (!MessageService.Confirm (GettextCatalog.GetString ("Are you sure you want to permanently delete the file {0}?", file.Path), AlertButton.Delete))
					return;
			} else {
				if (!MessageService.Confirm (GettextCatalog.GetString ("Are you sure you want to permanently delete all selected files?"), AlertButton.Delete))
					return;
			}
			foreach (var node in CurrentNodes) {
				var file = (SystemFile)node.DataItem;
				try {
					FileService.DeleteFile (file.Path);
					Tree.BuilderContext.GetTreeBuilder (node).Remove ();
					files.Add (file.Path);
				} catch {
					MessageService.ShowError (GettextCatalog.GetString ("The file {0} could not be deleted", file.Path));
				}
			}

			var project = CurrentNode.GetParentDataItem (typeof (FolderBasedProject), true) as FolderBasedProject;
			if (project == null) return;
			project.OnFilesRemoved (files);
		}
	}
}

