using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security.Permissions;
using Microsoft.Win32;

namespace FindInFiles
{
    public partial class FindForm : Form
    {
        private readonly ComboBoxHistory searchPathHistory;
        private readonly ComboBoxHistory searchPatternHistory;
        private readonly ComboBoxHistory replaceWithHistory;
        private readonly ComboBoxHistory searchExtensionsHistory;
        private readonly ComboBoxHistory excludeDirectoriesHistory;

        private readonly bool startInReplaceMode;


        public FindForm(bool startInReplaceMode)
        {
            this.startInReplaceMode = startInReplaceMode;

            InitializeComponent();
            textProgress.Text = "";

            searchPathHistory = new ComboBoxHistory("textSearchPath", comboSearchPath);
            searchPatternHistory = new ComboBoxHistory("textSearchPattern", comboSearchPattern);
            replaceWithHistory = new ComboBoxHistory("textReplaceWith", comboReplaceWith);
            searchExtensionsHistory = new ComboBoxHistory("textSearchExtensions", comboSearchExtensions);
            excludeDirectoriesHistory = new ComboBoxHistory("textDirectoryExcludes", comboExcludeDirectories);
        }

        private void SetProgressText(string progress_message)
        {
            textProgress.Text = Util.TrimLeadingText(progress_message ?? "", 40);
        }

        private void SafeInvoke(Action fn)
        {
            try
            {
                Invoke(fn);
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            /* swallow these as the form is going away */
        }

        private void SetButtonsEnabled(bool value)
        {
            buttonGo.Enabled = value;
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

        private void buttonGo_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == replaceTab)
                RunReplace();
            else
                RunFind();
        }

        private void RunFind()
        {
            SavePrefsToRegistry();
            SetButtonsEnabled(false);

            var fileOptions = new FindFileOptions(
                comboSearchPath.Text,
                Util.ParseSearchExtensions(comboSearchExtensions.Text),
                Util.ParseDirectoryExcludes(comboExcludeDirectories.Text));

            var lineOptions = new FindLineOptions(
                SearchText,
                checkMatchCase.Checked,
                checkUseRegex.Checked);

            RunFinderAsync(fileOptions, lineOptions);
        }

        private void RunReplace()
        {
            SavePrefsToRegistry();
            SetButtonsEnabled(false);

            var fileOptions = new FindFileOptions(
                comboSearchPath.Text,
                Util.ParseSearchExtensions(comboSearchExtensions.Text),
                Util.ParseDirectoryExcludes(comboExcludeDirectories.Text));

            var lineOptions = new FindLineOptions(
                SearchText,
                checkMatchCase.Checked,
                checkUseRegex.Checked,
                comboReplaceWith.Text);

            RunFinderAsync(fileOptions, lineOptions);
        }

        private void RunFinderAsync(FindFileOptions fileOptions, FindLineOptions lineOptions)
        {
            var finder = new Finder(fileOptions, lineOptions);
            finder.FileScanned += OnFinderOnFileScanned;

            // Do the find in the background
            var b = new BackgroundWorker();
            b.DoWork += delegate { finder.Find(); };
            b.RunWorkerCompleted += delegate { SafeInvoke(WorkerFinished); };
            b.RunWorkerAsync();
        }

        private void OnFinderOnFileScanned(string text)
        {
            SafeInvoke(() => SetProgressText(text));
        }

        private void WorkerFinished()
        {
            ParamsChanged();
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

            ParamsChanged();

            tabControl.SelectedTab = startInReplaceMode ? replaceTab : findTab;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
                SavePrefsToRegistry();
        }

        private void ParamsChanged()
        {
            SetButtonsEnabled(IsValid());
        }

        private bool IsValid()
        {
            return comboSearchPath.Text.Length > 0 && comboSearchPattern.Text.Length > 0;
        }

        private static RegistryKey OpenOrCreate(RegistryKey parent, string subKeyName)
        {
            return parent.OpenSubKey(subKeyName, true) ?? parent.CreateSubKey(subKeyName);
        }

        private static RegistryKey OpenPrefsRegistryKey()
        {
            var softwareKey = Registry.CurrentUser.OpenSubKey("Software", true);
            var eAddonsKey = OpenOrCreate(softwareKey, "e-addons");

            return OpenOrCreate(eAddonsKey, "FindInFiles");
        }

        private void LoadPrefsFromRegistry()
        {
            using (var prefsKey = OpenPrefsRegistryKey())
            {
                searchPathHistory.Load(prefsKey);
                searchPatternHistory.Load(prefsKey);
                searchExtensionsHistory.Load(prefsKey);
                excludeDirectoriesHistory.Load(prefsKey);

                object tmp = prefsKey.GetValue("checkUseRegex");
                checkUseRegex.Checked = (tmp is int && (int)tmp == 1);

                tmp = prefsKey.GetValue("checkMatchCase");
                checkMatchCase.Checked = (tmp is int && (int)tmp == 1);

                tmp = prefsKey.GetValue("windowLeft");
                if (tmp is int)
                    Left = (int)tmp;

                tmp = prefsKey.GetValue("windowTop");
                if (tmp is int)
                    Top = (int)tmp;
            }
        }

        private void SavePrefsToRegistry()
        {
            searchPathHistory.Grab();
            replaceWithHistory.Grab();
            searchPatternHistory.Grab();
            searchExtensionsHistory.Grab();
            excludeDirectoriesHistory.Grab();

            using (RegistryKey prefsKey = OpenPrefsRegistryKey())
            {
                searchPathHistory.Save(prefsKey);
                replaceWithHistory.Save(prefsKey);
                searchPatternHistory.Save(prefsKey);
                searchExtensionsHistory.Save(prefsKey);
                excludeDirectoriesHistory.Save(prefsKey);

                prefsKey.SetValue("checkUseRegex", checkUseRegex.Checked ? 1 : 0);
                prefsKey.SetValue("checkMatchCase", checkMatchCase.Checked ? 1 : 0);
                prefsKey.SetValue("windowLeft", Left);
                prefsKey.SetValue("windowTop", Top);
            }
        }

        private void OnButtonBrowse_Click(object sender, EventArgs e)
        {
            SelectWorkingFolder();
        }

        private void SelectWorkingFolder()
        {
            var d = new FolderBrowserDialog
                        {
                            ShowNewFolderButton = false,
                            SelectedPath = comboSearchPath.Text
                        };

            if (d.ShowDialog() == DialogResult.OK)
                comboSearchPath.Text = d.SelectedPath;
        }

        private void UseProjectDirectory_Click(object sender, EventArgs e)
        {
            UseProjectFolder(true);
        }

        private void UseProjectFolder(bool manuallyInvoked)
        {
            UseFolder(manuallyInvoked, "TM_PROJECT_DIRECTORY");
        }

        private void UseCurrentDirectory_Click(object sender, EventArgs e)
        {
            UseCurrentFolder(true);
        }

        private void UseCurrentFolder(bool manuallyInvoked)
        {
            UseFolder(manuallyInvoked, "TM_DIRECTORY");
        }

        private void UseCurrentWord_Click(object sender, EventArgs e)
        {
            UseSelectedWord(true);
        }

        private void UseSelectedWord(bool manuallyInvoked)
        {
            string word = Environment.GetEnvironmentVariable("TM_CURRENT_WORD");
            if (string.IsNullOrEmpty(word) && manuallyInvoked)
            {
                MessageBox.Show("No Current Word is set");
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
            string text = Environment.GetEnvironmentVariable("TM_SELECTED_TEXT");
            if (!string.IsNullOrEmpty(text))
                SearchText = text;
        }

        private void LoadDefaults()
        {
            if (SearchText.Length < 1 || textReplaceSearchPattern.Text.Length < 1)
                UseSelectedWord(false);

            if (comboSearchPath.Text.Length < 1 || textReplaceSearchPath.Text.Length < 1)
                UseProjectFolder(false);

            if (comboSearchExtensions.Text.Length < 1 || textReplaceSearchExtensions.Text.Length < 1)
                comboSearchExtensions.Text = "*.*";
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
                Close();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            comboSearchPattern.Select(0, SearchText.Length);
            comboSearchPattern.Focus();
        }

        private void comboSearchPattern_TextChanged(object sender, EventArgs e)
        {
            ParamsChanged();
        }

        private void comboSearchPath_TextChanged(object sender, EventArgs e)
        {
            ParamsChanged();
        }
    }
}