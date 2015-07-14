// 
// CDocumentParser.cs
//  
// Author:
//	   Levi Bard <taktaktaktaktaktaktaktaktaktak@gmail.com>
// 
// Copyright (c) 2009 Levi Bard
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using MonoDevelop.Ide.TypeSystem;
using System.Threading;

namespace CBinding.Parser
{
	

	/// <summary>
	/// clang-based document parser helper
	/// </summary>
	public class CDocumentParser:  TypeSystemParser
	{
		
		public override System.Threading.Tasks.Task<ParsedDocument> Parse(ParseOptions options, CancellationToken cancellationToken = default(CancellationToken))
		{
			var fileName = options.FileName;
			var project = (CProject)options.Project;
			if (project == null)
				return System.Threading.Tasks.Task.FromResult ((ParsedDocument)new DefaultParsedDocument (fileName));
			var doc = new CParsedDocument (project, fileName);
			doc.Flags |= ParsedDocumentFlags.NonSerializable;
			doc.ParseAndDiagnose (cancellationToken);
			return System.Threading.Tasks.Task.FromResult ((ParsedDocument)doc);
		}
		/*
		/// <summary>
		/// Finds the end of a function's definition by matching braces.
		/// </summary>
		/// <param name="content">
		/// A <see cref="System.String"/> array: each line of the content to be searched.
		/// </param>
		/// <param name="startLine">
		/// A <see cref="System.Int32"/>: The earliest line at which the function may start.
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>: The detected end of the function.
		/// </returns>
		static int FindFunctionEnd (string[] content, int startLine) {
			int start = FindFunctionStart (content, startLine);
			if (0 > start){ return startLine; }
			
			int count = 0;
			
			for (int i= start; i<content.Length; ++i) {
				foreach (char c in content[i]) {
					switch (c) {
					case '{':
						++count;
						break;
					case '}':
						if (0 >= --count) {
							return i;
						}
						break;
					}
				}
			}
			
			return startLine;
		}
		
		/// <summary>
		/// Finds the start of a function's definition.
		/// </summary>
		/// <param name="content">
		/// A <see cref="System.String"/> array: each line of the content to be searched.
		/// </param>
		/// <param name="startLine">
		/// A <see cref="System.Int32"/>: The earliest line at which the function may start.
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>: The detected start of the function 
		/// definition, or -1.
		/// </returns>
		static int FindFunctionStart (string[] content, int startLine) {
			int semicolon = -1;
			int bracket = -1;
			
			for (int i=startLine; i<content.Length; ++i) {
				semicolon = content[i].IndexOf (';');
				bracket = content[i].IndexOf ('{');
				if (0 <= semicolon) {
					return (0 > bracket ^ semicolon < bracket)? -1: i;
				} else if (0 <= bracket) {
					return i;
				}
			}
			
			return -1;
		}
		
		static readonly Regex paramExpression = new Regex (@"(?<type>[^\s]+)\s+(?<subtype>[*&]*)(?<name>[^\s[]+)(?<array>\[.*)?", RegexOptions.Compiled);

		static object AddLanguageItem (ClangProjectSymbolDatabase db, DefaultUnresolvedTypeDefinition klass, Symbol sym, string[] contentLines)
		{
			
			if (sym is Class || sym is Struct || sym is Enumeration) {
				var type = LanguageItemToIType (db, sym, contentLines);
				klass.NestedTypes.Add (type);
				return type;
			}
			
			if (sym is Function) {
				var method = FunctionToIMethod (db, klass, (Function)sym, contentLines);
				klass.Members.Add (method);
				return method;
			}
			
			var field = LanguageItemToIField (klass, sym, contentLines);
			klass.Members.Add (field);
			return field;
		}

		/// <summary>
		/// Create an IMember from a Symbol,
		/// using the source document to locate declaration bounds.
		/// </summary>
		/// <param name="pi">
		/// A <see cref="ProjectInformation"/> for the current project.
		/// </param>
		/// <param name="item">
		/// A <see cref="Symbol"/>: The item to convert.
		/// </param>
		/// <param name="contentLines">
		/// A <see cref="System.String[]"/>: The document in which item is defined.
		/// </param>
		static DefaultUnresolvedTypeDefinition LanguageItemToIType (ClangSymbolDatabase db, Symbol sym, string[] contentLines)
		{
			var klass = new DefaultUnresolvedTypeDefinition ("", sym.File);
			if (sym is Class || sym is Struct) {
				klass.Region = new DomRegion ((int)sym.Line, 1, FindFunctionEnd (contentLines, (int)sym.Line-1) + 2, 1);
				klass.Kind = sym is Class ? TypeKind.Class : TypeKind.Struct;
				foreach (Symbol iteratorSym in db.AllItems ()) {
					if (klass.Equals (iteratorSym.Parent) && FilePath.Equals (iteratorSym.File, iteratorSym.File))
						AddLanguageItem (db, klass, iteratorSym, contentLines);
				}
				return klass;
			}
			
			klass.Region = new DomRegion ((int)sym.Line, 1, (int)sym.Line + 1, 1);
			klass.Kind = TypeKind.Enum;
			return klass;
		}
		
		static IUnresolvedField LanguageItemToIField (IUnresolvedTypeDefinition type, Symbol item, string[] contentLines)
		{
			var result = new DefaultUnresolvedField (type, item.Name);
			result.Region = new DomRegion ((int)item.Line, 1, (int)item.Line + 1, 1);
			return result;
		}
		
		static IUnresolvedMethod FunctionToIMethod (ClangProjectSymbolDatabase db, IUnresolvedTypeDefinition type, Function function, string[] contentLines)
		{
			var method = new DefaultUnresolvedMethod (type, function.Name);
			method.Region = new DomRegion ((int)function.Line, 1, FindFunctionEnd (contentLines, (int)function.Line-1)+2, 1);
			
			Match match;
			bool abort = false;
			var parameters = new List<IUnresolvedParameter> ();
			foreach (string parameter in function.Parameters) {
				match = paramExpression.Match (parameter);
				if (null == match) {
					abort = true;
					break;
				}
				var typeRef = new DefaultUnresolvedTypeDefinition (string.Format ("{0}{1}{2}", match.Groups["type"].Value, match.Groups["subtype"].Value, match.Groups["array"].Value));
				var p =  new DefaultUnresolvedParameter (typeRef, match.Groups["name"].Value);
				parameters.Add (p);
			}
			if (!abort)
				parameters.ForEach (p => method.Parameters.Add (p));
			return method;
		}*/
	}
}
