using System;

namespace CBinding.Parser
{
	public class SourceLocation
	{
		public string FileName { get; }

		public int Line { get; }

		public int Column { get; }

		public int Offset { get; }

		public SourceLocation(string fileName, uint line, uint column, uint offset) {
			FileName = fileName;
			Line = Convert.ToInt32(line);
			Column = Convert.ToInt32(column);
			Offset = Convert.ToInt32(offset);
		}
	}
}
