//
// CMakeProject.cs
//
// Author:
//       Elsayed Awdallah <comando4ever@gmail.com>
//
// Copyright (c) 2015 Xamarin Inc. (http://xamarin.com)
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
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Projects;

namespace CBinding {
	public class CMakeProject : SolutionItem {
		FilePath file;
		string name;
		string outputDirectory = "./bin";
		CMakeFileFormat fileFormat;
		
		Stream executeCommand (string command, string args, string workingDir, ProgressMonitor monitor)
		{
			MemoryStream stream = new MemoryStream ();
			StreamWriter streamWriter = new StreamWriter (stream);
			string path = Path.Combine (file.ParentDirectory.ToString (), workingDir);
			ProcessWrapper p = Runtime.ProcessService.StartProcess (command, args, path, monitor.Log, streamWriter, null);
			p.WaitForExit ();
			streamWriter.Flush ();
			stream.Position = 0;
			return stream;
		}
		
		bool checkCMake ()
		{
			try {
				ProcessWrapper p = Runtime.ProcessService.StartProcess ("cmake", "--version", null, null);
				p.WaitForOutput ();
				return true;
			} catch {
				return false;
			}
		}
		
		Tuple<int, string> getFileAndLine (string line, string separator)
		{
			int lineNumber = 0;
			string fileName = "";
			string s = line.Split (new String[] {separator}, StringSplitOptions.RemoveEmptyEntries) [1].Trim ();
			string[] args = s.Split(':');
			if (args [0].Length > 0) fileName = args [0];
			if (args.Length > 1 && args [1].Length > 0) {
				if(args [1].Contains ("("))
					int.TryParse (args [1].Split ('(') [0], out lineNumber);
				else
					int.TryParse (args [1], out lineNumber);
			}
			
			return new Tuple<int, string> (lineNumber, fileName);
		}
		
		BuildResult parseGenerationResult (Stream result, ProgressMonitor monitor)
		{
			BuildResult results = new BuildResult();
			result.Position = 0;
			StreamReader sr = new StreamReader (result);
			StringBuilder sb = new StringBuilder ();
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
						if(isWarning)
							results.AddWarning (BaseDirectory.Combine(fileName), lineNumber, 0, "", sb.ToString());
						else
							results.AddError (BaseDirectory.Combine(fileName), lineNumber, 0, "", sb.ToString());
					}
					
					sb.Clear ();
					fileName = "";
					lineNumber = 0;
					isWarning = true;
					
					// in/at CMakeLists.txt:10 (COMMAND):
					if (line.Contains (" in ")) {
						var t = getFileAndLine (line, " in ");
						lineNumber = t.Item1;
						fileName = t.Item2;
					} else if (line.Contains (" at ")) {
						var t = getFileAndLine (line, " at ");
						lineNumber = t.Item1;
						fileName = t.Item2;
					} else {
						string[] warning = line.Split (':');
						if (!String.IsNullOrEmpty (warning.ElementAtOrDefault (1))) {
							sb.Append (warning [1]);
						}
					}
				} else if (line.StartsWith ("CMake Error", StringComparison.OrdinalIgnoreCase)) {
					//reset everything and add last error or warning.
					if (sb.Length > 0) {
						if(isWarning)
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
						var t = getFileAndLine (line, " in ");
						lineNumber = t.Item1;
						fileName = t.Item2;
					} else if (line.Contains (" at ")) {
						var t = getFileAndLine (line, " at ");
						lineNumber = t.Item1;
						fileName = t.Item2;
					} else {
						string[] error = line.Split (':');
						if (!String.IsNullOrEmpty (error.ElementAtOrDefault (1))) {
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
			return file.ParentDirectory.ToString();
		}
		
		protected override string OnGetName ()
		{
			return this.name;
		}
		
		public void LoadFrom (FilePath file)
		{
			this.file = file;
			FileName = file;
			CMakeFileFormat fileFormat = new CMakeFileFormat (file, this);
			fileFormat.Parse ();
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
		
		/*
		List<FilePath> files = new List<FilePath> ();
		protected override IEnumerable<FilePath> OnGetItemFiles (bool includeReferencedFiles)
		{
			files.Clear ();
			foreach (CMakeTarget target in fileFormat.Targets.Values) {
				files = files.Concat (target.GetFiles ()).ToList ();
			}
			return files;
		}
		
		CMakeTarget getTarget (FilePath fileName)
		{
			string filename = fileName.FileName;
			foreach (CMakeTarget target in fileFormat.Targets.Values) {
				foreach (string file in target.Files.Keys) {
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
		
		void RemoveFile (FilePath fileName)
		{
			fileName = fileName.ToRelative (file.ParentDirectory);
			foreach (CMakeTarget target in fileFormat.Targets.Values) {
				target.RemoveFile (fileName);
			}
		}
		//*/
		
		protected override Task<BuildResult> OnBuild (ProgressMonitor monitor, ConfigurationSelector configuration, OperationContext operationContext)
		{
			return Task.Factory.StartNew (() => {
				BuildResult results;
				
				if (!checkCMake ()) {
					results = new BuildResult();
					results.AddError ("CMake cannot be found.");
					return results;
				}
				
				FileService.CreateDirectory (Path.Combine (file.ParentDirectory.ToString (), outputDirectory));
				
				monitor.BeginStep ("Generating build files.");
				Stream generationResult = executeCommand ("cmake", "../", outputDirectory, monitor);
				results = parseGenerationResult(generationResult, monitor);
				monitor.EndStep ();
				
				monitor.BeginStep ("Building...");
				Stream buildResult = executeCommand ("cmake", "--build ./ --clean-first", outputDirectory, monitor);
				//TODO: Parse results.
				monitor.EndStep ();
				
				return results;
			});
		}
		
		protected override Task<BuildResult> OnClean (ProgressMonitor monitor, ConfigurationSelector configuration, OperationContext buildSession)
		{
			return Task.Factory.StartNew (() => {
				BuildResult results = new BuildResult();
				
				string path = Path.Combine (BaseDirectory, outputDirectory);
				if (Directory.Exists (path)) {
					FileService.DeleteDirectory(path);
				}
				
				return results;
			});
		}
		
		protected override Task OnExecute(ProgressMonitor monitor, ExecutionContext context, ConfigurationSelector configuration)
		{
			return Task.Factory.StartNew (async () => {
				ExternalConsole console = context.ExternalConsoleFactory.CreateConsole (false, monitor.CancellationToken);
				string targetName = "";
				foreach (var target in fileFormat.Targets.Values) {
					if (target.Type == CMakeTarget.Types.BINARY) {
						targetName = target.Name;
						break;
					}
				}
				
				if (String.IsNullOrEmpty (targetName)) {
					monitor.ReportError ("Can't find an executable target.");
					return;
				}
				
				string outputFullPath = Path.Combine (BaseDirectory, outputDirectory);
				FilePath f = new FilePath(outputFullPath);
				NativeExecutionCommand cmd;
				if (File.Exists(f.Combine(targetName)))
					cmd = new NativeExecutionCommand(f.Combine(targetName));
				else if (File.Exists(f.Combine(String.Format("{0}.{1}", targetName, "exe"))))
					cmd = new NativeExecutionCommand(f.Combine(String.Format("{0}.{1}", targetName, "exe")));
				else if (File.Exists(f.Combine("./Debug", targetName)))
					cmd = new NativeExecutionCommand(f.Combine("./Debug", targetName));
				else if (File.Exists(f.Combine("./Debug", String.Format("{0}.{1}", targetName, "exe"))))
					cmd = new NativeExecutionCommand(f.Combine("./Debug", String.Format("{0}.{1}", targetName, "exe")));
				else {
					monitor.ReportError ("Can't determine executable path.");
					return;
				}
				
				try {
					var handler = Runtime.ProcessService.GetDefaultExecutionHandler(cmd);
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
		
		protected override bool OnGetCanExecute (ExecutionContext context, ConfigurationSelector configuration)
		{
			foreach (CMakeTarget target in fileFormat.Targets.Values) {
				if (target.Type == CMakeTarget.Types.BINARY) return true;
			}
			return false;
		}
		
		public CMakeProject ()
		{
			Initialize (this);
		}
	}
}

