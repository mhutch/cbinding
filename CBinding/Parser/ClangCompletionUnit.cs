namespace CBinding
{
	public class ClangCompletionUnit {
		public uint priority;
		public MonoDevelop.Ide.CodeCompletion.CompletionData data;
		public ClangCompletionUnit(uint prio, MonoDevelop.Ide.CodeCompletion.CompletionData dat){
			priority = prio;
			data = dat;
		}
	}
	
}
