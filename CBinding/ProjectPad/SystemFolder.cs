using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding
{
	public class SystemFolder : IFolderItem
	{
		readonly FilePath absolutePath;
		readonly WorkspaceObject parent;
		bool showTransparent;

		public FilePath BaseDirectory {
			get {
				return Path;
			}
		}

		public SystemFolder (FilePath absolutePath, WorkspaceObject parent, bool showTransparent)
		{
			this.absolutePath = absolutePath;
			this.parent = parent;
			this.showTransparent = showTransparent;
		}

		public SystemFolder (FilePath absolutePath, WorkspaceObject parent) : this (absolutePath, parent, true)
		{
		}


		public FilePath Path {
			get { return absolutePath; }
		}

		public string Name {
			get { return System.IO.Path.GetFileName (absolutePath); }
		}

		public WorkspaceObject ParentWorkspaceObject {
			get { return parent; }
		}

		public bool ShowTransparent {
			get { return showTransparent; }
			set { showTransparent = value; }
		}

		public override bool Equals (object obj)
		{
			var f = obj as SystemFolder;
			return f != null && absolutePath == f.absolutePath && parent == f.parent;
		}

		public override int GetHashCode ()
		{
			if (parent != null)
				return (absolutePath + parent.Name).GetHashCode ();
			else
				return absolutePath.GetHashCode ();
		}
	}
}

