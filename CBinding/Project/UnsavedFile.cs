using System;

namespace CBinding
{
	public class UnsavedFile {
		public bool IsDirty { get; set; }
		public string Text { get; set; }

		public UnsavedFile (bool dirtyness, string text) 
		{
			IsDirty = dirtyness;
			Text = text;
		}
	}
	
}
