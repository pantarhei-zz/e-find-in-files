using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace FindInFiles
{
	/// <summary>
	/// Represents all the lines in a file which match the given options
	/// </summary>
	static class LineMatcher
	{
		static IStringScanner CreateScanner(FindLineOptions options)
		{
			if (options.UseRegex)
				return new RegexScanner(options.Pattern, options.Replacement, options.MatchCase);
			else
				return new TextScanner(options.Pattern, options.Replacement, options.MatchCase);
		}

		// worker function which scans all the files and matches all the lines
		public static IEnumerable<Match> Filter(IEnumerable<string> files, FindLineOptions options )
		{
			var scanner = CreateScanner(options);
			bool performReplace = options.Replacement != null; 

			foreach(string file in files)
			{
				bool fileWasModified = false;
				string[] lines = File.ReadAllLines(file); // make this read in chunks rather than all at once
				
				// Scan each line and yield a match if found
				for (int lineNumber = 0; lineNumber < lines.Length; ++lineNumber)
				{
					var ranges = performReplace ? 
						scanner.ScanAndReplace(lines[lineNumber], (s) => lines[lineNumber] = s).AsCounted() :
						scanner.Scan(lines[lineNumber]).AsCounted();

					foreach( var range in ranges )
						yield return new Match( file, lineNumber + 1, lines[lineNumber], range );

					if (ranges.Count > 0 && performReplace )
					{
						fileWasModified = true;
					}
				}

				if( fileWasModified ) // write the file back to disk if we did anything
					File.WriteAllLines(file, lines);
			}
		}
	}
}
