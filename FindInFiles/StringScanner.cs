using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FindInFiles
{

	/// <summary>
	/// abstracts the differences between searching with a string and with a regex so they both can be used in the same way
	/// </summary>
	interface IStringScanner
	{
		IEnumerable<Range> Scan( string text );
		IEnumerable<Range> ScanAndReplace( string text, Action<string> replaceCallback );
	}

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

		public IEnumerable<Range> Scan(string text)
		{
			return null;
		}

		public IEnumerable<Range> ScanAndReplace(string text, Action<string> replaceCallback)
		{
			return null;
		}
	}

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

		public IEnumerable<Range> Scan(string text)
		{
			var startIndex = 0;
			
			while ((startIndex = text.IndexOf(Pattern, startIndex, ComparisonType)) != -1)
				yield return new Range(startIndex, startIndex += Pattern.Length);
		}

		public IEnumerable<Range> ScanAndReplace(string text, Action<string> replaceCallback)
		{
			StringBuilder replaceBuffer = new StringBuilder(); ;

			int lastIndex = 0, nextIndex = 0;

			while ((nextIndex = text.IndexOf(Pattern, nextIndex, ComparisonType)) != -1)
			{
				replaceBuffer.Append(text.Substring(lastIndex, nextIndex-lastIndex));
				replaceBuffer.Append(Replacement);

				yield return new Range(nextIndex, nextIndex + Replacement.Length);
				nextIndex += Pattern.Length;
				lastIndex = nextIndex;
			}

			replaceBuffer.Append(text.Substring(lastIndex)); //stick the trailing bit on

			if( replaceBuffer != null && replaceCallback != null )
				replaceCallback( replaceBuffer.ToString() );
		}
	}
}
