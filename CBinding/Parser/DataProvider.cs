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

using System.Collections.Generic;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Core;
using ClangSharp;

namespace CBinding.Parser
{
	public class ParameterDataProvider : MonoDevelop.Ide.CodeCompletion.ParameterHintingResult
	{
		List<OverloadCandidate> ParameterInformation { get; }

		public ParameterDataProvider (int startOffset, List<OverloadCandidate> parameterInformation) :base (startOffset)
		{
			ParameterInformation = parameterInformation;
			foreach (var pi in ParameterInformation) {
				data.Add (new DataWrapper (pi));
			}
		}
	}
	
	public class CompletionData : MonoDevelop.Ide.CodeCompletion.CompletionData
	{
		IconId image;
		string text;
		string description;
		string completion_string;
		
		public CompletionData (CXCompletionResult item, string dataString){
			if (item.CursorKind == CXCursorKind.CXCursor_ClassDecl) {
				image = Stock.Class;
				CompletionCategory = new ClangCompletionCategory (ClangCompletionCategory.classCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_ClassTemplate) {
				image = Stock.Class;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.classTemplateCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_ClassTemplatePartialSpecialization) {
				image = Stock.Class;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.classTemplatePartialCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_StructDecl) {
				image = Stock.Struct;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.structCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_UnionDecl) {
				image = "md-union";
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.unionCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_EnumDecl) {
				image = Stock.Enum;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.enumerationCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_EnumConstantDecl) {
				image = Stock.Literal;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.enumeratorCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_FunctionDecl) {
				image = Stock.Method;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.functionCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_FunctionTemplate) {
				image = Stock.Method;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.functionTemplateCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_Namespace) {
				image = Stock.NameSpace;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.namespaceCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_TypedefDecl) {
				image = Stock.Interface;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.typedefCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_CXXMethod) {
				image = Stock.Field;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.methodCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_FieldDecl) {
				image = Stock.Field;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.fieldCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_VarDecl) {
				image = Stock.Field;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.variablesCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_MacroDefinition) {
				image = Stock.Literal;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.macroCategory);
}			else if (item.CursorKind == CXCursorKind.CXCursor_ParmDecl) {
				image = Stock.Field;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.parameterCategory);
}			else {
				image = Stock.Literal;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.otherCategory);
}
			text = dataString;
			completion_string = dataString;
			description = string.Empty;
		}

		public CompletionData (Symbol item)
		{
			if (item is Class) {
				image = Stock.Class;
				CompletionCategory = new ClangCompletionCategory (ClangCompletionCategory.classCategory);
}			else if (item is ClassTemplate) {
				image = Stock.Class;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.classTemplateCategory);
}			else if (item is ClassTemplatePartial) {
				image = Stock.Class;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.classTemplatePartialCategory);
}			else if (item is Struct) {
				image = Stock.Struct;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.structCategory);
}			else if (item is Union) {
				image = "md-union";
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.unionCategory);
}			else if (item is Enumeration) {
				image = Stock.Enum;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.enumerationCategory);
}			else if (item is Enumerator) {
				image = Stock.Literal;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.enumeratorCategory);
}			else if (item is Function || item is FunctionTemplate) {
				image = Stock.Method;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.functionTemplateCategory);
}			else if (item is Namespace) {
				image = Stock.NameSpace;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.namespaceCategory);
}			else if (item is Typedef) {
				image = Stock.Interface;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.typedefCategory);
}			else if (item is MemberFunction) {
				image = Stock.Field;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.methodCategory);
}			else if (item is Variable) {
				image = Stock.Field;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.variablesCategory);
}			else if (item is Field) {
				image = Stock.Field;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.fieldCategory);
}			else if (item is Macro) {
				image = Stock.Literal;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.macroCategory);
}			else {
				image = Stock.Literal;
				CompletionCategory = new ClangCompletionCategory(ClangCompletionCategory.otherCategory);
}			
			text = item.Signature;
			completion_string = item.Signature;
			description = string.Empty;
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
