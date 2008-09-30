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
		Range<int>[] Scan(string text);
		Range<int>[] ScanAndReplace(ref string text);
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

		public Range<int>[] Scan(string text)
		{
			return null;
		}

		public Range<int>[] ScanAndReplace(ref string text)
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
			ComparisonType = matchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
		}

		public Range<int>[] Scan(string text)
		{
			List<Range<int>> ranges = null;

			int startIndex = 0;
			if ((startIndex = text.IndexOf(Pattern, startIndex, ComparisonType)) != -1)
			{
				// avoid creating a buffer unless we actually find something
				ranges = new List<Range<int>>{
					new Range<int>(startIndex, startIndex += Pattern.Length)
				};

				while ((startIndex = text.IndexOf(Pattern, startIndex)) != -1)
					ranges.Add(new Range<int>(startIndex, startIndex += Pattern.Length));
			}

			return (ranges == null) ?
				new Range<int>[0] :
				ranges.ToArray();
		}

		public Range<int>[] ScanAndReplace(ref string text)
		{
			List<Range<int>> ranges = null;
			StringBuilder replaceBuffer = null;

			int lastIndex = 0, nextIndex = 0;
			if ((nextIndex = text.IndexOf(Pattern, nextIndex, ComparisonType)) != -1)
			{
				// avoid creating a buffer unless we actually find something
				replaceBuffer = new StringBuilder();
				replaceBuffer.Append(text.Substring(lastIndex, nextIndex-lastIndex));
				replaceBuffer.Append(Replacement);
				ranges = new List<Range<int>>{
					new Range<int>(nextIndex, nextIndex + Replacement.Length)
				};
				nextIndex += Pattern.Length; // the indexes refer to the search text, the ranges map to the replaced text
				lastIndex = nextIndex;

				while ((nextIndex = text.IndexOf(Pattern, nextIndex)) != -1)
				{
					replaceBuffer.Append(text.Substring(lastIndex, nextIndex-lastIndex));
					replaceBuffer.Append(Replacement);

					ranges.Add(new Range<int>(nextIndex, nextIndex + Replacement.Length));
					nextIndex += Pattern.Length;
					lastIndex = nextIndex;
				}

				replaceBuffer.Append(text.Substring(lastIndex)); //stick the trailing bit on
			}

			if (replaceBuffer != null)
				text = replaceBuffer.ToString();

			return (ranges == null) ?
				new Range<int>[0] :
				ranges.ToArray();
		}
	}
}
