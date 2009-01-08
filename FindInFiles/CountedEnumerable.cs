using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FindInFiles
{
	class CountedEnumerable<T> : IEnumerable<T>
	{
		private int m_count = 0;
		private IEnumerable<T> SourceEnumerable { get; set; }

		public int Count
		{ get { return m_count; } }

		public CountedEnumerable( IEnumerable<T> sourceEnumerable )
		{ SourceEnumerable = sourceEnumerable; }

		public IEnumerator<T> GetEnumerator()
		{
			foreach( var item in SourceEnumerable )
			{
				Interlocked.Increment( ref m_count );
				yield return item;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{ return GetEnumerator(); }
	}

	static class IEnumerableExtensions
	{
		public static CountedEnumerable<T> AsCounted<T>( this IEnumerable<T> sourceEnumerable )
		{
			return new CountedEnumerable<T>( sourceEnumerable );
		}
	}
}
