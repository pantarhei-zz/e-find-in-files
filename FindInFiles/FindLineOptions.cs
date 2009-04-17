using System;

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

        public FindLineOptions(string pattern, bool matchCase, bool useRegex) :
            this(pattern, matchCase, useRegex, null) {}

        public FindLineOptions(string pattern, bool matchCase, bool useRegex, string replacement)
        {
            Pattern = pattern;
            MatchCase = matchCase;
            UseRegex = useRegex;
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