using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security.Permissions;
using JetBrains.Annotations;
using Microsoft.Win32;
using System.Diagnostics;

namespace FindInFiles
{
	public partial class FindForm : Form
	{
		private const int FIND_HEIGHT = 336;
		private const int REPLACE_HEIGHT = 356;

		private readonly ComboBoxHistory SearchPathHistory;
		private readonly ComboBoxHistory SearchPatternHistory;
		private readonly ComboBoxHistory ReplaceWithHistory;
		private readonly ComboBoxHistory SearchExtensionsHistory;
		private readonly ComboBoxHistory ExcludeDirectoriesHistory;

		private readonly bool StartInReplaceMode;


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

	    private void SetProgressText([NotNull] string txt )
		{
			Debug.Assert( txt != null );
			textProgress.Text = Util.TrimLeadingText(txt, 40);
		}

	    private void SafeInvoke( Action fn )
		{
			try
			{
				Invoke( fn );
			}
			catch( ObjectDisposedException ) { }
			catch( InvalidOperationException ) { }
			/* swallow these as the form is going away */
		}

	    private void SetButtonsEnabled(bool value)
	    {
	        buttonFind.Enabled = value;
	        buttonReplace.Enabled = value;
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
		    if (tabControl.SelectedTab == replaceTab)
		        RunReplace();
		    else
		        RunFind();
		}

	    private void RunFind()
	    {
	        SavePrefsToRegistry();

	        var fileOptions = new FindFileOptions(
	            comboSearchPath.Text,
	            Util.ParseSearchExtensions(comboSearchExtensions.Text),
	            Util.ParseDirectoryExcludes(comboExcludeDirectories.Text) );

	        var lineOptions = new FindLineOptions(
	            SearchText,
	            checkMatchCase.Checked,
	            checkUseRegex.Checked );

	        SetButtonsEnabled(false);

	        var finder = new Finder( fileOptions, lineOptions );
	        finder.FileScanned += ( text ) => SafeInvoke( () => SetProgressText( text ) );

	        // Do the find in the background
	        var b = new BackgroundWorker();
	        b.DoWork += delegate { finder.Find(); };
	        b.RunWorkerCompleted += delegate { SafeInvoke(WorkerFinished); };
	        b.RunWorkerAsync();
	    }

	    private void OnButtonReplace_Click( object sender, EventArgs e )
	    {
	        RunReplace();
	    }

	    private void RunReplace()
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

	        SetButtonsEnabled(false);

	        var finder = new Finder(findFileOptions, findLineOptions);
	        finder.FileScanned += (text) => SafeInvoke(() => SetProgressText(text));

	        // Do the find in the background
	        var b = new BackgroundWorker();
	        b.DoWork += delegate { finder.Find(); };
	        b.RunWorkerCompleted += delegate { SafeInvoke(WorkerFinished); };
	        b.RunWorkerAsync();
	    }

	    private void WorkerFinished()
        {
            OnParamsChanged();
            SetButtonsEnabled(true);
            SetProgressText("");
            Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

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

            OnParamsChanged();

            tabControl.SelectedTab = StartInReplaceMode ? replaceTab : findTab;

            UpdateFormHeightForSelectedTab();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
                SavePrefsToRegistry();
        }

		private void OnParamsChanged()
		{
			SetButtonsEnabled(IsValid());
		}

		private bool IsValid()
		{
			return comboSearchPath.Text.Length > 0 && SearchText.Length > 0;
		}

		private static RegistryKey OpenOrCreate( RegistryKey parent, string subKeyName )
		{
		    return parent.OpenSubKey( subKeyName, true ) ?? parent.CreateSubKey( subKeyName );
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
		    SelectWorkingFolder();
		}

	    private void SelectWorkingFolder()
	    {
	        var fd = new FolderBrowserDialog {
	                                             ShowNewFolderButton = false,
	                                             SelectedPath = comboSearchPath.Text
	                                         };

	        if( fd.ShowDialog() == DialogResult.OK )
	            comboSearchPath.Text = fd.SelectedPath;
	    }

	    private void UseProjectDirectory_Click( object sender, EventArgs e )
	    {
	        UseProjectFolder(true);
	    }

	    private void UseProjectFolder(bool manuallyInvoked)
	    {
            UseFolder(manuallyInvoked, "TM_PROJECT_DIRECTORY");
	    }

	    private void UseCurrentDirectory_Click( object sender, EventArgs e )
	    {
	        UseCurrentFolder(true);
	    }

	    private void UseCurrentFolder(bool manuallyInvoked)
	    {
	        UseFolder(manuallyInvoked, "TM_DIRECTORY");
	    }

	    private void UseCurrentWord_Click( object sender, EventArgs e )
	    {
	        UseSelectedWord(true);
	    }

	    private void UseSelectedWord(bool manuallyInvoked)
	    {
	        string word = Environment.GetEnvironmentVariable( "TM_CURRENT_WORD" );
	        if( string.IsNullOrEmpty(word) && manuallyInvoked )
	        {
	            MessageBox.Show( "No Current Word is set" );
	            return;
	        }
	        SearchText = word;
	    }

        private void UseFolder(bool manuallyInvoked, string envvar)
        {
            var dir = Environment.GetEnvironmentVariable(envvar);
            if (string.IsNullOrEmpty(dir) && manuallyInvoked)
            {
                MessageBox.Show("\"" + envvar + "\" is not set.");
                return;
            }

            comboSearchPath.Text = Util.CleanAndConvertCygpath(dir);
        }

	    private void LoadSelectedText()
		{
			string text = Environment.GetEnvironmentVariable( "TM_SELECTED_TEXT" );
			if( !string.IsNullOrEmpty(text) )
				SearchText = text;
		}

		private void LoadDefaults()
		{
		    if (SearchText.Length < 1 || textReplaceSearchPattern.Text.Length < 1)
		        UseSelectedWord(false);

		    if (comboSearchPath.Text.Length < 1 || textReplaceSearchPath.Text.Length < 1)
		        UseProjectFolder(false);

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

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            comboSearchPattern.Select(0, SearchText.Length);
            comboSearchPattern.Focus();
        }

		private void OnTabChanged( object sender, EventArgs e )
		{
		    UpdateFormHeightForSelectedTab();
		}

	    private void UpdateFormHeightForSelectedTab()
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