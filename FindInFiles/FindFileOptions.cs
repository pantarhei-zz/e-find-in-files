using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FindInFiles
{
    /// <summary>
    /// Options for finding which files match on the file system
    /// </summary>
    class FindFileOptions
    {
        public readonly string Directory;
        public readonly ICollection<string> FileExtensions;
        public readonly ICollection<string> DirectoryExclusions;

        public FindFileOptions(string directory, ICollection<string> fileExtensions, ICollection<string> directoryExclusions)
        {
            Debug.Assert(directory != null);
            Debug.Assert(fileExtensions != null);
            Debug.Assert(directoryExclusions != null);

            Directory = directory;
            FileExtensions = fileExtensions;
            DirectoryExclusions = directoryExclusions;
        }
    }
}