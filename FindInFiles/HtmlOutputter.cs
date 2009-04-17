using System;
using System.Collections.Generic;
using System.Text;

namespace FindInFiles
{
    /// <summary>
    /// Outputs things to STDOUT in HTML format
    /// </summary>
    class HtmlOutputter : IOutputMatches
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
            var b = new StringBuilder();
            int lastIndex = 0;
            foreach (var r in ranges)
            {
                b.Append(EscapeHtml(line.Substring(lastIndex, r.Lower - lastIndex)));
                b.Append("<span style='background-color:yellow;'>");
                b.Append(EscapeHtml(line.Substring(r.Lower, r.Upper - r.Lower)));
                b.Append("</span>");

                lastIndex = r.Upper;
            }

            b.Append(line.Substring(lastIndex)); //stick the trailing bit on
            return b.ToString();
        }

        private static string MakeShortPath(string basePath, string fullPath)
        {
            if (fullPath.IndexOf(basePath, StringComparison.CurrentCultureIgnoreCase) == -1)
                return fullPath;
            return fullPath.Substring(basePath.Length);
        }

        private readonly DateTime startTime;
        private readonly string pattern;
        private readonly string directory;
        private readonly ICountedEnumerable<string> files;
        private readonly ICountedEnumerable<Match> matches;

        public HtmlOutputter(string pattern, string directory, ICountedEnumerable<string> files, ICountedEnumerable<Match> matches)
        {
            startTime = DateTime.Now;

            this.pattern = pattern;
            this.directory = directory;
            this.files = files;
            this.matches = matches;
        }

        public void OutputHeader()
        {
            Console.WriteLine("<style type=text/css>PRE{ font-size:11px; } PRE A{ text-decoration:none; } PRE A:HOVER{ background-color:#eeeeee; }</style>");
            Console.WriteLine("<pre>");
        }

        public void OutputMatch(Match match)
        {
            Console.WriteLine(
                "<a href=\"{0}\">{2}({1}): {3}</a>",
                Util.MakeTxmtUrl(match.File, match.LineNumber),
                match.LineNumber,
                MakeShortPath(directory, match.File),
                EscapedHighlight(match.LineText, new[] { match.Characters })
            );
        }

        public void OutputFooter()
        {
            var timeTaken = DateTime.Now - startTime;

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("Searched For '{0}' in {1}", pattern, directory);
            Console.WriteLine("{0} matches found, {1} files Scanned in {2}s.",
                matches.Count, files.Count, timeTaken.TotalSeconds);
            Console.WriteLine("</pre>");
        }
    }
}
