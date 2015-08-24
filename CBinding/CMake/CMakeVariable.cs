using System;
using System.Collections.Generic;

namespace CBinding
{
	public class CMakeVariable
	{
		public bool IsEditable {
			get { return isEditable; }
			set { isEditable = value; }
		}
		public string Value {
			get { return value; }
			set { this.value = value; }
		}
		public List<CMakeCommand> Commands {
			get { return commands; }
		}

		readonly string key;
		string value;
		bool isEditable;
		readonly List<CMakeCommand> commands = new List<CMakeCommand> ();

		public CMakeVariable (string variableName, string value, bool isEditable = true, CMakeCommand command = null)
		{
			key = variableName;
			this.value = value;
			this.isEditable = isEditable;
			commands.Add (command);
		}
	}
}
