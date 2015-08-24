using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using MonoDevelop.Core;

namespace CBinding
{
	public class CMakeVariableManager
	{
		readonly CMakeFileFormat parent;
		readonly Dictionary<string, CMakeVariable> variables = new Dictionary<string, CMakeVariable> ();
		static readonly List<string> toIgnore = new List<string> () {
			"filepath", "path", "string", "bool", "internal", "cache", "force", "parent_scope"
		};
		static readonly Regex variableRegex = new Regex (@"\$\{.*?\}", RegexOptions.Singleline);
		static readonly Regex fullVariableRegex = new Regex (@"^\$\{.*?\}$", RegexOptions.Singleline);
		static readonly Regex unquotedElement = new Regex (@"^[^\s\(\)\#\""\\]+$");

		public bool Contains (string variableName)
		{
			return !string.IsNullOrEmpty (variableName) && variables.ContainsKey (variableName);
		}

		public bool IsValidUnquotedElement (string text)
		{
			return unquotedElement.IsMatch (text);
		}

		public bool IsVariable (string text)
		{
			return fullVariableRegex.IsMatch (text);
		}

		public void TraverseVariables (Action<string, CMakeCommand> callback, HashSet<CMakeCommand> visited = null)
		{
			if (visited == null)
				visited = new HashSet<CMakeCommand> ();

			foreach (var command in parent.SetCommands) {

			}
		}

		public CMakeVariable GetVariable (string variableName)
		{
			if (Contains (variableName))
				return variables [variableName];

			LoggingService.LogDebug ("Undefined variable: {0}", variableName);
			return null;
		}

		public string GetStringValueOf (string variableName)
		{
			if (Contains (variableName)) {
				return variables [variableName].Value;
			}
			LoggingService.LogDebug ("Undefined variable: {0}", variableName);
			return string.Empty;
		}

		public List<string> GetListValueOf (string variableName)
		{
			if (!Contains (variableName))
				return new List<string> ();

			List<CMakeArgument> args = CMakeArgument.ArgumentsFromString (GetStringValueOf (variableName));

			var result = new List<string> ();
			foreach (CMakeArgument arg in args) {
				result = result.Concat (arg.GetValues ()).ToList ();
			}

			return result;
		}

		public string ResolveString (string value)
		{
			foreach (Match match in variableRegex.Matches (value)) {
				string variable = match.Value.Substring (2, match.Value.Length - 3);
				value = value.Replace (match.Value, GetStringValueOf (variable));
			}
			return value;
		}

		public void Set (string variableName, string value, bool isEditable = true, CMakeCommand command = null)
		{
			variableName = ResolveString (variableName);
			value = ResolveString (value);

			if (Contains (variableName)) {
				CMakeVariable v = variables [variableName];
				v.Value = value;
				v.IsEditable = false;
				v.Commands.Add (command);
				return;
			}

			AddVariable (variableName, value, isEditable, command);
		}

		void AddVariable (string variableName, string value, bool isEditable = true, CMakeCommand command = null)
		{
			variables.Add (variableName, new CMakeVariable (variableName, value, isEditable, command));
		}

		void PopulateVariables ()
		{

			// Initialize important cache/default variables.
			AddVariable ("CMAKE_SOURCE_DIR", new FilePath (parent.Project.BaseDirectory), false);
			AddVariable ("CMAKE_CURRENT_SOURCE_DIR", new FilePath (parent.File.ParentDirectory), false);

			if (string.IsNullOrEmpty (parent.ProjectName) && parent.Parent != null) {
				AddVariable ("PROJECT_SOURCE_DIR", new FilePath (parent.Parent.File.ParentDirectory), false);
				AddVariable (string.Format ("{0}{1}", parent.ProjectName, "_SOURCE_DIR"),
							 new FilePath (parent.File.ParentDirectory), false);
			} else {
				AddVariable (string.Format ("{0}{1}", parent.ProjectName, "_SOURCE_DIR"),
							 new FilePath (parent.File.ParentDirectory), false);
				AddVariable ("PROJECT_SOURCE_DIR", new FilePath (parent.File.ParentDirectory), false);
			}

			if (!Contains (string.Format ("{0}{1}", parent.Project.Name, "_SOURCE_DIR")))
				AddVariable (string.Format ("{0}{1}", parent.Project.Name, "_SOURCE_DIR"),
							 new FilePath (parent.Project.BaseDirectory), false);

			// Iterate over `set` commands and add variables.
			var commands = parent.SetCommands;
			foreach (var command in commands.Values) {
				if (command.Arguments.Count == 0)
					continue;

				bool isEditable = !command.IsNested;

				if (command.Arguments.Count == 1) {
					Set (command.Arguments [0].ToString (), "", isEditable, command);
					continue;
				}

				var sb = new StringBuilder ();
				for (int i = 1; i < command.Arguments.Count; i++) {
					sb.Append (command.Arguments [i].ToString ()).Append (' ');
				}

				Set (command.Arguments [0].ToString (), sb.ToString (), isEditable, command);
			}
		}

		public CMakeVariableManager (CMakeFileFormat file)
		{
			parent = file;
			PopulateVariables ();
		}
	}
}
