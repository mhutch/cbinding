namespace CBinding
{

	public class ClangCompletionCategory : MonoDevelop.Ide.CodeCompletion.CompletionCategory
	{
		public ClangCompletionCategory(){
			
		}

		public ClangCompletionCategory(string categ) 
			: base(categ, null)
		{
		}

		#region implemented abstract members of CompletionCategory
		public override int CompareTo (MonoDevelop.Ide.CodeCompletion.CompletionCategory other)
		{
			return 0;
		}
		#endregion
	}
	
}
