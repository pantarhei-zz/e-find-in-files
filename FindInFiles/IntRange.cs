using System;

namespace FindInFiles
{
    public class IntRange
    {
        public readonly int Lower;
        public readonly int Upper;

        public IntRange(int lower, int upper)
        {
            Lower = lower;
            Upper = upper;
        }
    }
}