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
	static class FileMatcher
	{
		private static IEnumerable<string> FindFilesRecursive( string baseDirectory, Predicate<string> fileFilter, Predicate<string> directoryFilter )
		{
			Debug.Assert( baseDirectory != null );
			Debug.Assert( directoryFilter != null );

			Func<string, IEnumerable<string>> recurse = ( s ) => FindFilesRecursive( s, fileFilter, directoryFilter );

			var files = from file in Directory.GetFiles( baseDirectory )
						where fileFilter( file )
						select Path.Combine( baseDirectory, file );

			var subfiles = from subdir in Directory.GetDirectories( baseDirectory )
						   where directoryFilter( subdir )
						   from file in recurse( Path.Combine( baseDirectory, subdir ) )
						   select Path.Combine( Path.Combine( baseDirectory, subdir ), file );

			foreach( var file in files.Concat(subfiles) )
				yield return file;
		}

		public static IEnumerable<string> Filter( FindFileOptions options )
		{
			Debug.Assert( options != null );

			if( options.Directory.Length < 1 )
				throw new ArgumentException( "Directory cannot be empty", options.Directory );

			if( !Directory.Exists( options.Directory ) )
				throw new ArgumentException( "Directory does not exist" );

			// build filter predicates
			Predicate<string> fileFilter, directoryFilter;

			if( options.DirectoryExclusions.Length < 1 )
				directoryFilter = ( dir ) => true;
			else
				directoryFilter = ( dir ) => !options.DirectoryExclusions.Any(
					exclusion => String.Compare( Path.GetFileName( dir ), exclusion, StringComparison.CurrentCultureIgnoreCase ) == 0 );

			// check for *.* (*'s have been stripped out so it will just be a .)
			if( options.FileExtensions.Length < 1 || options.FileExtensions.Any( ext => ext == "." ) )
				fileFilter = ( file ) => true;
			else
				fileFilter = ( file ) => options.FileExtensions.Any(
					ext => file.EndsWith( ext, StringComparison.CurrentCultureIgnoreCase ) );

			foreach( var file in FindFilesRecursive( options.Directory, fileFilter, directoryFilter ) )
				yield return file;
		}
	}
}
