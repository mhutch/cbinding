//
// CMakeCommand.cs
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
using System.Text.RegularExpressions;
using System.Collections.Generic;

using MonoDevelop.Core;

namespace CBinding
{
	public class CMakeCommand
	{
		readonly Match command;
		string argumentsText;
		readonly CMakeFileFormat parent;
		static readonly Regex openingRegex = new Regex (@"^[A-Za-z_][A-Za-z0-9_]*\s*\(");
		static readonly Regex argumentsRegex = new Regex (@"\"".*?\""|\[=[0-9]*\[.*?\]=[0-9]*\]|[^\s\""\[]+");

		public string Name {
			get { return name; }
		}

		readonly string name;

		public List<CMakeArgument> Arguments {
			get { return arguments; }
		}
		List<CMakeArgument> arguments = new List<CMakeArgument> ();

		public bool IsNested { get; set; }
		public bool IsDirty { get; private set; }
		public bool IsDeleted { get; set; }

		public int Offset {
			get {
				if (command != null)
					return command.Index;
				else
					return -1;
			}
		}
		public FilePath FileName {
			get {
				return parent.File;
			}
		}

		public string ArgumentsString {
			get {
				if (IsDirty) {
					if (Arguments.Count > 3)
						return string.Join (Environment.NewLine + "\t\t", Arguments);
					else return string.Join (" ", Arguments);
				} else return argumentsString;
			}
			private set {
				argumentsString = value;
			}
		}
		string argumentsString;

		public void AddArgument (string argument)
		{
			CMakeArgument arg = new CMakeArgument (argument);
			if (!Arguments.Contains (arg)) {
				Arguments.Add (arg);
				IsDirty = true;
			}
		}

		public bool RemoveArgument (string argument)
		{
			foreach (CMakeArgument arg in Arguments) {
				if (arg.ToString () == argument) {
					IsDirty = true;
					return Arguments.Remove (arg);
				} else {
					if (arg.Remove (argument)) {
						IsDirty = true;
						return true;
					}
				}
			}
			return false;
		}

		public bool EditArgument (string oldArgument, string newArgument)
		{
			foreach (var argument in Arguments) {
				if (argument.Edit (oldArgument, newArgument)) {
					IsDirty = true;
					return true;
				}
			}
			return false;
		}

		void ParseArguments ()
		{
			argumentsText = openingRegex.Replace (command.Value, "").TrimEnd (')').Trim ();

			ArgumentsString = argumentsText;

			if (argumentsText.Length == 0)
				return;

			foreach (Match m in argumentsRegex.Matches (argumentsText))
				Arguments.Add (new CMakeArgument (m.Value));
		}

		public string OldValue {
			get { return command.Value; }
		}

		public override string ToString ()
		{
			if (Arguments.Count > 3)
				return string.Format ("{0} ({1})", Name, string.Join (Environment.NewLine + "\t\t", Arguments));

			if (IsDeleted) return string.Empty;

			return string.Format ("{0} ({1})", Name, string.Join (" ", Arguments));
		}

		public CMakeCommand (string name, Match command, CMakeFileFormat parent)
		{
			this.name = name;
			this.command = command;
			this.parent = parent;
			if (command != null)
				ParseArguments ();
		}
	}
}
