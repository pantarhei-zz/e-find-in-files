using System;
using System.Diagnostics;

namespace FindInFiles
{
    /// <summary>
	/// Actually does the finding
	/// </summary>
	class Finder
	{
        private readonly FindLineOptions FindLineOptions;
        private readonly FindFileOptions FindFileOptions;

		public event Action<string> FileScanned;

        public Finder(FindFileOptions findFileOptions, FindLineOptions findLineOptions)
		{
			Debug.Assert(findFileOptions != null);
			Debug.Assert(findLineOptions != null);

			FindFileOptions = findFileOptions;
			FindLineOptions = findLineOptions;
		}

		private void FireFileScanned(string text)
		{
		    var scanned = FileScanned;
		    if (scanned != null)
				scanned(text);
		}

        public void Find()
		{
		    var files = FileMatcher.Filter(FindFileOptions).AsCounted();
		    var matches = LineMatcher.Filter(files, FindLineOptions).AsCounted();

		    IOutputMatches h = new HtmlOutputter(FindLineOptions.Pattern, FindFileOptions.Directory, files, matches);
		    h.OutputHeader();
		    foreach (var match in matches)
		        h.OutputMatch(match);
		    h.OutputFooter();
		}
	}
}
