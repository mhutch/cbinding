using System;
using System.IO;

using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Projects.Text;

namespace CBinding.ProjectPad
{
	public class CMakeTargetNodeBuilder : FolderBasedProjectNodeBuilder
	{
		public override Type NodeDataType {
			get {
				return typeof (CMakeTarget);
			}
		}

		public override Type CommandHandlerType {
			get {
				return typeof (CMakeTargetCommandHandler);
			}
		}
	}

	public class CMakeTargetCommandHandler : NodeCommandHandler
	{
		[CommandUpdateHandler (ProjectCommands.Build)]
		[CommandUpdateHandler (ProjectCommands.Rebuild)]
		[CommandUpdateHandler (ProjectCommands.Clean)]
		[CommandUpdateHandler (ProjectCommands.Reload)]
		[CommandUpdateHandler (ProjectCommands.Unload)]
		[CommandUpdateHandler (ProjectCommands.RunEntryWithList)]
		void updateDisabledCommands (CommandInfo info)
		{
			info.Enabled = info.Visible = false;
		}


		[CommandUpdateHandler (ProjectCommands.EditSolutionItem)]
		public void OnEditProjectUpdate (CommandInfo info)
		{
			var item = (CMakeTarget)CurrentNode.DataItem;
			info.Visible = info.Enabled = !string.IsNullOrEmpty (item.Command.FileName) && File.Exists (item.Command.FileName);
		}

		[CommandHandler (ProjectCommands.EditSolutionItem)]
		public void OnEditProject ()
		{
			var item = (CMakeTarget)CurrentNode.DataItem;
			TextFile file = new TextFile (item.Command.FileName);
			int line = 0, column = 0;
			file.GetLineColumnFromPosition (item.Command.Offset, out line, out column);
			IdeApp.Workbench.OpenDocument (item.Command.FileName, line, column, MonoDevelop.Ide.Gui.OpenDocumentOptions.Default);
		}

		public override void RenameItem (string newName)
		{
			CMakeTarget t = CurrentNode.DataItem as CMakeTarget;
			t.Rename (newName);
			t.Save ();
		}

		public override void DeleteItem ()
		{
			var t = CurrentNode.DataItem as CMakeTarget;
			var p = CurrentNode.GetParentDataItem (typeof (CMakeProject), false) as CMakeProject;
			p.RemoveTarget (t.Name);
			t.Save ();
			base.DeleteItem ();
		}
	}
}
