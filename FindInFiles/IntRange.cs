using System;

namespace FindInFiles
{
    /// <summary>
    /// Default implentation of range using an int
    /// </summary>
    class IntRange
    {
        public readonly int Lower, Upper;

        public IntRange(int lower, int upper)
        {
            Lower = lower;
            Upper = upper;
        }
    }
}