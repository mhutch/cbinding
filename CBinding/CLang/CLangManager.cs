using System;
using CBinding;
using ClangSharp;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System.Collections.Generic;
using System.Threading;
using MonoDevelop.Ide.Editor;
using System.Runtime.InteropServices;
using CBinding.Refactoring;
using CBinding.Parser;

namespace CBinding
{
	public class CLangManager : IDisposable
	{
		public readonly object syncroot = new object ();
		private CProject project;
		private CXIndex index;
		private Dictionary<string, CXTranslationUnit> translationUnits;

		public Dictionary<string, CXTranslationUnit> TranslationUnits {
			get {				
				lock (syncroot) {
					return translationUnits;
				}
			}
		}

		public CLangManager (CProject proj)
		{
			project = proj;
			index = clang.createIndex (0, 0);
			translationUnits = new Dictionary<string, CXTranslationUnit> ();
			project.DefaultConfigurationChanged += CompilerArgumentsUpdate;
		}

		public CXTranslationUnit createTranslationUnit(CProject project, string fileName,CXUnsavedFile[] unsavedFiles) 
		{
			lock (syncroot) {
				if (translationUnits.ContainsKey (fileName)) {
					return translationUnits [fileName];
				} else {
					AddToTranslationUnits (project, fileName, unsavedFiles);
					return translationUnits [fileName];
				}
			}
		}

		public void AddToTranslationUnits (CProject project, string fileName, CXUnsavedFile[] unsavedFiles)
		{
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
						Convert.ToUInt32 (unsavedFiles.Length),
						unsavedFiles)
					);
				} catch (ArgumentException) {
					Console.WriteLine (fileName + " is already added, not adding");
				}
			}
		}

		public void UpdateDatabase(CProject project, string fileName, CXTranslationUnit TU)
		{
			lock (syncroot) {
				project.db.Reset (fileName);
				TranslationUnitParser parser = new TranslationUnitParser (project.db, fileName);
				clang.visitChildren (clang.getTranslationUnitCursor (TU), parser.Visit, new CXClientData (new IntPtr (0)));
			}
		}

		public void RemoveTranslationUnit (CProject project, string fileName)
		{
			lock (syncroot) {
				clang.disposeTranslationUnit (translationUnits [fileName]);
				translationUnits.Remove (fileName);
			}
		}

		public void CompilerArgumentsUpdate (object sender, EventArgs args) 
		{
			lock (syncroot) {
				ClangCCompiler compiler = new ClangCCompiler ();
				CProjectConfiguration active_configuration =
					(CProjectConfiguration)project.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);
				while (active_configuration == null) {
					Thread.Sleep (20);
					active_configuration = (CProjectConfiguration)project.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);
				}
				string[] compilerArgs = compiler.GetCompilerFlagsAsArray (project, active_configuration);
				foreach (var TU in translationUnits) {
					clang.disposeTranslationUnit (translationUnits [TU.Key]);
					translationUnits [TU.Key] = clang.createTranslationUnitFromSourceFile (
						index,
						TU.Key,
						compilerArgs.Length,
						compilerArgs,
						//CDocumentParser.Parse will reparse with unsaved files, risky to get them from here
						0,
						null
					);
				}
			}
		}

		public IntPtr codeComplete (
			DocumentContext documentContext,
			CXUnsavedFile[] unsavedFiles,
			CTextEditorExtension editor)
		{
			lock (syncroot) {
				string name = documentContext.Name;
				CXTranslationUnit TU = TranslationUnits [name];
				string complete_filename = editor.Editor.FileName;
				uint complete_line = Convert.ToUInt32 (editor.Editor.CaretLine);
				uint complete_column = Convert.ToUInt32 (editor.Editor.CaretColumn);
				uint numUnsavedFiles = Convert.ToUInt32 (unsavedFiles.Length);
				uint options = (uint)CXCodeComplete_Flags.CXCodeComplete_IncludeCodePatterns | (uint)CXCodeComplete_Flags.CXCodeComplete_IncludeCodePatterns;
				return clang.codeCompleteAt (
					                  TU,
					                  complete_filename, 
					                  complete_line, 
					                  complete_column, 
					                  unsavedFiles, 
									  numUnsavedFiles, 
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
			lock (syncroot) {
				foreach (var T in translationUnits) {
					clang.visitChildren (
						clang.getTranslationUnitCursor (T.Value),
						visitor.Visit,
						new CXClientData (new IntPtr (0))
					);
				}
			}
		}

		public void findReferences(RenameHandlerDialog visitor) {
			lock (syncroot) {
				foreach (var T in translationUnits) {
					clang.visitChildren (
						clang.getTranslationUnitCursor (T.Value),
						visitor.Visit,
						new CXClientData (new IntPtr (0))
					);
				}
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

		#region IDisposable implementation

		void IDisposable.Dispose ()
		{
			lock (syncroot) {
				foreach (CXTranslationUnit unit in translationUnits.Values)
					clang.disposeTranslationUnit (unit);
				clang.disposeIndex (index);
			}
		}

		#endregion
	}
}

