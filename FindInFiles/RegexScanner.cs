using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FindInFiles
{
    class RegexScanner : IStringScanner
    {
        private readonly Regex Regex;
        private readonly string Replacement;

        public RegexScanner(string pattern, string replacement, bool matchCase)
        {
            var regexOptions = RegexOptions.Compiled;
            if (!matchCase)
                regexOptions |= RegexOptions.IgnoreCase;

            Regex = new Regex(pattern, regexOptions);
            Replacement = replacement;
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