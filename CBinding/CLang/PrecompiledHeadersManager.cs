using System;
using System.Collections.Generic;
using ClangSharp;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using System.Linq;
using System.IO;

namespace CBinding
{
	public class PrecompiledHeadersManager
	{
		CLangManager Manager { get; }
		CProject project { get; }
		CXIndex Index { get; }
		public List<string> Headers { get; } = new List<string> ();

		public PrecompiledHeadersManager (CProject proj, CLangManager man, CXIndex ind)
		{
			Manager = man;
			project = proj;
			Index = ind;
		}

		public static bool PchIsUpToDate (string name)
		{
			var source = new FileInfo (name);
			if (!File.Exists (name + ".pch"))
				return false;
			var pch = new FileInfo (name + ".pch");
			return source.LastWriteTime.Equals (pch.LastWriteTime);
		}

		void GeneratePch (string name)
		{
			bool error = false;
			var ec = CXErrorCode.Success;
			var se = CXSaveError.CXSaveError_None;
			if (!PchIsUpToDate(name)) {
				CXTranslationUnit pch;
				// TODO
				// saving existing TU more than once crashes the native clang thread internally and therefore MD
				// might be fixed in clang 3.7.0 stable release (2015. 08. 21.) || might be intended to work this way
				Console.WriteLine ("Parsing PCH: " + (ec = clang.parseTranslationUnit2 (Index, name, new [] { "-I/usr/include/clang/3.7.0/include" }, 1, null, 0, (uint)CXTranslationUnit_Flags.ForSerialization, out pch)));
				error |= ec != CXErrorCode.Success;
				if (!error) {
					uint numDiag = clang.getNumDiagnostics (pch);
					for (uint i = 0; i < numDiag; i++) {
						CXDiagnostic diag = clang.getDiagnostic (pch, i);
						Console.WriteLine ("Clang PCH saving diagnostic: " + diag);
					}
					lock (Manager.SyncRoot)
						Console.WriteLine ("Saving PCH: " + (se = (CXSaveError)clang.saveTranslationUnit (pch, name + ".pch", clang.defaultSaveOptions (pch))));
					File.SetLastWriteTime (name + ".pch", new FileInfo (name).LastWriteTime); //Parsing takes time, we check up-to-dateness by comparing these values
					error |= se != CXSaveError.CXSaveError_None;
				}
				clang.disposeTranslationUnit (pch);
			}

			if (error)
				Gtk.Application.Invoke (
					delegate {
						IdeApp.Workbench.StatusBar.ShowError ("Generating the PCH failed. Is the header you edited valid by all means? (" + ec + "&" + se + ")");
					}
				);
		}

		void AddToIncludes (string name)
		{
			Headers.Add (name);
		}

		public void Add (string name)
		{
			if (!CProject.HeaderExtensions.Any (o => o.Equals (new FilePath (name).Extension.ToUpper ())))
				return;
			GeneratePch (name);
			AddToIncludes (name);
		}

		public void Update (string name, string [] args)
		{
			if (!CProject.HeaderExtensions.Any (o => o.Equals (new FilePath (name).Extension.ToUpper ())))
				return;
			GeneratePch (name);
		}

		public void Remove (string name)
		{
			if (Headers.Contains (name)) {
				Headers.Remove (name);
				File.Delete (name + ".pch");
			}
		}
	}
}

