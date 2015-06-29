//
// CMakeTarget.cs
//
// Author:
//       Elsayed Awdallah <comando4ever@gmail.com>
//
// Copyright (c) 2015 Xamarin Inc. (http://xamarin.com)
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

namespace CBinding {
	public class CMakeTarget : SolutionItem {
		public enum Types {
			BINARY,
			SHARED_LIBRARY,
			STATIC_LIBRARY,
			MODULE_LIBRARY,
			OBJECT_LIBRARY,
			UNKOWN
		};
		public Types? Type;
		public bool IsAliasOrImported = false;
		
		string name;
		List<string> ignored = new List<string> () {
			"WIN32", "MACOSX_BUNDLE", "EXCLUDE_FROM_ALL"
		};
		CMakeCommand command;
		public Dictionary<string, CMakeCommand> Files = new Dictionary<string, CMakeCommand> ();
		CMakeFileFormat parent;
		
		public CMakeTarget (CMakeCommand command, CMakeFileFormat parent)
		{
			Initialize (this);
			this.command = command;
			this.parent = parent;
			if (command.Name.ToLower () == "add_executable")
				Type = Types.BINARY;

			getFiles ();
		}
		
		/* Overrides */
		
		protected override string OnGetName()
		{
			return this.name;
		}
		
		protected override void OnNameChanged(SolutionItemRenamedEventArgs e)
		{
			this.command.EditArgument (e.OldName, e.NewName);
		}
		
		protected override IEnumerable<FilePath> OnGetItemFiles(bool includeReferencedFiles)
		{
			List<FilePath> files = new List<FilePath> ();
			string path = parent.File.ParentDirectory.ToString ();
			foreach (var file in Files) {
				files.Add (Path.Combine (path, file.Key));
			}
			return files;
		}
		
		public override FilePath FileName
		{
			get
			{
				return this.command.FileName;
			}
		}

		public List<FilePath> GetFiles () {
			return OnGetItemFiles (false).ToList ();
		}
		
		void parseValue (string value)
		{
			if (String.IsNullOrEmpty (name)) {
				name = value;
				return;
			}

			if (Type == null) {
				switch (value.ToLower ()) {
				case "shared":
					Type = Types.SHARED_LIBRARY;	
					return;
				case "static":
					Type = Types.SHARED_LIBRARY;
					return;
				case "module":
					Type = Types.MODULE_LIBRARY;
					return;
				case "unknown":
					Type = Types.UNKOWN;
					return;
				default:
					break;
				}
			}

			if (value.ToLower () == "alias" || value.ToLower () == "imported") {
				IsAliasOrImported = true;
				return;
			}

			if (ignored.Contains (value, StringComparer.OrdinalIgnoreCase))
               return;

			if (!Files.ContainsKey (new FilePath (value)))
				Files.Add (new FilePath (value), this.command);
		}
		
		void getFiles ()
		{
			List<CMakeArgument> args = this.command.Arguments;
			foreach (var arg in args) {
				if (IsAliasOrImported) return;
				foreach (string val in arg.GetValues ()) {
					if (!val.StartsWith("${")) {
						parseValue (val);
						continue;
					}

					CMakeVariableParser cmvp = new CMakeVariableParser(val, command, parent);
					Files = Files.Concat(cmvp.Values).ToDictionary(x=>x.Key, x=>x.Value);
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
			if (Files [oldName].IsEditable)
				return Files [oldName].EditArgument (oldName, newName);

			return false;
		}
		
		public void PrintTarget ()
		{
			LoggingService.LogDebug (String.Format ("[Target Name : {0}]", name));
			foreach (var file in Files)
			{
				LoggingService.LogDebug (file.ToString ());
			}
			LoggingService.LogDebug ("[End Target]");
		}
	}
}

