using System;
using System.Diagnostics;

namespace FindInFiles
{
    /// <summary>
    /// Options for finding which lines match in a file (and optionally replacing them if the alternate constructor is used)
    /// </summary>
    class FindLineOptions
    {
        public readonly string Pattern;
        public readonly bool MatchCase;
        public readonly bool UseRegex;
        public readonly string Replacement;

        public FindLineOptions(string pattern, bool matchCase, bool useRegex)
        {
            Debug.Assert(pattern != null);

            Pattern = pattern;
            MatchCase = matchCase;
            UseRegex = useRegex;
            Replacement = null;
        }

        public FindLineOptions( string pattern, bool matchCase, bool useRegex, string replacement )
            : this(pattern, matchCase, useRegex)
        {
            Debug.Assert( replacement != null );

            Replacement = replacement;
        }

        public IStringScanner CreateScanner()
        {
            if (UseRegex)
                return new RegexScanner(Pattern, Replacement, MatchCase);
            return new TextScanner(Pattern, Replacement, MatchCase);
        }
    }
}