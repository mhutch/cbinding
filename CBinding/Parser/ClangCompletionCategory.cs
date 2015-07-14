namespace CBinding.Parser
{
	/// <summary>
	/// Completion categories.
	/// </summary>
	public class ClangCompletionCategory : MonoDevelop.Ide.CodeCompletion.CompletionCategory
	{
		public static string FunctionCategory = "Function";
		public static string NamespaceCategory = "Namespace";
		public static string FunctionTemplateCategory = "Function template";
		public static string MethodCategory = "Class method";
		public static string ClassCategory = "Class";
		public static string ClassTemplateCategory = "Class template";
		public static string ClassTemplatePartialCategory = "Class template partial specialization";
		public static string FieldCategory = "Field";
		public static string StructCategory = "Struct";
		public static string EnumerationCategory = "Enumeration";
		public static string EnumeratorCategory = "Enumerator";
		public static string UnionCategory = "Union";
		public static string TypedefCategory = "Typedef";
		public static string VariablesCategory = "Variable";
		public static string ParameterCategory = "Parameter";
		public static string MacroCategory = "Macro";
		public static string OtherCategory = "Other";


		public ClangCompletionCategory ()
		{
		}

		public ClangCompletionCategory (string categ) : base(categ, null)
		{
		}

		#region implemented abstract members of CompletionCategory
		public override int CompareTo (MonoDevelop.Ide.CodeCompletion.CompletionCategory other)
		{
			return DisplayText.CompareTo(other.DisplayText);
		}
		#endregion
	}
	
}
