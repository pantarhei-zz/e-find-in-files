using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

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
			DirectoryExclusions = directoryExclusions;
		}
	}

	/// <summary>
	/// Options for finding which lines match in a file (and optionally replacing them if the alternate constructor is used)
	/// </summary>
	class FindLineOptions
	{
		public readonly string Pattern;
		public readonly bool MatchCase;
		public readonly bool UseRegex;
		public readonly string Replacement;

		public FindLineOptions(string pattern, bool matchCase, bool useRegex)
		{
			Debug.Assert(pattern != null);

			Pattern = pattern;
			MatchCase = matchCase;
			UseRegex = useRegex;

			Replacement = null;
		}

		public FindLineOptions( string pattern, bool matchCase, bool useRegex, string replacement )
			: this(pattern, matchCase, useRegex)
		{
			Debug.Assert( replacement != null );

			Replacement = replacement;
		}
	}
}
