using System;
using ClangSharp;
using ICSharpCode.NRefactory6.CSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;

namespace CBinding
{	
	public class ClangProjectSymbolDatabase{
		protected CProject project;
		protected Dictionary<string, ClangFileSymbolDatabase> db;

		public ClangProjectSymbolDatabase(CProject proj) {
			project = proj;
			db = new Dictionary<string, ClangFileSymbolDatabase> ();
		}

		public void AddToDatabase (string file, CXCursor cursor)
		{
			if (!db.ContainsKey (file))
				db.Add (file, new ClangFileSymbolDatabase(file));
			db [file].AddToDatabase (cursor);
		}

		public void Reset(string file){
			if (db.ContainsKey (file))
				db [file] = new ClangFileSymbolDatabase (file);
			else
				db.Add (file, new ClangFileSymbolDatabase (file));
		}

		public List<Namespace> Namespaces {
			get {
				List<Namespace> ret = new List<Namespace>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Namespaces);
				return ret;
			}
		}

		public List<Function> Functions {
			get {
				List<Function> ret = new List<Function>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Functions);
				return ret;			}
		}

		public List<MemberFunction> MemberFunctions {
			get {
				List<MemberFunction> ret = new List<MemberFunction>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.MemberFunctions);
				return ret;			}
		}

		public List<FunctionTemplate> FunctionTemplates {
			get {
				List<FunctionTemplate> ret = new List<FunctionTemplate>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.FunctionTemplates);
				return ret;			}
		}

		public List<Class> Classes {
			get {
				List<Class> ret = new List<Class>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Classes);
				return ret;			}
		}

		public List<Struct> Structs {
			get {
				List<Struct> ret = new List<Struct>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Structs);
				return ret;			}
		}

		public List<Enumeration> Enumerations {
			get {
				List<Enumeration> ret = new List<Enumeration>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Enumerations);
				return ret;			}
		}

		public List<Enumerator> Enumerators {
			get {
				List<Enumerator> ret = new List<Enumerator>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Enumerators);
				return ret;			}
		}

		public List<Variable> Variables {
			get {
				List<Variable> ret = new List<Variable>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Variables);
				return ret;			}
		}

		public List<Macro> Macros {
			get {
				List<Macro> ret = new List<Macro>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Macros);
				return ret;			}
		}

		public List<Union> Unions {
			get {
				List<Union> ret = new List<Union>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Unions);
				return ret;			}
		}

		public List<Typedef> Typedefs {
			get {
				List<Typedef> ret = new List<Typedef>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Typedefs);
				return ret;			}
		}

		public List<Symbol> Others {
			get {
				List<Symbol> ret = new List<Symbol>();
				foreach (var iter in db)
					ret.AddRange (iter.Value.Others);
				return ret;			}
		}
	}	
}