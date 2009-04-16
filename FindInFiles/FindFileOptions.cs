using System;
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
}