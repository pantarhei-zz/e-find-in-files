using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;

namespace FindInFiles
{
	/// <summary>
	/// C# has no built in range type like ruby, so make one
	/// </summary>
	/// <typeparam name="T">Numeric type the range is of (int, float, etc)</typeparam>
	class Range<T> where T : IComparable<T>
	{
		public readonly T Lower, Upper;

		public Range(T lower, T upper)
		{
			Lower = lower;
			Upper = upper;
		}
	}

	/// <summary>
	/// Represents a single matched result
	/// </summary>
	class FindResult
	{
		public readonly string File;
		public readonly int LineNumber;
		public readonly string LineText;
		public readonly Range<int>[] CharactersMatched;

		public FindResult(string file, int lineNumber, string lineText, params Range<int>[] charactersMatched)
		{
			Debug.Assert(file != null);
			Debug.Assert(lineNumber != 0);
			Debug.Assert(lineText != null);
			Debug.Assert(charactersMatched != null);

			File = file;
			LineNumber = lineNumber;
			LineText = lineText;
			CharactersMatched = charactersMatched;
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

		public IEnumerable<string> Find()
		{
			var searcher = new Searcher(FindLineOptions);

			var files = new MatchingFilePathCollection( FindFileOptions );

			var results = MatchInFiles(
				files,
				(lineNumber, lineText) => searcher.MatchLine(lineText) );

			return new HtmlSummariser( FindLineOptions.Pattern, FindFileOptions.Directory, FindLineOptions.Replacement, results );
		}

		public IEnumerable<string> Replace()
		{
			return null;
		}

		class MatchingFilePathCollection : IEnumerable<string>
		{
			private readonly FindFileOptions Options;

			public int Count { get; private set; }

			public MatchingFilePathCollection( FindFileOptions options )
			{
				Debug.Assert( options != null );

				Options = options;
			}

			public IEnumerator<string> GetEnumerator()
			{
				return matchFiles( Options.Directory ).GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			// Internal actual recursive worker function
			private IEnumerable<string> matchFiles( string directory )
			{
				Count = 0;
				Debug.Assert( directory != null );

				if( Options.Directory.Length < 1 )
					throw new ArgumentException( "Directory cannot be empty", directory );

				if( !Directory.Exists( directory ) )
					throw new ArgumentException( "Directory does not exist" );

				// check for *.* (*'s have been stripped out so it will just be a .)
				if( Array.Exists( Options.FileExtensions, ext => ext == "." ) )
				{
					foreach( var f in Directory.GetFiles( directory ).Select( file => Path.Combine( directory, file ) ) )
					{
						++Count;
						yield return f;
					}
				}
				else
				{
					foreach( string file in Directory.GetFiles( directory ) )
					{
						if( Options.FileExtensions.Length < 1 ||
							(Array.Exists( Options.FileExtensions, ext => file.EndsWith( ext, StringComparison.CurrentCultureIgnoreCase ) )) )
						{
							++Count;
							yield return Path.Combine( directory, file );
						}
					}
				}

				foreach( string dir in Directory.GetDirectories( directory ) )
				{
					if( Options.DirectoryExclusions.Length < 1 ||
						(!Array.Exists( Options.DirectoryExclusions, dx => String.Compare( Path.GetFileName( dir ), dx, StringComparison.CurrentCultureIgnoreCase ) == 0 )) )
					{
						foreach( var f in matchFiles( dir ) )
						{
							++Count;
							yield return f;
						}
					}
				}
			}
		}

		/// <summary>
		/// Abstracts a search and replace operation, so we can treat plain text and regex searches
		/// in the same manner
		/// </summary>
		private class Searcher
		{
			private readonly Regex searchRegex;

			private readonly string searchPattern;
			private readonly StringComparison stringComparisonType;

			private readonly string replaceWith;

			public Searcher(FindLineOptions options)
			{
				if (options.UseRegex)
				{
					var regexOptions = RegexOptions.Compiled;
					if (!options.MatchCase)
						regexOptions |= RegexOptions.IgnoreCase;

					searchRegex = new Regex(options.Pattern, regexOptions);
				}
				else
				{
					searchPattern = options.Pattern;
					stringComparisonType = options.MatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
				}

				replaceWith = options.Replacement;
			}

			public bool MatchLine(string lineText)
			{
				return ((searchRegex != null && searchRegex.IsMatch(lineText)) ||
					(searchPattern != null && lineText.IndexOf(searchPattern, stringComparisonType) > -1));
			}

			public string Replace(string lineText)
			{
				if (searchRegex != null)
				{
					return searchRegex.Replace(lineText, replaceWith);
				}
				else if (searchPattern != null)
				{
					return StringReplace(lineText, searchPattern, replaceWith, stringComparisonType);
				}
				else
				{
					throw new InvalidOperationException("Shouldn't get here");
				}
			}

			// Graciously borrowed from
			// http://www.codeproject.com/KB/string/fastestcscaseinsstringrep.aspx (in the comments)
			// copyright and credit goes to Michael Epner
			private static string StringReplace(string original, string pattern, string replacement, StringComparison comparisonType)
			{
				if (original == null)
				{
					return null;
				}

				if (String.IsNullOrEmpty(pattern))
				{
					return original;
				}

				int lenPattern = pattern.Length;
				int idxPattern = -1;
				int idxLast = 0;

				StringBuilder result = new StringBuilder();
				while (true)
				{
					idxPattern = original.IndexOf(pattern, idxPattern + 1, comparisonType);

					if (idxPattern < 0)
					{
						result.Append(original, idxLast, original.Length - idxLast);

						break;
					}

					result.Append(original, idxLast, idxPattern - idxLast);
					result.Append(replacement);

					idxLast = idxPattern + lenPattern;
				}

				return result.ToString();
			}
		}

		private IEnumerable<FindResult> MatchInFiles(IEnumerable<string> files, Func<int, string, bool> lineMatcher)
		{
			return MatchInFiles( files, lineMatcher, null );
		}

		private IEnumerable<FindResult> MatchInFiles(IEnumerable<string> files, Func<int, string, bool> lineMatcher, Func<string, string> lineReplacer)
		{
			foreach (string file in files)
			{
				FireFileScanned(file);

				bool fileModified = false;
				string[] lines = File.ReadAllLines(file); // make this read in chunks rather than all at once
				
				// Scan each line and yield a match if found
				for (int lineNumber = 0; lineNumber < lines.Length; ++lineNumber)
				{
					if (lineMatcher(lineNumber, lines[lineNumber]))
					{
						if( lineReplacer != null )
						{
							lines[lineNumber] = lineReplacer( lines[lineNumber] );
							fileModified = true;
						}

						// we found a match
						yield return new FindResult(file, lineNumber + 1, lines[lineNumber] /*, Ranges go here */);
					}
				}

				if( lineReplacer != null && fileModified ) // write the file back to disk if we did anything
					File.WriteAllLines(file, lines);
			}
		}
	}
}
