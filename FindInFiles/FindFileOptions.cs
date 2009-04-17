using System;
using System.Collections.Generic;

namespace FindInFiles
{
    /// <summary>
    /// Options for finding which files match on the file system
    /// </summary>
    public class FindFileOptions
    {
        public readonly string Directory;
        public readonly ICollection<string> FileExtensions;
        public readonly ICollection<string> DirectoryExclusions;

        public FindFileOptions(string directory, ICollection<string> fileExtensions, ICollection<string> directoryExclusions)
        {
            Directory = directory;
            FileExtensions = fileExtensions;
            DirectoryExclusions = directoryExclusions;
        }
    }
}