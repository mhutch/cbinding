//
// CMakeVariableParser.cs
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
using System.Collections.Generic;
using System.Linq;

using MonoDevelop.Core;

namespace CBinding {
	public class CMakeVariableParser {
		//This will only return file and file lists.
		Dictionary<string, string> builtInVariables = new Dictionary<string, string> ();
		CMakeFileFormat parent;
		public Dictionary<string, CMakeCommand> Values = new Dictionary<string, CMakeCommand> ();
		CMakeCommand command;
		List<string> toIgnore = new List<string>() {
			"filepath", "path", "string", "bool", "internal", "cache", "force", "parent_scope"
		};
		
		void initializeBuiltInVaiables ()
		{
			builtInVariables.Add ("CMAKE_SOURCE_DIR", parent.Project.BaseDirectory);
			builtInVariables.Add ("CMAKE_CURRENT_SOURCE_DIR", parent.File.ParentDirectory);
			builtInVariables.Add ("PROJECT_SOURCE_DIR",
			                      (String.IsNullOrEmpty (parent.ProjectName) && parent.Parent != null) ?
			                      parent.Parent.Project.BaseDirectory : parent.Project.BaseDirectory);
			builtInVariables.Add (String.Format("{0}{1}", parent.ProjectName, "_SOURCE_DIR"),
			                      parent.File.ParentDirectory);
		}

		void getFromSets (string variable)
		{
			IEnumerable<KeyValuePair<string, CMakeCommand>> commands =
					parent.SetCommands.Where((KeyValuePair<string, CMakeCommand> arg) =>
			                                   arg.Key.StartsWith(variable, StringComparison.OrdinalIgnoreCase));

			foreach (var command in commands)
			{
				bool isFirst = true;
				foreach (var arg in command.Value.Arguments)
				{
					if (isFirst)
					{
						isFirst = false;
						continue;
					}

					string argument = arg.ToString ();

					if (toIgnore.Contains (argument, StringComparer.OrdinalIgnoreCase))
						continue;
					
					if (argument.StartsWith ("${")) {
						string variableName = argument.Substring (2, argument.Length - 3);
						if (variableName == variable)
							continue;

						if (builtInVariables.ContainsKey (variableName)) {
							Values.Add (builtInVariables [variableName], command.Value);
							continue;
						} else {
							CMakeVariableParser cmvp = new CMakeVariableParser (argument, command.Value, parent);
							Values = Values.Concat (cmvp.Values).ToDictionary (x=>x.Key, x=>x.Value);
						}
					} else {
						if (!Values.ContainsKey (argument))
							Values.Add (argument, command.Value);
					}
				}
			}

		}

		public CMakeVariableParser(string variable, CMakeCommand command, CMakeFileFormat parent)
		{
			//Assuming filename.c, ${list_of_vars} or ${CMAKE_SOMETHING}/somefile.c
			this.parent = parent;
			this.command = command;
			initializeBuiltInVaiables ();
			string[] vals = variable.Split ('}');
			
			if (String.IsNullOrWhiteSpace (vals [1])) {
				string variableName = variable.Substring (2, variable.Length - 3);
				getFromSets (variableName);
				return;
			} else {
				string variableName = vals [0].Substring (2, vals [0].Length - 2);
				string file;
				
				if (builtInVariables.ContainsKey (variableName)) {
					if (!vals [1].StartsWith ("${")) {
						file = Path.Combine(builtInVariables [variableName], vals[1]);
						Values.Add(file, command);
					}
				}
				return;
			}
			
			throw new Exception(String.Format("Couldn't parse variable {0}", variable));
		}
		
	}
}

