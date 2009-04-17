using System;

namespace FindInFiles
{
    /// <summary>
    /// Actually does the finding
    /// </summary>
    class Finder
    {
        private readonly FindLineOptions lineOptions;
        private readonly FindFileOptions fileOptions;

        public event Action<string> FileScanned;

        public Finder(FindFileOptions fileOptions, FindLineOptions lineOptions)
        {
            this.fileOptions = fileOptions;
            this.lineOptions = lineOptions;
        }

        private void FireFileScanned(string text)
        {
            var scanned = FileScanned;
            if (scanned != null)
                scanned(text);
        }

        public void Find()
        {
            var files = FileMatcher.Filter(fileOptions).AsCounted();
            var matches = LineMatcher.Filter(files, lineOptions).AsCounted();

            IOutputMatches h = new HtmlOutputter(lineOptions.Pattern, fileOptions.Directory, files, matches);
            h.OutputHeader();
            foreach (var match in matches)
                h.OutputMatch(match);
            h.OutputFooter();
        }
    }
}
