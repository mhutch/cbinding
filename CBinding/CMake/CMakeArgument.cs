//
// CMakeArgument.cs
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
using System.Text.RegularExpressions;

namespace CBinding
{
	public class CMakeArgument
	{
		public CMakeArgument (string value)
		{
			if (value.StartsWith ("\"")) {
				value = value.Substring (1, value.Length - 2);
				values.InsertRange (0, Regex.Split (value, @"(?<!\\);"));
				IsString = true;
			} else {
				this.value = value;
			}
		}

		List<string> values = new List<string> ();
		string value;
		
		public bool IsString;

		public bool Remove (string arg)
		{
			return values.Remove (arg);
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
		
		public override string ToString ()
		{
			if (IsString) {
				return String.Format ("\"{0}\"", String.Join (";", values));
			} else {
				return value;
			}
		}
	}
}

