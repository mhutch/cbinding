using System;
using System.IO;

using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Projects;

namespace CBinding.ProjectPad
{
	public class FolderBasedProjectNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get {
				return typeof (FolderBasedProject);
			}
		}

		public override Type CommandHandlerType {
			get {
				return typeof (FolderBasedProjectCommandHandler);
			}
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			var item = (SolutionItem)dataObject;
			return item.Name;
		}

		public override void GetNodeAttributes (ITreeNavigator parentNode, object dataObject, ref NodeAttributes attributes)
		{
			attributes |= NodeAttributes.AllowRename;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var item = (SolutionItem)dataObject;
			nodeInfo.Label = GLib.Markup.EscapeText (item.Name);
			nodeInfo.Icon = Context.GetIcon (Stock.Project);
			nodeInfo.ClosedIcon = Context.GetIcon (Stock.Project);
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var item = (FolderBasedProject)dataObject;
			treeBuilder.AddChild (new SystemFolder (item.BaseDirectory, item, false));
		}

		public override object GetParentObject (object dataObject)
		{
			var item = (FolderBasedProject)dataObject;
			return item.ParentObject;
		}
	}

	public class FolderBasedProjectCommandHandler : NodeCommandHandler
	{
		public override void ActivateItem ()
		{
			CurrentNode.Expanded = !CurrentNode.Expanded;
		}

		[CommandUpdateHandler (ProjectCommands.EditSolutionItem)]
		public void OnEditProjectUpdate (CommandInfo info)
		{
			var item = (FolderBasedProject)CurrentNode.DataItem;
			info.Visible = info.Enabled = !string.IsNullOrEmpty (item.FileName) && File.Exists (item.FileName);
		}

		[CommandHandler (ProjectCommands.EditSolutionItem)]
		public void OnEditProject ()
		{
			var item = (FolderBasedProject)CurrentNode.DataItem;
			IdeApp.Workbench.OpenDocument (item.FileName);
		}
	}
}
