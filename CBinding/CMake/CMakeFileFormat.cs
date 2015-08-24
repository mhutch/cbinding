//
// CMakeFileFormat.cs
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
using System.Text.RegularExpressions;

using MonoDevelop.Core;
using MonoDevelop.Projects.Text;

namespace CBinding
{
	public class CMakeFileFormat
	{
		//Comes from `project (project_name)` cmake command.
		public string ProjectName {
			get { return projectName; }
		}
		string projectName = "";

		//File path of the current CMakeLists.txt
		public FilePath File {
			get { return file; }
		}
		readonly FilePath file;

		public CMakeProject Project {
			get { return project; }
		}
		readonly CMakeProject project;

		public CMakeFileFormat Parent {
			get { return parent; }
		}
		readonly CMakeFileFormat parent;

		public Dictionary<string, CMakeCommand> SetCommands {
			get { return setCommands; }
		}
		Dictionary<string, CMakeCommand> setCommands = new Dictionary<string, CMakeCommand> ();

		public Dictionary<string, CMakeTarget> Targets {
			get { return targets; }
		}
		Dictionary<string, CMakeTarget> targets = new Dictionary<string, CMakeTarget> ();

		public Dictionary<FilePath, CMakeFileFormat> Children {
			get { return children; }
		}
		Dictionary<FilePath, CMakeFileFormat> children = new Dictionary<FilePath, CMakeFileFormat> ();

		Dictionary<string, CMakeCommand> allCommands = new Dictionary<string, CMakeCommand> ();
		Stack<CMakeCommand> blocks = new Stack<CMakeCommand> ();
		TextFile contentFile;

		//[A-Za-z_][A-Za-z0-9_]*	: ID
		//\[=[0-9]*\[.*?\]=[0-9]*\]	: bracket argument.
		static readonly string commandPattern = @"[A-Za-z_][A-Za-z0-9_]*\s*\((\"".*?\""|\[=[0-9]*\[.*?\]=[0-9]*\]|\(.*?\)|.)*?\)";
		static readonly string blockCommentPattern = @"#\[\[.*?\]\]";
		static readonly string lineCommentPattern = @"#.*?\n";
		static readonly string filePartPattern = string.Format ("{0}|{1}|{2}", commandPattern,
																lineCommentPattern, blockCommentPattern);

		static readonly Regex command = new Regex (commandPattern, RegexOptions.Singleline);
		static readonly Regex blockComment = new Regex (blockCommentPattern, RegexOptions.Singleline);
		static readonly Regex lineComment = new Regex (lineCommentPattern, RegexOptions.Singleline);
		static readonly Regex spaces = new Regex (@"\s+");
		static readonly Regex filePart = new Regex (filePartPattern, RegexOptions.Singleline);
		static readonly Regex id = new Regex ("^[A-Za-z_][A-Za-z0-9_]*");

		public CMakeVariableManager VariableManager {
			get; private set;
		}

		readonly List<CMakeCommand> targetCommands = new List<CMakeCommand> ();

		bool CheckSyntax ()
		{
			var checkText = lineComment.Replace (contentFile.Text + '\n', "\n");
			checkText = blockComment.Replace (checkText, " ");
			checkText = command.Replace (checkText, " ");
			checkText = spaces.Replace (checkText, ",");

			if (checkText.Trim (',').Length > 0) {
				throw new Exception (string.Format ("Syntax error found in {0} unrecognized strings ('{1}') were found.",
													File, string.Join ("', '", checkText.Trim (',').Split (','))));
			}
			return true;
		}

		IEnumerable<Match> ReadNextCommand ()
		{
			if (CheckSyntax ()) {
				foreach (Match m in filePart.Matches (contentFile.Text)) {
					if (m.Value.StartsWith ("#", StringComparison.Ordinal))
						continue; //Skip comments.
					yield return m;
				}
			}
		}

		void ParseCommand (Match commandMatch)
		{
			string commandName = id.Match (commandMatch.Value).Value;
			CMakeCommand command = new CMakeCommand (commandName, commandMatch, this);

			switch (command.Name.ToLower ()) {
			case "if":
				blocks.Push (command);
				break;
			case "elseif":
				blocks.Pop ();
				blocks.Push (command);
				break;
			case "else":
				blocks.Pop ();
				blocks.Push (command);
				break;
			case "endif":
				blocks.Pop ();
				break;
			case "while":
				blocks.Push (command);
				break;
			case "endwhile":
				blocks.Pop ();
				break;
			case "foreach":
				blocks.Push (command);
				break;
			case "endforeach":
				blocks.Pop ();
				break;
			case "project":
				projectName = command.Arguments [0].ToString ();
				break;
			case "set":
				SetCommands.Add (string.Format ("{0}:{1}", command.Arguments [0].GetValues () [0], commandMatch.Index),
								 command);
				break;
			case "add_library":
			case "add_executable":
				targetCommands.Add (command);
				break;
			case "add_subdirectory":
				var temp = new FilePath (command.Arguments [0].ToString ());
				var fullPath = file.ParentDirectory.Combine (temp).Combine ("CMakeLists.txt");

				if (!System.IO.File.Exists (fullPath)) break;

				if (Parent == null) {
					if (!Children.ContainsKey (temp))
						Children.Add (temp, new CMakeFileFormat (fullPath, Project, this));
				} else {
					if (!Parent.Children.ContainsKey (temp))
						Children.Add (temp, new CMakeFileFormat (fullPath, Project, Parent));
				}
				break;
			}

			command.IsNested = blocks.Count == 0;

			allCommands.Add (string.Format ("{0}:{1}", commandName, commandMatch.Index), command);
		}

		public void Parse ()
		{
			Targets.Clear ();
			SetCommands.Clear ();
			allCommands.Clear ();
			blocks.Clear ();
			Children.Clear ();
			projectName = "";

			contentFile = new TextFile (file);
			foreach (Match m in ReadNextCommand ()) {
				try {
					ParseCommand (m);
				} catch (Exception ex) {
					throw new Exception (string.Format ("Exception: {0}\nFile: {1}", ex.Message, file));
				}
			}

			VariableManager = new CMakeVariableManager (this);

			foreach (CMakeCommand command in targetCommands) {
				CMakeTarget target = new CMakeTarget (command, this);

				if (!target.IsAliasOrImported) {
					Targets.Add (string.Format ("{0}:{1}", target.Name, command.Offset), target);
					target.PrintTarget ();
				}
			}

			targetCommands.Clear ();

			foreach (CMakeFileFormat f in Children.Values)
				targets = targets.Concat (f.Targets).ToDictionary (x => x.Key, x => x.Value);

			if (blocks.Count > 0) {
				CMakeCommand c = blocks.Pop ();
				throw new Exception (string.Format ("Unmatched block command '{0}' in file '{1}' offset '{2}'.", c.Name, file, c.Offset));
			}

			LoggingService.LogDebug ("CMake file '{0}' is Loaded.", file);
		}

		public void NewTarget (string name, CMakeTarget.Types type)
		{
			string commandName = "";
			string libraryType = "";

			switch (type) {
			case CMakeTarget.Types.Binary:
				commandName = "add_executable";
				break;
			case CMakeTarget.Types.SharedLibrary:
				commandName = "add_library";
				libraryType = "SHARED";
				break;
			case CMakeTarget.Types.StaticLibrary:
				commandName = "add_library";
				libraryType = "STATIC";
				break;
			case CMakeTarget.Types.Module:
				commandName = "add_library";
				libraryType = "MODULE";
				break;
			case CMakeTarget.Types.ObjectLibrary:
				commandName = "add_library";
				libraryType = "OBJECT";
				break;
			case CMakeTarget.Types.Unknown:
				commandName = "add_library";
				libraryType = "UNKNOWN";
				break;
			default:
				commandName = "add_executable";
				LoggingService.LogDebug ("Unknown target type: {0}", type);
				break;
			}

			CMakeCommand command = new CMakeCommand (commandName, null, this);
			command.AddArgument (name);
			if (!string.IsNullOrEmpty (libraryType))
				command.AddArgument (libraryType);

			allCommands.Add (string.Format ("{0}:{1}{2}", commandName, "new", allCommands.Count), command);

			SaveAll ();
			Parse ();
		}

		public void NewVariable (string name, string value)
		{
			CMakeCommand command = new CMakeCommand ("set", null, this);
			command.AddArgument (name);
			command.AddArgument (value);

			setCommands.Add (string.Format ("{0}:{1}{2}", "set", "new", setCommands.Count), command);
			allCommands.Add (string.Format ("{0}:{1}{2}", "set", "new", allCommands.Count), command);
		}

		public void Save ()
		{
			foreach (var command in allCommands) {
				if (command.Value.Offset == -1)
					contentFile.Text += Environment.NewLine + command.Value.ToString ();
				else if (command.Value.IsDirty)
					contentFile.Text = contentFile.Text.Replace (command.Value.OldValue, command.Value.ToString ());
				else if (command.Value.IsDeleted)
					contentFile.Text = contentFile.Text.Replace (command.Value.OldValue, string.Empty);
			}
			contentFile.Save ();
		}

		public void SaveAll ()
		{
			Save ();
			foreach (var file in Children)
				file.Value.Save ();

			Parse ();
		}

		public bool Rename (string oldName, string newName)
		{
			CMakeCommand c = allCommands.FirstOrDefault ((arg) => {
				return arg.Key.ToLower ().StartsWith ("project", StringComparison.Ordinal);
			}).Value;

			if (c == null)
				return false;

			if (c.EditArgument (oldName, newName)) {
				Save ();
				return true;
			}
			return false;
		}

		public void RemoveTarget (string targetName)
		{
			var commands = Targets.Where ((arg) => {
				return arg.Key.StartsWith (targetName, StringComparison.OrdinalIgnoreCase);
			}).ToList ();

			foreach (var command in commands) {
				command.Value.Command.IsDeleted = true;
				Targets.Remove (command.Key);
			}
		}

		public CMakeFileFormat (FilePath file, CMakeProject project) : this (file, project, null)
		{
		}

		public CMakeFileFormat (FilePath file, CMakeProject project, CMakeFileFormat parent)
		{
			this.file = file;
			this.project = project;
			this.parent = parent;
			Parse ();
		}
	}
}
