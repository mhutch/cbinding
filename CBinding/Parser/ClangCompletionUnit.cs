using ClangSharp;

namespace CBinding.Parser
{
	/// <summary>
	/// Code completion suggestions wrapper which contains clang-given priority too and compares by that priority
	/// </summary>
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
