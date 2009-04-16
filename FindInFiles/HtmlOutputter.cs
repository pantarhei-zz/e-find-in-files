using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace FindInFiles
{
    internal interface IOutputMatch
    {
        void OutputHeader();
        void OutputMatch( Match match );
        void OutputFooter();
    }

    /// <summary>
	/// Outputs things to STDOUT in HTML format
	/// </summary>
	class HtmlOutputter : IOutputMatch
    {
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
		private static string EscapedHighlight(string line, IEnumerable<IntRange> ranges)
		{
			Debug.Assert(line != null);
			Debug.Assert(ranges != null);

			var b = new StringBuilder();
			int lastIndex = 0;
			foreach (var r in ranges)
			{
				b.Append( EscapeHtml(line.Substring(lastIndex, r.Lower-lastIndex)));
				b.Append( "<span style='background-color:yellow;'>");
				b.Append( EscapeHtml(line.Substring(r.Lower, r.Upper-r.Lower)));
				b.Append( "</span>");

				lastIndex = r.Upper;
			}
			b.Append( line.Substring( lastIndex ) ); //stick the trailing bit on

			return b.ToString();
		}

		private static string MakeShortPath(string basePath, string fullPath)
		{
		    if (fullPath.IndexOf(basePath, StringComparison.CurrentCultureIgnoreCase) == -1)
		        return fullPath;
		    return fullPath.Substring(basePath.Length);
		}

	    private readonly DateTime StartTime;
		private readonly string Pattern, Directory;
		private readonly CountedEnumerable<string> Files;
		private readonly CountedEnumerable<Match> Matches;

		public HtmlOutputter( string pattern, string directory, CountedEnumerable<string> files, CountedEnumerable<Match> matches )
		{
			StartTime = DateTime.Now;

			Debug.Assert( pattern != null );
			Debug.Assert( directory != null );
			Debug.Assert( files != null );
			Debug.Assert( matches != null );

			Pattern = pattern;
			Directory = directory;
			Files = files;
			Matches = matches;
		}

        public void OutputHeader()
        {
            Console.WriteLine("<style type=text/css>PRE{ font-size:11px; } PRE A{ text-decoration:none; } PRE A:HOVER{ background-color:#eeeeee; }</style>");
            Console.WriteLine("<pre>");
        }

        public void OutputMatch( Match match )
		{
			Console.WriteLine( 
				"<a href=\"txmt://open/?url=file://{0}&amp;line={1}\">{2}({1}): {3}</a>",
				match.File,
				match.LineNumber,
				MakeShortPath( Directory, match.File ),
				EscapedHighlight(match.LineText, new[]{ match.Characters })
			);
		}

        public void OutputFooter()
        {
            var timeTaken = DateTime.Now - StartTime;

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("Searched For '{0}' in {1}", Pattern, Directory);
            Console.WriteLine("{0} Matches found. {1} Files Scanned in {2}s",
                Matches.Count, Files.Count, timeTaken.TotalSeconds);
            Console.WriteLine("</pre>");
        }
    }
}
