using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using Microsoft.Win32;

namespace FindInFiles
{
	public delegate void Func();

	public partial class FindForm : Form
	{
		private readonly ComboBoxHistory SearchPathHistory;
		private readonly ComboBoxHistory SearchPatternHistory;
		private readonly ComboBoxHistory SearchExtensionsHistory;
		private readonly ComboBoxHistory DirectoryExcludesHistory;

		public FindForm()
		{
			InitializeComponent();

			SearchPathHistory = new ComboBoxHistory( "textSearchPath", textSearchPath );
			SearchPatternHistory = new ComboBoxHistory( "textSearchPattern", textSearchPattern );
			SearchExtensionsHistory = new ComboBoxHistory( "textSearchExtensions", textSearchExtensions );
			DirectoryExcludesHistory = new ComboBoxHistory( "textDirectoryExcludes", textDirectoryExcludes );
		}

		public void SetProgressText( string txt )
		{
			if( txt.Length > 40 )
				this.textProgress.Text = "..." + txt.Substring( txt.Length - 37, 37 );
			else
				this.textProgress.Text = txt;
		}

		public void SafeInvoke( Func fn )
		{
			try
			{
				Invoke( fn );
			}
			catch( ObjectDisposedException ) { }
			catch( InvalidOperationException ) { }
			/* swallow these as the form is going away */
		}

		private void OnButtonFind_Click( object sender, EventArgs e )
		{
			SavePrefsToRegistry();

			FindOptions options = new FindOptions(
				textSearchPath.Text,
				textSearchPattern.Text,
				checkMatchCase.Checked,
				checkUseRegex.Checked,
				textSearchExtensions.Text,
				textDirectoryExcludes.Text );

			buttonFind.Enabled = false;

			Finder f = new Finder( options );
			f.ScanningFile += delegate( string text ) {
				SafeInvoke( delegate() {
					SetProgressText( text );
				} );
			};

			new Thread( new ThreadStart( delegate() {
				string output = "";
				try
				{
					FindResults results = f.Find();

					SafeInvoke( (Func)delegate() {
						SetProgressText( "..." );
					} );
					Console.Write( results.ToString() );

					SafeInvoke( (Func)delegate() {
						OnParamsChanged( null, null );
						buttonFind.Enabled = true;
						SetProgressText( "" );
						Close();
					} );
				}
				catch( ArgumentException ex )
				{
					output = ex.Message;
					SafeInvoke( (Func)delegate() {
						OnParamsChanged( null, null );
						SetProgressText( "" );
						MessageBox.Show( this, output, "Error" );
					} );
				}
			} ) ).Start();
		}

		private void OnThis_Load( object sender, EventArgs e )
		{
			LoadPrefsFromRegistry();
			LoadSelectedText();
			LoadDefaults();

			OnParamsChanged( this, null );
		}

		private void OnThis_Closing( object sender, FormClosingEventArgs e )
		{
			SavePrefsToRegistry();
		}

		private void OnParamsChanged( object sender, EventArgs e )
		{
			buttonFind.Enabled = IsValid();
		}

		private bool IsValid()
		{
			return textSearchPath.Text.Length > 0 && textSearchPattern.Text.Length > 0;
		}

		// ------ Preference Loading Utils ------------------------------------

		private RegistryKey OpenOrCreate( RegistryKey parent, string subKeyName )
		{
			RegistryKey ret = parent.OpenSubKey( subKeyName, true );
			if( ret == null )
				ret = parent.CreateSubKey( subKeyName );

			return ret;
		}

		private RegistryKey OpenPrefsRegistryKey()
		{
			RegistryKey software = Registry.CurrentUser.OpenSubKey( "Software", true );
			RegistryKey eAddons = OpenOrCreate( software, "e-addons" );

			return OpenOrCreate( eAddons, "FindInFiles" );
		}

		private void LoadPrefsFromRegistry()
		{
			using( RegistryKey prefsKey = OpenPrefsRegistryKey() )
			{
				SearchPathHistory.Load( prefsKey );
				SearchPatternHistory.Load( prefsKey );
				SearchExtensionsHistory.Load( prefsKey );
				DirectoryExcludesHistory.Load( prefsKey );

				object tmp = prefsKey.GetValue( "checkUseRegex" );
				checkUseRegex.Checked = (tmp is int && (int)tmp == 1);

				tmp = prefsKey.GetValue( "checkMatchCase" );
				checkMatchCase.Checked = (tmp is int && (int)tmp == 1);
			}
		}

		private void SavePrefsToRegistry()
		{
			SearchPathHistory.Grab();
			SearchPatternHistory.Grab();
			SearchExtensionsHistory.Grab();
			DirectoryExcludesHistory.Grab();

			using( RegistryKey prefsKey = OpenPrefsRegistryKey() )
			{
				SearchPathHistory.Save( prefsKey );
				SearchPatternHistory.Save( prefsKey );
				SearchExtensionsHistory.Save( prefsKey );
				DirectoryExcludesHistory.Save( prefsKey );

				prefsKey.SetValue( "checkUseRegex", checkUseRegex.Checked ? 1 : 0 );
				prefsKey.SetValue( "checkMatchCase", checkMatchCase.Checked ? 1 : 0 );
			}
		}

		private void OnButtonBrowse_Click( object sender, EventArgs e )
		{
			FolderBrowserDialog fd = new FolderBrowserDialog();
			fd.ShowNewFolderButton = false;
			fd.SelectedPath = textSearchPath.Text;
			if( fd.ShowDialog() == DialogResult.OK )
				textSearchPath.Text = fd.SelectedPath;
		}

		private void UseProjectDirectory_Click( object sender, EventArgs e )
		{
			string projectDir = Environment.GetEnvironmentVariable( "TM_PROJECT_DIRECTORY" );
			if( projectDir == null || projectDir.Length < 1 && sender != null ) //only show message if we were invoked by a button press
			{
				MessageBox.Show( "No Project Directory is set" );
				return;
			}
			textSearchPath.Text = projectDir;
		}

		private void UseCurrentDirectory_Click( object sender, EventArgs e )
		{
			string dir = Environment.GetEnvironmentVariable( "TM_DIRECTORY" );
			if( dir == null || dir.Length < 1 && sender != null ) //only show message if we were invoked by a button press
			{
				MessageBox.Show( "No Current Directory is set" );
				return;
			}
			textSearchPath.Text = dir;
		}

		private void UseCurrentWord_Click( object sender, EventArgs e )
		{
			string word = Environment.GetEnvironmentVariable( "TM_CURRENT_WORD" );
			if( word == null || word.Length < 1 && sender != null ) //only show message if we were invoked by a button press
			{
				MessageBox.Show( "No Current Word is set" );
				return;
			}
			textSearchPattern.Text = word;
		}

		private void LoadSelectedText()
		{
			string text = Environment.GetEnvironmentVariable( "TM_SELECTED_TEXT" );
			if( text != null && text.Length > 0 )
				textSearchPattern.Text = text;
		}

		private void LoadDefaults()
		{
			if( textSearchPattern.Text.Length < 1 )
				UseCurrentWord_Click( null, null );

			if( textSearchPath.Text.Length < 1 )
				UseProjectDirectory_Click( null, null );

			if( textSearchPath.Text.Length < 1 )
				UseCurrentDirectory_Click( null, null );

			if( textSearchExtensions.Text.Length < 1 )
				textSearchExtensions.Text = "*.*";
		}

		private void FindForm_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Escape )
				Close();
		}

		private void OnThis_Shown( object sender, EventArgs e )
		{
			textSearchPattern.Select( 0, textSearchPattern.Text.Length );
			textSearchPattern.Focus();
		}
	}
}