namespace CBinding
{
	public class SourceLocation
	{
		string fileName;
		uint line;
		uint column;
		uint offset;

		public SourceLocation(string fileName, uint line, uint column, uint offset) {
			this.fileName = fileName;
			this.line = line;
			this.column = column;
			this.offset = offset;
		}

		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}

		public uint Line {
			get {
				return line;
			}
			set {
				line = value;
			}
		}

		public uint Column {
			get {
				return column;
			}
			set {
				column = value;
			}
		}

		public uint Offset {
			get {
				return offset;
			}
		}
	}

}
