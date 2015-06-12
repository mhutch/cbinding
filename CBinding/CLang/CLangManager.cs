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
				if (translationUnits.ContainsKey (fileName)) {
					clang.disposeTranslationUnit (translationUnits [fileName]);
					translationUnits.Remove (fileName);
				}
				AddToTranslationUnits (project, fileName);
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
	}
}

