using ClangSharp;

namespace CBinding.Parser
{
	public class ClangCompletionUnit : CompletionData {
		public uint priority;
		public ClangCompletionUnit(CXCompletionResult item, string dataString, uint prio) : base(item, dataString){
			priority = prio;
		}
		public override int CompareTo (object obj)
		{
			return priority.CompareTo ((obj as ClangCompletionUnit).priority);
		}
	}
}
