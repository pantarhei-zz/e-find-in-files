using System;
using System.Collections.Generic;
using System.Text;

namespace FindInFiles
{
	public delegate TResult Func<TResult>();
	public delegate TResult Func<T, TResult>( T arg );
	public delegate TResult Func<T1, T2, TResult>( T1 arg1, T2 arg2 );
	public delegate TResult Func<T1, T2, T3, TResult>( T1 arg1, T2 arg2, T3 arg3 );
	public delegate TResult Func<T1, T2, T3, T4, TResult>( T1 arg1, T2 arg2, T3 arg3, T4 arg4 );

	public delegate void Action();
	// Action<T> is already part of the base framework
	public delegate void Action<T1, T2>( T1 arg1, T2 arg2 );
	public delegate void Action<T1, T2, T3>( T1 arg1, T2 arg2, T3 arg3 );
	public delegate void Action<T1, T2, T3, T4>( T1 arg1, T2 arg2, T3 arg3, T4 arg4 );

	/// <summary>
	/// Selected methods borrowed from LinqBridge
	/// http://code.google.com/p/linqbridge/
	/// Copyright/license to them
	/// </summary>
	static class LinqBridge
	{
		public static IEnumerable<TOutput> Select<TInput, TOutput>( this IEnumerable<TInput> collection, Converter<TInput, TOutput> converter )
		{
			foreach( var item in collection )
				yield return converter( item );
		}

		public static IEnumerable<T> Where<T>( this IEnumerable<T> collection, Predicate<T> predicate )
		{
			foreach( var item in collection )
				if( predicate( item ) )
					yield return item;
		}

		public static bool Any<TSource>( this IEnumerable<TSource> source, Func<TSource, bool> predicate )
		{
			if( source == null ) throw new ArgumentNullException( "source" );
			if( predicate == null ) throw new ArgumentNullException( "predicate" );

			foreach( TSource element in source )
				if( predicate( element ) )
					return true;

			return false;
		}

		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
					   this IEnumerable<TSource> source,
					   Func<TSource, IEnumerable<TCollection>> collectionSelector,
					   Func<TSource, TCollection, TResult> resultSelector )
		{
			if( source == null ) throw new ArgumentNullException( "source" );
			if( collectionSelector == null ) throw new ArgumentNullException( "collectionSelector" );
			if( resultSelector == null ) throw new ArgumentNullException( "resultSelector" );

			foreach( TSource element in source )
				foreach( TCollection innerElement in collectionSelector( element ) )
					yield return resultSelector( element, innerElement );
		}

		public static IEnumerable<TSource> Concat<TSource>( this IEnumerable<TSource> first, IEnumerable<TSource> second )
		{
			if( first == null ) throw new ArgumentException( "first" );
			if( second == null ) throw new ArgumentException( "second" );

			foreach( TSource element in first )
				yield return element;

			foreach( TSource element in second )
				yield return element;
		}
	}
}
