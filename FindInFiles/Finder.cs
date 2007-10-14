using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FindInFiles
{
    class FindOptions
    {
        private string m_searchPath;
        private string m_searchPattern;
        private bool m_matchCase = false;
        private bool m_useRegex = false;
        private string[] m_searchExtensions = new string[0];
        private string[] m_directoryExcludes = new string[0];

        public string SearchPath
        {
            get { return m_searchPath; }
            set { m_searchPath = value; }
        }

        public string SearchPattern
        {
            get { return m_searchPattern; }
            set { m_searchPattern = value; }
        }

        public bool MatchCase
        {
            get { return m_matchCase; }
            set { m_matchCase = value; }
        }

        public bool UseRegex
        {
            get { return m_useRegex; }
            set { m_useRegex = value; }
        }

        public string[] SearchExtensions
        {
            get { return m_searchExtensions; }
        }

        public void SetSearchExtensions(string e)
        {
            m_searchExtensions =
             e.Replace("*", "").Replace(" ", "").ToLower().Split(new char[] { ',', ';' });
        }

        public string[] DirectoryExcludes
        {
            get { return m_directoryExcludes; }
        }

        public void SetDirectoryExcludes(string e)
        {
            m_directoryExcludes =
             e.ToLower().Split(new char[] { ',', ';' });
        }
    }

    class FindResult
    {
        public FindResult(string file, int lineNumber, string lineText)
        {
            m_file = file;
            m_lineNumber = lineNumber;
            m_lineText = lineText;
        }

        private readonly string m_file;
        private readonly int m_lineNumber;
        private readonly string m_lineText;

        public string File
        {
            get { return m_file; }
        }

        public int LineNumber
        {
            get { return m_lineNumber; }
        }
        public string LineText
        {
            get { return m_lineText; }
        }
    }

    class FindResults
    {
        public TimeSpan TimeTaken;
        public int m_numFilesSearched;
        public int m_numFilesMatched;
        public int m_numLinesMatched;

        public List<FindResult> m_matches;

        public override string ToString()
        {
            StringBuilder str = new StringBuilder(m_matches.Count * 40);

            foreach (FindResult match in m_matches)
            {
                str.AppendFormat(
                    "<a href=\"txmt://open/?url=file://{0}&amp;line={1}\">{0}({1}): {2}</a><br>",
                    match.File, match.LineNumber, match.LineText);
                //str.AppendFormat(");
            }

            str.AppendFormat("Matching Lines: {0}\t Matching Files: {1}\t Total files searched: {2} in {3}s\n",
                m_numLinesMatched, m_numFilesMatched, m_numFilesSearched, TimeTaken.TotalSeconds);

            return str.ToString();
        }
    }

    static class Finder
    {
        private static List<string> MakeFileList(string searchPath, string[] searchExtensions, string[] directoryExcludes)
        {
            List<string> ret = new List<string>();

            foreach (string file in Directory.GetFiles(searchPath))
            {
                if (searchExtensions.Length < 1 ||
                    (Array.Exists(searchExtensions, delegate(string ext) { return file.ToLower().EndsWith(ext); })))
                    ret.Add(Path.Combine(searchPath, file));
            }

            foreach (string dir in Directory.GetDirectories(searchPath))
            {
                if (directoryExcludes.Length < 1 ||
                    (!Array.Exists(directoryExcludes, delegate(string dx) { return Path.GetFileName(dir).ToLower() == dx; })))
                    ret.AddRange(MakeFileList(dir, searchExtensions, directoryExcludes));
            }

            return ret;
        }

        private static FindResults FindInFiles(string[] files, string searchPattern, bool matchCase, bool useRegex, CurrentFileCallback currentFileCallback)
        {
            Regex searchRegex = null;
            if (useRegex)
            {
                RegexOptions regexOptions = RegexOptions.Compiled;
                if (!matchCase)
                    regexOptions |= RegexOptions.IgnoreCase;

                searchRegex = new Regex(searchPattern, regexOptions);
            }
            else if (!matchCase)
            {
                searchPattern = searchPattern.ToLower();
            }

            FindResults results = new FindResults();
            results.m_numFilesSearched = files.Length;

            List<FindResult> matches = new List<FindResult>();

            foreach (string file in files)
            {
                if (currentFileCallback != null)
                    currentFileCallback(file);
                bool fileMatches = false;

                string[] lines = File.ReadAllLines(file);

                if (useRegex && searchRegex != null)
                {
                    for (int lineNumber = 0; lineNumber < lines.Length; ++lineNumber)
                    {
                        string lineText = lines[lineNumber];

                        if (searchRegex.IsMatch(lineText))
                        {
                            results.m_numLinesMatched++;
                            fileMatches = true;
                            matches.Add(new FindResult(file, lineNumber, lineText));
                        }
                    }
                }
                else
                {
                    for (int lineNumber = 0; lineNumber < lines.Length; ++lineNumber)
                    {
                        string lineText = matchCase ? lines[lineNumber] : lines[lineNumber].ToLower();
                        if (lineText.IndexOf(searchPattern) > 0)
                        {
                            results.m_numLinesMatched++;
                            fileMatches = true;
                            matches.Add(new FindResult(file, lineNumber, lineText));
                        }
                    }
                }

                if (fileMatches)
                    results.m_numFilesMatched++;
            }

            results.m_matches = matches;
            return results;
        }

        public delegate void CurrentFileCallback(string currentFile);

        public static FindResults Find(FindOptions options, CurrentFileCallback currentFileCallback)
        {
            DateTime started_at = DateTime.Now;

            if (currentFileCallback != null)
                currentFileCallback("Scanning...");

            List<string> filesToSearch = MakeFileList(options.SearchPath, options.SearchExtensions, options.DirectoryExcludes);

            FindResults results = FindInFiles(filesToSearch.ToArray(), options.SearchPattern, options.MatchCase, options.UseRegex, currentFileCallback);
            results.TimeTaken = DateTime.Now - started_at;
            return results;
        }
    }
}
