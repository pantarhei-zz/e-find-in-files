using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace FindInFiles
{
	/// <summary>
	/// Represents all the files which match the given options
	/// </summary>
	class MatchedFileCollection : IEnumerable<string>
	{
		private readonly FindFileOptions Options;

		public int Count { get; private set; }

		public MatchedFileCollection(FindFileOptions options)
		{
			Debug.Assert(options != null);

			Options = options;
		}

		public IEnumerator<string> GetEnumerator()
		{
			return matchFiles(Options.Directory).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		// Internal actual recursive worker function
		private IEnumerable<string> matchFiles(string directory)
		{
			Count = 0;
			Debug.Assert(directory != null);

			if (Options.Directory.Length < 1)
				throw new ArgumentException("Directory cannot be empty", directory);

			if (!Directory.Exists(directory))
				throw new ArgumentException("Directory does not exist");

			// check for *.* (*'s have been stripped out so it will just be a .)
			if (Array.Exists(Options.FileExtensions, ext => ext == "."))
			{
				foreach (var f in Directory.GetFiles(directory).Select(file => Path.Combine(directory, file)))
				{
					++Count;
					yield return f;
				}
			}
			else
			{
				foreach (string file in Directory.GetFiles(directory))
				{
					if (Options.FileExtensions.Length < 1 ||
						(Array.Exists(Options.FileExtensions, ext => file.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase))))
					{
						++Count;
						yield return Path.Combine(directory, file);
					}
				}
			}

			foreach (string dir in Directory.GetDirectories(directory))
			{
				if (Options.DirectoryExclusions.Length < 1 ||
					(!Array.Exists(Options.DirectoryExclusions, dx => String.Compare(Path.GetFileName(dir), dx, StringComparison.CurrentCultureIgnoreCase) == 0)))
				{
					foreach (var f in matchFiles(dir))
					{
						++Count;
						yield return f;
					}
				}
			}
		}
	}
}
