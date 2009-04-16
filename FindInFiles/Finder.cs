using System;
using System.Diagnostics;

namespace FindInFiles
{
	/// <summary>
	/// Default implentation of range using an int
	/// </summary>
	class IntRange
	{
		public readonly int Lower, Upper;

		public IntRange(int lower, int upper)
		{
			Lower = lower;
			Upper = upper;
		}
	}

	/// <summary>
	/// Actually does the finding
	/// </summary>
	class Finder
	{
		public readonly FindLineOptions FindLineOptions;
		public readonly FindFileOptions FindFileOptions;

		public event ScanningFileCallback FileScanned;

		public delegate void ScanningFileCallback(string text);

		public Finder(FindFileOptions findFileOptions, FindLineOptions findLineOptions)
		{
			Debug.Assert(findFileOptions != null);
			Debug.Assert(findLineOptions != null);

			FindFileOptions = findFileOptions;
			FindLineOptions = findLineOptions;
		}

		private void FireFileScanned(string text)
		{
			if (FileScanned != null)
				FileScanned(text);
		}

		public void Find()
		{
			var files = FileMatcher.Filter( FindFileOptions ).AsCounted();
			var matches = LineMatcher.Filter( files, FindLineOptions ).AsCounted();

			using( var h = new HtmlOutputter( FindLineOptions.Pattern, FindFileOptions.Directory, files, matches ) )
			{
				foreach( var match in matches )
					h.Write( match );
			}
		}
	}
}
