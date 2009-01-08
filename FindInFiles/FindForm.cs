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
	public partial class FindForm : Form
	{
		private const int FIND_HEIGHT = 336;
		private const int REPLACE_HEIGHT = 356;

		// ----- Member Variables ---------------------------------------------

		private readonly ComboBoxHistory SearchPathHistory;
		private readonly ComboBoxHistory SearchPatternHistory;
		private readonly ComboBoxHistory ReplaceWithHistory;
		private readonly ComboBoxHistory SearchExtensionsHistory;
		private readonly ComboBoxHistory ExcludeDirectoriesHistory;

		private readonly bool StartInReplaceMode;

		// ----- Member Functions ---------------------------------------------

		public FindForm( bool startInReplaceMode )
		{
			StartInReplaceMode = startInReplaceMode;

			InitializeComponent();

			SearchPathHistory         = new ComboBoxHistory( "textSearchPath", comboSearchPath );
			SearchPatternHistory      = new ComboBoxHistory( "textSearchPattern", comboSearchPattern );
			ReplaceWithHistory        = new ComboBoxHistory( "textReplaceWith", comboReplaceWith );
			SearchExtensionsHistory   = new ComboBoxHistory( "textSearchExtensions", comboSearchExtensions );
			ExcludeDirectoriesHistory = new ComboBoxHistory( "textDirectoryExcludes", comboExcludeDirectories );
		}

		private string TrimLeadingText(string str, int length)
		{
			Debug.Assert( str != null );
			Debug.Assert( length > 3 );

			if (str.Length > length)
				return "..." + str.Substring(str.Length - (length - 3), (length - 3));
			else
				return str;
		}

		public void SetProgressText( string txt )
		{
			Debug.Assert( txt != null );
			textProgress.Text = TrimLeadingText(txt, 40);
		}

		public void SetReplaceProgressText( string txt )
		{
			Debug.Assert( txt != null );
			textReplaceProgress.Text = TrimLeadingText(txt, 40);
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

		private string SearchText
		{
			get { return comboSearchPattern.Text; }
			set { comboSearchPattern.Text = value; }
		}

		private string ReplaceText
		{
			get { return comboReplaceWith.Text; }
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
				comboSearchPath.Text,
				Util.ParseSearchExtensions(comboSearchExtensions.Text),
				Util.ParseDirectoryExcludes(comboExcludeDirectories.Text) );

			var findLineOptions = new FindLineOptions(
				SearchText,
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
				comboSearchPath.Text,
				Util.ParseSearchExtensions(comboSearchExtensions.Text),
				Util.ParseDirectoryExcludes(comboExcludeDirectories.Text));

			var findLineOptions = new FindLineOptions(
				SearchText,
				checkMatchCase.Checked,
				checkUseRegex.Checked,
				comboReplaceWith.Text);

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

			comboSearchPattern.Target = textReplaceSearchPattern;
			//textReplaceWith isn't linked anywhere
			comboSearchPath.Target = textReplaceSearchPath;
			comboSearchExtensions.Target = textReplaceSearchExtensions;
			comboExcludeDirectories.Target = textReplaceDirectoryExcludes;

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
			ButtonsEnabled = IsValid();
		}

		private bool IsValid()
		{
			return comboSearchPath.Text.Length > 0 && SearchText.Length > 0;
		}

		// ------ Preference Loading Utils ------------------------------------

		private static RegistryKey OpenOrCreate( RegistryKey parent, string subKeyName )
		{
			Debug.Assert( parent != null );
			Debug.Assert( subKeyName != null );

			var subKey = parent.OpenSubKey( subKeyName, true );
			if( subKey == null )
				subKey = parent.CreateSubKey( subKeyName );

			return subKey;
		}

		private static RegistryKey OpenPrefsRegistryKey()
		{
			var softwareKey = Registry.CurrentUser.OpenSubKey( "Software", true );
			var eAddonsKey = OpenOrCreate( softwareKey, "e-addons" );

			return OpenOrCreate( eAddonsKey, "FindInFiles" );
		}

		private void LoadPrefsFromRegistry()
		{
			using( var prefsKey = OpenPrefsRegistryKey() )
			{
				SearchPathHistory.Load( prefsKey );
				SearchPatternHistory.Load( prefsKey );
				SearchExtensionsHistory.Load( prefsKey );
				ExcludeDirectoriesHistory.Load( prefsKey );

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
			ExcludeDirectoriesHistory.Grab();

			using( RegistryKey prefsKey = OpenPrefsRegistryKey() )
			{
				SearchPathHistory.Save( prefsKey );
				ReplaceWithHistory.Save( prefsKey );
				SearchPatternHistory.Save( prefsKey );
				SearchExtensionsHistory.Save( prefsKey );
				ExcludeDirectoriesHistory.Save( prefsKey );

				prefsKey.SetValue( "checkUseRegex", checkUseRegex.Checked ? 1 : 0 );
				prefsKey.SetValue( "checkMatchCase", checkMatchCase.Checked ? 1 : 0 );
				prefsKey.SetValue( "windowLeft", Left );
				prefsKey.SetValue( "windowTop", Top );
			}
		}

		private void OnButtonBrowse_Click( object sender, EventArgs e )
		{
			var fd = new FolderBrowserDialog {
				ShowNewFolderButton = false,
				SelectedPath = comboSearchPath.Text
			};

			if( fd.ShowDialog().IsOK() )
				comboSearchPath.Text = fd.SelectedPath;
		}

		private void UseProjectDirectory_Click( object sender, EventArgs e )
		{
			var projectDir = Environment.GetEnvironmentVariable( "TM_PROJECT_DIRECTORY" );
			if( projectDir == null || projectDir.Length < 1 && sender != null ) //only show message if we were invoked by a button press
			{
				MessageBox.Show( "No Project Directory is set" );
				return;
			}
			comboSearchPath.Text = Util.CleanAndConvertCygpath(projectDir);
		}

		private void UseCurrentDirectory_Click( object sender, EventArgs e )
		{
			var dir = Environment.GetEnvironmentVariable( "TM_DIRECTORY" );
			if( dir == null || dir.Length < 1 && sender != null ) //only show message if we were invoked by a button press
			{
				MessageBox.Show( "No Current Directory is set" );
				return;
			}
			comboSearchPath.Text = Util.CleanAndConvertCygpath(dir);
		}

		private void UseCurrentWord_Click( object sender, EventArgs e )
		{
			string word = Environment.GetEnvironmentVariable( "TM_CURRENT_WORD" );
			if( word == null || word.Length < 1 && sender != null ) //only show message if we were invoked by a button press
			{
				MessageBox.Show( "No Current Word is set" );
				return;
			}
			SearchText = word;
		}

		private void LoadSelectedText()
		{
			string text = Environment.GetEnvironmentVariable( "TM_SELECTED_TEXT" );
			if( text != null && text.Length > 0 )
				SearchText = text;
		}

		private void LoadDefaults()
		{
			if( SearchText.Length < 1 || textReplaceSearchPattern.Text.Length < 1 )
				UseCurrentWord_Click( null, null );

			if( comboSearchPath.Text.Length < 1 || textReplaceSearchPath.Text.Length < 1 )
				UseProjectDirectory_Click( null, null );

			if( comboSearchExtensions.Text.Length < 1 || textReplaceSearchExtensions.Text.Length < 1 )
				comboSearchExtensions.Text = "*.*";
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
			comboSearchPattern.Select( 0, SearchText.Length );
			comboSearchPattern.Focus();
		}

		private void OnTabChanged( object sender, EventArgs e )
		{
			if( tabControl.SelectedTab == replaceTab )
			{
				Height = REPLACE_HEIGHT;
				tabControl.Height = REPLACE_HEIGHT - 25;
				replaceTab.Height = 305;
			}
			else
			{
				Height = FIND_HEIGHT;
				tabControl.Height = FIND_HEIGHT - 25;
			}
		}
	}
}