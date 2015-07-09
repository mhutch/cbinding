//
// DataProvider.cs
//
// Authors:
//   Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// Copyright (C) 2007 Marcos David Marin Amador
//
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;

 
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.CodeCompletion;

using MonoDevelop.Core;
using MonoDevelop.Ide.Editor;
using ClangSharp;
using MonoDevelop.Ide.Gui.Pads.ClassBrowser;
using ICSharpCode.NRefactory.CSharp;
using System.IO;

namespace CBinding.Parser
{
	public class ParameterDataProvider : MonoDevelop.Ide.CodeCompletion.ParameterHintingResult
	{
		private TextEditor editor;

		public ParameterDataProvider (int startOffset, TextEditor editor, List<Function> functions, string functionName) :base (startOffset)
		{
			this.editor = editor;

			foreach (Function f in functions) {
				if (f.Spelling.Equals (functionName)) {
					data.Add (new DataWrapper (f));
				}
			}
		}
		
		// Returns the markup to use to represent the specified method overload
		// in the parameter information window.
		public string GetHeading (int overload, string[] parameterMarkup, int currentParameter)
		{
			Function function = ((DataWrapper)this[overload]).Function;
			string paramTxt = string.Join (", ", parameterMarkup);
			
			int len = function.Signature.LastIndexOf ("::");
			string prename = null;
			
			if (len > 0)
				prename = GLib.Markup.EscapeText (function.Signature.Substring (0, len + 2));
			
			string cons = string.Empty;
			
			if (function.IsConst)
				cons = " const";
			
			return prename + "<b>" + function.Signature + "</b>" + " (" + paramTxt + ")" + cons;
		}
		
		public string GetDescription (int overload, int currentParameter)
		{
			return "";
		}
		
		// Returns the text to use to represent the specified parameter
		public string GetParameterDescription (int overload, int paramIndex)
		{
			Function function = ((DataWrapper)this[overload]).Function;
			
			return GLib.Markup.EscapeText (function.Parameters[paramIndex]);
		}
	}
	
	public class CompletionData : MonoDevelop.Ide.CodeCompletion.CompletionData
	{
		private IconId image;
		private string text;
		private string description;
		private string completion_string;
		
		public CompletionData (CXCompletionResult item, string dataString){
			if (item.CursorKind == CXCursorKind.CXCursor_ClassDecl) {
				image = Stock.Class;
				this.CompletionCategory = new ClangCompletionCategory (ClangCompletionCategory.classCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_ClassTemplate) {
				image = Stock.Class;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.classTemplateCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_ClassTemplatePartialSpecialization) {
				image = Stock.Class;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.classTemplatePartialCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_StructDecl) {
				image = Stock.Struct;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.structCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_UnionDecl) {
				image = "md-union";
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.unionCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_EnumDecl) {
				image = Stock.Enum;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.enumerationCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_EnumConstantDecl) {
				image = Stock.Literal;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.enumeratorCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_FunctionDecl) {
				image = Stock.Method;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.functionCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_FunctionTemplate) {
				image = Stock.Method;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.functionTemplateCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_Namespace) {
				image = Stock.NameSpace;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.namespaceCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_TypedefDecl) {
				image = Stock.Interface;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.typedefCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_CXXMethod) {
				image = Stock.Field;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.methodCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_FieldDecl) {
				image = Stock.Field;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.fieldCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_VarDecl) {
				image = Stock.Field;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.variablesCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_MacroDefinition) {
				image = Stock.Literal;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.macroCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_ParmDecl) {
				image = Stock.Field;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.parameterCategory);
}			else {
				image = Stock.Literal;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.otherCategory);
}
			this.text = dataString;
			this.completion_string = dataString;
			this.description = string.Empty;
		}

		public CompletionData (Symbol item)
		{
			if (item is Class) {
				image = Stock.Class;
				this.CompletionCategory = new ClangCompletionCategory (ClangCompletionCategory.classCategory);
}			else if (item is ClassTemplate) {
				image = Stock.Class;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.classTemplateCategory);
}			else if (item is ClassTemplatePartial) {
				image = Stock.Class;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.classTemplatePartialCategory);
}			else if (item is Struct) {
				image = Stock.Struct;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.structCategory);
}			else if (item is Union) {
				image = "md-union";
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.unionCategory);
}			else if (item is Enumeration) {
				image = Stock.Enum;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.enumerationCategory);
}			else if (item is Enumerator) {
				image = Stock.Literal;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.enumeratorCategory);
}			else if (item is Function || item is FunctionTemplate) {
				image = Stock.Method;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.functionTemplateCategory);
}			else if (item is Namespace) {
				image = Stock.NameSpace;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.namespaceCategory);
}			else if (item is Typedef) {
				image = Stock.Interface;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.typedefCategory);
}			else if (item is MemberFunction) {
				image = Stock.Field;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.methodCategory);
}			else if (item is Variable) {
				image = Stock.Field;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.variablesCategory);
}			else if (item is Field) {
				image = Stock.Field;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.fieldCategory);
}			else if (item is Macro) {
				image = Stock.Literal;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.macroCategory);
}			else {
				image = Stock.Literal;
				this.CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.otherCategory);
}			
			this.text = item.Signature;
			this.completion_string = item.Signature;
			this.description = string.Empty;
		}
		
		public override IconId Icon {
			get { return image; }
		}
		
		public override string DisplayText {
			get { return text; }
		}
		
		public override string Description {
			get { return description; }
		}
		
		public override string CompletionText {
			get { return completion_string; }
		}
	}
}
