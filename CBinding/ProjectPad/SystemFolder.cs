using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;

namespace CBinding
{
	public class SystemFolder : SystemFile
	{
		public SystemFolder (FilePath absolutePath, WorkspaceObject parent, bool showTransparent) : base (absolutePath, parent, showTransparent)
		{
		}

		public SystemFolder (FilePath absolutePath, WorkspaceObject parent) : base (absolutePath, parent, true)
		{
		}

	}
}

