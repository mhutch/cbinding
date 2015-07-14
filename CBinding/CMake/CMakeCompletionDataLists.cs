//
// CMakeTextEditorExtension.cs
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

using System.Collections.Generic;

using MonoDevelop.Core;

namespace CBinding
{
	public static class CMakeCompletionDataLists
	{

		public class CMakeCompletionData : MonoDevelop.Ide.CodeCompletion.CompletionData
		{
			public CMakeCompletionData (string text, IconId icon) : base (text, icon)
			{
			}
		}

		public static readonly List<CMakeCompletionData> Commands = new List<CMakeCompletionData> () {
			new CMakeCompletionData ("add_compile_options", new IconId ("md-method")),
			new CMakeCompletionData ("add_custom_command", new IconId ("md-method")),
			new CMakeCompletionData ("add_custom_target", new IconId ("md-method")),
			new CMakeCompletionData ("add_definitions", new IconId ("md-method")),
			new CMakeCompletionData ("add_dependencies", new IconId ("md-method")),
			new CMakeCompletionData ("add_executable", new IconId ("md-method")),
			new CMakeCompletionData ("add_library", new IconId ("md-method")),
			new CMakeCompletionData ("add_subdirectory", new IconId ("md-method")),
			new CMakeCompletionData ("add_test", new IconId ("md-method")),
			new CMakeCompletionData ("aux_source_directory", new IconId ("md-method")),
			new CMakeCompletionData ("break", new IconId ("md-method")),
			new CMakeCompletionData ("build_command", new IconId ("md-method")),
			new CMakeCompletionData ("cmake_host_system_information", new IconId ("md-method")),
			new CMakeCompletionData ("cmake_minimum_required", new IconId ("md-method")),
			new CMakeCompletionData ("cmake_policy", new IconId ("md-method")),
			new CMakeCompletionData ("configure_file", new IconId ("md-method")),
			new CMakeCompletionData ("create_test_sourcelist", new IconId ("md-method")),
			new CMakeCompletionData ("define_property", new IconId ("md-method")),
			new CMakeCompletionData ("elseif", new IconId ("md-method")),
			new CMakeCompletionData ("else", new IconId ("md-method")),
			new CMakeCompletionData ("enable_language", new IconId ("md-method")),
			new CMakeCompletionData ("enable_testing", new IconId ("md-method")),
			new CMakeCompletionData ("endforeach", new IconId ("md-method")),
			new CMakeCompletionData ("endfunction", new IconId ("md-method")),
			new CMakeCompletionData ("endif", new IconId ("md-method")),
			new CMakeCompletionData ("endmacro", new IconId ("md-method")),
			new CMakeCompletionData ("endwhile", new IconId ("md-method")),
			new CMakeCompletionData ("execute_process", new IconId ("md-method")),
			new CMakeCompletionData ("export", new IconId ("md-method")),
			new CMakeCompletionData ("file", new IconId ("md-method")),
			new CMakeCompletionData ("find_file", new IconId ("md-method")),
			new CMakeCompletionData ("find_library", new IconId ("md-method")),
			new CMakeCompletionData ("find_package", new IconId ("md-method")),
			new CMakeCompletionData ("find_path", new IconId ("md-method")),
			new CMakeCompletionData ("find_program", new IconId ("md-method")),
			new CMakeCompletionData ("fltk_wrap_ui", new IconId ("md-method")),
			new CMakeCompletionData ("foreach", new IconId ("md-method")),
			new CMakeCompletionData ("function", new IconId ("md-method")),
			new CMakeCompletionData ("get_cmake_property", new IconId ("md-method")),
			new CMakeCompletionData ("get_directory_property", new IconId ("md-method")),
			new CMakeCompletionData ("get_filename_component", new IconId ("md-method")),
			new CMakeCompletionData ("get_property", new IconId ("md-method")),
			new CMakeCompletionData ("get_source_file_property", new IconId ("md-method")),
			new CMakeCompletionData ("get_target_property", new IconId ("md-method")),
			new CMakeCompletionData ("get_test_property", new IconId ("md-method")),
			new CMakeCompletionData ("if", new IconId ("md-method")),
			new CMakeCompletionData ("include_directories", new IconId ("md-method")),
			new CMakeCompletionData ("include_external_msproject", new IconId ("md-method")),
			new CMakeCompletionData ("include_regular_expression", new IconId ("md-method")),
			new CMakeCompletionData ("include", new IconId ("md-method")),
			new CMakeCompletionData ("install", new IconId ("md-method")),
			new CMakeCompletionData ("link_directories", new IconId ("md-method")),
			new CMakeCompletionData ("list", new IconId ("md-method")),
			new CMakeCompletionData ("load_cache", new IconId ("md-method")),
			new CMakeCompletionData ("load_command", new IconId ("md-method")),
			new CMakeCompletionData ("macro", new IconId ("md-method")),
			new CMakeCompletionData ("mark_as_advanced", new IconId ("md-method")),
			new CMakeCompletionData ("math", new IconId ("md-method")),
			new CMakeCompletionData ("message", new IconId ("md-method")),
			new CMakeCompletionData ("option", new IconId ("md-method")),
			new CMakeCompletionData ("project", new IconId ("md-method")),
			new CMakeCompletionData ("qt_wrap_cpp", new IconId ("md-method")),
			new CMakeCompletionData ("qt_wrap_ui", new IconId ("md-method")),
			new CMakeCompletionData ("remove_definitions", new IconId ("md-method")),
			new CMakeCompletionData ("return", new IconId ("md-method")),
			new CMakeCompletionData ("separate_arguments", new IconId ("md-method")),
			new CMakeCompletionData ("set_directory_properties", new IconId ("md-method")),
			new CMakeCompletionData ("set_property", new IconId ("md-method")),
			new CMakeCompletionData ("set", new IconId ("md-method")),
			new CMakeCompletionData ("set_source_files_properties", new IconId ("md-method")),
			new CMakeCompletionData ("set_target_properties", new IconId ("md-method")),
			new CMakeCompletionData ("set_tests_properties", new IconId ("md-method")),
			new CMakeCompletionData ("site_name", new IconId ("md-method")),
			new CMakeCompletionData ("source_group", new IconId ("md-method")),
			new CMakeCompletionData ("string", new IconId ("md-method")),
			new CMakeCompletionData ("target_compile_definitions", new IconId ("md-method")),
			new CMakeCompletionData ("target_compile_options", new IconId ("md-method")),
			new CMakeCompletionData ("target_include_directories", new IconId ("md-method")),
			new CMakeCompletionData ("target_link_libraries", new IconId ("md-method")),
			new CMakeCompletionData ("try_compile", new IconId ("md-method")),
			new CMakeCompletionData ("try_run", new IconId ("md-method")),
			new CMakeCompletionData ("unset", new IconId ("md-method")),
			new CMakeCompletionData ("variable_watch", new IconId ("md-method")),
			new CMakeCompletionData ("while", new IconId ("md-method"))
		};
	}
}
