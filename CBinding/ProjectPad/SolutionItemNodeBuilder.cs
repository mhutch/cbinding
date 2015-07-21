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
	public class SolutionItemNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get {
				return typeof (SolutionItem);
			}
		}

		public override Type CommandHandlerType {
			get {
				return typeof (SolutionItemCommandHandler);
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
			var item = (SolutionItem)dataObject;
			return item.GetItemFiles (true).GetEnumerator ().MoveNext () ||
				item.GetChildren ().GetEnumerator ().MoveNext ();
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var item = (SolutionItem)dataObject;
			foreach (FilePath file in item.GetItemFiles (true)) {
				bool transparent = !File.Exists (file);
				var systemFile = new MonoDevelop.Ide.Gui.Pads.ProjectPad.SystemFile (file, item, transparent);
				treeBuilder.AddChild (systemFile);
			}

			foreach (WorkspaceObject obj in item.GetChildren ())
				treeBuilder.AddChild (obj);
		}

		public override object GetParentObject (object dataObject)
		{
			var item = (SolutionItem)dataObject;
			return item.ParentObject;
		}
	}

	public class SolutionItemCommandHandler : NodeCommandHandler
	{
		[CommandUpdateHandler (ProjectCommands.EditSolutionItem)]
		public void OnEditProjectUpdate (CommandInfo info)
		{
			var item = (SolutionItem)CurrentNode.DataItem;
			info.Visible = info.Enabled = !string.IsNullOrEmpty (item.FileName) && File.Exists (item.FileName);
		}

		[CommandHandler (ProjectCommands.EditSolutionItem)]
		public void OnEditProject ()
		{
			var item = (SolutionItem)CurrentNode.DataItem;
			IdeApp.Workbench.OpenDocument (item.FileName);
		}
	}
}
