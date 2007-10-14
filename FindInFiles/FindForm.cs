using System;
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
        public FindForm()
        {
            InitializeComponent();
        }

        public void SetProgressText( string txt )
        {
            if (txt.Length > 40)
                this.textProgress.Text = "..."+txt.Substring(txt.Length - 37, 37);
            else
                this.textProgress.Text = txt;
        }

        private void OnButtonFind_Click(object sender, EventArgs e)
        {
            SavePrefsToRegistry();

            FindOptions opts = new FindOptions();
            opts.SearchPath = textSearchPath.Text;
            opts.MatchCase = checkMatchCase.Checked;
            opts.UseRegex = checkUseRegex.Checked;
            opts.SetSearchExtensions( textSearchExtensions.Text );
            opts.SetDirectoryExcludes( textDirectoryExcludes.Text );
            opts.SearchPattern = textSearchPattern.Text;

            
            buttonFind.Enabled = false;

            FindForm self = this;
            Finder.CurrentFileCallback callback = delegate(string currentFile)
            {
                self.Invoke( (Func)delegate()
                {
                    self.SetProgressText( currentFile );
                } );
            };

            Thread thr = new Thread(new ThreadStart(delegate()
            {
                Console.Write(
                    (Finder.Find(opts, callback).ToString()));
                     

                self.Invoke((Func)delegate()
                {
                    self.buttonFind.Enabled = true;
                    self.SetProgressText("");
                    self.Close();
                });
            }));

            thr.Start();                
        }

        private RegistryKey OpenOrCreate(RegistryKey parent, string subKeyName)
        {
            RegistryKey ret = parent.OpenSubKey(subKeyName, true);
            if (ret == null)
                ret = parent.CreateSubKey(subKeyName);
            
            return ret;
        }

        private RegistryKey OpenPrefsRegistryKey()
        {
            RegistryKey software = Registry.CurrentUser.OpenSubKey( "Software", true );
            RegistryKey eAddons = OpenOrCreate( software, "e-addons" );
            
            return OpenOrCreate(eAddons, "FindInFiles");            
        }

        private void LoadPrefsFromRegistry()
        {
            using (RegistryKey prefsKey = OpenPrefsRegistryKey())
            {
                textSearchPath.Text = (prefsKey.GetValue("textSearchPath") as string) ?? "";
                textSearchPattern.Text = (prefsKey.GetValue("textSearchPattern") as string) ?? "";
                textSearchExtensions.Text = (prefsKey.GetValue("textSearchExtensions") as string) ?? "";
                textDirectoryExcludes.Text = (prefsKey.GetValue("textDirectoryExcludes") as string) ?? "";

                object tmp = prefsKey.GetValue("checkUseRegex");
                checkUseRegex.Checked = (tmp is int && (int)tmp == 1);

                tmp = prefsKey.GetValue("checkMatchCase");
                checkMatchCase.Checked = (tmp is int && (int)tmp == 1);
            }
        }

        private void SavePrefsToRegistry()
        {
            using (RegistryKey prefsKey = OpenPrefsRegistryKey())
            {
                prefsKey.SetValue("textSearchPath",textSearchPath.Text);
                prefsKey.SetValue("textSearchPattern", textSearchPattern.Text);
                prefsKey.SetValue("textSearchExtensions", textSearchExtensions.Text);
                prefsKey.SetValue("textDirectoryExcludes", textDirectoryExcludes.Text);

                prefsKey.SetValue("checkUseRegex", checkUseRegex.Checked ? 1 : 0);
                prefsKey.SetValue("checkMatchCase", checkMatchCase.Checked ? 1 : 0);
            }
        }

        private void OnThis_Load(object sender, EventArgs e)
        {
            LoadPrefsFromRegistry();
        }

        private void OnThis_Closing(object sender, FormClosingEventArgs e)
        {
            SavePrefsToRegistry();
        }
    }
}