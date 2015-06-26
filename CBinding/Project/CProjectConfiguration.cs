//
// CProjectConfiguration.cs: Configuration for C/C++ projects
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
using System.Collections;

using Mono.Addins;

using MonoDevelop.Projects;
using MonoDevelop.Core.Serialization;

namespace CBinding
{
	/// <summary>
	/// C/C++ standard version to use in configuration.
	/// </summary>
	public enum CVersion
	{
		CustomVersionString,
		ISOC,
		C99,
		C11,
		ISOCPP,
		CPP03,
		CPP11
	}

	// TODO: Warning levels should be compiler specific...
	/// <summary>
	/// Warning level.
	/// </summary>
	public enum WarningLevel {
		None,
		Normal,
		All
	}

	// I believe it would be in the C/C++ binding's best interest to let the configuration determine
	// which compiler to use... currently the project as a whole does this - which isn't necessarily as flexible
	// as some may require...
	public class CProjectConfiguration : ProjectConfiguration
	{
		/// <summary>
		/// The output name.
		/// </summary>
		[ItemProperty("OutputName")]
		string output = string.Empty;

		/// <summary>
		/// The compile target.
		/// </summary>
		[ItemProperty("OutputType")]
		CompileTarget target = CompileTarget.Exe;

		[ItemProperty ("Includes")]
		[ItemProperty ("Include", Scope = "*", ValueType = typeof(string))]
    	private ArrayList includes = new ArrayList ();
		
		[ItemProperty ("LibPaths")]
		[ItemProperty ("LibPath", Scope = "*", ValueType = typeof(string))]
    	private ArrayList libpaths = new ArrayList ();
		
		[ItemProperty ("Libs")]
		[ItemProperty ("Lib", Scope = "*", ValueType = typeof(string))]
    	private ArrayList libs = new ArrayList ();

		/// <summary>
		/// The C/C++ standard version in use.
		/// </summary>
		[ItemProperty ("CVersion")]
		private CVersion cVersion = CVersion.CustomVersionString;

		/// <summary>
		/// The custom version string.
		/// </summary>
		[ItemProperty ("CustomCVersionString", DefaultValue = "")]
		private string customVersionString = string.Empty;

		/// <summary>
		/// The warning level.
		/// </summary>
		[ItemProperty ("WarningLevel", DefaultValue=WarningLevel.Normal)]
		private WarningLevel warning_level = WarningLevel.Normal;

		/// <summary>
		/// Specifies if warnings should be treated as errors or not.
		/// </summary>
		[ItemProperty ("WarningsAsErrors", DefaultValue=false)]
		private bool warnings_as_errors = false;

		/// <summary>
		/// The optimization level.
		/// </summary>
		[ItemProperty ("OptimizationLevel", DefaultValue=0)]
		private int optimization = 0;

		/// <summary>
		/// Extra compiler arguments given by user.
		/// </summary>
		[ItemProperty ("ExtraCompilerArguments", DefaultValue="")]
		private string extra_compiler_args = string.Empty;

		/// <summary>
		/// Extra linker arguments given by user.
		/// </summary>
		[ItemProperty ("ExtraLinkerArguments", DefaultValue="")]
		private string extra_linker_args = string.Empty;
		
		[ItemProperty ("DefineSymbols", DefaultValue="")]
		private string define_symbols = string.Empty;
		
		[ProjectPathItemProperty ("SourceDirectory", DefaultValue=null)]
		private string source_directory_path;
		
		[ItemProperty ("UseCcache", DefaultValue=false)]
		private bool use_ccache = false;
		
		[ItemProperty ("PrecompileHeaders", DefaultValue=true)]
		private bool precompileHeaders = true;
		
		public string Output {
			get { return output; }
			set { output = value; }
		}
		
		public CompileTarget CompileTarget {
			get { return target; }
			set { target = value; }
		}

		// TODO: This should be revised to use the naming conventions depending on OS & compiler...
		/// <summary>
		/// Determines the name of the compiled output.
		/// </summary>
		/// <value>The name of the compiled output.</value>
		public string CompiledOutputName {
			get {
				string suffix = string.Empty;
				string prefix = string.Empty;
				
				switch (target)
				{
				case CompileTarget.Exe:
					break;
				case CompileTarget.Library:
					if (!Output.StartsWith ("lib"))
						prefix = "lib";
					if (!Output.EndsWith (".a"))
						suffix = ".a";
					break;
				case CompileTarget.Module:
					if (!Output.StartsWith ("lib"))
						prefix = "lib";
					if (!Output.EndsWith (".so"))
						suffix = ".so";
					break;
				}
				
				return string.Format("{0}{1}{2}", prefix, Output, suffix);
			}
		}

		/// <summary>
		/// Gets or sets the source directory.
		/// </summary>
		/// <value>The source directory.</value>
		public string SourceDirectory {
			get { return source_directory_path; }
			set { source_directory_path = value; }
		}

		/// <summary>
		/// Gets or sets the includes list.
		/// </summary>
		/// <value>The includes.</value>
		public ArrayList Includes {
			get { return includes; }
			set { includes = value; }
		}

		/// <summary>
		/// Gets or sets the library paths.
		/// </summary>
		/// <value>The lib paths.</value>
		public ArrayList LibPaths {
			get { return libpaths; }
			set { libpaths = value; }
		}

		/// <summary>
		/// Gets or sets the libraries.
		/// </summary>
		/// <value>The libs.</value>
		public ArrayList Libs {
			get { return libs; }
			set { libs = value; }
		}
		
		public bool UseCcache {
			get { return use_ccache; }
			set { use_ccache = value; }
		}
		
		public bool PrecompileHeaders {
			get { return precompileHeaders; }
			set { precompileHeaders = value; }
		}

		/// <summary>
		/// Gets or sets the C/C++ standard version.
		/// </summary>
		/// <value>The C version.</value>
		public CVersion CVersion {
			get { return cVersion; }
			set { cVersion = value; }
		}

		/// <summary>
		/// Gets or sets the custom version string.
		/// </summary>
		/// <value>The custom version string.</value>
		public string CustomVersionString {
			get {
				return customVersionString;
			}
			set {
				customVersionString = value;
			}
		}

		/// <summary>
		/// Gets or sets the warning level.
		/// </summary>
		/// <value>The warning level.</value>
		public WarningLevel WarningLevel {
			get { return warning_level; }
			set { warning_level = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CBinding.CProjectConfiguration"/> warnings as errors.
		/// </summary>
		/// <value><c>true</c> if warnings as errors; otherwise, <c>false</c>.</value>
		public bool WarningsAsErrors {
			get { return warnings_as_errors; }
			set { warnings_as_errors = value; }
		}

		/// <summary>
		/// Gets or sets the optimization level.
		/// </summary>
		/// <value>The optimization level.</value>
		public int OptimizationLevel {
			get { return optimization; }
			set {
				if (value >= 0 && value <= 3)
					optimization = value;
				else
					optimization = 0;
			}
		}

		/// <summary>
		/// Gets or sets the extra compiler arguments.
		/// </summary>
		/// <value>The extra compiler arguments.</value>
		public string ExtraCompilerArguments {
			get { return extra_compiler_args; }
			set { extra_compiler_args = value; }
		}

		/// <summary>
		/// Gets or sets the extra linker arguments.
		/// </summary>
		/// <value>The extra linker arguments.</value>
		public string ExtraLinkerArguments {
			get { return extra_linker_args; }
			set { extra_linker_args = value; }
		}
			
		public string DefineSymbols {
			get { return define_symbols; }
			set { define_symbols = value; }
		}

		/// <summary>
		/// "Copy constructor" for CProjectConfiguration
		/// </summary>
		/// <param name="configuration">Configuration.</param>
		public override void CopyFrom (ItemConfiguration configuration)
		{
			base.CopyFrom (configuration);
			CProjectConfiguration conf = (CProjectConfiguration)configuration;
			
			output = conf.output;
			target = conf.target;
			includes = conf.includes;
			libpaths = conf.libpaths;
			libs = conf.libs;
			source_directory_path = conf.source_directory_path;
			use_ccache = conf.use_ccache;
			cVersion = conf.cVersion;
			customVersionString = conf.customVersionString;
			warning_level = conf.warning_level;
			warnings_as_errors = conf.warnings_as_errors;
			optimization = conf.optimization;
			extra_compiler_args = conf.extra_compiler_args;
			extra_linker_args = conf.extra_linker_args;
			define_symbols = conf.define_symbols;
		}
	}
}
