using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FindInFiles
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			bool startInReplaceMode = (Environment.CommandLine.IndexOf( "replace", StringComparison.InvariantCultureIgnoreCase ) != -1);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run( new FindForm( startInReplaceMode ) );
		}
	}
}
