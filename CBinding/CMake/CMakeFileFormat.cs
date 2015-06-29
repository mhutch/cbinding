//
// CMakeFileFormat.cs
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Tasks;
using MonoDevelop.Core.Logging;

namespace CBinding {
	public class CMakeFileFormat
	{
		//File path of the current CMakeLists.txt
		public FilePath File;
		public CMakeProject Project;
		public string ProjectName = ""; //Comes from `project (project_name)` cmake command.
		public Dictionary<string, CMakeCommand> SetCommands = new Dictionary<string, CMakeCommand> ();
		public Dictionary<string, CMakeTarget> Targets = new Dictionary<string, CMakeTarget> ();
		public Dictionary<FilePath, CMakeFileFormat> Children = new Dictionary<FilePath, CMakeFileFormat> ();
		
		public CMakeFileFormat Parent;
		Dictionary<string, CMakeCommand> allCommands = new Dictionary<string, CMakeCommand> ();
		Stack<string> blocks = new Stack<string> ();
		MonoDevelop.Projects.Text.TextFile contentFile;
		
		//[A-Za-z_][A-Za-z0-9_]*	: ID
		//\[=[0-9]*\[.*?\]=[0-9]*\]	: bracket argument.
		string commandPattern = @"[A-Za-z_][A-Za-z0-9_]*\s*\((\"".*?\""|\[=[0-9]*\[.*?\]=[0-9]*\]|.)*?\)";
		string blockCommentPattern = @"#\[\[.*?\]\]";
		string lineCommentPattern = @"#.*?\n";

		bool checkSyntax ()
		{
			var checkText = Regex.Replace (contentFile.Text, lineCommentPattern, "\n", RegexOptions.Singleline);
			checkText = Regex.Replace (checkText, blockCommentPattern, "", RegexOptions.Singleline);
			checkText = Regex.Replace (checkText, @"\s+", " ", RegexOptions.Singleline);
			checkText = Regex.Replace (checkText, commandPattern, "", RegexOptions.Singleline);
			if (checkText.Trim ().Length > 0) {
				MonoDevelop.Ide.MessageService.ShowError ("Couldn't the load project.", String.Format ("Syntax errors found in {0}", File.ToString ()));
				throw new Exception (String.Format ("Syntax errors found in {0}", File.ToString ()));
			}
			return true;
		}

		IEnumerable<Match> readNextCommand ()
		{
			if (checkSyntax ()) {
				string fullPattern = commandPattern + '|' + lineCommentPattern + '|' + blockCommentPattern;
				foreach (Match m in Regex.Matches (contentFile.Text, fullPattern, RegexOptions.Singleline)) {
					if (m.Value.StartsWith ("#"))
						continue; //Skip comments.
					yield return m;
				}
            }
		}

		void parseCommand (Match commandMatch)
		{
			string commandName = Regex.Match (commandMatch.Value, @"^[A-Za-z_][A-Za-z0-9_]*").Value;
			CMakeCommand command = new CMakeCommand (commandName, commandMatch, this);

			switch (command.Name.ToLower ()) {
			case "if":
				blocks.Push ("if");
				break;
			case "elseif":
				blocks.Pop ();
				blocks.Push ("elseif");
				break;
			case "else":
				blocks.Pop ();
				blocks.Push ("else");
				break;
			case "endif":
				blocks.Pop ();
				break;
			case "while":
				blocks.Push ("while");
				break;
			case "endwhile":
				blocks.Pop ();
				break;
			case "foreach":
				blocks.Push ("foreach");
				break;
			case "project":
				ProjectName = command.Arguments[0].ToString();
				break;
			case "set":
				SetCommands.Add (string.Format ("{0}:{1}", command.Arguments [0].GetValues () [0], commandMatch.Index), command);
				break;
			case "add_library":
			case "add_executable":
				CMakeTarget target = new CMakeTarget (command, this);

				if (!target.IsAliasOrImported) {
					Targets.Add (string.Format ("{0}:{1}", target.Name, commandMatch.Index), target);
					target.PrintTarget ();
				}
				break;
			case "add_subdirectory":
				if (Parent == null) {
					FilePath temp = new FilePath (command.Arguments [0].ToString ());
					if (!Children.ContainsKey (temp))
						Children.Add (temp, new CMakeFileFormat (temp, Project, this));
				} else {
					FilePath temp = new FilePath (command.Arguments [0].ToString ());
					if (!Parent.Children.ContainsKey (temp))
						Children.Add (temp, new CMakeFileFormat (temp, Project, Parent));
				}
				break;
			default:
				break;
			}
			
			command.IsEditable = blocks.Count == 0;
			
			allCommands.Add (string.Format ("{0}:{1}", commandName, commandMatch.Index), command);
		}

		public void Parse ()
		{
			Targets.Clear ();
			SetCommands.Clear ();
			allCommands.Clear ();
			blocks.Clear ();
			Children.Clear ();
			ProjectName = "";
			
			contentFile = new MonoDevelop.Projects.Text.TextFile(File);
			foreach (Match m in readNextCommand ()) {
				parseCommand (m);
			}
			
			if (blocks.Count > 0)
				throw new Exception ("Unmatched block command (if, while or foreach).");
		}

		//for debugging purposes. 
		void printArgs ()
		{
			foreach (var command in SetCommands)
			{
				LoggingService.LogDebug (String.Format ("{0}\n{1}", command.Key, command.Value.ToString ()));
			}
		}
		
		public void NewTarget (string name, CMakeTarget.Types type)
		{
			string commandName = "";
			string libraryType = "";
			
			switch (type) {
			case CMakeTarget.Types.BINARY:
				commandName = "add_executable";
				break;
			case CMakeTarget.Types.SHARED_LIBRARY:
				commandName = "add_library";
				libraryType = "SHARED";
				break;
			case CMakeTarget.Types.STATIC_LIBRARY:
				commandName = "add_library";
				libraryType = "SHARED";
				break;
			case CMakeTarget.Types.MODULE_LIBRARY:
				commandName = "add_library";
				libraryType = "SHARED";
				break;
			case CMakeTarget.Types.OBJECT_LIBRARY:
				commandName = "add_library";
				libraryType = "SHARED";
				break;
			case CMakeTarget.Types.UNKOWN:
				commandName = "add_library";
				libraryType = "SHARED";
				break;
			default:
				commandName = "add_library";
				break;
			}
			
			CMakeCommand command = new CMakeCommand (commandName, null, this);
			command.AddArgument (name);
			if (!String.IsNullOrEmpty (libraryType))
				command.AddArgument (libraryType);
				
			allCommands.Add (String.Format ("{0}:{1}{2}", commandName, "new", allCommands.Count), command);
		}
		
		public void NewVariable (string name, string value)
		{
			CMakeCommand command = new CMakeCommand ("set", null, this);
			command.AddArgument (name);
			command.AddArgument (value);
			
			SetCommands.Add (String.Format ("{0}:{1}{2}", "set", "new", SetCommands.Count), command);
			allCommands.Add (String.Format ("{0}:{1}{2}", "set", "new", allCommands.Count), command);
		}
		
		public void Save ()
		{
			contentFile.Text = "";
			foreach (var command in allCommands)
				contentFile.Text += command.Value.ToString () + '\n';

			contentFile.Save ();
		}
		
		public void SaveAll ()
		{
			Save ();
			foreach (var file in Children)
				file.Value.Save ();
		}
		
		public bool Rename (string oldName, string newName)
		{
			CMakeCommand c = allCommands.FirstOrDefault ((arg) => arg.Key.ToLower ().StartsWith ("project")).Value;
			if (c == null)
				return false;
			
			return c.EditArgument (oldName, newName);
		}

		public CMakeFileFormat (FilePath file, CMakeProject project)
		{
			File = file;
			Project = project;
		}
		
		public CMakeFileFormat (FilePath file, CMakeProject project, CMakeFileFormat parent)
		{
			File = file;
			Project = project;
			this.Parent = parent;
		}
	}
}

