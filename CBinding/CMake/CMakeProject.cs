//
// CMakeProject.cs
//
// Author:
//       Elsayed Awdallah <comando4ever@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gtk;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace CBinding
{
	public class CMakeProject : FolderBasedProject
	{
		FilePath file;
		string name;
		FilePath outputDirectory = new FilePath ("./bin");
		CMakeFileFormat fileFormat;

		static readonly string [] supportedLanguages = { "C", "CPP", "Objective C", "Objective C++" };

		Regex extensions = new Regex (@"(\.c|\.c\+\+|\.cc|\.cpp|\.cxx|\.m|\.mm|\.h|\.hh|\.h\+\+|\.hm|\.hpp|\.hxx|\.in|\.txx)$",
									  RegexOptions.IgnoreCase);

		public override FilePath FileName {
			get {
				return file;
			}
			set {
				file = value;
			}
		}

		Stream ExecuteCommand (string command, string args, string workingDir, ProgressMonitor monitor)
		{
			var stream = new MemoryStream ();
			var streamWriter = new StreamWriter (stream);
			FilePath path = file.ParentDirectory.Combine (workingDir);
			ProcessWrapper p = Runtime.ProcessService.StartProcess (command, args, path, monitor.Log, streamWriter, null);
			p.WaitForExit ();
			streamWriter.Flush ();
			stream.Position = 0;
			return stream;
		}

		bool CheckCMake ()
		{
			try {
				ProcessWrapper p = Runtime.ProcessService.StartProcess ("cmake", "--version", null, null);
				p.WaitForOutput ();
				return true;
			} catch {
				return false;
			}
		}

		Tuple<int, string> GetFileAndLine (string line, string separator)
		{
			int lineNumber = 0;
			string fileName = "";
			string s = line.Split (new string [] { separator }, StringSplitOptions.RemoveEmptyEntries) [1].Trim ();
			string [] args = s.Split (':');
			if (args [0].Length > 0) fileName = args [0];
			if (args.Length > 1 && args [1].Length > 0) {
				if (args [1].Contains ("("))
					int.TryParse (args [1].Split ('(') [0], out lineNumber);
				else
					int.TryParse (args [1], out lineNumber);
			}

			return Tuple.Create (lineNumber, fileName);
		}

		BuildResult ParseGenerationResult (Stream result, ProgressMonitor monitor)
		{
			var results = new BuildResult ();
			result.Position = 0;
			var sr = new StreamReader (result);
			var sb = new StringBuilder ();
			string line;
			string fileName = "";
			int lineNumber = 0;
			bool isWarning = false;

			while ((line = sr.ReadLine ()) != null) {
				//e.g.	CMake Warning in/at CMakeLists.txt:10 (COMMAND):
				//or:	CMake Warning:
				if (line.StartsWith ("CMake Warning", StringComparison.OrdinalIgnoreCase)) {
					//reset everything and add last error or warning.
					if (sb.Length > 0) {
						if (isWarning)
							results.AddWarning (BaseDirectory.Combine (fileName), lineNumber, 0, "", sb.ToString ());
						else
							results.AddError (BaseDirectory.Combine (fileName), lineNumber, 0, "", sb.ToString ());
					}

					sb.Clear ();
					fileName = "";
					lineNumber = 0;
					isWarning = true;

					// in/at CMakeLists.txt:10 (COMMAND):
					if (line.Contains (" in ")) {
						Tuple<int, string> t = GetFileAndLine (line, " in ");
						lineNumber = t.Item1;
						fileName = t.Item2;
					} else if (line.Contains (" at ")) {
						Tuple<int, string> t = GetFileAndLine (line, " at ");
						lineNumber = t.Item1;
						fileName = t.Item2;
					} else {
						string [] warning = line.Split (':');
						if (!string.IsNullOrEmpty (warning.ElementAtOrDefault (1))) {
							sb.Append (warning [1]);
						}
					}
				} else if (line.StartsWith ("CMake Error", StringComparison.OrdinalIgnoreCase)) {
					//reset everything and add last error or warning.
					if (sb.Length > 0) {
						if (isWarning)
							results.AddWarning (BaseDirectory.Combine (fileName), lineNumber, 0, "", sb.ToString ());
						else
							results.AddError (BaseDirectory.Combine (fileName), lineNumber, 0, "", sb.ToString ());
					}

					sb.Clear ();
					fileName = "";
					lineNumber = 0;
					isWarning = false;

					// in/at CMakeLists.txt:10 (COMMAND):
					if (line.Contains (" in ")) {
						Tuple<int, string> t = GetFileAndLine (line, " in ");
						lineNumber = t.Item1;
						fileName = t.Item2;
					} else if (line.Contains (" at ")) {
						Tuple<int, string> t = GetFileAndLine (line, " at ");
						lineNumber = t.Item1;
						fileName = t.Item2;
					} else {
						string [] error = line.Split (':');
						if (!string.IsNullOrEmpty (error.ElementAtOrDefault (1))) {
							sb.Append (error [1]);
						}
					}
				} else {
					sb.Append (line);
				}
			}

			return results;
		}

		public void RemoveTarget (string targetName)
		{
			fileFormat.RemoveTarget (targetName);
		}

		protected override string OnGetBaseDirectory ()
		{
			return file.ParentDirectory.ToString ();
		}

		protected override string OnGetName ()
		{
			return name;
		}

		public void LoadFrom (FilePath file)
		{
			this.file = file;
			CMakeFileFormat fileFormat = new CMakeFileFormat (file, this);
			name = fileFormat.ProjectName;
			this.fileFormat = fileFormat;
		}

		protected override Task OnSave (ProgressMonitor monitor)
		{
			return Task.Factory.StartNew (() => {
				fileFormat.SaveAll ();
			});
		}

		protected override IEnumerable<WorkspaceObject> OnGetChildren ()
		{
			foreach (CMakeTarget target in fileFormat.Targets.Values)
				target.ParentObject = this;
			return fileFormat.Targets.Values.ToList ();
		}

		List<FilePath> files = new List<FilePath> ();

		protected override IEnumerable<FilePath> OnGetItemFiles (bool includeReferencedFiles)
		{
			files.Clear ();
			foreach (CMakeTarget target in fileFormat.Targets.Values) {
				files = files.Concat (target.GetFiles ()).ToList ();
			}
			return files;
		}

		CMakeTarget GetTarget (FilePath fileName)
		{
			string filename = fileName.FileName;
			foreach (CMakeTarget target in fileFormat.Targets.Values) {
				foreach (string file in target.Files) {
					if (file.EndsWith (filename, StringComparison.OrdinalIgnoreCase))
						return target;
				}
			}
			return null;
		}

		void AddFile (FilePath fileName, string targetName)
		{
			fileName = fileName.ToRelative (file.ParentDirectory);
			foreach (string target in fileFormat.Targets.Keys) {
				if (target.StartsWith (targetName, StringComparison.OrdinalIgnoreCase))
					fileFormat.Targets [target].AddFile (fileName.ToString ());
			}
		}

		protected override Task<BuildResult> OnBuild (ProgressMonitor monitor, ConfigurationSelector configuration,
													  OperationContext operationContext)
		{
			return Task.Factory.StartNew (() => {
				BuildResult results;

				if (!CheckCMake ()) {
					results = new BuildResult ();
					results.AddError ("CMake cannot be found.");
					return results;
				}

				FileService.CreateDirectory (file.ParentDirectory.Combine (outputDirectory));

				monitor.BeginStep ("Generating build files.");
				Stream generationResult = ExecuteCommand ("cmake", "../", outputDirectory, monitor);
				results = ParseGenerationResult (generationResult, monitor);
				monitor.EndStep ();

				monitor.BeginStep ("Building...");
				Stream buildResult = ExecuteCommand ("cmake", "--build ./ --clean-first", outputDirectory, monitor);
				//TODO: Parse results.
				monitor.EndStep ();

				return results;
			});
		}

		protected override Task<BuildResult> OnClean (ProgressMonitor monitor, ConfigurationSelector configuration,
													  OperationContext buildSession)
		{
			return Task.Factory.StartNew (() => {
				var results = new BuildResult ();

				FilePath path = BaseDirectory.Combine (outputDirectory);
				if (Directory.Exists (path)) {
					FileService.DeleteDirectory (path);
				}

				return results;
			});
		}

		protected override Task OnExecute (ProgressMonitor monitor, ExecutionContext context, ConfigurationSelector configuration)
		{
			return Task.Factory.StartNew (async () => {
				ExternalConsole console = context.ExternalConsoleFactory.CreateConsole (false, monitor.CancellationToken);
				string targetName = "";
				foreach (var target in fileFormat.Targets.Values) {
					if (target.Type == CMakeTarget.Types.Binary) {
						targetName = target.Name;
						break;
					}
				}

				if (string.IsNullOrEmpty (targetName)) {
					monitor.ReportError ("Can't find an executable target.");
					return;
				}

				FilePath f = BaseDirectory.Combine (outputDirectory);
				NativeExecutionCommand cmd;
				if (File.Exists (f.Combine (targetName)))
					cmd = new NativeExecutionCommand (f.Combine (targetName));
				else if (File.Exists (f.Combine (string.Format ("{0}.{1}", targetName, "exe"))))
					cmd = new NativeExecutionCommand (f.Combine (string.Format ("{0}.{1}", targetName, "exe")));
				else if (File.Exists (f.Combine ("./Debug", targetName)))
					cmd = new NativeExecutionCommand (f.Combine ("./Debug", targetName));
				else if (File.Exists (f.Combine ("./Debug", string.Format ("{0}.{1}", targetName, "exe"))))
					cmd = new NativeExecutionCommand (f.Combine ("./Debug", string.Format ("{0}.{1}", targetName, "exe")));
				else {
					monitor.ReportError ("Can't determine executable path.");
					return;
				}

				try {
					var handler = Runtime.ProcessService.GetDefaultExecutionHandler (cmd);
					var op = handler.Execute (cmd, console);

					using (var t = monitor.CancellationToken.Register (op.Cancel))
						await op.Task;

					monitor.Log.WriteLine ("The operation exited with code: {0}", op.ExitCode);
				} catch (Exception ex) {
					monitor.ReportError ("Can't execute the target.", ex);
				} finally {
					console.Dispose ();
				}
			});
		}

		public override string [] OnGetSupportedLanguages ()
		{
			return supportedLanguages;
		}

		protected override bool OnGetCanExecute (ExecutionContext context, ConfigurationSelector configuration)
		{
			foreach (CMakeTarget target in fileFormat.Targets.Values) {
				if (target.Type == CMakeTarget.Types.Binary) return true;
			}
			return false;
		}

		public override void OnFileRemoved (FilePath file)
		{
			base.OnFileRemoved (file);

			foreach (var target in fileFormat.Targets.Values.ToList ()) {
				target.RemoveFile (file);
			}

			fileFormat.SaveAll ();
		}

		public override void OnFilesRemoved (List<FilePath> files)
		{
			base.OnFilesRemoved (files);

			foreach (var target in fileFormat.Targets.Values.ToList ()) {
				target.RemoveFiles (files);
			}

			fileFormat.SaveAll ();
		}

		public override void OnFileRenamed (FilePath oldFile, FilePath newFile)
		{
			base.OnFileRenamed (oldFile, newFile);

			var oldFiles = new List<FilePath> () { oldFile };
			var newFiles = new List<FilePath> () { newFile };

			foreach (var target in fileFormat.Targets.Values.ToList ()) {
				target.RenameFiles (oldFiles, newFiles);
			}

			fileFormat.SaveAll ();
		}

		public override void OnFileMoved (FilePath src, FilePath dst)
		{
			base.OnFileMoved (src, dst);

			var oldFiles = new List<FilePath> () { src };
			var newFiles = new List<FilePath> () { dst };

			foreach (var target in fileFormat.Targets.Values.ToList ()) {
				target.RenameFiles (oldFiles, newFiles);
			}

			fileFormat.SaveAll ();
		}

		public override void OnFilesMoved (List<FilePath> src, List<FilePath> dst)
		{
			base.OnFilesMoved (src, dst);

			foreach (var target in fileFormat.Targets.Values.ToList ()) {
				target.RenameFiles (src, dst);
			}

			fileFormat.SaveAll ();
		}

		public override void OnFilesRenamed (List<FilePath> oldFiles, List<FilePath> newFiles)
		{
			base.OnFilesRenamed (oldFiles, newFiles);

			foreach (var target in fileFormat.Targets.Values.ToList ()) {
				target.RenameFiles (oldFiles, newFiles);
			}

			fileFormat.SaveAll ();
		}

		public override void OnFileAdded (FilePath file)
		{
			base.OnFileAdded (file);

			if (!extensions.IsMatch (file))
				return;

			using (var dlg = new TargetPickerDialog ("Pick a target", fileFormat)) {
				if (MessageService.ShowCustomDialog (dlg) != (int)ResponseType.Ok)
					return;

				foreach (var target in dlg.SelectedTargets) {
					target.AddFile (file.CanonicalPath.ToRelative (fileFormat.File.ParentDirectory));
				}
			}

			fileFormat.SaveAll ();
		}

		public override void OnFilesAdded (List<FilePath> files)
		{
			base.OnFilesAdded (files);

			var filesToAdd = new List<FilePath> ();
			foreach (var file in files) {
				if (extensions.IsMatch (file))
					filesToAdd.Add (file);
			}

			if (filesToAdd.Count == 0)
				return;

			using (var dlg = new TargetPickerDialog ("Pick a target", fileFormat)) {
				if (MessageService.ShowCustomDialog (dlg) != (int)ResponseType.Ok)
					return;

				foreach (var target in dlg.SelectedTargets) {
					foreach (var file in filesToAdd)
						target.AddFile (file.CanonicalPath.ToRelative (fileFormat.File.ParentDirectory));
				}
			}

			fileFormat.SaveAll ();
		}

		public override void OnFileCopied (FilePath src, FilePath dst)
		{
			base.OnFileCopied (src, dst);

			if (dst.IsDirectory) {
				dst = dst + Path.DirectorySeparatorChar + src.FileName;
			}

			OnFileAdded (dst);
		}

		public override void OnFilesCopied (List<FilePath> src, List<FilePath> dst)
		{
			base.OnFilesCopied (src, dst);

			for (int i = 0; i < src.Count; i++) {
				if (dst [i].IsDirectory) {
					dst [i] = dst [i] + Path.DirectorySeparatorChar + src [i].FileName;
				}
			}

			OnFilesAdded (dst);
		}

		public CMakeProject ()
		{
			Initialize (this);
		}
	}
}
