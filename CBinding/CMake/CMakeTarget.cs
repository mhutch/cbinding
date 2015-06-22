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

using MonoDevelop.Core;

namespace CBinding {
	public class CMakeTarget {
		public enum Types {
			BINARY,
			SHARED_LIBRARY,
			STATIC_LIBRARY,
			MODULE_LIBRARY,
			OBJECT_LIBRARY,
			UNKOWN
		};
		public string Name;
		public Types? Type;
		public bool IsAliasOrImported = false;

		List<string> ignored = new List<string> () {
			"WIN32", "MACOSX_BUNDLE", "EXCLUDE_FROM_ALL"
		};
		CMakeCommand command;
		Dictionary<string, CMakeCommand> files = new Dictionary<string, CMakeCommand> ();
		CMakeFileFormat parent;
		
		public CMakeTarget (CMakeCommand command, CMakeFileFormat parent)
		{
			this.command = command;
			this.parent = parent;
			if (command.Name.ToLower () == "add_executable")
				Type = Types.BINARY;

			getFiles ();
		}

		void parseValue (string value)
		{
			if (String.IsNullOrEmpty (Name)) {
				Name = value;
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

			if (!files.ContainsKey (new FilePath (value)))
				files.Add (new FilePath (value), this.command);
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
					files = files.Concat(cmvp.Values).ToDictionary(x=>x.Key, x=>x.Value);
				}
			}
		}
		
		public void AddFile (string filename)
		{
			command.AddArgument (filename);
		}
		
		public bool RemoveFile (string filename)
		{
			if (files [filename].IsEditable)
				return files [filename].RemoveArgument (filename);
				
			return false;
		}
		
		public bool RenameFile (string oldName, string newName)
		{
			if (files [oldName].IsEditable)
				return files [oldName].EditArgument (oldName, newName);

			return false;
		}
		
		public void PrintTarget ()
		{
			LoggingService.LogDebug (String.Format ("[Target Name : {0}]", Name));
			foreach (var file in files)
			{
				LoggingService.LogDebug (file.ToString ());
			}
			LoggingService.LogDebug ("[End Target]");
		}
	}
}

