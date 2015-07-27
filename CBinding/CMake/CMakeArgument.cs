//
// CMakeArgument.cs
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

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CBinding
{
	public class CMakeArgument
	{
		public CMakeArgument (string value)
		{
			if (value.StartsWith ("\"", System.StringComparison.Ordinal)) {
				value = value.Substring (1, value.Length - 2);
				values.InsertRange (0, Regex.Split (value, @"(?<!\\);"));
				isString = true;
			} else {
				this.value = value;
			}
		}

		readonly List<string> values = new List<string> ();
		string value;

		public bool IsString {
			get { return isString; }
		}
		bool isString;

		public bool Remove (string arg)
		{
			return values.Remove (arg);
		}

		public bool Edit (string oldArg, string newArg)
		{
			if (value == oldArg) {
				value = newArg;
				return true;
			}

			if (values.Contains (oldArg)) {
				values [values.IndexOf (oldArg)] = newArg;
				return true;
			}
			return false;
		}

		public List<string> GetValues ()
		{
			if (IsString) {
				return values;
			} else {
				var list = new List<string> ();
				list.Add (value);
				return list;
			}
		}

		public static List<CMakeArgument> ArgumentsFromString (string text)
		{
			var args = new List<CMakeArgument> ();

			foreach (Match m in Regex.Matches (text, @"\"".*?\""|\[=[0-9]*\[.*?\]=[0-9]*\]|[^\s\""\[]+"))
				args.Add (new CMakeArgument (m.Value));

			return args;
		}

		public override string ToString ()
		{
			if (IsString) {
				return string.Format ("\"{0}\"", string.Join (";", values));
			} else {
				return value;
			}
		}
	}
}
