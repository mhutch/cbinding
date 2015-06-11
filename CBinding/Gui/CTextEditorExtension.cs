//
// CTextEditorExtension.cs
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
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Components;
using MonoDevelop.Components.Commands;

using MonoDevelop.Ide.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory6.CSharp.Completion;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Core.Text;
using MonoDevelop.Ide.Editor.Extension;
using System.Threading.Tasks;
using System.Threading;
using ClangSharp;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.MonoCSharp;
using MonoDevelop.Projects;

namespace CBinding
{
	public class CTextEditorExtension : CompletionTextEditorExtension, IPathedDocument
	{
		// Allowed chars to be next to an identifier
		private static char[] allowedChars = new char[] {
			'.', ':', ' ', '\t', '=', '*', '+', '-', '/', '%', ',', '&',
			'|', '^', '{', '}', '[', ']', '(', ')', '\n', '!', '?', '<', '>'
		};
		
		// Allowed Chars to be next to an identifier excluding ':' (to get the full name in '::' completion).
		private static char[] allowedCharsMinusColon = new char[] {
			'.', ' ', '\t', '=', '*', '+', '-', '/', '%', ',', '&', '|',
			'^', '{', '}', '[', ']', '(', ')', '\n', '!', '?', '<', '>'
		};

		public override string CompletionLanguage {
			get {
				return "C/C++";
			}
		}
		static bool IsOpenBrace (char c)
		{
			return c == '(' || c == '{' || c == '<' || c == '[';
		}
		static bool IsCloseBrace (char c)
		{
			return c == ')' || c == '}' || c == '>' || c == ']';
		}
		
		static bool IsBrace (char c)
		{
			return IsOpenBrace  (c) || IsCloseBrace (c);
		}
		
		static int SearchMatchingBracket (IReadonlyTextDocument editor, int offset, char openBracket, char closingBracket, int direction)
		{
			bool isInString       = false;
			bool isInChar         = false;	
			bool isInBlockComment = false;
			int depth = -1;
			while (offset >= 0 && offset < editor.Length) {
				char ch = editor.GetCharAt (offset);
				switch (ch) {
					case '/':
						if (isInBlockComment) 
							isInBlockComment = editor.GetCharAt (offset + direction) != '*';
						if (!isInString && !isInChar && offset - direction < editor.Length) 
							isInBlockComment = offset > 0 && editor.GetCharAt (offset - direction) == '*';
						break;
					case '"':
						if (!isInChar && !isInBlockComment) 
							isInString = !isInString;
						break;
					case '\'':
						if (!isInString && !isInBlockComment) 
							isInChar = !isInChar;
						break;
					default :
						if (ch == closingBracket) {
							if (!(isInString || isInChar || isInBlockComment)) 
								--depth;
						} else if (ch == openBracket) {
							if (!(isInString || isInChar || isInBlockComment)) {
								++depth;
								if (depth == 0) 
									return offset;
							}
						}
						break;
				}
				offset += direction;
			}
			return -1;
		}
		
		static int GetClosingBraceForLine (IReadonlyTextDocument editor, IDocumentLine line, out int openingLine)
		{
			int offset = SearchMatchingBracket (editor, line.Offset, '{', '}', -1);
			if (offset == -1) {
				openingLine = -1;
				return -1;
			}
				
			openingLine = editor.OffsetToLineNumber (offset);
			return offset;
		}
		
		public override bool KeyPress (KeyDescriptor descriptor)
		{
			var line = Editor.GetLine (Editor.CaretLine);
			string lineText = Editor.GetLineText (Editor.CaretLine);
			int lineCursorIndex = Math.Min (lineText.Length, Editor.CaretColumn);
			
			// Smart Indentation
			if (Editor.Options.IndentStyle == IndentStyle.Smart)
			{
				if (descriptor.KeyChar == '}') {
					// Only indent if the brace is preceeded by whitespace.
					if(AllWhiteSpace(lineText.Substring(0, lineCursorIndex))) {
						int braceOpeningLine;
						if(GetClosingBraceForLine(Editor, line, out braceOpeningLine) >= 0)
						{
							Editor.ReplaceText (line.Offset, line.Length, GetIndent(Editor, braceOpeningLine, 0) + "}" + lineText.Substring(lineCursorIndex));
							return false;
						}
					}
				} else {
					switch(descriptor.SpecialKey)
					{
						case SpecialKey.Return:
							// Calculate additional indentation, if any.
							char finalChar = '\0';
							char nextChar = '\0';
							string indent = String.Empty;
							if (!String.IsNullOrEmpty (Editor.SelectedText)) {
								int cursorPos = Editor.SelectionRange.Offset;

								Editor.RemoveText (Editor.SelectionRange);
							
								Editor.CaretOffset = cursorPos;
								
								lineText = Editor.GetLineText (Editor.CaretLine);
								lineCursorIndex = Editor.CaretColumn;
	//							System.Console.WriteLine(TextEditorData.Caret.Offset);
							}
							if(lineText.Length > 0)
							{
								if(lineCursorIndex > 0)
									finalChar = lineText[Math.Min(lineCursorIndex, lineText.Length) - 1];
								
								if(lineCursorIndex < lineText.Length)
									nextChar = lineText[lineCursorIndex];
	
								if(finalChar == '{')
									indent = Editor.Options.GetIndentationString ();
							}
	
							// If the next character is an closing brace, indent it appropriately.
							if(IsBrace(nextChar) && !IsOpenBrace(nextChar))
							{
								int openingLine;
								if(GetClosingBraceForLine (Editor, line, out openingLine) >= 0)
								{
									Editor.InsertAtCaret (Editor.EolMarker + GetIndent(Editor, openingLine, 0));
									return false;
								}
							}
						
							// Default indentation method
							Editor.InsertAtCaret (Editor.EolMarker + indent + GetIndent(Editor, Editor.OffsetToLineNumber (line.Offset), lineCursorIndex));
							
							return false;
						
					}
				}
			}
			
			return base.KeyPress (descriptor);
		}

		public override Task<ICompletionDataList> HandleCodeCompletionAsync (CodeCompletionContext completionContext, char completionChar, CancellationToken token = default(CancellationToken))
		{
			CProject project = DocumentContext.Project as CProject;
			ICompletionDataList list = new CompletionDataList ();
			List<ClangCompletionUnit> listbuilder = new List <ClangCompletionUnit> ();
			if (allowedChars.Contains (completionChar) && completionChar != '(') {
				IntPtr pResults = project.cLangManager.codeComplete (completionContext, this.DocumentContext, this);
				CXCodeCompleteResults results = Marshal.PtrToStructure<CXCodeCompleteResults> (pResults);
				if (results.Results.ToInt64 () != 0) {
					for(int i = 0; i < results.NumResults; i++) {
						IntPtr iteratingPointer = results.Results + i * Marshal.SizeOf<CXCompletionResult>();
						CXCompletionResult resultItem = Marshal.PtrToStructure<CXCompletionResult> (iteratingPointer);
						CXCompletionString completionString = new CXCompletionString(resultItem.CompletionString);
						uint completionchunknum = clang.getNumCompletionChunks (completionString.Pointer);
						for (uint j = 0; j < completionchunknum; j++) {
							if (clang.getCompletionChunkKind(completionString.Pointer, j) != CXCompletionChunkKind.CXCompletionChunk_TypedText)
								continue;
							uint priority = clang.getCompletionPriority (completionString.Pointer);
							CXString cxstring = clang.getCompletionChunkText(completionString.Pointer, j);
							string realstring = Marshal.PtrToStringAnsi (clang.getCString(cxstring));
							clang.disposeString (cxstring);
							CompletionData item = new CompletionData (resultItem, realstring);
							listbuilder.Add (new ClangCompletionUnit(priority, item));
						}
					}
				}
				clang.disposeCodeCompleteResults (pResults);
			}
			listbuilder.Sort (
				(ClangCompletionUnit x, ClangCompletionUnit y) => { 
					return x.priority.CompareTo (y.priority);
				});
			int pos = 0;
			foreach (var t in listbuilder)
				list.Insert (pos++, t.data);
			return Task.FromResult (list);
		}

		public override Task<ICompletionDataList> CodeCompletionCommand (CodeCompletionContext completionContext)
		{
			return HandleCodeCompletionAsync (completionContext, ' ');
		}




		public override Task<MonoDevelop.Ide.CodeCompletion.ParameterHintingResult> HandleParameterCompletionAsync (CodeCompletionContext completionContext, char completionChar, CancellationToken token = default(CancellationToken))
		{
			if (completionChar != '(')
				return null;
			
			List<MonoDevelop.Ide.CodeCompletion.ParameterHintingData> data = new List <MonoDevelop.Ide.CodeCompletion.ParameterHintingData> ();
			var functions = (DocumentContext.Project as CProject).db.Functions;
			//Get the current line, then cut the end after the caret position, get the last "word", which will be the function name before '('
			string functionName = Editor.GetLineText (Editor.CaretLine);
			functionName = functionName.Substring (0, Editor.CaretColumn-1);
			string[] words = functionName.Split(new char[]{' ', '\t'});
			functionName = words [words.Length - 1].Split (new char[]{' ', '\t', '('})[0];
			foreach (var func in functions) {
				if (func.SimpleName.Equals (functionName))
					data.Add (new DataWrapper (func));
			}
			return Task.FromResult (new MonoDevelop.Ide.CodeCompletion.ParameterHintingResult (data,0));
		}
		
		private bool AllWhiteSpace (string lineText)
		{
			// We will almost definately need a faster method than this
			foreach (char c in lineText)
				if (!char.IsWhiteSpace (c))
					return false;
			
			return true;
		}
		
		// Snatched from DefaultFormattingStrategy
		private string GetIndent (IReadonlyTextDocument d, int lineNumber, int terminateIndex)
		{
			string lineText = d.GetLineText (lineNumber);
			if(terminateIndex > 0)
				lineText = terminateIndex < lineText.Length ? lineText.Substring(0, terminateIndex) : lineText;
			
			StringBuilder whitespaces = new StringBuilder ();
			
			foreach (char ch in lineText) {
				if (!char.IsWhiteSpace (ch))
					break;
				whitespaces.Append (ch);
			}
			
			return whitespaces.ToString ();
		}
		
		[CommandHandler (MonoDevelop.DesignerSupport.Commands.SwitchBetweenRelatedFiles)]
		protected void Run ()
		{
			var cp = this.DocumentContext.Project as CProject;
			if (cp != null) {
				string match = cp.MatchingFile (this.DocumentContext.Name);
				if (match != null)
					MonoDevelop.Ide.IdeApp.Workbench.OpenDocument (match, cp, true);
			}
		}
		
		[CommandUpdateHandler (MonoDevelop.DesignerSupport.Commands.SwitchBetweenRelatedFiles)]
		protected void Update (CommandInfo info)
		{
			var cp = this.DocumentContext.Project as CProject;
			info.Visible = info.Visible = cp != null && cp.MatchingFile (this.DocumentContext.Name) != null;
		}
		
		#region IPathedDocument implementation
		
		public event EventHandler<DocumentPathChangedEventArgs> PathChanged;
		
		public Gtk.Widget CreatePathWidget (int index)
		{
			PathEntry[] path = CurrentPath;
			if (null == path || 0 > index || path.Length <= index) {
				return null;
			}
			
			object tag = path[index].Tag;
			DropDownBoxListWindow.IListDataProvider provider = null;
			if (tag is ParsedDocument) {
			} else {
				// TODO: Roslyn port
				//provider = new DataProvider (Editor, DocumentContext, tag, new NetAmbience ());
			}
			
			DropDownBoxListWindow window = new DropDownBoxListWindow (provider);
			window.SelectItem (tag);
			return window;
		}

		public PathEntry[] CurrentPath {
			get;
			private set;
		}
		
		protected virtual void OnPathChanged (DocumentPathChangedEventArgs args)
		{
			if (PathChanged != null)
				PathChanged (this, args);
		}
		
		#endregion
		
		protected override void Initialize ()
		{
			base.Initialize ();
		}
			
		public override void Dispose ()
		{
			base.Dispose ();
		}

		[CommandHandler (MonoDevelop.Refactoring.RefactoryCommands.GotoDeclaration)]
		public void GotoDeclaration ()
		{
		}
		
		[CommandUpdateHandler (MonoDevelop.Refactoring.RefactoryCommands.GotoDeclaration)]
		public void CanGotoDeclaration (CommandInfo item)
		{
		}
	
	}
}
