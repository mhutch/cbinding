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

using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding {
	public class CMakeProject : SolutionItem {
		FilePath file;
		string name;
		CMakeFileFormat fileFormat;
		
		string executeCommand (string command, string args, string workingDir, ProgressMonitor monitor)
		{
			MemoryStream stream = new MemoryStream ();
			StreamWriter streamWriter = new StreamWriter (stream);
			string path = Path.Combine (file.ParentDirectory.ToString (), workingDir);
			Runtime.ProcessService.StartProcess (command, args, path, monitor.Log, streamWriter, null);
			stream.Position = 0;
			StreamReader streamReader = new StreamReader (stream);
			return streamReader.ReadToEnd ();
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
			CMakeFileFormat fileFormat = new CMakeFileFormat (file, this);
			fileFormat.Parse ();
			name = fileFormat.ProjectName;
			this.fileFormat = fileFormat;
		}
		
		protected override Task OnSave (ProgressMonitor monitor)
		{
			return Task.Factory.StartNew (delegate {
				fileFormat.SaveAll ();
			});
		}
		
		protected override IEnumerable<WorkspaceObject> OnGetChildren ()
		{
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
		
		protected override void OnNameChanged (SolutionItemRenamedEventArgs e)
		{
			fileFormat.Rename (e.OldName, e.NewName);
		}
		
		protected override Task<BuildResult> OnBuild (ProgressMonitor monitor, ConfigurationSelector configuration, OperationContext operationContext)
		{
			return Task.Factory.StartNew (delegate {
				BuildResult results = new BuildResult ();
				
				FileService.CreateDirectory (Path.Combine (file.ParentDirectory.ToString (), "./bin"));
				
				monitor.BeginStep ("Generating build files.");
				string generationResult = executeCommand ("cmake", "../", "./bin", monitor);
				monitor.EndStep ();
				
				monitor.BeginStep ("Building...");
				string buildResult = executeCommand ("cmake", "--build ./ --config 'debug'", "./bin", monitor);
				monitor.EndStep ();
				
				//TODO parse results.
				return results;
			});
		}
		
		protected override Task<BuildResult> OnClean (ProgressMonitor monitor, ConfigurationSelector configuration, OperationContext buildSession)
		{
			return Task.Factory.StartNew (delegate {
				BuildResult results = new BuildResult();
				
				monitor.BeginStep ("Cleaning...");
				string buildResult = executeCommand ("cmake", "--build ./ --target 'clean' --config 'debug'", "./bin", monitor);
				monitor.EndStep ();
				
				//TODO parse results.
				return results;
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

