using System;
using CBinding;
using ClangSharp;
using System.Security.Cryptography;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using System.Text;
using MonoDevelop.Core;
using System.IO;
using MonoDevelop.Projects;
using System.Collections.Generic;
using System.Threading;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Components.PropertyGrid;

using System.Linq;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.CSharp;
using CBinding.Refactoring;
using CBinding.Parser;
using MonoDevelop.Components.Docking;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Ide.Tasks;
using Mono.TextEditor;
using MonoDevelop.Ide.TypeSystem;

namespace CBinding
{
	public class CLangManager
	{
		private static object syncroot = new object ();
		private CProject project;
		private CXIndex index;
		private Dictionary<string, CXTranslationUnit> translationUnits;
		private bool started = false;
		private List<KeyValuePair<string, ITextSegmentMarker>> markers = new List<KeyValuePair<string, ITextSegmentMarker>>();
		private Dictionary<string, bool> shouldReparse = new Dictionary<string, bool>();

		public Dictionary<string, CXTranslationUnit> TranslationUnits {
			get {				
				lock (syncroot) {
					return translationUnits;
				}
			}
		}

		private CXUnsavedFile[] UnsavedFiles {
			get {
				lock (syncroot) {
					List<CXUnsavedFile> unsavedFiles = new List<CXUnsavedFile> ();
					foreach (Document doc in MonoDevelop.Ide.IdeApp.Workbench.Documents) {
						if (doc.IsDirty) {
							CXUnsavedFile unsavedFile = new CXUnsavedFile ();
							unsavedFile.Filename = doc.FileName;
							unsavedFile.Length = doc.Editor.Text.Length;
							unsavedFile.Contents = doc.Editor.Text;
							unsavedFiles.Add (unsavedFile);
						}
					}
					return unsavedFiles.ToArray ();
				}
			}
		}

		public CLangManager (CProject proj)
		{
			project = proj;
			index = clang.createIndex (0, 0);
			translationUnits = new Dictionary<string, CXTranslationUnit> ();
		}

		~CLangManager ()
		{
			foreach (CXTranslationUnit unit in translationUnits.Values)
				clang.disposeTranslationUnit (unit);
			clang.disposeIndex (index);
		}

		public void AddToTranslationUnits (CProject project, string fileName)
		{
			while (project.Loading)
				Thread.Sleep (50);
			lock (syncroot) {
				ClangCCompiler compiler = new ClangCCompiler ();
				CProjectConfiguration active_configuration =
					(CProjectConfiguration)project.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);
				while (active_configuration == null) {
					Thread.Sleep (20);
					active_configuration = (CProjectConfiguration)project.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);
				}
				string[] args = compiler.GetCompilerFlagsAsArray (project, active_configuration);
				try {
					translationUnits.Add (fileName, clang.createTranslationUnitFromSourceFile (
						index,
						fileName,
						args.Length,
						args,
						Convert.ToUInt32 (UnsavedFiles.Length),
						UnsavedFiles)
					);
					shouldReparse.Add (fileName, false);
					UpdateTranslationUnit (project, fileName);
				} catch (ArgumentException) {
					Console.WriteLine (fileName + " is already added, not adding");
				}
			}
		}

		public void UpdateTranslationUnit (CProject project, string fileName)
		{
			lock (syncroot) {
				CXUnsavedFile[] unsavedFiles = UnsavedFiles;
				if (translationUnits.ContainsKey (fileName)) {
					clang.reparseTranslationUnit (
						translationUnits[fileName],
						Convert.ToUInt32 (unsavedFiles.Length),
						unsavedFiles,
						clang.defaultReparseOptions (translationUnits[fileName])
					);
					project.db.Reset (fileName);
					CXTranslationUnit TU = translationUnits [fileName];
					TranslationUnitParser parser = new TranslationUnitParser(project.db, fileName);
					clang.visitChildren (clang.getTranslationUnitCursor (TU), parser.Visit, new CXClientData (new IntPtr(0)));
					diagnoseTranslationUnit (fileName);
				} else {
					AddToTranslationUnits (project, fileName);
				}
			}
		}

		public void RemoveTranslationUnit (CProject project, string fileName)
		{
			lock (syncroot) {
				clang.disposeTranslationUnit (translationUnits [fileName]);
				translationUnits.Remove (fileName);
				shouldReparse.Remove (fileName);
				foreach (var f in project.Files)
					shouldReparse [f.Name] = true;
			}
		}

		public void CompilerArgumentsUpdate () {
			lock (syncroot) {
				ClangCCompiler compiler = new ClangCCompiler ();
				CProjectConfiguration active_configuration =
					(CProjectConfiguration)project.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);
				while (active_configuration == null) {
					Thread.Sleep (20);
					active_configuration = (CProjectConfiguration)project.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);
				}
				string[] args = compiler.GetCompilerFlagsAsArray (project, active_configuration);
				var unsavedFiles = UnsavedFiles;
				foreach (var TU in translationUnits) {
					clang.disposeTranslationUnit (translationUnits [TU.Key]);
					translationUnits [TU.Key] = clang.createTranslationUnitFromSourceFile (
						index,
						TU.Key,
						args.Length,
						args,
						Convert.ToUInt32 (unsavedFiles.Length),
						unsavedFiles					
					);
				}
			}
		}

		public IntPtr codeComplete (
			CodeCompletionContext completionContext,
			DocumentContext documentContext,
			CTextEditorExtension editor)
		{
			lock (syncroot) {
				string name = documentContext.Name;
				CXTranslationUnit TU = TranslationUnits [name];
				string complete_filename = editor.Editor.FileName;
				uint complete_line = Convert.ToUInt32 (editor.Editor.CaretLine);
				uint complete_column = Convert.ToUInt32 (editor.Editor.CaretColumn);
				CXUnsavedFile[] unsaved_files = UnsavedFiles;
				uint num_unsaved_files = Convert.ToUInt32 (unsaved_files.Length);
				uint options = (uint)CXCodeComplete_Flags.CXCodeComplete_IncludeCodePatterns | (uint)CXCodeComplete_Flags.CXCodeComplete_IncludeCodePatterns;
				return clang.codeCompleteAt (
					                  TU,
					                  complete_filename, 
					                  complete_line, 
					                  complete_column, 
					                  unsaved_files, 
					                  num_unsaved_files, 
					                  options);
			}
		}

		public CXCursor getCursor (string fileName, MonoDevelop.Ide.Editor.DocumentLocation location) {
			lock (syncroot) {
				CXTranslationUnit TU = TranslationUnits [fileName];
				CXFile file = clang.getFile (TU, fileName);
				CXSourceLocation loc = clang.getLocation (
					TU,
					file,
					Convert.ToUInt32 (location.Line),
					Convert.ToUInt32 (location.Column)
				);
				return clang.getCursor (TU, loc);
			}
		}

		public CXCursor getCursorReferenced (CXCursor refereeCursor) {
			lock (syncroot) {
				return clang.getCursorReferenced (refereeCursor);
			}
		}

		public CXCursor getCursorDefinition (CXCursor cursor) {
			lock (syncroot) {
				return clang.getCursorDefinition (cursor);
			}
		}

		public SourceLocation getCursorLocation (CXCursor cursor) {
			lock (syncroot) {
				CXSourceLocation loc = clang.getCursorLocation (cursor);
				CXFile file;
				uint line, column, offset;
				clang.getExpansionLocation (loc, out file, out line, out column, out offset);
				var fileName = getFileNameString (file);
				return new SourceLocation (fileName, line, column, offset);
			}
		}

		public SourceLocation getSourceLocation(CXSourceLocation loc) {
			lock (syncroot) {
				CXFile file;
				uint line, column, offset;
				clang.getExpansionLocation (loc, out file, out line, out column, out offset);
				var fileName = getFileNameString (file);
				return new SourceLocation (fileName, line, column, offset);
			}
		}

		public void findReferences(FindReferencesHandler visitor) {
			foreach (var T in translationUnits) {
				clang.visitChildren (
					clang.getTranslationUnitCursor (T.Value),
					visitor.Visit,
					new CXClientData (new IntPtr(0))
				);
			}
		}

		public void findReferences(RenameHandlerDialog visitor) {
			foreach (var T in translationUnits) {
				clang.visitChildren (
					clang.getTranslationUnitCursor (T.Value),
					visitor.Visit,
					new CXClientData (new IntPtr(0))
				);
			}
		}

		public string getCursorSpelling(CXCursor cursor){
			lock(syncroot){
				CXString cxstring = clang.getCursorSpelling (cursor);
				string spelling = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
				clang.disposeString (cxstring);
				return spelling;
			}
		}

		public string getCursorDisplayName(CXCursor cursor){
			lock(syncroot){
				CXString cxstring = clang.getCursorDisplayName (cursor);
				string spelling = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
				clang.disposeString (cxstring);
				return spelling;
			}
		}

		public string getCursorUSRString (CXCursor cursor)
		{
			lock (syncroot) {
				CXString cxstring = clang.getCursorUSR (cursor);
				string USR = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
				clang.disposeString (cxstring);
				return USR;
			}
		}

		public string getFileNameString (CXFile file)
		{
			lock (syncroot) {
				CXString cxstring = clang.getFileName (file);
				string fileName = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
				clang.disposeString (cxstring);
				return fileName;
			}
		}

		public void startBackgroundParsingThread () {
			lock (syncroot) {
				if (!started) {
					started = true;
					ThreadPool.QueueUserWorkItem (doStartBackgroundParsingThread);
				}
			}
		}

		private void doStartBackgroundParsingThread (object state) {
			while (project.Loading)
				Thread.Sleep (50);
			while (true) {
				lock (syncroot) {
					CXUnsavedFile[] unsavedFiles = UnsavedFiles;
					foreach (var TU in translationUnits) {
						if (shouldReparse[TU.Key]) {
							clang.reparseTranslationUnit (
								translationUnits [TU.Key],
								Convert.ToUInt32 (unsavedFiles.Length),
								unsavedFiles,
								clang.defaultReparseOptions (translationUnits [TU.Key])
							);
							project.db.Reset (TU.Key);
							TranslationUnitParser parser = new TranslationUnitParser (project.db, TU.Key);
							clang.visitChildren (clang.getTranslationUnitCursor (translationUnits [TU.Key]), parser.Visit, new CXClientData (new IntPtr (0)));
							diagnoseTranslationUnit (TU.Key);
							shouldReparse [TU.Key] = false;
						}
					}
				}
				Thread.Sleep (300);
			}
		}

		public string getDiagnosticSpelling (CXDiagnostic diag)
		{
			CXString cxstring = clang.getDiagnosticSpelling (diag);
			string spelling = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
			clang.disposeString (cxstring);
			return spelling;
		}

		private void diagnoseTranslationUnit (string fileName)
		{
			lock (syncroot) {
				Document doc = null;
				foreach (var d in IdeApp.Workbench.Documents)
					if (d.Name.Equals (fileName))
						doc = d;
				if (doc == null)
					return;
				foreach (var marker in markers) {
					if(doc.FileName.ToString ().Equals (marker.Key)) {
						doc.Editor.RemoveMarker (marker.Value);
					}				
				}
				CXTranslationUnit TU = translationUnits [fileName];
				uint numDiag = clang.getNumDiagnostics (TU);
				for (uint i = 0; i < numDiag; i++) {
					CXDiagnostic diag = clang.getDiagnostic (TU, i);
					string spelling = getDiagnosticSpelling (diag);
					uint numRanges = clang.getDiagnosticNumRanges (diag);
					if (numRanges != 0) {
						for (uint j = 0; j < numRanges; j++) {
							SourceLocation loc = getSourceLocation (clang.getRangeStart (clang.getDiagnosticRange (diag, j)));
							int len = (int)(getSourceLocation (clang.getRangeEnd (clang.getDiagnosticRange (diag, j))).Offset - loc.Offset);
							ITextSegmentMarker m = doc.Editor.TextMarkerFactory.CreateErrorMarker (
								                      doc.Editor,
								                      new Error (ErrorType.Error, spelling),
								                      (int)loc.Offset,
								                      len
							                      );

							markers.Add (new KeyValuePair<string, ITextSegmentMarker> (doc.FileName, m));
							doc.Editor.AddMarker (m);
						}
					} else {
						SourceLocation loc = getSourceLocation (clang.getDiagnosticLocation (diag));
						ITextSegmentMarker m = doc.Editor.TextMarkerFactory.CreateErrorMarker (
							                      doc.Editor,
							                      new Error (ErrorType.Error, spelling),
							                      (int)loc.Offset,
						//when there is no range associated with a diagnostic, only a location
						//there is no way to get associated length, only by tampering more with editor
						//but diags without a range might not worth that effort, eg.: undeclared variables
							                      3
						                      );
						markers.Add (new KeyValuePair<string, ITextSegmentMarker> (doc.FileName, m));
						doc.Editor.AddMarker (m);
					}
				}
			}
		}
	
		public void reparseImminent (string fileName) {
			lock (syncroot) {
				shouldReparse [fileName] = true;
			}
		}
	}
}

