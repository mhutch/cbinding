using System;
using CBinding;
using ClangSharp;
using MonoDevelop.Ide;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using CBinding.Refactoring;
using CBinding.Parser;
using System.IO;
using MonoDevelop.Ide.CodeCompletion;

namespace CBinding
{
	/// <summary>
	/// Class to manage clang Translation units, thread safe, but the dictionary exposed
	/// and the translation units itself are not.
	/// For more information see the field SyncRoot
	/// </summary>
	public class CLangManager : IDisposable
	{
		/// <summary>
		/// The sync root.
		/// I couldn't find any information about libclang's internal threading solutions,
		/// the best i could find were some stackoverflow topic and a short mailing list discussion, where only assumptions were made.
		/// Lock on this is needed because clangmanager handles getting cursors,
		/// cursor references, and cursors between files sometimes are a mess and
		/// can only identified by its USR, and e.g. finding a cursor by its USR
		/// while reparsing is in progress in an other file could result in a fault.
		/// </summary>
		public readonly object SyncRoot = new object ();
		CProject project;
		CXIndex index;
		Dictionary<string, CXTranslationUnit> translationUnits { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="proj">
		/// A <see cref="CProject"/> reference: project which the manager manages
		/// </param>
		public CLangManager (CProject proj)
		{
			project = proj;
			index = clang.createIndex (0, 0);
			translationUnits = new Dictionary<string, CXTranslationUnit> ();
			project.DefaultConfigurationChanged += CompilerArgumentsUpdate;
		}

		/// <summary>
		/// Creates a new or gives back a previously created translation unit
		/// </summary>
		/// <param name="proj">
		/// A <see cref="CProject"/> reference: the project which the TU is associated. Contains the compiler arguments, in its configurations
		/// </param>
		/// <param name="fileName">
		/// A <see cref="string"/>: The filename associated with the translation unit. Basically the source file's name
		/// </param>
		/// <param name = "unsavedFiles">
		/// A <see cref="CXUnsavedFile"/> array: array with the contents of unsaved files in IDE. Safe to be a null sized array - CDocumentParser.Parse reparses the TU with properly initialized unsaved files.
		/// </param>
		/// <returns>
		/// A <see cref="CXTranslationUnit"/>: The Translation unit created
		/// </returns>
		public CXTranslationUnit CreateTranslationUnit (CProject proj, string fileName, CXUnsavedFile[] unsavedFiles) 
		{
			lock (SyncRoot) {
				if (translationUnits.ContainsKey (fileName)) {
					return translationUnits [fileName];
				} else {
					AddToTranslationUnits (proj, fileName, unsavedFiles);
					return translationUnits [fileName];
				}
			}
		}

		/// <summary>
		/// Does the "real" translation unit creating, adds it to TranslationUnits collection, from which its later available.
		/// </summary>
		/// <param name="proj">
		/// A <see cref="CProject"/> reference: the project which the TU is associated. Contains the compiler arguments, in its configurations
		/// </param>
		/// <param name="fileName">
		/// A <see cref="string"/>: The filename associated with the translation unit. Basically the source file's name
		/// </param>
		/// <param name = "unsavedFiles">
		/// A <see cref="CXUnsavedFile"/> array: array with the contents of unsaved files in IDE. Safe to be a null sized array - CDocumentParser.Parse reparses the TU with properly initialized unsaved files.
		/// </param>
		void AddToTranslationUnits (CProject proj, string fileName, CXUnsavedFile[] unsavedFiles)
		{
			lock (SyncRoot) {
				var compiler = new ClangCCompiler ();
				var active_configuration =
					(CProjectConfiguration)proj.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);
				string[] args = compiler.GetCompilerFlagsAsArray (proj, active_configuration);
				try {
					translationUnits.Add (fileName, clang.createTranslationUnitFromSourceFile (
						index,
						fileName,
						args.Length,
						args,
						(uint) (unsavedFiles.Length),
						unsavedFiles)
					);
					UpdateDatabase (proj, fileName, translationUnits[fileName]);
				} catch (ArgumentException) {
					Console.WriteLine (fileName + " is already added, not adding");
				}
			}
		}

		/// <summary>
		/// Updates Symbol database associated with the fileName
		/// </summary>
		/// <param name="proj">
		/// A <see cref="CProject"/> reference: the project which the symbol database is associated.
		/// </param>
		/// <param name="fileName">
		/// A <see cref="string"/>: The filename associated with the symbol database. Basically the source file's name
		/// </param>
		/// <param name = "TU">
		/// A <see cref="CXTranslationUnit"/>: the translation unit which's parsed content fills the symbol database
		/// </param>
		/// <param name = "cancellationToken"></param>
		public void UpdateDatabase (CProject proj, string fileName, CXTranslationUnit TU, CancellationToken cancellationToken = default(CancellationToken))
		{
			lock (SyncRoot) {
				proj.DB.Reset (fileName);
				CXCursor TUcursor = clang.getTranslationUnitCursor (TU);
				var parser = new TranslationUnitParser (proj.DB, fileName, cancellationToken, TUcursor);
				clang.visitChildren (TUcursor, parser.Visit, new CXClientData (new IntPtr (0)));
			}
		}

		/// <summary>
		/// Removes a translation unit from the collection and disposes its unmanaged resources.
		/// </summary>
		/// <param name="proj">
		/// A <see cref="CProject"/> reference: the project which the TU is associated.
		/// </param>
		/// <param name="fileName">
		/// A <see cref="string"/>: The filename associated with the TU. Basically the source file's name
		/// </param>
		public void RemoveTranslationUnit (CProject proj, string fileName)
		{
			lock (SyncRoot) {
				clang.disposeTranslationUnit (translationUnits [fileName]);
				translationUnits.Remove (fileName);
			}
		}

		/// <summary>
		/// Update Translation units with the correct compiler arguments. Subscribe to event: Project.DefaultConfigurationChanged
		/// </summary>
		public void CompilerArgumentsUpdate (object sender, EventArgs args) 
		{
			lock (SyncRoot) {
				if (project.Loading)
					//on project load its unnecessary to update this, because creating the TU's are already
					//done with the first active configuration - also doing so sometimes results in an exception
					return;
				var compiler = new ClangCCompiler ();
				var active_configuration =
					(CProjectConfiguration)project.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);
				string[] compilerArgs = compiler.GetCompilerFlagsAsArray (project, active_configuration);
				foreach (var TU in translationUnits) {
					clang.disposeTranslationUnit (translationUnits [TU.Key]);
					translationUnits [TU.Key] = clang.parseTranslationUnit (
						index,
						TU.Key,
						compilerArgs,
						compilerArgs.Length,
						//CDocumentParser.Parse will reparse with unsaved files, risky to get them from here
						null,
						0,
						clang.defaultEditingTranslationUnitOptions ()
					);
				}
			}
		}

		/// <summary>
		/// Code completion wrapper to expose clang_codeCompleteAt and handle threading locks-issues.
		/// The caller should dispose the returned IntPtr with clang.disposeCodeCompleteResults ()
		/// </summary>
		/// <param name="completionContext">
		/// A <see cref="DocumentContext"/> reference: the document context of the code completion request.
		/// </param>
		/// <param name="unsavedFiles">
		/// A <see cref="CXUnsavedFile"/> array: The unsaved files in the IDE. Obligatory to have valid suggestions.
		/// </param>
		/// <param name = "fileName"></param>
		/// <returns>IntPtr which should be marshalled as CXCodeCompleteResults</returns>
		public IntPtr CodeComplete (
			CodeCompletionContext completionContext,
			CXUnsavedFile[] unsavedFiles,
			string fileName)
		{
			lock (SyncRoot) {
				string name = fileName;
				CXTranslationUnit TU = translationUnits [name];
				string complete_filename = fileName;
				uint complete_line = (uint) (completionContext.TriggerLine);
				uint complete_column = (uint) (completionContext.TriggerLineOffset + 1);
				uint numUnsavedFiles = (uint) (unsavedFiles.Length);
				uint options = (uint) CXCodeComplete_Flags.IncludeCodePatterns | (uint)CXCodeComplete_Flags.IncludeCodePatterns;
				return clang.codeCompleteAt (
									  TU,
									  complete_filename, 
									  complete_line, 
									  complete_column, 
									  unsavedFiles, 
									  numUnsavedFiles, 
									  options);
			}
		}

		/// <summary>
		/// Gets a cursor
		/// </summary>
		/// <param name="fileName">
		/// A <see cref="string"/>: the filename which a Translation Unit (probably containing the cursor) is associated with.
		/// </param>
		/// <param name="location">
		/// A <see cref="DocumentLocation"/>: the location in the document (named fileName)
		/// </param>
		/// <returns>
		/// A <see cref="CXCursor"/>: the cursor under the location
		/// </returns>
		public CXCursor GetCursor (string fileName, MonoDevelop.Ide.Editor.DocumentLocation location)
		{
			lock (SyncRoot) {
				CXTranslationUnit TU = translationUnits [fileName];
				CXFile file = clang.getFile (TU, fileName);
				CXSourceLocation loc = clang.getLocation (
					TU,
					file,
					(uint) (location.Line),
					(uint) (location.Column)
				);
				return clang.getCursor (TU, loc);
			}
		}

		/// <summary>
		/// Gets the cursor refenced by refereeCursor. If the cursor is a declaration/definition, returns itself.
		/// </summary>
		/// <param name="refereeCursor">
		/// A <see cref="CXCursor"/>: a cursor referencing another
		/// </param>
		/// <returns>
		/// A <see cref="CXCursor"/>: the cursor referenced
		/// </returns>
		public CXCursor GetCursorReferenced (CXCursor refereeCursor)
		{
			lock (SyncRoot) {
				return clang.getCursorReferenced (refereeCursor);
			}
		}

		/// <summary>
		/// Gets the definition of a cursor
		/// </summary>
		/// <param name="cursor">
		/// A <see cref="CXCursor"/>: a cursor
		/// </param>
		/// <returns>
		/// A <see cref="CXCursor"/>: the defining cursor
		/// </returns>
		public CXCursor GetCursorDefinition (CXCursor cursor)
		{
			lock (SyncRoot) {
				return clang.getCursorDefinition (cursor);
			}
		}

		/// <summary>
		/// Gets the location of a cursor. The location points somewhere in a source file (Can be in an unsaved file's contents only present in the editor!).
		/// </summary>
		/// <param name="cursor">
		/// A <see cref="CXCursor"/>: a cursor
		/// </param>
		/// <returns>
		/// A <see cref="SourceLocation"/>: the location of the cursor
		/// </returns>
		public SourceLocation GetCursorLocation (CXCursor cursor)
		{
			lock (SyncRoot) {
				CXSourceLocation loc = clang.getCursorLocation (cursor);
				CXFile file;
				uint line, column, offset;
				clang.getExpansionLocation (loc, out file, out line, out column, out offset);
				var fileName = GetFileNameString (file);
				try {
					if (project.IsBomPresentInFile (fileName)) {
						if(line == 1) //if its in the first line, align column and offset too
							return new SourceLocation (fileName, line, column - 3, offset - 3);
						//else column is good as it is, only align offset
						return new SourceLocation (fileName, line, column, offset - 3);
					}
				} catch (KeyNotFoundException) { //if key is not found it means the file is an included, non-project file
					using (var s = new FileStream (fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
						var BOM = new byte[3];
						s.Read (BOM, 0, 3);
						if (!project.IsBomPresentInFile (fileName)) {
							project.BomPresentInFile (fileName, false);
						}					
						if (BOM [0] == 0xEF && BOM [1] == 0xBB && BOM [2] == 0xBF) {
							project.BomPresentInFile(fileName, true);
						} else {
							project.BomPresentInFile(fileName, false);
						}
					}
				}
				return new SourceLocation (fileName, line, column, offset);
			}
		}

		/// <summary>
		/// Translate clang CXSourceLocation to SourceLocation. Contains dealing with UTF-8 Byte order marking.
		/// </summary>
		/// <param name="loc">
		/// A <see cref="CXSourceLocation"/>: a location
		/// </param>
		/// <returns>
		/// A <see cref="SourceLocation"/>: the translated location
		/// </returns>
		public SourceLocation GetSourceLocation (CXSourceLocation loc)
		{
			lock (SyncRoot) {
				CXFile file;
				uint line, column, offset;
				clang.getExpansionLocation (loc, out file, out line, out column, out offset);
				var fileName = GetFileNameString (file);
				project.CheckForBom (fileName);
				if (project.IsBomPresentInFile (fileName)) {
					if(line == 1) //if its in the first line, align column and offset too
						return new SourceLocation (fileName, line, column - 3, offset - 3);
					//else column is good as it is, only align offset
					return new SourceLocation (fileName, line, column, offset - 3);
				}
				return new SourceLocation (fileName, line, column, offset);
			}
		}


		/// <summary>
		/// Finds references through the given visitor. Traverses the whole AST in all translation units.
		/// </summary>
		/// <param name="visitor">
		/// A <see cref="FindReferencesHandler"/>: a visitor
		/// </param>
		public void FindReferences (FindReferencesHandler visitor)
		{
			lock (SyncRoot) {
				foreach (var T in translationUnits) {
					clang.visitChildren (
						clang.getTranslationUnitCursor (T.Value),
						visitor.Visit,
						new CXClientData (new IntPtr (0))
					);
				}
			}
		}

		/// <summary>
		/// Find references and rename them with visitor. Traverses the whole AST in all translation units.
		/// </summary>
		/// <param name="visitor">
		/// A <see cref="RenameHandlerDialog"/>: a visitor
		/// </param>
		public void FindReferences(RenameHandlerDialog visitor)
		{
			lock (SyncRoot) {
				foreach (var T in translationUnits) {
					clang.visitChildren (
						clang.getTranslationUnitCursor (T.Value),
						visitor.Visit,
						new CXClientData (new IntPtr (0))
					);
				}
			}
		}

		/// <summary>
		/// Gets the spelling of a cursor. E.g.: a functions's spelling: int foo(char) ---> fo
		/// </summary>
		/// <param name="cursor">
		/// A <see cref="CXCursor"/>: a cursor
		/// </param>
		/// <returns>
		/// A <see cref="string"/>: the cursor's spelling
		/// </returns>
		public string GetCursorSpelling (CXCursor cursor)
		{
			lock(SyncRoot) {
				CXString cxstring = clang.getCursorSpelling (cursor);
				string spelling = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
				clang.disposeString (cxstring);
				return spelling;
			}
		}

		/// <summary>
		/// Gets the display name of a cursor. E.g.: a functions's display name its whole signature
		/// </summary>
		/// <param name="cursor">
		/// A <see cref="CXCursor"/>: a cursor
		/// </param>
		/// <returns>
		/// A <see cref="string"/>: the cursor's display name
		/// </returns>
		public string GetCursorDisplayName (CXCursor cursor)
		{
			lock(SyncRoot) {
				CXString cxstring = clang.getCursorDisplayName (cursor);
				string spelling = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
				clang.disposeString (cxstring);
				return spelling;
			}
		}

		/// <summary>
		/// Gets the Unified Symbol Resolution (USR) of a cursor.
		/// </summary>
		/// <param name="cursor">
		/// A <see cref="CXCursor"/>: a cursor
		/// </param>
		/// <returns>
		/// A <see cref="string"/>: the USR string
		/// </returns>
		public string GetCursorUsrString (CXCursor cursor)
		{
			lock (SyncRoot) {
				CXString cxstring = clang.getCursorUSR (cursor);
				string usr = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
				clang.disposeString (cxstring);
				return usr;
			}
		}

		/// <summary>
		/// Gets the filename if a given CXFile
		/// </summary>
		/// <param name="file">
		/// A <see cref="CXFile"/>: a CXFile instance
		/// </param>
		/// <returns>
		/// A <see cref="string"/>: the filename
		/// </returns>
		public string GetFileNameString (CXFile file)
		{
			lock (SyncRoot) {
				CXString cxstring = clang.getFileName (file);
				string fileName = Marshal.PtrToStringAnsi (clang.getCString (cxstring));
				clang.disposeString (cxstring);
				return fileName;
			}
		}

		/// <summary>
		/// Gets the type of a cursor. Wraps clang function.
		/// </summary>
		/// <param name="cursor">
		/// A <see cref="CXCursor"/>: cursor to inspect
		/// </param>
		/// <returns>
		/// A <see cref="CXType"/>: the type
		/// </returns>
		public CXType GetCursorType (CXCursor cursor)
		{
			lock (SyncRoot) {
				return clang.getCursorType (cursor);
			}
		}

		protected virtual void OnDispose(bool disposing)
		{
			lock (SyncRoot) {
				if (disposing) {
					project.DefaultConfigurationChanged -= CompilerArgumentsUpdate;
					foreach (CXTranslationUnit unit in translationUnits.Values)
						clang.disposeTranslationUnit (unit);
					clang.disposeIndex (index);
				}
			}
		}

		~CLangManager()
		{
			OnDispose(false);
		}

		#region IDisposable implementation

		void IDisposable.Dispose ()
		{
			OnDispose(true); 
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}

