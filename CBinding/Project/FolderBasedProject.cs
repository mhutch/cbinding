using System.Collections.Generic;

using MonoDevelop.Projects;
using MonoDevelop.Core;

namespace CBinding
{
	public class FolderBasedProject : SolutionItem
	{
		virtual public List<FilePath> GetExcludedPaths ()
		{
			return new List<FilePath> ();
		}

		virtual public string [] GetBuildActions ()
		{
			return BuildAction.StandardActions;
		}

		virtual public string [] OnGetSupportedLanguages ()
		{
			return new string [] { string.Empty };
		}

		virtual public void OnFileCopied (FilePath src, FilePath dst)
		{
		}

		virtual public void OnFileMoved (FilePath src, FilePath dst)
		{
		}

		virtual public void OnFilesCopied (List<FilePath> src, List<FilePath> dst)
		{
		}

		virtual public void OnFilesMoved (List<FilePath> src, List<FilePath> dst)
		{
		}

		virtual public void OnFileAdded (FilePath file)
		{
		}

		virtual public void OnFileRemoved (FilePath file)
		{
		}

		virtual public void OnFilesAdded (List<FilePath> files)
		{
		}

		virtual public void OnFilesRemoved (List<FilePath> files)
		{
		}

		virtual public void OnFileRenamed (FilePath oldFile, FilePath newFile)
		{
		}

		virtual public void OnFilesRenamed (List<FilePath> oldFiles, List<FilePath> newFiles)
		{
		}
	}
}
