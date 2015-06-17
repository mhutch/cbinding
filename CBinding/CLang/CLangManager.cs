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


using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.CSharp;

namespace CBinding
{
	public class CLangManager
	{
		private static object syncroot = new object ();
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

		private CXUnsavedFile[] UnsavedFiles {
			get {
				lock (syncroot) {
					List<CXUnsavedFile> unsavedFiles = new List<CXUnsavedFile> ();
					foreach (Document doc in MonoDevelop.Ide.IdeApp.Workbench.Documents) {
						CXUnsavedFile unsavedFile = new CXUnsavedFile ();
						unsavedFile.Filename = doc.FileName;
						unsavedFile.Length = doc.Editor.Text.Length;
						unsavedFile.Contents = doc.Editor.Text;
						unsavedFiles.Add (unsavedFile);
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
					CXTranslationUnit TU = new CXTranslationUnit();
					CXErrorCode error = clang.parseTranslationUnit2 (
						index,
						fileName,
						args,
						args.Length,
						UnsavedFiles,
						Convert.ToUInt32 (UnsavedFiles.Length),
						(uint)CXTranslationUnit_Flags.CXTranslationUnit_PrecompiledPreamble,
						out TU);
					project.db.Reset (fileName);
					TranslationUnitParser parser = new TranslationUnitParser(project.db, fileName);
					clang.visitChildren (clang.getTranslationUnitCursor (TU), parser.Visit, new CXClientData (new IntPtr(0)));
					if(error != CXErrorCode.CXError_Success) {
						throw new InvalidComObjectException (((uint)error).ToString ());
					}
					translationUnits.Add (fileName, TU);
				} catch (ArgumentException) {
					Console.WriteLine (fileName + " is already added, not adding");
				} catch (InvalidComObjectException ex) {
					Console.WriteLine ("Parse translation unit failed with error code:" + ex.Message);
					throw;
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
					TranslationUnitParser parser = new TranslationUnitParser(project.db, fileName);
					clang.visitChildren (clang.getTranslationUnitCursor (translationUnits[fileName]), parser.Visit, new CXClientData (new IntPtr(0)));
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
				foreach (var f in project.Files)
					UpdateTranslationUnit (project, f.Name);
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

		public CXCursor getCursor (string fileName, DocumentLocation location) {
			lock (syncroot) {
				//this update is needed to avoid saving (and reparsing) before asking a cursor
				UpdateTranslationUnit (project, fileName);
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
	}
}

