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
	public enum CVersion {
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
		[ItemProperty("OutputName", DefaultValue = "")]
		public string Output { get; set; }

		/// <summary>
		/// The compile target.
		/// </summary>
		[ItemProperty("OutputType", DefaultValue = CompileTarget.Exe)]
		public CompileTarget CompileTarget { get; set; }

		[ItemProperty ("Includes")]
		[ItemProperty ("Include", Scope = "*", ValueType = typeof(string))]
    	ArrayList includes = new ArrayList ();
		
		[ItemProperty ("LibPaths")]
		[ItemProperty ("LibPath", Scope = "*", ValueType = typeof(string))]
    	ArrayList libpaths = new ArrayList ();
		
		[ItemProperty ("Libs")]
		[ItemProperty ("Lib", Scope = "*", ValueType = typeof(string))]
    	ArrayList libs = new ArrayList ();

		/// <summary>
		/// The C/C++ standard version in use.
		/// </summary>
		[ItemProperty ("CVersion", DefaultValue = CVersion.CustomVersionString)]
		public CVersion CVersion{ get; set; }

		/// <summary>
		/// The custom version string.
		/// </summary>
		[ItemProperty ("CustomCVersionString", DefaultValue = "")]
		public string CustomVersionString { get; set; }

		/// <summary>
		/// The warning level.
		/// </summary>
		[ItemProperty ("WarningLevel", DefaultValue = WarningLevel.Normal)]
		public WarningLevel WarningLevel { get; set; }

		/// <summary>
		/// Specifies if warnings should be treated as errors or not.
		/// </summary>
		[ItemProperty ("WarningsAsErrors", DefaultValue = false)]
		public bool WarningsAsErrors { get; set; }

		/// <summary>
		/// The optimization level.
		/// </summary>
		[ItemProperty ("OptimizationLevel", DefaultValue = 0)]
		int optimization;

		/// <summary>
		/// Extra compiler arguments given by user.
		/// </summary>
		[ItemProperty ("ExtraCompilerArguments", DefaultValue = "")]
		public string ExtraCompilerArguments { get; set; }

		/// <summary>
		/// Extra linker arguments given by user.
		/// </summary>
		[ItemProperty ("ExtraLinkerArguments", DefaultValue = "")]
		public string ExtraLinkerArguments { get; set; }

		[ItemProperty ("DefineSymbols", DefaultValue = "")]
		public string DefineSymbols { get; set; }

		[ProjectPathItemProperty ("SourceDirectory", DefaultValue = "")]
		public string SourceDirectory { get; set; }

		[ItemProperty ("UseCcache", DefaultValue = false)]
		public bool UseCcache { get; set; }

		[ItemProperty ("PrecompileHeaders", DefaultValue = true)]
		public bool PrecompileHeaders { get; set; }

		public ArrayList Includes {
			get { return includes; }
			set { includes = value; }
		}

		public ArrayList LibPaths {
			get { return libpaths; }
			set { libpaths = value; }
		}
			
		public ArrayList Libs {
			get { return libs; }
			set { libs = value; }
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

				//TODO Win&Mac naming
				switch (CompileTarget) {
				case CompileTarget.Exe:
					break;
				case CompileTarget.Module:
					if (!Output.StartsWith ("lib"))
						prefix = "lib";
					if (!Output.EndsWith (".a"))
						suffix = ".a";
					break;
				case CompileTarget.Library:
					if (!Output.StartsWith ("lib"))
						prefix = "lib";
					if (!Output.EndsWith (".so"))
						suffix = ".so";
					break;
				}
				
				return string.Format("{0}{1}{2}", prefix, Output, suffix);
			}
		}

		public int OptimizationLevel {
			get { return optimization; }
			set {
				if (value >= 0 && value <= 3)
					optimization = value;
				else
					optimization = 0;
			}
		}
			
		public override void CopyFrom (ItemConfiguration configuration)
		{
			base.CopyFrom (configuration);
			CProjectConfiguration conf = (CProjectConfiguration)configuration;
			
			Output = conf.Output;
			CompileTarget = conf.CompileTarget;
			Includes = conf.Includes;
			LibPaths = conf.LibPaths;
			Libs = conf.Libs;
			SourceDirectory = conf.SourceDirectory;
			UseCcache = conf.UseCcache;
			CVersion = conf.CVersion;
			CustomVersionString = conf.CustomVersionString;
			WarningLevel = conf.WarningLevel;
			WarningsAsErrors = conf.WarningsAsErrors;
			OptimizationLevel = conf.OptimizationLevel;
			ExtraCompilerArguments = conf.ExtraCompilerArguments;
			ExtraLinkerArguments = conf.ExtraLinkerArguments;
			DefineSymbols = conf.DefineSymbols;
		}
	}
}
