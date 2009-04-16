using System;

namespace FindInFiles
{
    internal interface IOutputMatches
    {
        void OutputHeader();
        void OutputMatch( Match match );
        void OutputFooter();
    }
}