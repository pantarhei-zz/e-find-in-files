using System;

namespace FindInFiles
{
	/// <summary>
	/// Represents a single matching substring in a single line in a single file
	/// </summary>
	public class Match
	{
		public readonly string File;
		public readonly int LineNumber;
		public readonly string LineText;
		public readonly IntRange Characters;

		public Match(string file, int lineNumber, string lineText, IntRange characters)
		{
			File = file;
			LineNumber = lineNumber;
			LineText = lineText;
			Characters = characters;
		}
	}
}
