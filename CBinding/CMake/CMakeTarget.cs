//
// CMakeTarget.cs
//
// Author:
//       Elsayed Awdallah <comando4ever@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding
{
	public class CMakeTarget : SolutionItem
	{
		public enum Types
		{
			Binary,
			SharedLibrary,
			StaticLibrary,
			Module,
			ObjectLibrary,
			Unknown
		};

		public Types? Type {
			get { return type; }
		}
		Types? type;

		public bool IsAliasOrImported {
			get { return isAliasOrImported; }
		}
		bool isAliasOrImported = false;


		public List<FilePath> Files {
			get { return files; }
		}
		readonly List<FilePath> files = new List<FilePath> ();

		string name;
		List<string> ignored = new List<string> () {
			"WIN32", "MACOSX_BUNDLE", "EXCLUDE_FROM_ALL"
		};
		CMakeFileFormat parent;

		public CMakeCommand Command {
			get { return command; }
		}
		CMakeCommand command;

		public CMakeTarget (CMakeCommand command, CMakeFileFormat parent)
		{
			Initialize (this);
			this.command = command;
			this.parent = parent;
			if (command.Name.ToLower () == "add_executable")
				type = Types.Binary;

			PopulateFiles ();
		}

		/* Overrides */

		public new WorkspaceObject ParentObject;

		protected override string OnGetName ()
		{
			return name;
		}

		protected override void OnNameChanged (SolutionItemRenamedEventArgs e)
		{
			command.EditArgument (e.OldName, e.NewName);
		}

		protected override IEnumerable<FilePath> OnGetItemFiles (bool includeReferencedFiles)
		{
			return files;
		}

		public override FilePath FileName {
			get {
				return command.FileName;
			}
		}

		public List<FilePath> GetFiles ()
		{
			return OnGetItemFiles (false).ToList ();
		}

		void parseValue (string value)
		{
			if (string.IsNullOrEmpty (name)) {
				name = value;
				return;
			}

			if (Type == null) {
				switch (value.ToLower ()) {
				case "shared":
					type = Types.SharedLibrary;
					return;
				case "static":
					type = Types.StaticLibrary;
					return;
				case "module":
					type = Types.Module;
					return;
				case "object":
					type = Types.ObjectLibrary;
					return;
				case "unknown":
					type = Types.Unknown;
					return;
				}
			}

			if (value.ToLower () == "alias" || value.ToLower () == "imported") {
				isAliasOrImported = true;
				return;
			}

			if (ignored.Contains (value, StringComparer.OrdinalIgnoreCase))
				return;

			var file = new FilePath (value);
			if (!file.IsAbsolute)
				file = parent.File.ParentDirectory.Combine (file);

			if (!Files.Contains (file.CanonicalPath))
				Files.Add (file.CanonicalPath);
		}

		void PopulateFiles ()
		{
			string args = command.ArgumentsString;
			args = parent.VariableManager.ResolveString (command.ArgumentsString);
			List<CMakeArgument> arguments = CMakeArgument.ArgumentsFromString (args);

			foreach (var arg in arguments) {
				foreach (var val in arg.GetValues ()) {
					parseValue (val);
				}
			}
		}

		public void AddFile (string filename)
		{
			command.AddArgument (filename);
		}

		public void RenameFiles (List<FilePath> oldnames, List<FilePath> newnames)
		{
			var toRename = new List<Tuple<string, int, CMakeCommand>> ();
			TraverseArguments (command, (a, c) => {
				FilePath argfile = new FilePath (a);
				if (!argfile.IsAbsolute) argfile = parent.File.ParentDirectory.Combine (argfile).CanonicalPath;
				int i = oldnames.IndexOf (argfile);
				if (i >= 0) {
					toRename.Add (Tuple.Create (a, i, c));
				}
			});

			foreach (var t in toRename) {
				FilePath newname = !newnames [t.Item2].IsDirectory ? newnames [t.Item2] :
													  newnames [t.Item2].Combine ("./" + oldnames [t.Item2].FileName);

				newname = newname.CanonicalPath.ToRelative (parent.File.ParentDirectory);

				LoggingService.LogDebug ("{0} -> {1}", t.Item1, newname);
				t.Item3.EditArgument (t.Item1, newname);
			}
		}

		public void RemoveFile (string filename)
		{
			FilePath file = new FilePath (filename).CanonicalPath;
			if (!file.IsAbsolute) file = parent.File.ParentDirectory.Combine (file).CanonicalPath;

			TraverseArguments (command, (a, c) => {
				FilePath argfile = new FilePath (a).CanonicalPath;
				if (!argfile.IsAbsolute) argfile = parent.File.ParentDirectory.Combine (argfile).CanonicalPath;
				if (file.Equals (argfile)) {
					LoggingService.LogDebug ("Removing {0} from {1}", argfile, c.ToString ());
					c.RemoveArgument (a);
				}
			});
		}

		public void RemoveFiles (List<FilePath> files)
		{
			var toRemove = new List<Tuple<string, CMakeCommand>> ();
			TraverseArguments (command, (a, c) => {
				FilePath argfile = new FilePath (a);
				if (!argfile.IsAbsolute) argfile = parent.File.ParentDirectory.Combine (argfile).CanonicalPath;
				if (files.Contains (argfile)) {
					toRemove.Add (Tuple.Create (a, c));
				}
			});

			foreach (var t in toRemove) {
				t.Item2.RemoveArgument (t.Item1);
			}
		}

		void TraverseArguments (CMakeCommand command, Action<string, CMakeCommand> callback, HashSet<CMakeCommand> commands = null)
		{
			CMakeVariableManager vm = parent.VariableManager;
			if (commands == null)
				commands = new HashSet<CMakeCommand> ();

			if (commands.Contains (command))
				return;

			commands.Add (command);

			foreach (var arg in command.Arguments) {
				foreach (var val in arg.GetValues ()) {
					if (vm.IsVariable (val)) {
						CMakeVariable v = vm.GetVariable (val.Substring (2, val.Length - 3));
						if (v == null) continue;

						foreach (var c in v.Commands) {
							TraverseArguments (c, callback, commands);
						}
					} else {
						callback (val, command);
					}
				}
			}
		}

		public bool Rename (string newName)
		{
			if (command.EditArgument (name, newName)) {
				name = newName;
				return true;
			}
			return false;
		}

		public void Save ()
		{
			parent.Save ();
		}

		public override string ToString ()
		{
			if (isAliasOrImported) {
				return string.Format ("{0} ({1})", name, "Alias");
			}

			return string.Format ("{0} ({1})", name, type);
		}

		public void PrintTarget ()
		{
			LoggingService.LogDebug (string.Format ("[Target Name : {0}]", name));
			foreach (var file in Files) {
				LoggingService.LogDebug (file.ToString ());
			}
			LoggingService.LogDebug ("[End Target]");
		}
	}
}
