using System;
using ClangSharp;
using System.Collections.Generic;

namespace CBinding.Parser
{
	public class Function : Symbol
	{
		public int ParameterCount { get; }

		public string [] Parameters { get ; }

		public string Returns { get; }

		public Function (CProject proj, string fileN, CXCursor cursor, bool global) : base (proj, fileN, cursor, global)
		{
			//this method is to workaround template function differences, template and template template parameters
			//ugly as hell but what do? :(
			string paramlist = (Signature.Substring (Signature.IndexOf ("(") + 1 , Signature.LastIndexOf (")") - (Signature.IndexOf ("(") + 1)));
			int inTemplateSpecifier = 0;
			int inParentesis = 0;
			int inBracket = 0;
			int inCurvyBracket = 0;
			int lastParamStart = 0;
			List<string> paramBuilder = new List<string> ();
			var array = paramlist.ToCharArray ();

			for(int i = 0; i < paramlist.Length; ++i){
				switch (array[i])
				{
				case '<':
					++inTemplateSpecifier;
					break;
				case '>':
					--inTemplateSpecifier;
					break;
				case '(':
					++inParentesis;
					break;
				case ')':
					--inParentesis;
					break;
				case '[':
					++inBracket;
					break;
				case ']':
					--inBracket;
					break;
				case '{':
					++inCurvyBracket;
					break;
				case '}':
					--inCurvyBracket;
					break;
				case ',':
					if (inTemplateSpecifier == 0 &&
						inParentesis == 0 &&
						inBracket == 0 &&
						inCurvyBracket == 0) {
						paramBuilder.Add (paramlist.Substring (lastParamStart, i - lastParamStart));
						lastParamStart = i + 2;
					}
					break;
				}
				if(i == paramlist.Length -1)
					paramBuilder.Add (paramlist.Substring (lastParamStart, i + 1 - lastParamStart));
			}

			Parameters = paramBuilder.ToArray ();
			ParameterCount = Parameters.Length;
			Returns = clang.getTypeSpelling (clang.getResultType(clang.getCursorType (Represented))).ToString ();
		}
			
	}

}
