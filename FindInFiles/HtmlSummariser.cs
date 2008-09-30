using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace FindInFiles
{
	/// <summary>
	/// Converts the stream of incoming FindResults into strings (to be printed to stdout)
	/// Will also add a header and footer
	/// </summary>
	class HtmlSummariser : IEnumerable<string>
	{
		public readonly string Pattern;
		public readonly string Directory;
		public readonly string ReplacedWith;
		public readonly IEnumerable<FindResult> Matches;

		private int TotalFileCount;

		public HtmlSummariser( string pattern, string directory, string replacedWith, IEnumerable<FindResult> matches )
		{
			Debug.Assert( pattern != null );
			Debug.Assert( directory != null );
			// if we are only searching, replacedWith is designed to be null
			Debug.Assert( matches != null );

			Pattern = pattern;
			Directory = directory + (directory.EndsWith( "\\", StringComparison.CurrentCultureIgnoreCase ) ? "" : "\\"); // make sure it always ends with a \ so the output format is pretty
			ReplacedWith = replacedWith;
			Matches = matches;
		}

		private static string EscapeHtml( string x )
		{
			return x.
				Replace( "<", "&lt;" ).
				Replace( ">", "&gt;" ).
				Replace( "'", "&#39;" );
		}

		/// <summary>
		/// Returns a copy of line with HTML span tags inserted to hilight the character ranges defined by ranges
		/// </summary>
		/// <param name="line">The input text</param>
		/// <param name="ranges">Character start/ends to surround in span tags</param>
		/// <returns>A hilighted copy</returns>
		private static string Highlight( string line, Range<int>[] ranges )
		{
			Debug.Assert( line != null );
			Debug.Assert( ranges != null );

			if( ranges.Length == 0 )
				return line;

			var b = new StringBuilder();
			int lastIndex = 0;
			foreach( var r in ranges )
			{
				b.Append( line.Substring( lastIndex, r.Lower ) );
				b.Append( "<span style='background-color:yellow'>" );
				b.Append( line.Substring( r.Lower, r.Upper ) );
				b.Append( "</span>" );

				lastIndex = r.Upper;
			}
			return b.ToString();
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
			DateTime startTime = DateTime.Now;
			int numLinesMatched = 0;
			int numFilesMatched = 0;

			yield return "<style type=text/css>PRE{ font-size:11px; } PRE A{ text-decoration:none; } PRE A:HOVER{ background-color:#eeeeee; }</style>";
			yield return "<pre>";

			string lastFile = null;

			foreach( var match in Matches )
			{
				yield return String.Format( CultureInfo.CurrentCulture,
					"<a href=\"txmt://open/?url=file://{0}&amp;line={1}\">{2}({1}): {3}</a>",
					match.File,
					match.LineNumber,
					MakeShortPath( Directory, match.File ),
					Highlight( EscapeHtml( match.LineText ), match.CharactersMatched )
				);

				++numLinesMatched;

				if( lastFile != match.File )
				{
					lastFile = match.File;
					++numFilesMatched;
				}
			}

			yield return "--------------------------------------------------------------------------------";

			yield return String.Format( CultureInfo.CurrentCulture, "Searched For '{0}' in {1}", Pattern, Directory );

			yield return String.Format(CultureInfo.CurrentCulture, "{0} Lines in {1} Files Matched.  {2} Files Scanned in {3}s",
				numLinesMatched, numFilesMatched, (DateTime.Now - startTime).TotalSeconds );

			yield return "</pre>";
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<String>)this).GetEnumerator();
		}
	}
}
