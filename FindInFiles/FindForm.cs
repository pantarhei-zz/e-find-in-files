using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Diagnostics;

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

	public partial class FindForm : Form
	{
		private readonly ComboBoxHistory SearchPathHistory;
		private readonly ComboBoxHistory SearchPatternHistory;
		private readonly ComboBoxHistory ReplaceWithHistory;
		private readonly ComboBoxHistory SearchExtensionsHistory;
		private readonly ComboBoxHistory DirectoryExcludesHistory;

		private readonly bool StartInReplaceMode;

		private const int FIND_HEIGHT = 336;
		private const int REPLACE_HEIGHT = 356;

		public FindForm( bool startInReplaceMode )
		{
			StartInReplaceMode = startInReplaceMode;

			InitializeComponent();

			SearchPathHistory        = new ComboBoxHistory( "textSearchPath", textSearchPath );
			SearchPatternHistory     = new ComboBoxHistory( "textSearchPattern", textSearchPattern );
			ReplaceWithHistory       = new ComboBoxHistory( "textReplaceWith", textReplaceWith );
			SearchExtensionsHistory  = new ComboBoxHistory( "textSearchExtensions", textSearchExtensions );
			DirectoryExcludesHistory = new ComboBoxHistory( "textDirectoryExcludes", textDirectoryExcludes );
		}

		public void SetProgressText( string txt )
		{
			Debug.Assert( txt != null );

			if( txt.Length > 40 )
				this.textProgress.Text = "..." + txt.Substring( txt.Length - 37, 37 );
			else
				this.textProgress.Text = txt;
		}

		public void SetReplaceProgressText( string txt )
		{
			Debug.Assert( txt != null );

			if( txt.Length > 40 )
				this.textReplaceProgress.Text = "..." + txt.Substring( txt.Length - 37, 37 );
			else
				this.textReplaceProgress.Text = txt;
		}

		public void SafeInvoke( Action fn )
		{
			Debug.Assert( fn != null );

			try
			{
				Invoke( fn );
			}
			catch( ObjectDisposedException ) { }
			catch( InvalidOperationException ) { }
			/* swallow these as the form is going away */
		}

		private bool ButtonsEnabled
		{
			set
			{
				buttonFind.Enabled = 
				buttonReplace.Enabled =
					value;
			}
		}

		private void OnButtonFind_Click( object sender, EventArgs e )
		{
			// if the user presses the enter key, this fires even though the tab is wrong
			if( tabControl.SelectedTab == replaceTab )
			{
				OnButtonReplace_Click( sender, e );
				return;
			}

			SavePrefsToRegistry();

			var findFileOptions = new FindFileOptions(
				textSearchPath.Text,
				Util.ParseSearchExtensions(textSearchExtensions.Text),
				Util.ParseDirectoryExcludes(textDirectoryExcludes.Text) );

			var findLineOptions = new FindLineOptions(
				textSearchPattern.Text,
				checkMatchCase.Checked,
				checkUseRegex.Checked );

			ButtonsEnabled = false;

			var finder = new Finder( findFileOptions, findLineOptions );
			finder.FileScanned += ( text ) => SafeInvoke( () => SetProgressText( text ) );

			// Do the find in the background
			var b = new BackgroundWorker();
			b.DoWork += ( _sender, _eventargs ) => finder.Find();
			b.RunWorkerCompleted += ( _sender, _eventargs ) => {
				SafeInvoke( () => {
					OnParamsChanged( null, null );
					ButtonsEnabled = true;
					SetProgressText( "" );
					Close();
				} );
			};
			b.RunWorkerAsync();
		}

		private void OnButtonReplace_Click( object sender, EventArgs e )
		{
			SavePrefsToRegistry();

			var findFileOptions = new FindFileOptions(
				textSearchPath.Text,
				Util.ParseSearchExtensions(textSearchExtensions.Text),
				Util.ParseDirectoryExcludes(textDirectoryExcludes.Text));

			var findLineOptions = new FindLineOptions(
				textSearchPattern.Text,
				checkMatchCase.Checked,
				checkUseRegex.Checked,
				textReplaceWith.Text);

			ButtonsEnabled = false;

			var finder = new Finder(findFileOptions, findLineOptions);
			finder.FileScanned += (text) => SafeInvoke(() => SetProgressText(text));

			// Do the find in the background
			var b = new BackgroundWorker();
			b.DoWork += (_sender, _eventargs) => finder.Find();
			b.RunWorkerCompleted += (_sender, _eventargs) =>
			{
				SafeInvoke(() =>
				{
					OnParamsChanged(null, null);
					ButtonsEnabled = true;
					SetProgressText("");
					Close();
				});
			};
			b.RunWorkerAsync();
		}

		private void OnThis_Load( object sender, EventArgs e )
		{
			// link the controls

			checkUseRegex.Target = checkReplaceUseRegex;
			checkMatchCase.Target = checkReplaceMatchCase;

			textSearchPattern.Target = textReplaceSearchPattern;
			//textReplaceWith isn't linked anywhere
			textSearchPath.Target = textReplaceSearchPath;
			textSearchExtensions.Target = textReplaceSearchExtensions;
			textDirectoryExcludes.Target = textReplaceDirectoryExcludes;

			// load prefs from registry

			LoadPrefsFromRegistry();
			LoadSelectedText();
			LoadDefaults();

			OnParamsChanged( this, null );

			if( StartInReplaceMode )
				tabControl.SelectedTab = replaceTab;
			else
				tabControl.SelectedTab = findTab;

			OnTabChanged( this, null );
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

		private static RegistryKey OpenOrCreate( RegistryKey parent, string subKeyName )
		{
			Debug.Assert( parent != null );
			Debug.Assert( subKeyName != null );

			RegistryKey ret = parent.OpenSubKey( subKeyName, true );
			if( ret == null )
				ret = parent.CreateSubKey( subKeyName );

			return ret;
		}

		private static RegistryKey OpenPrefsRegistryKey()
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

				tmp = prefsKey.GetValue( "windowLeft" );
				if( tmp is int )
					Left = (int)tmp;

				tmp = prefsKey.GetValue( "windowTop" );
				if( tmp is int )
					Top = (int)tmp;
			}
		}

		private void SavePrefsToRegistry()
		{
			SearchPathHistory.Grab();
			ReplaceWithHistory.Grab();
			SearchPatternHistory.Grab();
			SearchExtensionsHistory.Grab();
			DirectoryExcludesHistory.Grab();

			using( RegistryKey prefsKey = OpenPrefsRegistryKey() )
			{
				SearchPathHistory.Save( prefsKey );
				ReplaceWithHistory.Save( prefsKey );
				SearchPatternHistory.Save( prefsKey );
				SearchExtensionsHistory.Save( prefsKey );
				DirectoryExcludesHistory.Save( prefsKey );

				prefsKey.SetValue( "checkUseRegex", checkUseRegex.Checked ? 1 : 0 );
				prefsKey.SetValue( "checkMatchCase", checkMatchCase.Checked ? 1 : 0 );
				prefsKey.SetValue( "windowLeft", Left );
				prefsKey.SetValue( "windowTop", Top );
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
			textSearchPath.Text = Util.CleanAndConvertCygpath(projectDir);
		}

		private void UseCurrentDirectory_Click( object sender, EventArgs e )
		{
			string dir = Environment.GetEnvironmentVariable( "TM_DIRECTORY" );
			if( dir == null || dir.Length < 1 && sender != null ) //only show message if we were invoked by a button press
			{
				MessageBox.Show( "No Current Directory is set" );
				return;
			}
			textSearchPath.Text = Util.CleanAndConvertCygpath(dir);
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
			if( textSearchPattern.Text.Length < 1 || textReplaceSearchPattern.Text.Length < 1 )
				UseCurrentWord_Click( null, null );

			if( textSearchPath.Text.Length < 1 || textReplaceSearchPath.Text.Length < 1 )
				UseProjectDirectory_Click( null, null );

			if( textSearchExtensions.Text.Length < 1 || textReplaceSearchExtensions.Text.Length < 1 )
				textSearchExtensions.Text = "*.*";
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( keyData == Keys.Escape )
				Close();

			return base.ProcessCmdKey( ref msg, keyData );
		}

		private void OnThis_Shown( object sender, EventArgs e )
		{
			textSearchPattern.Select( 0, textSearchPattern.Text.Length );
			textSearchPattern.Focus();
		}

		private void OnTabChanged( object sender, EventArgs e )
		{
			if( tabControl.SelectedTab == replaceTab )
			{
				this.Height = REPLACE_HEIGHT;
				tabControl.Height = REPLACE_HEIGHT - 25;
				replaceTab.Height = 305;
			}
			else
			{
				this.Height = FIND_HEIGHT;
				tabControl.Height = FIND_HEIGHT - 25;
			}
		}
	}
}