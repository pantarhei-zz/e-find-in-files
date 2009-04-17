using System;
using System.Collections.Generic;
using System.IO;

namespace FindInFiles
{
	/// <summary>
	/// Represents all the files which match the given options
	/// </summary>
	class FileMatcher
	{
	    private readonly FindFileOptions options;

	    private static IEnumerable<string> FindFilesRecursive( string baseDirectory, Predicate<string> fileFilter, Predicate<string> directoryFilter )
		{
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

        public FileMatcher(FindFileOptions options)
        {
            if (options.Directory.Length < 1)
                throw new ArgumentException("Directory cannot be empty", options.Directory);

            this.options = options;
        }

        public IEnumerable<string> Filter()
        {
            if (!Directory.Exists(options.Directory))
                throw new ArgumentException("Directory does not exist");

            foreach (var file in FindFilesRecursive(options.Directory, GetFileFilter(), GetDirectoryFilter()))
                yield return file;
        }

	    private Predicate<string> GetFileFilter()
	    {
	        // check for *.* (*'s have been stripped out so it will just be a .)
	        if (options.FileExtensions.Count == 0 || options.FileExtensions.Any(ext => ext == "."))
	            return Util.AlwaysTrue;

	        return file => options.FileExtensions.Any(
	                           ext => file.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase));
	    }

	    private Predicate<string> GetDirectoryFilter()
	    {
            if (options.DirectoryExclusions.Count == 0)
                return Util.AlwaysTrue;

            return dir => !options.DirectoryExclusions.Any(
                               exclusion =>
                               String.Compare(Path.GetFileName(dir), exclusion,
                                              StringComparison.CurrentCultureIgnoreCase) == 0);
	    }

	    public static IEnumerable<string> Filter( FindFileOptions options )
		{
		    var f = new FileMatcher(options);
            foreach (var o in f.Filter())
                yield return o;
		}
	}
}
