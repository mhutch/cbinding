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
		protected Dictionary<CXCursor,Symbol> globals = new Dictionary<CXCursor,Symbol> ();
		protected Dictionary<CXCursor,Symbol> canBeInClasses = new Dictionary<CXCursor,Symbol> ();
		protected Dictionary<CXCursor,Symbol> canBeInNamespaces = new Dictionary<CXCursor,Symbol> ();

		public ClangFileSymbolDatabase (CProject proj, string file)
		{
			this.file = file;
			project = proj;
		}

		/// <summary>
		/// Adds the given cursor to the database's collection which contains the symbols having the same CXCursorKind
		/// </summary>
		/// <param name="cursor">Cursor.</param>
		public void AddToDatabase (CXCursor cursor, bool global)
		{
			switch (cursor.kind) {
			case CXCursorKind.Namespace:
				Namespace n = new Namespace (project, file, cursor, global);
				namespaces.Add (cursor, n);
				canBeInClasses.Add (cursor, n);
				break;
			case CXCursorKind.ClassDecl:
				Class c = new Class (project, file, cursor, global);
				classes.Add (cursor, c);
				canBeInClasses.Add (cursor, c);
				canBeInNamespaces.Add (cursor,c);
				if(global)
					globals.Add (cursor, c);
				break;
			case CXCursorKind.FieldDecl:
				Field f = new Field (project, file, cursor, global);
				canBeInClasses.Add (cursor, f);
				fields.Add (cursor, f);
				break;
			case CXCursorKind.ClassTemplate:
				ClassTemplate ct = new ClassTemplate (project, file, cursor, global);
				classes.Add (cursor, ct);
				canBeInClasses.Add (cursor, ct);
				canBeInNamespaces.Add (cursor,ct);
				if(global)
					globals.Add (cursor, ct);
				break;
			case CXCursorKind.ClassTemplatePartialSpecialization:
				ClassTemplatePartial ctp = new ClassTemplatePartial (project, file, cursor, global);
				classTemplatePartials.Add (cursor, ctp);
				canBeInClasses.Add (cursor, ctp);
				canBeInNamespaces.Add (cursor,ctp);
				if(global)
					globals.Add (cursor, ctp);
				break;
			case CXCursorKind.StructDecl:
				Struct s = new Struct (project, file, cursor, global);
				structs.Add (cursor, s);
				canBeInClasses.Add (cursor, s);
				canBeInNamespaces.Add (cursor,s);
				if(global)
					globals.Add (cursor, s);
				break;
			case CXCursorKind.FunctionDecl:
				Function func = new Function (project, file, cursor, global);
				functions.Add (cursor, func);
				canBeInClasses.Add (cursor, func);
				canBeInNamespaces.Add (cursor,func);
				if(global)
					globals.Add (cursor, func);
				break;
			case CXCursorKind.CXXMethod:
				MemberFunction m = new MemberFunction (project, file, cursor, global);
				memberFunctions.Add (cursor, m);
				canBeInClasses.Add (cursor, m);
				break;
			case CXCursorKind.FunctionTemplate:
				FunctionTemplate ft = new FunctionTemplate (project, file, cursor, global);
				functionTemplates.Add (cursor, ft);
				canBeInClasses.Add (cursor, ft);
				canBeInNamespaces.Add (cursor,ft);
				if(global)
					globals.Add (cursor, ft);
				break;
			case CXCursorKind.EnumDecl:
				Enumeration en = new Enumeration (project, file, cursor, global);
				enumerations.Add (cursor, en);
				canBeInClasses.Add (cursor, en);
				canBeInNamespaces.Add (cursor,en);
				if(global)
					globals.Add (cursor, en);
				break;
			case CXCursorKind.EnumConstantDecl:
				enumerators.Add (cursor, new Enumerator (project, file, cursor, global));
				break;
			case CXCursorKind.UnionDecl:
				Union u = new Union (project, file, cursor, global);
				unions.Add (cursor, u);
				canBeInClasses.Add (cursor, u);
				canBeInNamespaces.Add (cursor,u);
				if(global)
					globals.Add (cursor, u);
				break;
			case CXCursorKind.TypedefDecl:
				Typedef t = new Typedef (project, file, cursor, global);
				typedefs.Add (cursor, t);
				canBeInClasses.Add (cursor, t);
				canBeInNamespaces.Add (cursor,t);
				if(global)
					globals.Add (cursor, t);
				break;
			case CXCursorKind.VarDecl:
				Variable v = new Variable (project, file, cursor, global);
				variables.Add (cursor, v);
				canBeInClasses.Add (cursor, v);
				canBeInNamespaces.Add (cursor,v);
				if(global)
					globals.Add (cursor, v);
				break;
			case CXCursorKind.MacroDefinition:
				macros.Add (cursor, new Macro (project, file, cursor, global));
				break;
			default:
				//Enabling this doesn't come with any benefits - but comes with a HUGE slowdown.
				//If something is left out of parsing add its kind to the case statement and make its own Dictionary
				//others.Add (cursor, new Symbol (project, file, cursor, global));
				break;
			}
		}
			
		public Dictionary<CXCursor, Namespace> Namespaces {
			get { return namespaces; }
		}

		public Dictionary<CXCursor, Function> Functions {
			get { return functions; }
		}

		public Dictionary<CXCursor, MemberFunction> MemberFunctions {
			get { return memberFunctions; }
		}

		public Dictionary<CXCursor, FunctionTemplate> FunctionTemplates {
			get { return functionTemplates; }
		}

		public Dictionary<CXCursor, Class> Classes {
			get { return classes; }
		}

		public Dictionary<CXCursor, Field> Fields {
			get { return fields; }
		}

		public Dictionary<CXCursor, ClassTemplate> ClassTemplates {
			get { return classTemplates; }
		}

		public Dictionary<CXCursor, ClassTemplatePartial> ClassTemplatePartials {
			get { return classTemplatePartials; }
		}

		public Dictionary<CXCursor, Struct> Structs {
			get { return structs; }
		}

		public Dictionary<CXCursor, Enumeration> Enumerations {
			get { return enumerations; }
		}

		public Dictionary<CXCursor, Enumerator> Enumerators {
			get { return enumerators; }
		}

		public Dictionary<CXCursor, Variable> Variables {
			get { return variables; }
		}

		public Dictionary<CXCursor, Macro> Macros {
			get { return macros; }
		}

		public Dictionary<CXCursor, Union> Unions {
			get { return unions; }
		}

		public Dictionary<CXCursor, Typedef> Typedefs {
			get { return typedefs; }
		}

		public Dictionary<CXCursor, Symbol> Others {
			get { return others; }
		}

		public Dictionary<CXCursor, Symbol> Globals {
			get { return globals; }
		}
			
		public Dictionary<CXCursor, Symbol> CanBeInClasses { 
			get { return canBeInClasses; } 
		}

		/// <summary>
		/// Nested namespaces are handled in the node builder!
		/// </summary>
		public Dictionary<CXCursor, Symbol> CanBeInNamespaces { 
			get { return canBeInNamespaces; } 
		}
	}
}