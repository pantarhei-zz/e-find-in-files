using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FindInFiles
{
    class RegexScanner : IStringScanner
    {
        private readonly Regex regex;
        private readonly string replacement;

        public RegexScanner(string pattern, string replacement, bool matchCase)
        {
            RegexOptions regexOptions = RegexOptions.Compiled;
            if (!matchCase)
                regexOptions |= RegexOptions.IgnoreCase;

            regex = new Regex(pattern, regexOptions);
            this.replacement = replacement;
        }

        public IEnumerable<IntRange> Scan(string text)
        {
            return null;
        }

        public IEnumerable<IntRange> ScanAndReplace(string text, Action<string> replaceCallback)
        {
            return null;
        }
    }
}