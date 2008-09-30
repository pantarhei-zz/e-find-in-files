using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;

namespace FindInFiles
{
	/// <summary>
	/// C# has no built in range type like ruby, so make one
	/// </summary>
	/// <typeparam name="T">Numeric type the range is of (int, float, etc)</typeparam>
	class Range<T> where T : IComparable<T>
	{
		public readonly T Lower, Upper;

		public Range(T lower, T upper)
		{
			Lower = lower;
			Upper = upper;
		}
	}


	/// <summary>
	/// Actually does the finding
	/// </summary>
	class Finder
	{
		public readonly FindLineOptions FindLineOptions;
		public readonly FindFileOptions FindFileOptions;

		public event ScanningFileCallback FileScanned;

		public delegate void ScanningFileCallback(string text);

		public Finder(FindFileOptions findFileOptions, FindLineOptions findLineOptions)
		{
			Debug.Assert(findFileOptions != null);
			Debug.Assert(findLineOptions != null);

			FindFileOptions = findFileOptions;
			FindLineOptions = findLineOptions;
		}

		private void FireFileScanned(string text)
		{
			if (FileScanned != null)
				FileScanned(text);
		}

		public void Find()
		{
			var files = new MatchedFileCollection( FindFileOptions );
			var lines = new MatchedLineCollection( FindLineOptions, files );

			using (var output = new HtmlOutputter(FindLineOptions.Pattern, FindFileOptions.Directory, files, lines))
			{
				foreach (var match in lines)
				{
					output.Write(match);
				}
			}
		}


		/// <summary>
		/// Abstracts a search and replace operation, so we can treat plain text and regex searches
		/// in the same manner
		/// </summary>
		private class Searcher
		{
			
		}
	}
}
