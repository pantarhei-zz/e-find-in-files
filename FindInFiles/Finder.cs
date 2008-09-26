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
	/// Options for finding things
	/// </summary>
	class FindOptions
	{
		public readonly string SearchPath;
		public readonly string SearchPattern;
		public readonly string ReplaceWith;
		public readonly bool MatchCase;
		public readonly bool UseRegex;
		public readonly string[] SearchExtensions;
		public readonly string[] DirectoryExcludes;

		protected FindOptions( string searchPath, string searchPattern, string replaceWith, bool matchCase, bool useRegex, string[] searchExtensions, string[] directoryExcludes )
		{
			Debug.Assert( searchPath != null );
			Debug.Assert( searchPattern != null );
			// replaceWith will be null if we're just searching
			Debug.Assert( searchExtensions != null );
			Debug.Assert( directoryExcludes != null );

			SearchPath = Util.CleanAndConvertCygpath( searchPath );
			SearchPattern = searchPattern;
			ReplaceWith = replaceWith;
			MatchCase = matchCase;
			UseRegex = useRegex;
			SearchExtensions = searchExtensions;
			DirectoryExcludes = directoryExcludes;
		}

		public FindOptions( string searchPath, string searchPattern, bool matchCase, bool useRegex, string searchExtensions, string directoryExcludes )
			: this( searchPath, searchPattern, null, matchCase, useRegex, ParseSearchExtensions( searchExtensions ), ParseDirectoryExcludes( directoryExcludes ) )
		{ }

		public FindOptions( string searchPath, string searchPattern, string replaceWith, bool matchCase, bool useRegex, string searchExtensions, string directoryExcludes )
			: this( searchPath, searchPattern, replaceWith, matchCase, useRegex, ParseSearchExtensions( searchExtensions ), ParseDirectoryExcludes( directoryExcludes ) )
		{ }

		public static string[] ParseSearchExtensions(string e)
		{
			return e.Replace("*", "").Replace(" ", "").Split( new[]{ ',', ';' });
		}

		public static string[] ParseDirectoryExcludes(string e)
		{
			return e.Split( new[]{ ',', ';' } );
		}
	}

	/// <summary>
	/// Represents a single matched result
	/// </summary>
	class FindResult
	{
		public FindResult(string file, int lineNumber, string lineText)
		{
			File = file;
			LineNumber = lineNumber;
			LineText = lineText;
		}

		public readonly string File;
		public readonly int LineNumber;
		public readonly string LineText;
	}

	/// <summary>
	/// The collection and summary of results
	/// </summary>
	class FindResults : IEnumerable<String>
	{
		public readonly string SearchPattern;
		public readonly string SearchPath;
		public readonly string ReplaceString;
		public readonly TimeSpan TimeTaken;
		public readonly int NumFilesSearched;
		public readonly int NumFilesMatched;
		public readonly int NumLinesMatched;

		public readonly IEnumerable<FindResult> Matches;

		public FindResults( string searchPattern, string searchPath, string replaceString, TimeSpan timeTaken, int numFilesSearched, int numFilesMatched, int numLinesMatched, IEnumerable<FindResult> matches )
		{
			SearchPattern = searchPattern;
			SearchPath = searchPath + (searchPath.EndsWith("\\", StringComparison.CurrentCultureIgnoreCase) ? "" : "\\"); // make sure it always ends with a \ so the output format is pretty
			ReplaceString = replaceString;
			
			TimeTaken = timeTaken;
			NumFilesSearched = numFilesSearched;
			NumFilesMatched = numFilesMatched;
			NumLinesMatched = numLinesMatched;
			Matches = matches;
		}

		private static string EscapeHtml( string x )
		{
			return x.
				Replace( "<", "&lt;" ).
				Replace( ">", "&gt;" ).
				Replace( "'", "&#39;" );
		}

		private static string HighlightMatch( string line, string match )
		{
			return line.Replace( match, "<span style='background-color:yellow'>" + match + "</span>" );
		}

		private static string MakeShortPath( string basePath, string fullPath )
		{
			if( fullPath.IndexOf( basePath, StringComparison.CurrentCultureIgnoreCase ) != -1 )
				return fullPath.Substring( basePath.Length );
			else
				return fullPath;
		}

		public IEnumerator<string> GetEnumerator()
		{
			yield return "<style type=text/css>PRE{ font-size:11px; } PRE A{ text-decoration:none; } PRE A:HOVER{ background-color:#eeeeee; }</style>";
			yield return "<pre>";

			foreach( var match in Matches )
			{
				yield return String.Format( CultureInfo.CurrentCulture,
					"<a href=\"txmt://open/?url=file://{0}&amp;line={1}\">{2}({1}): {3}</a>",
					match.File, 
					match.LineNumber, 
					MakeShortPath( SearchPath, match.File ), 
					HighlightMatch( EscapeHtml( match.LineText ), ReplaceString ?? SearchPattern ) 
				);
			}

			yield return "--------------------------------------------------------------------------------";

			yield return String.Format( CultureInfo.CurrentCulture, "Searched For '{0}' in {1}", SearchPattern, SearchPath );
			yield return String.Format( CultureInfo.CurrentCulture, "{0} Lines in {1} Files Matched.  {2} Files Scanned in {3}s",
				NumLinesMatched, NumFilesMatched, NumFilesSearched, TimeTaken.TotalSeconds );

			yield return "</pre>";
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<String>) this).GetEnumerator();
		}
	}

	/// <summary>
	/// Actually does the finding
	/// </summary>
	class Finder
	{
		public readonly FindOptions Options;
		public event ScanningFileCallback ScanningFile;
			
		public delegate void ScanningFileCallback(string text);

		public Finder( FindOptions options )
		{
			Debug.Assert( options != null );
			Options = options;
		}

		private void FireScanningFile( string text )
		{
			if( ScanningFile != null )
				ScanningFile( text );
		}

		private static IEnumerable<string> MakeFileList( string searchPath, string[] searchExtensions, string[] directoryExcludes )
		{
			Debug.Assert( searchPath != null );
			Debug.Assert( searchExtensions != null );
			Debug.Assert( directoryExcludes != null );

			if( searchPath.Length < 1 )
				throw new ArgumentException( "Directory cannot be empty", searchPath );

			if( !Directory.Exists( searchPath ) )
				throw new ArgumentException( "Directory does not exist" );

			// check for *.* (*'s have been stripped out so it will just be a .)
			if( Array.Exists( searchExtensions, ext => ext == "." ) )
			{
				foreach( var f in Directory.GetFiles( searchPath ).Map( file => Path.Combine( searchPath, file ) ) )
					yield return f;
			}
			else
			{
				foreach( string file in Directory.GetFiles( searchPath ) )
				{
					if( searchExtensions.Length < 1 ||
						(Array.Exists( searchExtensions, ext => file.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase) )) )
						yield return Path.Combine( searchPath, file );
				}
			}

			foreach( string dir in Directory.GetDirectories( searchPath ) )
			{
				if( directoryExcludes.Length < 1 ||
					(!Array.Exists( directoryExcludes, dx => String.Compare( Path.GetFileName( dir ), dx, StringComparison.CurrentCultureIgnoreCase ) == 0 )) )
				{
					foreach( var f in MakeFileList( dir, searchExtensions, directoryExcludes ) )
						yield return f;
				}
			}
		}

		private FindResults FindInFiles( IEnumerable<string> files )
		{
			var searcher = new Searcher( Options );
			return EachLineInFiles( files, (lineNumber, lineText) => searcher.MatchLine( lineText ) );
		}

		private FindResults ReplaceInFiles( IEnumerable<string> files )
		{
			var searcher = new Searcher( Options );
			return EachLineInFiles( files, ( lineNumber, lineText ) => searcher.MatchLine( lineText ), searcher.Replace );
		}

		public FindResults Find()
		{
			return FindInFiles( MakeFileList( Options.SearchPath, Options.SearchExtensions, Options.DirectoryExcludes ) );
		}

		public FindResults Replace()
		{
			return ReplaceInFiles( MakeFileList( Options.SearchPath, Options.SearchExtensions, Options.DirectoryExcludes ) );
		}

		private class Searcher
		{
			private readonly Regex searchRegex;

			private readonly string searchPattern;
			private readonly StringComparison stringComparisonType;

			private readonly string replaceWith;

			public Searcher( FindOptions options )
			{
				if( options.UseRegex )
				{
					var regexOptions = RegexOptions.Compiled;
					if( !options.MatchCase )
						regexOptions |= RegexOptions.IgnoreCase;

					searchRegex = new Regex( options.SearchPattern, regexOptions );
				}
				else
				{
					searchPattern = options.SearchPattern;
					stringComparisonType = options.MatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
				}

				replaceWith = options.ReplaceWith;
			}

			public bool MatchLine( string lineText )
			{
				return ((searchRegex != null && searchRegex.IsMatch( lineText )) ||
					(searchPattern != null && lineText.IndexOf( searchPattern, stringComparisonType ) > -1));
			}

			public string Replace( string lineText )
			{
				if( searchRegex != null )
				{
					return searchRegex.Replace( lineText, replaceWith );
				}
				else if( searchPattern != null )
				{
					return FancyReplace( lineText, searchPattern, replaceWith, stringComparisonType );
				}
				else
				{
					throw new InvalidOperationException( "Shouldn't get here" );
				}
			}

			// Graciously borrowed from
			// http://www.codeproject.com/KB/string/fastestcscaseinsstringrep.aspx (in the comments)
			// copyright and credit goes to Michael Epner
			private static string FancyReplace( string original, string pattern, string replacement, StringComparison comparisonType )
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

		private FindResults EachLineInFiles( IEnumerable<string> files, Func<int, string, bool> lineMatcher )
		{
			return EachLineInFiles( files, lineMatcher, null );
		}

		private FindResults EachLineInFiles( IEnumerable<string> files, Func<int, string, bool> lineMatcher, Func<string, string> lineReplacer )
		{
			DateTime startedAt = DateTime.Now;
			int numFilesMatched = 0;
			int numLinesMatched = 0;

			var filecount = 0;
			var matches = new List<FindResult>();
			foreach( string file in files )
			{
				filecount++;
				FireScanningFile( file );
				bool fileMatches = false;

				string[] lines = File.ReadAllLines(file); // make this use blocks
				// Scan each line and add a match if it matched
				for( int lineNumber = 0; lineNumber < lines.Length; ++lineNumber )
				{
					if( lineMatcher( lineNumber, lines[lineNumber] ) )
					{
						fileMatches = true;
						++numLinesMatched;

						if( lineReplacer != null )
							lines[lineNumber] = lineReplacer( lines[lineNumber] );

						// show the replaced text
						matches.Add( new FindResult( file, lineNumber + 1, lines[lineNumber] ) );
					}
				}
				if( lineReplacer != null ) // OH NO
					File.WriteAllLines( file, lines );

				if( fileMatches )
					++numFilesMatched;
			}

			return new FindResults(
				Options.SearchPattern,
				Options.SearchPath,
				Options.ReplaceWith,
				DateTime.Now - startedAt,
				filecount,
				numFilesMatched,
				numLinesMatched,
				matches );

		}
	}
}
