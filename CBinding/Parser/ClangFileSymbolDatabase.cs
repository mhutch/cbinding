using System;
using ClangSharp;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;

namespace CBinding.Parser
{	
	/// <summary>
	/// Symbol database of a translation unit represented by a file
	/// </summary>
	public class ClangFileSymbolDatabase
	{
		protected CProject project;
		protected string file;

		protected Dictionary<CXCursor,Namespace> namespaces = new Dictionary<CXCursor,Namespace> ();
		protected Dictionary<CXCursor,Function> functions = new Dictionary<CXCursor,Function> ();
		protected Dictionary<CXCursor,MemberFunction> memberFunctions = new Dictionary<CXCursor,MemberFunction> ();
		protected Dictionary<CXCursor,FunctionTemplate> functionTemplates = new Dictionary<CXCursor,FunctionTemplate> ();
		protected Dictionary<CXCursor,Class> classes = new Dictionary<CXCursor,Class> ();
		protected Dictionary<CXCursor,ClassTemplate> classTemplates = new Dictionary<CXCursor,ClassTemplate> ();
		protected Dictionary<CXCursor,ClassTemplatePartial> classTemplatePartials = new Dictionary<CXCursor,ClassTemplatePartial> ();
		protected Dictionary<CXCursor,Field> fields = new Dictionary<CXCursor,Field> ();
		protected Dictionary<CXCursor,Struct> structs = new Dictionary<CXCursor,Struct> ();
		protected Dictionary<CXCursor,Enumeration> enumerations = new Dictionary<CXCursor,Enumeration> ();
		protected Dictionary<CXCursor,Enumerator> enumerators = new Dictionary<CXCursor,Enumerator> ();
		protected Dictionary<CXCursor,Variable> variables = new Dictionary<CXCursor,Variable> ();
		protected Dictionary<CXCursor,Macro> macros = new Dictionary<CXCursor,Macro> ();
		protected Dictionary<CXCursor,Union> unions = new Dictionary<CXCursor,Union> ();
		protected Dictionary<CXCursor,Typedef> typedefs = new Dictionary<CXCursor,Typedef> ();
		protected Dictionary<CXCursor,Symbol> others = new Dictionary<CXCursor,Symbol> ();

		public ClangFileSymbolDatabase (CProject proj, string file)
		{
			this.file = file;
			project = proj;
		}

		/// <summary>
		/// Adds the given cursor to the database's collection which contains the symbols having the same CXCursorKind
		/// </summary>
		/// <param name="cursor">Cursor.</param>
		public void AddToDatabase (CXCursor cursor)
		{
			switch (cursor.kind) {
			case CXCursorKind.CXCursor_Namespace:
				namespaces.Add (cursor, new Namespace (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_ClassDecl:
				classes.Add (cursor, new Class (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_FieldDecl:
				fields.Add (cursor, new Field (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_ClassTemplate:
				classes.Add (cursor, new ClassTemplate (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_ClassTemplatePartialSpecialization:
				classTemplatePartials.Add (cursor, new ClassTemplatePartial (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_StructDecl:
				structs.Add (cursor, new Struct (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_FunctionDecl:
				functions.Add (cursor, new Function (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_CXXMethod:
				memberFunctions.Add (cursor, new MemberFunction (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_FunctionTemplate:
				functionTemplates.Add (cursor, new FunctionTemplate (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_EnumDecl:
				enumerations.Add (cursor, new Enumeration (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_EnumConstantDecl:
				enumerators.Add (cursor, new Enumerator (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_UnionDecl:
				unions.Add (cursor, new Union (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_TypedefDecl:
				typedefs.Add (cursor, new Typedef (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_VarDecl:
				variables.Add (cursor, new Variable (project, file, cursor));
				break;
			case CXCursorKind.CXCursor_MacroDefinition:
				macros.Add (cursor, new Macro (project, file, cursor));
				break;
			default:
				//Enabling this doesn't come with any benefits - but comes with a HUGE slowdown.
				//If something is left out of parsing add its kind to the case statement and make its own Dictionary
				//others.Add (cursor, new Symbol (project, file, cursor));
				break;
			}
		}

		public Dictionary<CXCursor, Namespace> Namespaces {
			get {
				return namespaces;
			}
		}

		public Dictionary<CXCursor, Function> Functions {
			get {
				return functions;
			}
		}

		public Dictionary<CXCursor, MemberFunction> MemberFunctions {
			get {
				return memberFunctions;
			}
		}

		public Dictionary<CXCursor, FunctionTemplate> FunctionTemplates {
			get {
				return functionTemplates;
			}
		}

		public Dictionary<CXCursor, Class> Classes {
			get {
				return classes;
			}
		}

		public Dictionary<CXCursor, Field> Fields {
			get {
				return fields;
			}
		}

		public Dictionary<CXCursor, ClassTemplate> ClassTemplates {
			get {
				return classTemplates;
			}
		}

		public Dictionary<CXCursor, ClassTemplatePartial> ClassTemplatePartials {
			get {
				return classTemplatePartials;
			}
		}

		public Dictionary<CXCursor, Struct> Structs {
			get {
				return structs;
			}
		}

		public Dictionary<CXCursor, Enumeration> Enumerations {
			get {
				return enumerations;
			}
		}

		public Dictionary<CXCursor, Enumerator> Enumerators {
			get {
				return enumerators;
			}
		}

		public Dictionary<CXCursor, Variable> Variables {
			get {
				return variables;
			}
		}

		public Dictionary<CXCursor, Macro> Macros {
			get {
				return macros;
			}
		}

		public Dictionary<CXCursor, Union> Unions {
			get {
				return unions;
			}
		}

		public Dictionary<CXCursor, Typedef> Typedefs {
			get {
				return typedefs;
			}
		}

		public Dictionary<CXCursor, Symbol> Others {
			get {
				return others;
			}
		}
	}
}