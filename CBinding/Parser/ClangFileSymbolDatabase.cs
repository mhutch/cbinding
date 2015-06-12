using System;
using ClangSharp;
using ICSharpCode.NRefactory6.CSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;

namespace CBinding
{	
	public class ClangFileSymbolDatabase
	{

		protected string file;

		protected List<Namespace> namespaces = new List<Namespace> ();
		protected List<Function> functions = new List<Function> ();
		protected List<MemberFunction> memberFunctions = new List<MemberFunction> ();
		protected List<FunctionTemplate> functionTemplates = new List<FunctionTemplate> ();
		protected List<Class> classes = new List<Class> ();
		protected List<Struct> structs = new List<Struct> ();
		protected List<Enumeration> enumerations = new List<Enumeration> ();
		protected List<Enumerator> enumerators = new List<Enumerator> ();
		protected List<Variable> variables = new List<Variable> ();
		protected List<Macro> macros = new List<Macro> ();
		protected List<Union> unions = new List<Union> ();
		protected List<Typedef> typedefs = new List<Typedef> ();
		protected List<Symbol> others = new List<Symbol> ();

		public ClangFileSymbolDatabase (string file)
		{
			this.file = file;
		}

		public void AddToDatabase (CXCursor cursor)
		{
			switch (cursor.kind) {
			case CXCursorKind.CXCursor_Namespace:
				namespaces.Add (new Namespace (cursor));
				break;
			case CXCursorKind.CXCursor_ClassDecl:
				classes.Add (new Class (cursor));
				break;
			case CXCursorKind.CXCursor_StructDecl:
				structs.Add (new Struct (cursor));
				break;
			case CXCursorKind.CXCursor_FunctionDecl:
				functions.Add (new Function (cursor));
				break;
			case CXCursorKind.CXCursor_CXXMethod:
				memberFunctions.Add (new MemberFunction (cursor));
				break;
			case CXCursorKind.CXCursor_FunctionTemplate:
				functionTemplates.Add (new FunctionTemplate (cursor));
				break;
			case CXCursorKind.CXCursor_EnumDecl:
				enumerations.Add (new Enumeration (cursor));
				break;
			case CXCursorKind.CXCursor_EnumConstantDecl:
				enumerators.Add (new Enumerator (cursor));
				break;
			case CXCursorKind.CXCursor_UnionDecl:
				unions.Add (new Union (cursor));
				break;
			case CXCursorKind.CXCursor_TypedefDecl:
				typedefs.Add (new Typedef (cursor));
				break;
			case CXCursorKind.CXCursor_VarDecl:
				variables.Add (new Variable (cursor));
				break;
			case CXCursorKind.CXCursor_MacroDefinition:
				macros.Add (new Macro (cursor));
				break;
			default:
				others.Add (new Symbol (cursor));
				break;
			}
		}

		public List<Namespace> Namespaces {
			get {
				return namespaces;
			}
		}

		public List<Function> Functions {
			get {
				return functions;
			}
		}

		public List<MemberFunction> MemberFunctions {
			get {
				return memberFunctions;
			}
		}

		public List<FunctionTemplate> FunctionTemplates {
			get {
				return functionTemplates;
			}
		}

		public List<Class> Classes {
			get {
				return classes;
			}
		}

		public List<Struct> Structs {
			get {
				return structs;
			}
		}

		public List<Enumeration> Enumerations {
			get {
				return enumerations;
			}
		}

		public List<Enumerator> Enumerators {
			get {
				return enumerators;
			}
		}

		public List<Variable> Variables {
			get {
				return variables;
			}
		}

		public List<Macro> Macros {
			get {
				return macros;
			}
		}

		public List<Union> Unions {
			get {
				return unions;
			}
		}

		public List<Typedef> Typedefs {
			get {
				return typedefs;
			}
		}

		public List<Symbol> Others {
			get {
				return others;
			}
		}
	}
}