using System;
using System.Collections.Generic;
using System.Text;

namespace FindInFiles
{
    class TextScanner : IStringScanner
    {
        private readonly string Pattern;
        private readonly string Replacement;
        private readonly StringComparison ComparisonType;

        public TextScanner(string pattern, string replacement, bool matchCase)
        {
            Pattern = pattern;
            Replacement = replacement;
            ComparisonType = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
        }

        public IEnumerable<IntRange> Scan(string text)
        {
            int startIndex = 0;
			
            while ((startIndex = text.IndexOf(Pattern, startIndex, ComparisonType)) != -1)
                yield return new IntRange(startIndex, startIndex += Pattern.Length);
        }

        public IEnumerable<IntRange> ScanAndReplace(string text, Action<string> replaceCallback)
        {
            StringBuilder sb = new StringBuilder();

            int lastIndex = 0, nextIndex = 0;

            while ((nextIndex = text.IndexOf(Pattern, nextIndex, ComparisonType)) != -1)
            {
                sb.Append(text.Substring(lastIndex, nextIndex-lastIndex));
                sb.Append(Replacement);

                yield return new IntRange(nextIndex, nextIndex + Replacement.Length);
                nextIndex += Pattern.Length;
                lastIndex = nextIndex;
            }

            sb.Append(text.Substring(lastIndex)); //stick the trailing bit on

            if( replaceCallback != null )
                replaceCallback( sb.ToString() );
        }
    }
}