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

namespace CBinding
{
	public class CLangManager
	{
		private static CLangManager instance;
		private static object syncroot = new object ();

		public static CLangManager Instance {
			get {
				lock (syncroot) {		
					if (instance == null)
						instance = new CLangManager ();
					return instance;
				}
			}
		}

		private CXIndex index;
		private Dictionary<string, CXTranslationUnit> translationUnits;

		private CXUnsavedFile[] UnsavedFiles {
			get {
				lock (syncroot) {
					List<CXUnsavedFile> unsavedFiles = new List<CXUnsavedFile> ();
					foreach (Document doc in MonoDevelop.Ide.IdeApp.Workbench.Documents) {
						if (doc.IsDirty) {
							CXUnsavedFile unsavedFile = new CXUnsavedFile ();
							unsavedFile.Length = doc.Editor.Text.Length;
							unsavedFile.Contents = doc.Editor.Text;
							unsavedFiles.Add (unsavedFile);
						}
					}
					return unsavedFiles.ToArray ();
				}
			}
		}

		private CLangManager ()
		{
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
					translationUnits.Add (fileName, 
						clang.createTranslationUnitFromSourceFile (
							index,
							fileName,
							args.Length,
							args,
							Convert.ToUInt32 (UnsavedFiles.Length),
							UnsavedFiles
						)
					);
				} catch (ArgumentException) {
					Console.WriteLine (fileName + " is already added, not adding");
				}
			}
		}

		public void UpdateTranslationUnit (CProject project, string fileName)
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
				if (translationUnits.ContainsKey (fileName)) {
					clang.disposeTranslationUnit (translationUnits [fileName]);
					translationUnits.Remove (fileName);
				}
				translationUnits.Add (fileName, clang.createTranslationUnitFromSourceFile (
					index,
					fileName,
					args.Length,
					args,
					Convert.ToUInt32 (UnsavedFiles.Length),
					UnsavedFiles
				));
			}

		}

		public void RemoveTranslationUnit (CProject project, string fileName)
		{
			lock (syncroot) {
				clang.disposeTranslationUnit (translationUnits [fileName]);
				translationUnits.Remove (fileName);
			}
		}
	}
}

