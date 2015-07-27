using System;
using System.IO;

using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;

namespace CBinding.ProjectPad
{
	class SystemFolderNodeBuilder : SystemFileNodeBuilder
	{
		public SystemFolderNodeBuilder () : base ()
		{
		}

		public override Type NodeDataType {
			get {
				return typeof (SystemFolder);
			}
		}

		public override string GetNodeName (MonoDevelop.Ide.Gui.Components.ITreeNavigator thisNode, object dataObject)
		{
			return string.Empty;
		}

		public override void BuildNode (MonoDevelop.Ide.Gui.Components.ITreeBuilder treeBuilder, object dataObject, MonoDevelop.Ide.Gui.Components.NodeInfo nodeInfo)
		{
			var item = (SystemFolder)dataObject;
			nodeInfo.Label = GLib.Markup.EscapeText (item.Path.FileName);
			nodeInfo.Icon = Context.GetIcon (Stock.OpenFolder);
			nodeInfo.ClosedIcon = Context.GetIcon (Stock.ClosedFolder);
		}

		public override bool HasChildNodes (MonoDevelop.Ide.Gui.Components.ITreeBuilder builder, object dataObject)
		{
			return true;
		}

		public override void BuildChildNodes (MonoDevelop.Ide.Gui.Components.ITreeBuilder treeBuilder, object dataObject)
		{
			var folder = (SystemFolder)dataObject;
			if (!Directory.Exists (folder.Path)) return;
			SolutionItem item = (SolutionItem)treeBuilder.GetParentDataItem (typeof (SolutionItem), true);

			foreach (string path in Directory.GetFiles (folder.Path)) {
				treeBuilder.AddChild (new SystemFile (new FilePath (path), item, false));
			}

			foreach (string path in Directory.GetDirectories (folder.Path)) {
				treeBuilder.AddChild (new SystemFolder (new FilePath (path), item, false));
			}
		}
	}
}

