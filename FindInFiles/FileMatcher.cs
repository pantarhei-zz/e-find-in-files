using System;
using System.Collections.Generic;
using System.IO;

namespace FindInFiles
{
    /// <summary>
    /// Represents all the files which match the given options
    /// </summary>
    public class FileMatcher
    {
        private readonly FindFileOptions options;

        private IEnumerable<string> FindFilesRecursive()
        {
            Predicate<string> fileFilter = GetFileFilter();
            Predicate<string> directoryFilter = GetDirectoryFilter();

            Func<string, IEnumerable<string>> recurse = arg => FindFilesRecursive();

            var files = from file in Directory.GetFiles(options.Directory)
                        where fileFilter(file)
                        select Path.Combine(options.Directory, file);

            var subfiles = from dir in Directory.GetDirectories(options.Directory)
                           where directoryFilter(dir)
                           from file in recurse(Path.Combine(options.Directory, dir))
                           select Path.Combine(Path.Combine(options.Directory, dir), file);

            foreach (var file in files.Concat(subfiles))
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

            foreach (var file in FindFilesRecursive())
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
    }
}
