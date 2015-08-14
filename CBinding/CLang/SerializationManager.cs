using System;
using System.Collections.Generic;
using ClangSharp;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using System.Linq;
using System.IO;

namespace CBinding
{
	public class SerializationManager
	{
		CLangManager Manager { get; }
		CProject project { get; }
		CXIndex Index { get; }
		public List<string> Headers { get; } = new List<string> ();

		public SerializationManager (CProject proj, CLangManager man, CXIndex ind)
		{
			Manager = man;
			project = proj;
			Index = ind;
		}

		public static bool SerFormIsUpToDate (string name)
		{
			var source = new FileInfo (name);
			if (!File.Exists (SerializedName (name)))
				return false;
			var pch = new FileInfo (SerializedName (name));
			return source.LastWriteTime.Equals (pch.LastWriteTime);
		}

		public static string SerializedName (string name)
		{
			return CProject.HeaderExtensions.Any (o => o.Equals (new FilePath (name).Extension.ToUpper ())) ?
			name + ".pch" 
				: 
			name + ".ser";
		}

		void GenerateSerialized (string name, string [] args)
		{
			bool error = false;
			var ec = CXErrorCode.Success;
			var se = CXSaveError.CXSaveError_None;
			string fname = SerializedName (name);
			if (!SerFormIsUpToDate(name)) {
				CXTranslationUnit serialized;
				// TODO
				// saving existing TU more than once crashes the native clang thread internally and therefore MD
				// might be fixed in clang 3.7.0 stable release (2015. 08. 21.) || might be intended to work this way
				Console.WriteLine ("Parsing: " + (ec = clang.parseTranslationUnit2 (Index, name, args, args.Length, null, 0, (uint)CXTranslationUnit_Flags.ForSerialization, out serialized)));
				error |= ec != CXErrorCode.Success;
				if (!error) {
					Console.WriteLine ("Saving: " + (se = (CXSaveError)clang.saveTranslationUnit (serialized, fname, clang.defaultSaveOptions (serialized))));
					File.SetLastWriteTime (fname, new FileInfo (name).LastWriteTime); //Parsing takes time, we check up-to-dateness by comparing these values
					error |= se != CXSaveError.CXSaveError_None;
				}
				clang.disposeTranslationUnit (serialized);
			}

			if (error) {
				File.Delete (fname);
				Gtk.Application.Invoke (
					delegate {
						IdeApp.Workbench.StatusBar.ShowError ("Generating the PCH failed. Is the header you edited valid by all means? (" + ec + "&" + se + ")");
					}
				);
			}
		}

		void AddToIncludes (string name)
		{
			if (!CProject.HeaderExtensions.Any (o => o.Equals (new FilePath (name).Extension.ToUpper ())))
				return;
			Headers.Add (name);
		}

		public void Add (string name, string [] args)
		{
			GenerateSerialized (name, args);
			AddToIncludes (name);
		}

		public void Update (string name, string [] args)
		{
			GenerateSerialized (name, args);
		}

		public void Remove (string name)
		{
			if (Headers.Contains (name))
				Headers.Remove (name);
			if (File.Exists (SerializedName (name)))
				File.Delete (SerializedName(name));
			
		}

		public void Rename (string oldName, string newName, string [] args)
		{
			Remove (oldName);
			Add (newName, args);
		}
	}
}

