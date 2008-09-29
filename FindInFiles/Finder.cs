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
	/// Converts the stream of incoming FindResults into strings (to be printed to stdout)
	/// Will also add a header and footer
	/// </summary>
	class PrettyPrinter : IEnumerable<string>
	{
		public readonly string Pattern;
		public readonly string Directory;
		public readonly string ReplacedWith;
		public readonly TimeSpan TimeTaken;
		public readonly IEnumerable<FindResult> Matches;

		private int TotalFileCount;
		private int MatchingFileCount;
		private int MatchingLineCount;

		public PrettyPrinter(string pattern, string directory, string replacedWith, TimeSpan timeTaken, IEnumerable<FindResult> matches)
		{
			Debug.Assert(pattern != null);
			Debug.Assert(directory != null);
			// if we are only searching, replacedWith is designed to be null
			Debug.Assert(matches != null);

			Pattern = pattern;
			Directory = directory + (directory.EndsWith("\\", StringComparison.CurrentCultureIgnoreCase) ? "" : "\\"); // make sure it always ends with a \ so the output format is pretty
			ReplacedWith = replacedWith;
			Matches = matches;
			TimeTaken = timeTaken;
		}

		private static string EscapeHtml(string x)
		{
			return x.
				Replace("<", "&lt;").
				Replace(">", "&gt;").
				Replace("'", "&#39;");
		}

		/// <summary>
		/// Returns a copy of line with HTML span tags inserted to hilight the character ranges defined by ranges
		/// </summary>
		/// <param name="line">The input text</param>
		/// <param name="ranges">Character start/ends to surround in span tags</param>
		/// <returns>A hilighted copy</returns>
		private static string Highlight(string line, Range<int>[] ranges)
		{
			Debug.Assert(line != null);
			Debug.Assert(ranges != null);

			if(ranges.Length == 0)
				return line;

			var b = new StringBuilder();
			int lastIndex = 0;
			foreach (var r in ranges)
			{
				b.Append(line.Substring(lastIndex, r.Lower));
				b.Append("<span style='background-color:yellow'>");
				b.Append(line.Substring(r.Lower, r.Upper));
				b.Append("</span>");

				lastIndex = r.Upper;
			}
			return b.ToString();
		}

		private static string MakeShortPath(string basePath, string fullPath)
		{
			if (fullPath.IndexOf(basePath, StringComparison.CurrentCultureIgnoreCase) != -1)
				return fullPath.Substring(basePath.Length);
			else
				return fullPath;
		}

		public IEnumerator<string> GetEnumerator()
		{
			yield return "<style type=text/css>PRE{ font-size:11px; } PRE A{ text-decoration:none; } PRE A:HOVER{ background-color:#eeeeee; }</style>";
			yield return "<pre>";

			foreach (var match in Matches)
			{
				yield return String.Format(CultureInfo.CurrentCulture,
					"<a href=\"txmt://open/?url=file://{0}&amp;line={1}\">{2}({1}): {3}</a>",
					match.File,
					match.LineNumber,
					MakeShortPath(Directory, match.File),
					Highlight(EscapeHtml(match.LineText), match.CharactersMatched)
				);
			}

			yield return "--------------------------------------------------------------------------------";

			yield return String.Format(CultureInfo.CurrentCulture, "Searched For '{0}' in {1}", Pattern, Directory);
			//yield return String.Format(CultureInfo.CurrentCulture, "{0} Lines in {1} Files Matched.  {2} Files Scanned in {3}s",
			//	NumLinesMatched, NumFilesMatched, NumFilesSearched, TimeTaken.TotalSeconds);

			yield return "</pre>";
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<String>)this).GetEnumerator();
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

			FindFileOptions = options;
		}

		private void FireFileScanned(string text)
		{
			if (FileScanned != null)
				FileScanned(text);
		}

		/// <summary>
		/// Stream of matching files which match options
		/// </summary>
		/// <param name="options">Options for where to look and what to look at</param>
		/// <returns>A stream of fully qualified file paths which match</returns>
		private static IEnumerable<string> FindMatchingFiles(FindFileOptions options)
		{
			return FindMatchingFiles(options.Directory, options);
		}

		// Internal actual recursive worker function
		private static IEnumerable<string> FindMatchingFiles(string directory, FindFileOptions options)
		{
			Debug.Assert(directory != null);
			Debug.Assert(options != null);

			if (options.Directory.Length < 1)
				throw new ArgumentException("Directory cannot be empty", directory);

			if (!Directory.Exists(directory))
				throw new ArgumentException("Directory does not exist");

			// check for *.* (*'s have been stripped out so it will just be a .)
			if (Array.Exists(options.FileExtensions, ext => ext == "."))
			{
				foreach (var f in Directory.GetFiles(directory).Select(file => Path.Combine(searchPath, file)))
					yield return f;
			}
			else
			{
				foreach (string file in Directory.GetFiles(directory))
				{
					if (options.FileExtensions.Length < 1 ||
						(Array.Exists(options.FileExtensions, ext => file.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase))))
						yield return Path.Combine(directory, file);
				}
			}

			foreach (string dir in Directory.GetDirectories(directory))
			{
				if (options.DirectoryExclusions.Length < 1 ||
					(!Array.Exists(options.DirectoryExclusions, dx => String.Compare(Path.GetFileName(dir), dx, StringComparison.CurrentCultureIgnoreCase) == 0)))
				{
					foreach (var f in FindMatchingFiles(dir, options))
						yield return f;
				}
			}
		}

		public IEnumerable<string> Find()
		{
			var searcher = new Searcher(Options);
			foreach (var match in MatchInFiles(files, (lineNumber, lineText) => searcher.MatchLine(lineText)))
			{

			}

			return MatchInFiles(
				FindMatchingFiles(Options.SearchPath, Options.SearchExtensions, Options.DirectoryExcludes),
				(lineNumber, lineText) => searcher.MatchLine(lineText));
		}

		public IEnumerable<string> Replace()
		{
			var searcher = new Searcher(Options);

			return MatchInFiles(
				FindMatchingFiles(Options.SearchPath, Options.SearchExtensions, Options.DirectoryExcludes),
				(lineNumber, lineText) => searcher.MatchLine(lineText), searcher.Replace);
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

					searchRegex = new Regex(options.SearchPattern, regexOptions);
				}
				else
				{
					searchPattern = options.SearchPattern;
					stringComparisonType = options.MatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
				}

				replaceWith = options.ReplaceWith;
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
			return MatchInFiles(files, lineMatcher, null);
		}

		private IEnumerable<FindResult> MatchInFiles(IEnumerable<string> files, Func<int, string, bool> lineMatcher, Func<string, string> lineReplacer)
		{
			DateTime startedAt = DateTime.Now;
			int numFilesMatched = 0;
			int numLinesMatched = 0;

			var filecount = 0;
			foreach (string file in files)
			{
				filecount++;
				FireFileScanned(file);
				bool fileMatches = false;

				string[] lines = File.ReadAllLines(file); // make this use blocks
				// Scan each line and add a match if it matched
				for (int lineNumber = 0; lineNumber < lines.Length; ++lineNumber)
				{
					if (lineMatcher(lineNumber, lines[lineNumber]))
					{
						fileMatches = true;
						++numLinesMatched;

						if (lineReplacer != null)
							lines[lineNumber] = lineReplacer(lines[lineNumber]);

						// we found a match
						yield return new FindResult(file, lineNumber + 1, lines[lineNumber]);
					}
				}
				if (lineReplacer != null) // OH NO
					File.WriteAllLines(file, lines);

				if (fileMatches)
					++numFilesMatched;
			}
		}
	}
}
