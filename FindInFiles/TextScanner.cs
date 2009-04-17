using System;
using System.Collections.Generic;
using System.Text;

namespace FindInFiles
{
    public class TextScanner : IStringScanner
    {
        private readonly string pattern;
        private readonly string replacement;
        private readonly StringComparison comparisonType;

        public TextScanner(string pattern, bool matchCase)
            : this(pattern, null, matchCase) {}

        public TextScanner(string pattern, string replacement, bool matchCase)
        {
            this.pattern = pattern;
            this.replacement = replacement;
            comparisonType = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
        }

        public IEnumerable<IntRange> Scan(string text)
        {
            int startIndex = 0;
			
            while ((startIndex = text.IndexOf(pattern, startIndex, comparisonType)) != -1)
                yield return new IntRange(startIndex, startIndex += pattern.Length);
        }

        public IEnumerable<IntRange> ScanAndReplace(string text, Action<string> replaceCallback)
        {
            if (replacement == null)
                throw new InvalidOperationException("Cannot scan and replace when the replacement text is null.");

            StringBuilder sb = new StringBuilder();

            int lastIndex = 0;
            int nextIndex = 0;

            while ((nextIndex = text.IndexOf(pattern, nextIndex, comparisonType)) != -1)
            {
                sb.Append(text.Substring(lastIndex, nextIndex-lastIndex));
                sb.Append(replacement);

                yield return new IntRange(nextIndex, nextIndex + replacement.Length);
                nextIndex += pattern.Length;
                lastIndex = nextIndex;
            }

            sb.Append(text.Substring(lastIndex)); //stick the trailing bit on

            if( replaceCallback != null )
                replaceCallback( sb.ToString() );
        }
    }
}