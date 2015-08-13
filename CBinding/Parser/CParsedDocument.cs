// 
// CDocumentParser.cs
//  
// Author:
//	   Levi Bard <taktaktaktaktaktaktaktaktaktak@gmail.com>
// 
// Copyright (c) 2009 Levi Bard
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
using System.Threading;
using ClangSharp;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Ide.TypeSystem;

namespace CBinding.Parser
{
	
	public class CParsedDocument : DefaultParsedDocument {
		public CXTranslationUnit TU;
		public CLangManager Manager { get; private set;}
		public CProject Project { get; set;}
		List<CXUnsavedFile> unsavedFiles;

		void Initialize (CProject proj)
		{
			Project = proj;
			Manager = proj.ClangManager;
			unsavedFiles = new List<CXUnsavedFile> ();
		}

		public CParsedDocument(CProject proj, string fileName) : base(fileName)
		{
			Initialize (proj);
			unsavedFiles = Project.UnsavedFiles.Get ();
			TU = Manager.CreateTranslationUnit(fileName, unsavedFiles.ToArray ());
		}

		/// <summary>
		/// Reparse the Translation Unit contained by this instance.
		/// Updates Symbol Database
		/// Places error markers on document
		/// </summary>
		public void ParseAndDiagnose (CancellationToken cancellationToken)
		{
			lock (Manager.SyncRoot) {
				var unsavedFilesArray = unsavedFiles.ToArray ();
				TU = Manager.Reparse (FileName, unsavedFilesArray, cancellationToken);
				uint numDiag = clang.getNumDiagnostics (TU);
				for (uint i = 0; i < numDiag; i++) {
					CXDiagnostic diag = clang.getDiagnostic (TU, i);
					string spelling = diag.ToString ();
					uint numRanges = clang.getDiagnosticNumRanges (diag);
					if (numRanges != 0) {
						for (uint j = 0; j < numRanges; j++) {
							try {
								SourceLocation begin = Manager.GetSourceLocation (clang.getRangeStart (clang.getDiagnosticRange (diag, j)));
								SourceLocation end = Manager.GetSourceLocation (clang.getRangeEnd (clang.getDiagnosticRange (diag, j)));
								Add (new Error (ErrorType.Error, spelling, new DocumentRegion (begin.Line, begin.Column, end.Line, end.Column)));
							} catch {
								//it seems sometimes "expression result unused" diagnostics appear multiple times
								//for the same problem, when there is only e.g.
								//an '1;' line in the code, and not every indicator has a valid filename in their location
								//this crashes the thread, so we ignore it
							}
						}
					} else {
						try {
							SourceLocation loc = Manager.GetSourceLocation (clang.getDiagnosticLocation (diag));
							Add (new Error (ErrorType.Error, spelling, new DocumentRegion (loc.Line, loc.Column, loc.Line, loc.Column + 1)));
						} catch {
							//same goes here
						}
					}
					clang.disposeDiagnostic (diag);
				}
				Manager.UpdateDatabase (FileName, TU, cancellationToken, true);
			}
		}
	}
}
