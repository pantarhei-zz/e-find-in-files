using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace FindInFiles
{
    internal interface ICountedEnumerable<T> : IEnumerable<T>
    {
        int Count { get; }
    }

    class CountedEnumerable<T> : ICountedEnumerable<T>
    {
        private int m_count;
        private readonly IEnumerable<T> sourceEnumerable;

        public int Count { get { return m_count; } }

        public CountedEnumerable(IEnumerable<T> sourceEnumerable)
        {
            this.sourceEnumerable = sourceEnumerable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in sourceEnumerable)
            {
                Interlocked.Increment(ref m_count);
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }

    static class IEnumerableExtensions
    {
        public static CountedEnumerable<T> AsCounted<T>(this IEnumerable<T> sourceEnumerable)
        {
            return new CountedEnumerable<T>(sourceEnumerable);
        }
    }
}
