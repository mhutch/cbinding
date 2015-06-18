namespace CBinding.Parser
{

	public class ClangCompletionCategory : MonoDevelop.Ide.CodeCompletion.CompletionCategory
	{
		public static string functionCategory = "Function";
		public static string namespaceCategory = "Namespace";
		public static string functionTemplateCategory = "Function template";
		public static string methodCategory = "Class method";
		public static string classCategory = "Class";
		public static string classTemplateCategory = "Class template";
		public static string classTemplatePartialCategory = "Class template partial specialization";
		public static string fieldCategory = "Field";
		public static string structCategory = "Struct";
		public static string enumerationCategory = "Enumeration";
		public static string enumeratorCategory = "Enumerator";
		public static string unionCategory = "Union";
		public static string typedefCategory = "Typedef";
		public static string variablesCategory = "Variable";
		public static string parameterCategory = "Parameter";
		public static string macroCategory = "Macro";
		public static string otherCategory = "Other";


		public ClangCompletionCategory(){
			
		}

		public ClangCompletionCategory(string categ) 
			: base(categ, null)
		{
		}

		#region implemented abstract members of CompletionCategory
		public override int CompareTo (MonoDevelop.Ide.CodeCompletion.CompletionCategory other)
		{
			return this.DisplayText.CompareTo(other.DisplayText);
		}
		#endregion
	}
	
}
