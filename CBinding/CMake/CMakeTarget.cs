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
using System.IO;

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


		public Dictionary<string, CMakeCommand> Files {
			get { return files; }
		}
		Dictionary<string, CMakeCommand> files = new Dictionary<string, CMakeCommand> ();

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
			var files = new List<FilePath> ();
			FilePath path = parent.File.ParentDirectory;
			foreach (var file in Files) {
				files.Add (path.Combine (file.Key));
			}
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
					type = Types.SharedLibrary;
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

			if (!Files.ContainsKey (new FilePath (value)))
				Files.Add (new FilePath (value), command);
		}

		void PopulateFiles ()
		{
			List<CMakeArgument> args = command.Arguments;
			foreach (var arg in args) {
				if (isAliasOrImported) return;
				foreach (string val in arg.GetValues ()) {
					if (!val.StartsWith ("${", StringComparison.Ordinal)) {
						parseValue (val);
						continue;
					}

					var cmvp = new CMakeVariableParser (val, command, parent);
					files = Files.Concat (cmvp.Values).ToDictionary (x => x.Key, x => x.Value);
				}
			}
		}

		public void AddFile (string filename)
		{
			command.AddArgument (filename);
		}

		public bool RemoveFile (string filename)
		{
			if (Files [filename].IsEditable)
				return Files [filename].RemoveArgument (filename);

			return false;
		}

		public bool RenameFile (string oldName, string newName)
		{
			return Files [oldName].EditArgument (oldName, newName);
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
