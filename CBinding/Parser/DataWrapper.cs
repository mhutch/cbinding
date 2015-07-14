//
// DataProvider.cs
//
// Authors:
//   Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// Copyright (C) 2007 Marcos David Marin Amador
//
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide.Editor;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace CBinding.Parser
{
	sealed class DataWrapper : ParameterHintingData
	{
		readonly OverloadCandidate Overload;

		public DataWrapper (OverloadCandidate overload) : base (null)
		{
			Overload = overload;
		}

		public DataWrapper () : base (null)
		{
		}

		public override int ParameterCount {
			get { return Overload.Parameters.Count; }
		}

		public override bool IsParameterListAllowed {
			get { return false; }
		}

		public override string GetParameterName (int parameter)
		{
			return Overload.Parameters[parameter];
		}

		public override Task<TooltipInformation> CreateTooltipInformation (TextEditor editor, DocumentContext ctx, int currentParameter, bool smartWrap, CancellationToken ctoken)
		{
			var tooltip = new TooltipInformation ();
			string sig = Overload.Returns + " " + Overload.Name;
			StringBuilder builder = new StringBuilder(sig);

			builder.Append ("(" + Environment.NewLine);
			int i = 0;
			foreach(string t in Overload.Parameters) {
				if(i.Equals (currentParameter)) {
					builder.Append ("\t<b>" + GLib.Markup.EscapeText (t) + "</b>" + Environment.NewLine);
				}
				else {
					builder.Append ("\t" + GLib.Markup.EscapeText (t) + Environment.NewLine);
				}
				i++;
			}
			builder.Append (")");

			tooltip.SignatureMarkup = builder.ToString ();
			tooltip.SummaryMarkup = "";

			return Task.FromResult<TooltipInformation> (tooltip);
		}
	}
}
