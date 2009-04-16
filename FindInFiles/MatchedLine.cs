using System;
using System.Diagnostics;

namespace FindInFiles
{
	/// <summary>
	/// Represents a single matching substring in a single line in a single file
	/// </summary>
	class Match
	{
		public readonly string File;
		public readonly int LineNumber;
		public readonly string LineText;
		public readonly IntRange Characters;

		public Match(string file, int lineNumber, string lineText, IntRange characters)
		{
			Debug.Assert(file != null);
			Debug.Assert(lineNumber != 0);
			Debug.Assert(lineText != null);
			Debug.Assert(characters != null);

			File = file;
			LineNumber = lineNumber;
			LineText = lineText;
			Characters = characters;
		}
	}
}
