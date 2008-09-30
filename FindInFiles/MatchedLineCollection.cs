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
	class MatchedLineCollection : IEnumerable<MatchedLine>
	{
		public readonly FindLineOptions Options;
		public readonly IEnumerable<string> Files;

		public int Count { get; private set; }

		public MatchedLineCollection(FindLineOptions options, IEnumerable<string> files)
		{
			Debug.Assert(options != null);
			Debug.Assert(files != null);

			Options = options;
			Files = files;
		}

		public IEnumerator<MatchedLine> GetEnumerator()
		{
			return matchLines().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		static IStringScanner CreateScanner(FindLineOptions options)
		{
			if (options.UseRegex)
				return new RegexScanner(options.Pattern, options.Replacement, options.MatchCase);
			else
				return new TextScanner(options.Pattern, options.Replacement, options.MatchCase);
		}

		// worker function which scans all the files and matches all the lines
		private IEnumerable<MatchedLine> matchLines()
		{
			Count = 0;
			var scanner = CreateScanner(Options);
			bool replace = Options.Replacement != null; 

			foreach(string file in Files)
			{
				//FireFileScanned(file);

				bool fileModified = false;
				string[] lines = File.ReadAllLines(file); // make this read in chunks rather than all at once
				
				// Scan each line and yield a match if found
				for (int lineNumber = 0; lineNumber < lines.Length; ++lineNumber)
				{
					var ranges = replace ? 
						scanner.ScanAndReplace(ref lines[lineNumber]) :
						scanner.Scan(lines[lineNumber]);

					if (ranges.Length > 0)
					{
						if (replace)
							fileModified = true;

						// we found a match
						++Count;
						yield return new MatchedLine(file, lineNumber + 1, lines[lineNumber], ranges );
					}
				}

				if( replace && fileModified ) // write the file back to disk if we did anything
					File.WriteAllLines(file, lines);
			}
		}
	}
}
