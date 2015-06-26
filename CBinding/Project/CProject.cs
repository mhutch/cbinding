//
// CProject.cs: C/C++ Project
//
// Authors:
//   Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// Copyright (C) 2007 Marcos David Marin Amador
//
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using Mono.Addins;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Projects;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Deployment;
using MonoDevelop.Deployment.Linux;
using MonoDevelop.Ide;
using System.Threading.Tasks;
using System.Threading;
using CBinding.Parser;
using ClangSharp;

namespace CBinding
{
	public enum Language {
		C,
		CPP,
		OBJC,
		OBJCPP
	}
	
	public enum CProjectCommands {
		AddPackage,
		UpdateClassPad,
		ShowPackageDetails,
		GotoDeclaration,
	}

	[ExportProjectType ("{2857B73E-F847-4B02-9238-064979017E93}", Extension="cproj", Alias="C/C++")]
	public class CProject : Project, IDeployable
	{
		[ItemProperty ("Compiler", ValueType = typeof(CCompiler))]
		private ICompiler compiler_manager;
		
		[ItemProperty ("Language")]
		private Language language;
		
		[ItemProperty ("OutputType")]
		CompileTarget target = CompileTarget.Exe;

		public CLangManager cLangManager;
		public ClangProjectSymbolDatabase db;
		
    	private ProjectPackageCollection packages = new ProjectPackageCollection ();
		
		public event ProjectPackageEventHandler PackageAddedToProject;
		public event ProjectPackageEventHandler PackageRemovedFromProject;

		/// <summary>
		/// Extensions for C/C++ source files
		/// </summary>
		public static string[] SourceExtensions = { ".C", ".CC", ".CPP", ".CXX", ".M", ".MM" };
		
		/// <summary>
		/// Extensions for C/C++ header files
		/// </summary>
		public static string[] HeaderExtensions = { ".H", ".HH", ".HPP", ".HXX" };

		public Dictionary<string, bool> BOMPresentInFile = new Dictionary<string, bool>();

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		protected override void OnInitialize ()
		{
			base.OnInitialize ();
			packages.Project = this;
			cLangManager = new CLangManager (this);
			db = new ClangProjectSymbolDatabase (this);
		}

		/// <summary>
		/// Initializes from template.
		/// </summary>
		/// <param name="projectCreateInfo">Project create info.</param>
		/// <param name="template">Template.</param>
		protected override void OnInitializeFromTemplate (ProjectCreateInformation projectCreateInfo, XmlElement template)
		{
			base.OnInitializeFromTemplate (projectCreateInfo, template);
			string binPath = ".";
			if (projectCreateInfo != null) {
				Name = projectCreateInfo.ProjectName;
				binPath = projectCreateInfo.BinPath;
			}
			Compiler = null; // use default compiler depending on language
			CProjectConfiguration configuration =
				(CProjectConfiguration)CreateConfiguration ("Debug");
			configuration.DefineSymbols = "DEBUG MONODEVELOP";		
			configuration.DebugSymbols = true;
			Configurations.Add (configuration);

			configuration =
				(CProjectConfiguration)CreateConfiguration ("Release");
			configuration.DebugSymbols = false;
			configuration.OptimizationLevel = 3;
			configuration.DefineSymbols = "MONODEVELOP";
			Configurations.Add (configuration);

			foreach (CProjectConfiguration c in Configurations) {
				c.OutputDirectory = Path.Combine (binPath, c.Id);
				c.SourceDirectory = projectCreateInfo.ProjectBasePath;
				c.Output = Name;

				if (template != null) {
					if (template.Attributes ["LanguageName"] != null) {
						string languageName = template.Attributes ["LanguageName"].InnerText;
						switch (languageName) {
						case "C":
							this.language = Language.C;
							break;
						case "CPP":
							this.language = Language.CPP;
							break;
						case "Objective C":
							this.language = Language.OBJC;
							break;
						case "Objective C++":
							this.language = Language.OBJCPP;
							break;
						}
					}
					if (template.Attributes ["Target"] != null) {
						c.CompileTarget = (CompileTarget)Enum.Parse (
							typeof(CompileTarget),
							template.Attributes ["Target"].InnerText);
					}
					if (template.GetAttribute ("ExternalConsole") == "True") {
						c.ExternalConsole = true;
						c.PauseConsoleOutput = true;
					}
					if (template.Attributes ["PauseConsoleOutput"] != null) {
						c.PauseConsoleOutput = bool.Parse (
							template.Attributes ["PauseConsoleOutput"].InnerText);
					}
					if (template.Attributes ["CompilerArgs"].InnerText != null) {
						c.ExtraCompilerArguments = template.Attributes ["CompilerArgs"].InnerText;
					}
					if (template.Attributes ["LinkerArgs"].InnerText != null) {
						c.ExtraLinkerArguments = template.Attributes ["LinkerArgs"].InnerText;
					}
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CBinding.CProject"/> class.
		/// Must call Initialize* after.
		/// </summary>
		public CProject ()
		{
		}

		/// <summary>
		/// Returns with the languages supported by the project
		/// </summary>
		protected override string[] OnGetSupportedLanguages ()
		{
			return new string[] { "C", "CPP", "Objective C", "Objective C++" };
		}

		/// <summary>
		/// Gets or sets the compile target.
		/// </summary>
		/// <value>The compile target.</value>
		public CompileTarget CompileTarget {
			get { return target; }
			set { target = value; }
		}

		/// <summary>
		/// Returns if the file represented by fileName is a compileable file.
		/// </summary>
		/// <param name="fileName">File name.</param>
		protected override bool OnGetIsCompileable (string fileName)
		{
			string ext = Path.GetExtension (fileName.ToUpper ());
			return (-1 != Array.IndexOf (SourceExtensions, ext));
		}

		/// <summary>
		/// Returns with the referenced items of the project.
		/// </summary>
		/// <param name="configuration">Configuration.</param>
		protected override IEnumerable<SolutionItem> OnGetReferencedItems (ConfigurationSelector configuration)
		{
			foreach (var p in base.OnGetReferencedItems (configuration))
				yield return p;

			if (ParentSolution == null)
				yield break;

			foreach (Package p in Packages) {
				if (p.IsProject && p.Name != Name) {
					var cp = ParentSolution.FindProjectByName (p.Name) as CProject;
					if (cp != null)
						yield return cp;
				}
			}
		}

		/// <summary>
		/// Determines if a header file is specified by filename.
		/// </summary>
		/// <returns><c>true</c> if a header file is specified by filename; otherwise, <c>false</c>.</returns>
		/// <param name="filename">Filename.</param>
		public static bool IsHeaderFile (string filename)
		{
			return (0 <= Array.IndexOf (HeaderExtensions, Path.GetExtension (filename.ToUpper ())));
		}
		
		/// <summary>
		/// Ths pkg-config package is for internal MonoDevelop use only, it is not deployed.
		/// </summary>
		public void WriteMDPkgPackage (ConfigurationSelector configuration)
		{
			string pkgfile = Path.Combine (BaseDirectory, Name + ".md.pc");
			
			CProjectConfiguration config = (CProjectConfiguration)GetConfiguration (configuration);
			while (config == null) {
				Thread.Sleep (20);
				config = (CProjectConfiguration)GetConfiguration (configuration);
			}
			
			List<string> headerDirectories = new List<string> ();
			
			foreach (ProjectFile f in Files) {
				if (IsHeaderFile (f.Name)) {
					string dir = Path.GetDirectoryName (f.FilePath);
					
					if (!headerDirectories.Contains (dir)) {
						headerDirectories.Add (dir);
					}
				}
			}
			
			using (StreamWriter writer = new StreamWriter (pkgfile)) {
				writer.WriteLine ("Name: {0}", Name);
				writer.WriteLine ("Description: {0}", Description);
				writer.WriteLine ("Version: {0}", Version);
				writer.WriteLine ("Libs: -L\"{0}\" -l{1}", config.OutputDirectory, config.Output.StartsWith ("lib", StringComparison.OrdinalIgnoreCase)?
				                                                                                                config.Output.Substring (3):
				                                                                                                config.Output);
//				writer.WriteLine ("Cflags: -I{0}", BaseDirectory);
				writer.WriteLine ("Cflags: -I\"{0}\"", string.Join ("\" -I\"", headerDirectories.ToArray ()));
			}
			
			// If this project compiles into a shared object we need to
			// export the output path to the LD_LIBRARY_PATH
			string literal = "LD_LIBRARY_PATH";
			string ld_library_path = Environment.GetEnvironmentVariable (literal);
			
			if (string.IsNullOrEmpty (ld_library_path)) {
				Environment.SetEnvironmentVariable (literal, config.OutputDirectory);
			} else if (!ld_library_path.Contains (config.OutputDirectory)) {
				ld_library_path = string.Format ("{0}:{1}", config.OutputDirectory, ld_library_path);
				Environment.SetEnvironmentVariable (literal, ld_library_path);
			}
		}
		
		/// <summary>
		/// This is the pkg-config package that gets deployed.
		/// <returns>The pkg-config package's filename</returns>
		/// </summary>
		private string WriteDeployablePgkPackage (Project project, CProjectConfiguration config)
		{
			// FIXME: This should probably be grabed from somewhere.
			string prefix = "/usr/local";
			string pkgfile = Path.Combine (BaseDirectory, Name + ".pc");
			
			using (StreamWriter writer = new StreamWriter (pkgfile)) {
				writer.WriteLine ("prefix={0}", prefix);
				writer.WriteLine ("exec_prefix=${prefix}");
				writer.WriteLine ("libdir=${exec_prefix}/lib");
				writer.WriteLine ("includedir=${prefix}/include");
				writer.WriteLine ();
				writer.WriteLine ("Name: {0}", Name);
				writer.WriteLine ("Description: {0}", Description);
				writer.WriteLine ("Version: {0}", Version);
				writer.WriteLine ("Requires: {0}", string.Join (" ", Packages.ToStringArray ()));
				// TODO: How should I get this?
				writer.WriteLine ("Conflicts: {0}", string.Empty);
				writer.Write ("Libs: -L${libdir} ");
				writer.WriteLine ("-l{0}", config.Output.StartsWith ("lib", StringComparison.OrdinalIgnoreCase)?
				                                                            config.Output.Substring (3):
				                                                            config.Output);
				writer.Write ("Cflags: -I${includedir}/");
				writer.WriteLine ("{0} {1}", Name, Compiler.GetDefineFlags (project, config));
			}
			
			return pkgfile;
		}

		/// <summary>
		/// Builds the project.
		/// </summary>
		/// <returns>The build result.</returns>
		/// <param name="monitor">Progress monitor.</param>
		/// <param name="configuration">Configuration to build.</param>
		/// <remarks>This method is invoked to build the project. Support files such as files with the Copy to Output flag will
		///  be copied before calling this method.</remarks>
		protected override Task<BuildResult> DoBuild (ProgressMonitor monitor, ConfigurationSelector configuration)
		{
			CProjectConfiguration pc = (CProjectConfiguration) GetConfiguration (configuration);
			pc.SourceDirectory = BaseDirectory;

			return Task<BuildResult>.Factory.StartNew (delegate {
				if (pc.CompileTarget != CompileTarget.Exe)
					WriteMDPkgPackage (configuration);

				return compiler_manager.Compile (this,
					Files, packages,
					pc,
					monitor);
			});
		}

		/// <summary>
		/// Cleans the files produced by this solution item
		/// </summary>
		/// <param name="monitor">A progress monitor</param>
		/// <param name="configuration">Configuration to use to clean the project</param>
		/// <param name="operationContext">Operation context.</param>
		protected async override Task<BuildResult> OnClean (ProgressMonitor monitor, ConfigurationSelector configuration, OperationContext operationContext)
		{
			CProjectConfiguration conf = (CProjectConfiguration) GetConfiguration (configuration);

			var res = await base.OnClean (monitor, configuration, operationContext);
			if (res.HasErrors)
				return res;

			await Task.Run (() => Compiler.Clean (Files, conf, monitor));

			return res;
		}

		/// <summary>
		/// Creates the execution command for the project.
		/// </summary>
		/// <returns>The execution command.</returns>
		/// <param name="conf">Conf.</param>
		protected virtual ExecutionCommand CreateExecutionCommand (CProjectConfiguration conf)
		{
			string app = Path.Combine (conf.OutputDirectory, conf.Output);
			NativeExecutionCommand cmd = new NativeExecutionCommand (app);
			cmd.Arguments = conf.CommandLineParameters;
			cmd.WorkingDirectory = Path.GetFullPath (conf.OutputDirectory);
			cmd.EnvironmentVariables = conf.EnvironmentVariables;
			return cmd;
		}

		/// <summary>
		/// Determines whether this solution item can be executed using the specified context and configuration.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="solutionConfiguration">Solution configuration.</param>
		protected override bool OnGetCanExecute (MonoDevelop.Projects.ExecutionContext context, ConfigurationSelector solutionConfiguration)
		{
			CProjectConfiguration conf = (CProjectConfiguration) GetConfiguration (solutionConfiguration);
			ExecutionCommand cmd = CreateExecutionCommand (conf);
			return (target == CompileTarget.Exe) && context.ExecutionHandler.CanExecute (cmd);
		}

		/// <summary>
		/// Executes the project
		/// </summary>
		/// <param name="monitor">Progress monitor.</param>
		/// <param name="context">Execution context.</param>
		/// <param name="configuration">Configuration to execute.</param>
		/// <returns>The execute.</returns>
		protected async override Task DoExecute (ProgressMonitor monitor, MonoDevelop.Projects.ExecutionContext context, ConfigurationSelector configuration)
		{
			CProjectConfiguration conf = (CProjectConfiguration) GetConfiguration (configuration);
			bool pause = conf.PauseConsoleOutput;
			OperationConsole console;
			
			if (conf.CompileTarget != CompileTarget.Exe) {
				MessageService.ShowMessage ("Compile target is not an executable!");
				return;
			}
			
			monitor.Log.WriteLine ("Running project...");
			
			if (conf.ExternalConsole)
				console = context.ExternalConsoleFactory.CreateConsole (!pause, monitor.CancellationToken);
			else
				console = context.ConsoleFactory.CreateConsole (monitor.CancellationToken);
			
			try {
				ExecutionCommand cmd = CreateExecutionCommand (conf);
				if (!context.ExecutionHandler.CanExecute (cmd)) {
					monitor.ReportError ("Cannot execute \"" + conf.Output + "\". The selected execution mode is not supported for C projects.", null);
					return;
				}

				ProcessAsyncOperation op = context.ExecutionHandler.Execute (cmd, console);
				using (var t = monitor.CancellationToken.Register (op.Cancel))
					await op.Task;
				
				monitor.Log.WriteLine ("The operation exited with code: {0}", op.ExitCode);
			} catch (Exception ex) {
				LoggingService.LogError (string.Format ("Cannot execute \"{0}\"", conf.Output), ex);
				monitor.ReportError ("Cannot execute \"" + conf.Output + "\"", ex);
			} finally {			
				console.Dispose ();
			}
		}

		/// <summary>
		/// Returns with output file name.
		/// </summary>
		/// <param name="configuration">Configuration.</param>
		protected override FilePath OnGetOutputFileName (ConfigurationSelector configuration)
		{
			CProjectConfiguration conf = (CProjectConfiguration) GetConfiguration (configuration);
			return conf.OutputDirectory.Combine (conf.CompiledOutputName);
		}

		/// <summary>
		/// Create an empty configuration for project. Must fill configuration after.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="kind">Kind.</param>
		protected override SolutionItemConfiguration OnCreateConfiguration (string name, ConfigurationKind kind)
		{
			CProjectConfiguration conf = new CProjectConfiguration ();
			
			conf.Name = name;
			
			return conf;
		}

		/// <summary>
		/// Returns with supported languages as tags. Obligatory for Add files dialog.
		/// </summary>
		/// <param name="types">Types.</param>
		protected override void OnGetTypeTags (HashSet<string> types)
		{
			base.OnGetTypeTags (types);
			types.Add ("C/C++");
			types.Add ("Native");
		}

		/// <summary>
		/// Gets or sets the language.
		/// </summary>
		/// <value>The language.</value>
		public Language Language {
			get { return language; }
			set { language = value; }
		}

		/// <summary>
		/// Gets or sets the compiler in use.
		/// </summary>
		/// <value>The compiler.</value>
		public ICompiler Compiler {
			get { return compiler_manager; }
			set {
				if (value != null) {
					compiler_manager = value;
				} else {
					object[] compilers = AddinManager.GetExtensionObjects ("/CBinding/Compilers");
					string compiler;

					// TODO: This should depend on platform (eg: windows would be mingw or msvc)
					if (language == Language.C)
						compiler = PropertyService.Get ("CBinding.DefaultCCompiler", new GccCompiler ().Name);
					else
						compiler = PropertyService.Get ("CBinding.DefaultCppCompiler", new GppCompiler ().Name);
					
					foreach (ICompiler c in compilers) {
						if (compiler == c.Name) {
							compiler_manager = c;
						}
					}
				}
			}
		}

		// TODO NPM: not supported
		[Browsable(false)]
		[ItemProperty ("Packages")]
		public ProjectPackageCollection Packages {
			get { return packages; }
			set {
				packages = value;
				packages.Project = this;
			}
		}

		/// <summary>
		/// Invoked when a file is added to project. Detects UTF-8 BOM.
		/// </summary>
		/// <param name="args">Arguments.</param>
		protected override void OnFileAddedToProject (ProjectFileEventArgs args)
		{
			base.OnFileAddedToProject (args);
			foreach(var f in Files){
				using (var s = new FileStream (f.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
					byte[] BOM = new byte[3];
					s.Read (BOM, 0, 3);
					if (!BOMPresentInFile.ContainsKey (f.Name)) {
						BOMPresentInFile.Add (f.Name, false);
					}
					if(BOM[0] == 0xEF && BOM[1] == 0xBB && BOM[2] == 0xBF){
						BOMPresentInFile[f.Name] = true;
					} else {
						BOMPresentInFile[f.Name] = false;
					}
				}
			}
			foreach (ProjectFileEventInfo e in args) {
				if (!Loading && !IsCompileable (e.ProjectFile.Name) &&
					e.ProjectFile.BuildAction == BuildAction.Compile) {
					e.ProjectFile.BuildAction = BuildAction.None;
				}

				if (e.ProjectFile.BuildAction == BuildAction.Compile)
					ThreadPool.QueueUserWorkItem (o => {
						cLangManager.createTranslationUnit (this, e.ProjectFile.Name, new CXUnsavedFile[0]);
					});
			}
		}

		/// <summary>
		/// Invoked when a file is changed to project. Detects UTF-8 BOM.
		/// </summary>
		/// <param name="e">E.</param>
		protected override void OnFileChangedInProject (ProjectFileEventArgs e)
		{
			base.OnFileChangedInProject (e);
			foreach(var f in Files){
				using (var s = new FileStream (f.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
					byte[] BOM = new byte[3];
					s.Read (BOM, 0, 3);
					if (!BOMPresentInFile.ContainsKey (f.Name)) {
						BOMPresentInFile.Add (f.Name, false);
					}					
					if(BOM[0] == 0xEF && BOM[1] == 0xBB && BOM[2] == 0xBF){
						BOMPresentInFile[f.Name] = true;
					} else {
						BOMPresentInFile[f.Name] = false;
					}
				}
			}
		}

		/// <summary>
		/// Invoked when a file is removed from project.
		/// </summary>
		/// <param name="args">Arguments.</param>
		protected override void OnFileRemovedFromProject (ProjectFileEventArgs args)
		{
			base.OnFileRemovedFromProject (args);
			foreach (ProjectFileEventInfo e in args) {
				if (!Loading && !IsCompileable (e.ProjectFile.Name) &&
				    e.ProjectFile.BuildAction == BuildAction.Compile) {
					e.ProjectFile.BuildAction = BuildAction.None;
				}
				if (e.ProjectFile.BuildAction == BuildAction.Compile)
					cLangManager.RemoveTranslationUnit (this, e.ProjectFile.Name);
			}
		}

		internal void NotifyPackageRemovedFromProject (Package package)
		{
			Runtime.AssertMainThread ();
			PackageRemovedFromProject (this, new ProjectPackageEventArgs (this, package));
		}
		
		internal void NotifyPackageAddedToProject (Package package)
		{
			Runtime.AssertMainThread ();
			PackageAddedToProject (this, new ProjectPackageEventArgs (this, package));
		}

		public DeployFileCollection GetDeployFiles (ConfigurationSelector configuration)
		{
			DeployFileCollection deployFiles = new DeployFileCollection ();
			
			CProjectConfiguration conf = (CProjectConfiguration) GetConfiguration (configuration);
			CompileTarget target = conf.CompileTarget;
			
			// Headers and resources
			foreach (ProjectFile f in Files) {
				if (f.BuildAction == BuildAction.Content) {
					string targetDirectory =
						(IsHeaderFile (f.Name) ? TargetDirectory.Include : TargetDirectory.ProgramFiles);
					
					deployFiles.Add (new DeployFile (this, f.FilePath, f.ProjectVirtualPath, targetDirectory));
				}
			}
			
			// Output
			string output = GetOutputFileName (configuration);		
			if (!string.IsNullOrEmpty (output)) {
				string targetDirectory = string.Empty;
				
				switch (target) {
				case CompileTarget.Exe:
					targetDirectory = TargetDirectory.ProgramFiles;
					break;
				case CompileTarget.Module:
					targetDirectory = TargetDirectory.ProgramFiles;
					break;
				case CompileTarget.Library:
					targetDirectory = TargetDirectory.ProgramFiles;
					break;
				}					
				
				deployFiles.Add (new DeployFile (this, output, Path.GetFileName (output), targetDirectory));
			}
			
			// PkgPackage
			if (target != CompileTarget.Exe) {
				string pkgfile = WriteDeployablePgkPackage (this, conf);
				deployFiles.Add (new DeployFile (this, Path.Combine (BaseDirectory, pkgfile), pkgfile, LinuxTargetDirectory.PkgConfig));
			}
			
			return deployFiles;
		}
		
		/// <summary>
		/// Finds the corresponding source or header file
		/// </summary>
		/// <param name="sourceFile">
		/// The name of the file to be matched
		/// <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// The corresponding file, or null if not found
		/// <see cref="System.String"/>
		/// </returns>
		public string MatchingFile (string sourceFile) {
			string filenameStub = Path.GetFileNameWithoutExtension (sourceFile);
			bool wantHeader = !CProject.IsHeaderFile (sourceFile);
			
			foreach (ProjectFile file in this.Files) {
				if (filenameStub == Path.GetFileNameWithoutExtension (file.Name) 
				   && (wantHeader == IsHeaderFile (file.Name))) {
					return file.Name;
				}
			}
			
			return null;
		}
	}
}
