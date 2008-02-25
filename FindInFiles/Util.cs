using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Globalization;

// Extensions
namespace System.Runtime.CompilerServices
{
	sealed class ExtensionAttribute : Attribute { }
}

namespace FindInFiles
{
	public static class Util
	{
		/// <summary>Lazy-evaluated version of Array.ConvertAll which works over any IEnumerable</summary>
		/// <typeparam name="TInput">Type of input item</typeparam>
		/// <typeparam name="TOutput">Type of output item</typeparam>
		/// <param name="collection">Collection of input items</param>
		/// <param name="converter">Delegate to use to do the conversion</param>
		/// <returns>Collection of output items</returns>
		public static IEnumerable<TOutput> Map<TInput, TOutput>( this IEnumerable<TInput> collection, Converter<TInput, TOutput> converter )
		{
			foreach( TInput item in collection )
				yield return converter( item );
		}

		/// <summary>Removes invalid characters from a path</summary>
		/// <param name="path">The input path</param>
		/// <returns>The input path, with all characters from Path.GetInvalidPathChars() stripped out</returns>
		public static string CleanPath( string path )
		{
			foreach( char c in Path.GetInvalidPathChars() )
				path = path.Replace( c.ToString(), "" );
			return path;
		}

		/// <summary>
		/// Invokes 'cygpath' to convert a cygwin path to a windows path. 
		/// Note the cygpath executable must be in your PATH, or an exception will 
		/// silently occur and your original unconverted path will be returned
		/// </summary>
		/// <param name="path">The input path, potentially in cygwin format</param>
		/// <returns>If the path does not contain the / character, it is simply returned.
		/// If it does, then cygpath is invoked, and the value returned from cygpath is returned.
		/// If an error occurs invoking cygpath, then the error is swallowed and the input path is simply returned.</returns>
		public static string ConvertCygpath( string path )
		{
			if( !path.Contains( "/" ) )
				return path;

			var startInfo = new ProcessStartInfo( "cygpath", "-w '" + path.Replace( "'", "\\'" ) + "'" ) {
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true
			};

			try
			{
				return CleanPath( Process.Start(startInfo).StandardOutput.ReadToEnd() );
			}
			catch( Win32Exception) // file not found - can't find cygpath.exe. Don't care about being more specific with our error
			{ }
			return path;
		}

		/// <summary>
		/// Cleans the path and runs ConvertCygpath over it
		/// </summary>
		/// <param name="path">Dirty cygwin path</param>
		/// <returns>Clean windows friendly path</returns>
		public static string CleanAndConvertCygpath( string path )
		{
			return ConvertCygpath( CleanPath( path ) );
		}
	}
}
