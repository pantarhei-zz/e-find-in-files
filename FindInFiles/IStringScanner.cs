using System;
using System.Collections.Generic;

namespace FindInFiles
{
    /// <summary>
    /// Abstracts the differences between searching with a string and with a regex so they both can be used in the same way
    /// </summary>
    interface IStringScanner
    {
        IEnumerable<IntRange> Scan( string text );
        IEnumerable<IntRange> ScanAndReplace( string text, Action<string> replaceCallback );
    }
}