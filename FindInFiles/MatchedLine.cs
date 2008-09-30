using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace FindInFiles
{
	/// <summary>
	/// Represents a single matching line in a file
	/// </summary>
	class MatchedLine
	{
		public readonly string File;
		public readonly int LineNumber;
		public readonly string LineText;
		public readonly Range<int>[] MatchingCharacters;

		public MatchedLine(string file, int lineNumber, string lineText, params Range<int>[] matchingCharacters)
		{
			Debug.Assert(file != null);
			Debug.Assert(lineNumber != 0);
			Debug.Assert(lineText != null);
			Debug.Assert(matchingCharacters != null);

			File = file;
			LineNumber = lineNumber;
			LineText = lineText;
			MatchingCharacters = matchingCharacters;
		}
	}
}
