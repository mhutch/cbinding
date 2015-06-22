//
// CMakeCommand.cs
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
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using MonoDevelop.Core;

namespace CBinding {
	public class CMakeCommand {
		Match command;
		string argumentsText;
		CMakeFileFormat parent;
	
		public string Name;
		public List<CMakeArgument> Arguments = new List<CMakeArgument> ();
		public bool IsEditable;
		public int Offset {
			get {
				return command.Index;
			}
		}

		public void AddArgument (string argument)
		{
			Arguments.Add (new CMakeArgument (argument));
		}

		public bool RemoveArgument (string argument)
		{
			foreach (CMakeArgument arg in Arguments) {
				if (arg.ToString () == argument)
					return Arguments.Remove (arg);
				else {
					if (arg.Remove (argument))
						return true;
				}
			}
			return false;
		}
		
		public bool EditArgument (string oldArgument, string newArgument)
		{
			if (RemoveArgument (oldArgument)) {
				AddArgument (newArgument);
				return true;
			}
				
			return false;
		}

		void parseArguments ()
		{	
			argumentsText = Regex.Replace (command.Value, @"^[A-Za-z_][A-Za-z0-9_]*\s*\(", "").TrimEnd (')').Trim ();
			
			if (argumentsText.Length == 0)
				return;

			foreach (Match m in Regex.Matches (argumentsText, @"\"".*?\""|\[=[0-9]*\[.*?\]=[0-9]*\]|[^\s\""\[]+"))
				Arguments.Add (new CMakeArgument (m.Value));
		}
		
		public override string ToString ()
		{
			if (Arguments.Count > 3)
				return string.Format ("{0} ({1})", Name, string.Join(Environment.NewLine+"\t\t", Arguments));
				
			return string.Format ("{0} ({1})", Name, string.Join (" ", Arguments));
		}
		
		public CMakeCommand (string name, Match command, CMakeFileFormat parent)
		{
			Name = name;
			this.command = command;
			this.parent = parent;
			parseArguments ();
		}
	}
}
