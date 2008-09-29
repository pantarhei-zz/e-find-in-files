using System;
using System.Collections.Generic;
using System.Text;

namespace FindInFiles
{
	/// <summary>
	/// Options for finding which files match on the file system
	/// </summary>
	class FindFileOptions
	{
		public readonly string Directory;
		public readonly string[] FileExtensions;
		public readonly string[] DirectoryExclusions;

		public FindFileOptions(string directory, string[] fileExtensions, string[] directoryExclusions)
		{
			Debug.Assert(directory != null);
			Debug.Assert(fileExtensions != null);
			Debug.Assert(directoryExclusions != null);

			Directory = directory;
			FileExtensions = fileExtensions;
			directoryExclusions = directoryExclusions;
		}
	}

	/// <summary>
	/// Options for finding which lines match in a file
	/// </summary>
	class FindLineOptions
	{
		public readonly string Pattern;
		public readonly bool MatchCase;
		public readonly bool UseRegex;

		public FindLineOptions(string pattern, bool matchCase, bool useRegex)
		{
			Debug.Assert(pattern != null);

			Pattern = pattern;
			MatchCase = matchCase;
			UseRegex = useRegex;
		}
	}

	/// <summary>
	/// Options for finding and replacing lines in a file
	/// </summary>
	class ReplaceLineOptions : FindLineOptions
	{
		public readonly string Replacement;

		public ReplaceLineOptions(string replacement)
		{
			Debug.Assert(replacement != null);

			Replacement = replacement;
		}
	}
}
