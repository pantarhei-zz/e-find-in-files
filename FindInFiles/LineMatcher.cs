using System;
using System.Collections.Generic;
using System.IO;

namespace FindInFiles
{
	/// <summary>
	/// Represents all the lines in a file which match the given options
	/// </summary>
	class LineMatcher
	{
	    private readonly FindLineOptions options;

	    public LineMatcher(FindLineOptions options)
        {
            this.options = options;
        }

        public IEnumerable<Match> Filter(IEnumerable<string> filenames)
        {
            var scanner = options.CreateScanner();
            bool performReplace = options.Replacement != null;

            foreach (string filename in filenames)
            {
                bool fileWasModified = false;
                string[] lines = File.ReadAllLines(filename); // make this read in chunks rather than all at once

                // Scan each line and yield a match if found
                for (int lineNumber = 0; lineNumber < lines.Length; ++lineNumber)
                {
                    int number = lineNumber;
                    var ranges = performReplace ?
                        scanner.ScanAndReplace(lines[lineNumber], s => lines[number] = s).AsCounted() :
                        scanner.Scan(lines[lineNumber]).AsCounted();

                    foreach (var range in ranges)
                        yield return new Match(filename, lineNumber + 1, lines[lineNumber], range);

                    if (ranges.Count > 0 && performReplace)
                        fileWasModified = true;
                }

                if (fileWasModified) // write the file back to disk if we did anything
                    File.WriteAllLines(filename, lines);
            }
        }
	}
}
