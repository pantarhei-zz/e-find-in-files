using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FindInFiles
{
	/// <summary>
	/// Options for finding things
	/// </summary>
	class FindOptions
	{
		public readonly string SearchPath;
		public readonly string SearchPattern;
		public readonly bool MatchCase;
		public readonly bool UseRegex;
		public readonly string[] SearchExtensions;
		public readonly string[] DirectoryExcludes;

		public FindOptions( string searchPath, string searchPattern, bool matchCase, bool useRegex, string[] searchExtensions, string[] directoryExcludes )
		{
			Debug.Assert( searchPath != null );
			Debug.Assert( searchPattern != null );
			Debug.Assert( searchExtensions != null );
			Debug.Assert( directoryExcludes != null );

			SearchPath = searchPath;
			SearchPattern = searchPattern;
			MatchCase = matchCase;
			UseRegex = useRegex;
			SearchExtensions = searchExtensions;
			DirectoryExcludes = directoryExcludes;
		}

		public FindOptions( string searchPath, string searchPattern, bool matchCase, bool useRegex, string searchExtensions, string directoryExcludes )
			: this( searchPath, searchPattern, matchCase, useRegex, ParseSearchExtensions(searchExtensions), ParseDirectoryExcludes( directoryExcludes ) )
		{ }

		public static string[] ParseSearchExtensions(string e)
		{
			return e.Replace("*", "").Replace(" ", "").ToLower().Split(new char[] { ',', ';' });
		}

		public static string[] ParseDirectoryExcludes(string e)
		{
			return e.ToLower().Split(new char[] { ',', ';' });
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
	class FindResults
	{
		public readonly TimeSpan TimeTaken;
		public readonly int NumFilesSearched;
		public readonly int NumFilesMatched;
		public readonly int NumLinesMatched;

		public readonly List<FindResult> Matches;

		public FindResults( TimeSpan timeTaken, int numFilesSearched, int numFilesMatched, int numLinesMatched, List<FindResult> matches )
		{
			TimeTaken = timeTaken;
			NumFilesSearched = numFilesSearched;
			NumFilesMatched = numFilesMatched;
			NumLinesMatched = numLinesMatched;
			Matches = matches;
		}

		private string EscapeHtml( string x )
		{
			return x.
				Replace( "<", "&lt;" ).
				Replace( ">", "&gt;" ).
				Replace( "'", "&#39;" );
		}

		public override string ToString()
		{
			StringBuilder str = new StringBuilder(Matches.Count * 60); //guesstimate 60 characters per line for buffer size

			str.Append( "<style type=text/css>PRE{ font-size:11px; } PRE A{ text-decoration:none; } PRE A:HOVER{ background-color:#eeeeee; }</style>" );
			str.Append( "<pre>" );

			foreach (FindResult match in Matches)
			{
				str.AppendFormat(
					"<a href=\"txmt://open/?url=file://{0}&amp;line={1}\">{0}({1}): {2}</a>\n",
					match.File, match.LineNumber, EscapeHtml(match.LineText) );
			}

			str.AppendLine( "--------------------------------------------------------------------------------" );
			str.AppendFormat("Matching Lines: {0}\t Matching Files: {1}\t Total files searched: {2} in {3}s\n",
				NumLinesMatched, NumFilesMatched, NumFilesSearched, TimeTaken.TotalSeconds);

			str.AppendLine( "</pre>" );
			return str.ToString();
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

		private static List<string> MakeFileList( string searchPath, string[] searchExtensions, string[] directoryExcludes )
		{
			Debug.Assert( searchPath != null );
			Debug.Assert( searchExtensions != null );
			Debug.Assert( directoryExcludes != null );

			if( searchPath.Length < 1 )
				throw new ArgumentException( "Directory cannot be empty", searchPath );

			if( !Directory.Exists( searchPath ) )
				throw new ArgumentException( "Directory does not exist" );

			List<string> ret = new List<string>();

			// check for *.* (*'s have been stripped out so it will just be a .)
			if( Array.Exists( searchExtensions, delegate( string ext ) { return ext == "."; } ) )
			{
				foreach( string file in Directory.GetFiles( searchPath ) )
					ret.Add( Path.Combine( searchPath, file ) );
			}
			else
			{
				foreach( string file in Directory.GetFiles( searchPath ) )
				{
					if( searchExtensions.Length < 1 ||
						(Array.Exists( searchExtensions, delegate( string ext ) { return file.ToLower().EndsWith( ext ); } )) )
						ret.Add( Path.Combine( searchPath, file ) );
				}
			}

			foreach( string dir in Directory.GetDirectories( searchPath ) )
			{
				if( directoryExcludes.Length < 1 ||
					(!Array.Exists( directoryExcludes, delegate( string dx ) { return Path.GetFileName( dir ).ToLower() == dx; } )) )
					ret.AddRange( MakeFileList( dir, searchExtensions, directoryExcludes ) );
			}

			return ret;
		}

		private void PrepareSearch( out Regex searchRegex, out string searchString )
		{
			searchRegex = null;
			searchString = null;

			if( Options.UseRegex )
			{
				RegexOptions regexOptions = RegexOptions.Compiled;
				if( !Options.MatchCase )
					regexOptions |= RegexOptions.IgnoreCase;

				searchRegex = new Regex( Options.SearchPattern, regexOptions );
			}
			else
			{
				if( !Options.MatchCase )
					searchString = Options.SearchPattern.ToLower();
				else
					searchString = Options.SearchPattern;
			}
		}

		private FindResults FindInFiles( ICollection<string> files )
		{
			DateTime startedAt = DateTime.Now;
			int numFilesMatched = 0;
			int numLinesMatched = 0;

			Regex searchRegex;
			string searchPattern;
			PrepareSearch( out searchRegex, out searchPattern );

			List<FindResult> matches = new List<FindResult>();
			foreach( string file in files )
			{
				FireScanningFile( file );
				bool fileMatches = false;

				string[] lines = File.ReadAllLines(file); // make this use blocks
				// Scan each line and add a match if it matched
				for( int lineNumber = 0; lineNumber < lines.Length; ++lineNumber )
				{
					string lineText = Options.MatchCase ? lines[lineNumber] : lines[lineNumber].ToLower();

					if( (searchRegex != null && searchRegex.IsMatch(lineText)) ||
						(searchPattern != null && lineText.IndexOf( searchPattern ) > -1) )
					{
						++numLinesMatched;
						fileMatches = true;
						matches.Add( new FindResult( file, lineNumber + 1, lines[lineNumber] ) );
					}
				}

				if( fileMatches )
					++numFilesMatched;
			}

			return new FindResults(
				DateTime.Now - startedAt,
				files.Count,
				numFilesMatched,
				numLinesMatched,
				matches );
		}

		public FindResults Find()
		{
			FireScanningFile( "Scanning..." );

			List<string> filesToSearch = MakeFileList( Options.SearchPath, Options.SearchExtensions, Options.DirectoryExcludes );

			return FindInFiles( filesToSearch );
		}
	}
}
