namespace FindInFiles
{
    partial class FindForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolTip toolTip;
			this.buttonUseCurrentWord = new System.Windows.Forms.Button();
			this.buttonUseCurrentDirectory = new System.Windows.Forms.Button();
			this.buttonUseProjectDir = new System.Windows.Forms.Button();
			this.buttonBrowse = new System.Windows.Forms.Button();
			this.buttonReplaceUseCurrentWord = new System.Windows.Forms.Button();
			this.buttonReplaceUseCurrentDirectory = new System.Windows.Forms.Button();
			this.buttonReplaceUseProjectDir = new System.Windows.Forms.Button();
			this.buttonReplaceBrowse = new System.Windows.Forms.Button();
			this.comboReplaceWith = new System.Windows.Forms.ComboBox();
			this.comboSearchPattern = new FindInFiles.Controls.LinkedComboBox();
			this.comboSearchPath = new FindInFiles.Controls.LinkedComboBox();
			this.comboExcludeDirectories = new FindInFiles.Controls.LinkedComboBox();
			this.comboSearchExtensions = new FindInFiles.Controls.LinkedComboBox();
			this.checkMatchCase = new FindInFiles.Controls.LinkedCheckBox();
			this.checkUseRegex = new FindInFiles.Controls.LinkedCheckBox();
			this.textReplaceSearchPattern = new FindInFiles.Controls.LinkedComboBox();
			this.textReplaceSearchPath = new FindInFiles.Controls.LinkedComboBox();
			this.textReplaceDirectoryExcludes = new FindInFiles.Controls.LinkedComboBox();
			this.textReplaceSearchExtensions = new FindInFiles.Controls.LinkedComboBox();
			this.checkReplaceMatchCase = new FindInFiles.Controls.LinkedCheckBox();
			this.checkReplaceUseRegex = new FindInFiles.Controls.LinkedCheckBox();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.findTab = new System.Windows.Forms.TabPage();
			this.textProgress = new System.Windows.Forms.Label();
			this.groupFindOptions = new System.Windows.Forms.GroupBox();
			this.labelExcludeDirectories = new System.Windows.Forms.Label();
			this.labelFileTypes = new System.Windows.Forms.Label();
			this.labelLookIn = new System.Windows.Forms.Label();
			this.labelFindWhat = new System.Windows.Forms.Label();
			this.buttonFind = new System.Windows.Forms.Button();
			this.replaceTab = new System.Windows.Forms.TabPage();
			this.labelReplaceWith = new System.Windows.Forms.Label();
			this.textReplaceProgress = new System.Windows.Forms.Label();
			this.groupReplaceOptions = new System.Windows.Forms.GroupBox();
			this.labelReplaceExcludeDirectories = new System.Windows.Forms.Label();
			this.labelReplaceFileTypes = new System.Windows.Forms.Label();
			this.labelReplaceLookIn = new System.Windows.Forms.Label();
			this.labelReplaceFindWhat = new System.Windows.Forms.Label();
			this.buttonReplace = new System.Windows.Forms.Button();
			toolTip = new System.Windows.Forms.ToolTip( this.components );
			this.tabControl.SuspendLayout();
			this.findTab.SuspendLayout();
			this.groupFindOptions.SuspendLayout();
			this.replaceTab.SuspendLayout();
			this.groupReplaceOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonUseCurrentWord
			// 
			this.buttonUseCurrentWord.Location = new System.Drawing.Point( 414, 22 );
			this.buttonUseCurrentWord.Name = "buttonUseCurrentWord";
			this.buttonUseCurrentWord.Size = new System.Drawing.Size( 25, 21 );
			this.buttonUseCurrentWord.TabIndex = 23;
			this.buttonUseCurrentWord.TabStop = false;
			this.buttonUseCurrentWord.Text = "&W";
			toolTip.SetToolTip( this.buttonUseCurrentWord, "Use the current word under the cursor in E" );
			this.buttonUseCurrentWord.UseVisualStyleBackColor = true;
			this.buttonUseCurrentWord.Click += new System.EventHandler( this.UseCurrentWord_Click );
			// 
			// buttonUseCurrentDirectory
			// 
			this.buttonUseCurrentDirectory.Location = new System.Drawing.Point( 366, 68 );
			this.buttonUseCurrentDirectory.Name = "buttonUseCurrentDirectory";
			this.buttonUseCurrentDirectory.Size = new System.Drawing.Size( 21, 21 );
			this.buttonUseCurrentDirectory.TabIndex = 20;
			this.buttonUseCurrentDirectory.TabStop = false;
			this.buttonUseCurrentDirectory.Text = "&D";
			toolTip.SetToolTip( this.buttonUseCurrentDirectory, "Use the current file\'s directory" );
			this.buttonUseCurrentDirectory.UseVisualStyleBackColor = true;
			this.buttonUseCurrentDirectory.Click += new System.EventHandler( this.UseCurrentDirectory_Click );
			// 
			// buttonUseProjectDir
			// 
			this.buttonUseProjectDir.Location = new System.Drawing.Point( 388, 68 );
			this.buttonUseProjectDir.Name = "buttonUseProjectDir";
			this.buttonUseProjectDir.Size = new System.Drawing.Size( 21, 21 );
			this.buttonUseProjectDir.TabIndex = 22;
			this.buttonUseProjectDir.TabStop = false;
			this.buttonUseProjectDir.Text = "&P";
			toolTip.SetToolTip( this.buttonUseProjectDir, "Use the current project directory" );
			this.buttonUseProjectDir.UseVisualStyleBackColor = true;
			this.buttonUseProjectDir.Click += new System.EventHandler( this.UseProjectDirectory_Click );
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Location = new System.Drawing.Point( 409, 68 );
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size( 30, 21 );
			this.buttonBrowse.TabIndex = 18;
			this.buttonBrowse.Text = "...";
			toolTip.SetToolTip( this.buttonBrowse, "Browse for a directory" );
			this.buttonBrowse.UseVisualStyleBackColor = true;
			this.buttonBrowse.Click += new System.EventHandler( this.OnButtonBrowse_Click );
			// 
			// buttonReplaceUseCurrentWord
			// 
			this.buttonReplaceUseCurrentWord.Location = new System.Drawing.Point( 414, 22 );
			this.buttonReplaceUseCurrentWord.Name = "buttonReplaceUseCurrentWord";
			this.buttonReplaceUseCurrentWord.Size = new System.Drawing.Size( 25, 21 );
			this.buttonReplaceUseCurrentWord.TabIndex = 6;
			this.buttonReplaceUseCurrentWord.TabStop = false;
			this.buttonReplaceUseCurrentWord.Text = "&W";
			toolTip.SetToolTip( this.buttonReplaceUseCurrentWord, "Use the current word under the cursor in E" );
			this.buttonReplaceUseCurrentWord.UseVisualStyleBackColor = true;
			this.buttonReplaceUseCurrentWord.Click += new System.EventHandler( this.UseCurrentWord_Click );
			// 
			// buttonReplaceUseCurrentDirectory
			// 
			this.buttonReplaceUseCurrentDirectory.Location = new System.Drawing.Point( 366, 107 );
			this.buttonReplaceUseCurrentDirectory.Name = "buttonReplaceUseCurrentDirectory";
			this.buttonReplaceUseCurrentDirectory.Size = new System.Drawing.Size( 21, 21 );
			this.buttonReplaceUseCurrentDirectory.TabIndex = 7;
			this.buttonReplaceUseCurrentDirectory.TabStop = false;
			this.buttonReplaceUseCurrentDirectory.Text = "&D";
			toolTip.SetToolTip( this.buttonReplaceUseCurrentDirectory, "Use the current file\'s directory" );
			this.buttonReplaceUseCurrentDirectory.UseVisualStyleBackColor = true;
			this.buttonReplaceUseCurrentDirectory.Click += new System.EventHandler( this.UseCurrentDirectory_Click );
			// 
			// buttonReplaceUseProjectDir
			// 
			this.buttonReplaceUseProjectDir.Location = new System.Drawing.Point( 388, 107 );
			this.buttonReplaceUseProjectDir.Name = "buttonReplaceUseProjectDir";
			this.buttonReplaceUseProjectDir.Size = new System.Drawing.Size( 21, 21 );
			this.buttonReplaceUseProjectDir.TabIndex = 8;
			this.buttonReplaceUseProjectDir.TabStop = false;
			this.buttonReplaceUseProjectDir.Text = "&P";
			toolTip.SetToolTip( this.buttonReplaceUseProjectDir, "Use the current project directory" );
			this.buttonReplaceUseProjectDir.UseVisualStyleBackColor = true;
			this.buttonReplaceUseProjectDir.Click += new System.EventHandler( this.UseProjectDirectory_Click );
			// 
			// buttonReplaceBrowse
			// 
			this.buttonReplaceBrowse.Location = new System.Drawing.Point( 409, 107 );
			this.buttonReplaceBrowse.Name = "buttonReplaceBrowse";
			this.buttonReplaceBrowse.Size = new System.Drawing.Size( 30, 21 );
			this.buttonReplaceBrowse.TabIndex = 9;
			this.buttonReplaceBrowse.Text = "...";
			toolTip.SetToolTip( this.buttonReplaceBrowse, "Browse for a directory" );
			this.buttonReplaceBrowse.UseVisualStyleBackColor = true;
			this.buttonReplaceBrowse.Click += new System.EventHandler( this.OnButtonBrowse_Click );
			// 
			// textReplaceWith
			// 
			this.comboReplaceWith.FormattingEnabled = true;
			this.comboReplaceWith.Location = new System.Drawing.Point( 7, 65 );
			this.comboReplaceWith.Name = "textReplaceWith";
			this.comboReplaceWith.Size = new System.Drawing.Size( 404, 21 );
			this.comboReplaceWith.TabIndex = 3;
			toolTip.SetToolTip( this.comboReplaceWith, "The string or regular expression to search for" );
			// 
			// textSearchPattern
			// 
			this.comboSearchPattern.FormattingEnabled = true;
			this.comboSearchPattern.Location = new System.Drawing.Point( 7, 22 );
			this.comboSearchPattern.Name = "textSearchPattern";
			this.comboSearchPattern.Size = new System.Drawing.Size( 404, 21 );
			this.comboSearchPattern.TabIndex = 14;
			toolTip.SetToolTip( this.comboSearchPattern, "The string or regular expression to search for" );
			// 
			// textSearchPath
			// 
			this.comboSearchPath.FormattingEnabled = true;
			this.comboSearchPath.Location = new System.Drawing.Point( 7, 68 );
			this.comboSearchPath.Name = "textSearchPath";
			this.comboSearchPath.Size = new System.Drawing.Size( 356, 21 );
			this.comboSearchPath.TabIndex = 16;
			toolTip.SetToolTip( this.comboSearchPath, "The directory to search in" );
			// 
			// textDirectoryExcludes
			// 
			this.comboExcludeDirectories.FormattingEnabled = true;
			this.comboExcludeDirectories.Location = new System.Drawing.Point( 9, 127 );
			this.comboExcludeDirectories.Name = "textDirectoryExcludes";
			this.comboExcludeDirectories.Size = new System.Drawing.Size( 417, 21 );
			this.comboExcludeDirectories.TabIndex = 5;
			toolTip.SetToolTip( this.comboExcludeDirectories, "Directories to exclude - eg: .svn,tmp" );
			// 
			// textSearchExtensions
			// 
			this.comboSearchExtensions.FormattingEnabled = true;
			this.comboSearchExtensions.Location = new System.Drawing.Point( 9, 82 );
			this.comboSearchExtensions.Name = "textSearchExtensions";
			this.comboSearchExtensions.Size = new System.Drawing.Size( 417, 21 );
			this.comboSearchExtensions.TabIndex = 3;
			toolTip.SetToolTip( this.comboSearchExtensions, "File types to search for - eg: *.rb, *.rhtml, *.js" );
			// 
			// checkMatchCase
			// 
			this.checkMatchCase.AutoSize = true;
			this.checkMatchCase.Location = new System.Drawing.Point( 9, 19 );
			this.checkMatchCase.Name = "checkMatchCase";
			this.checkMatchCase.Size = new System.Drawing.Size( 82, 17 );
			this.checkMatchCase.TabIndex = 0;
			this.checkMatchCase.Text = "Match &case";
			toolTip.SetToolTip( this.checkMatchCase, "If ticked, the search will be case-sensitive" );
			this.checkMatchCase.UseVisualStyleBackColor = true;
			// 
			// checkUseRegex
			// 
			this.checkUseRegex.AutoSize = true;
			this.checkUseRegex.Location = new System.Drawing.Point( 9, 41 );
			this.checkUseRegex.Name = "checkUseRegex";
			this.checkUseRegex.Size = new System.Drawing.Size( 144, 17 );
			this.checkUseRegex.TabIndex = 1;
			this.checkUseRegex.Text = "Use Regular &Expressions";
			toolTip.SetToolTip( this.checkUseRegex, "If ticked, .NET regular expressions will be used to match the search pattern" );
			this.checkUseRegex.UseVisualStyleBackColor = true;
			// 
			// textReplaceSearchPattern
			// 
			this.textReplaceSearchPattern.FormattingEnabled = true;
			this.textReplaceSearchPattern.Location = new System.Drawing.Point( 7, 22 );
			this.textReplaceSearchPattern.Name = "textReplaceSearchPattern";
			this.textReplaceSearchPattern.Size = new System.Drawing.Size( 404, 21 );
			this.textReplaceSearchPattern.TabIndex = 1;
			toolTip.SetToolTip( this.textReplaceSearchPattern, "The string or regular expression to search for" );
			// 
			// textReplaceSearchPath
			// 
			this.textReplaceSearchPath.FormattingEnabled = true;
			this.textReplaceSearchPath.Location = new System.Drawing.Point( 7, 107 );
			this.textReplaceSearchPath.Name = "textReplaceSearchPath";
			this.textReplaceSearchPath.Size = new System.Drawing.Size( 356, 21 );
			this.textReplaceSearchPath.TabIndex = 5;
			toolTip.SetToolTip( this.textReplaceSearchPath, "The directory to search in" );
			// 
			// textReplaceDirectoryExcludes
			// 
			this.textReplaceDirectoryExcludes.FormattingEnabled = true;
			this.textReplaceDirectoryExcludes.Location = new System.Drawing.Point( 9, 107 );
			this.textReplaceDirectoryExcludes.Name = "textReplaceDirectoryExcludes";
			this.textReplaceDirectoryExcludes.Size = new System.Drawing.Size( 417, 21 );
			this.textReplaceDirectoryExcludes.TabIndex = 5;
			toolTip.SetToolTip( this.textReplaceDirectoryExcludes, "Directories to exclude - eg: .svn,tmp" );
			// 
			// textReplaceSearchExtensions
			// 
			this.textReplaceSearchExtensions.FormattingEnabled = true;
			this.textReplaceSearchExtensions.Location = new System.Drawing.Point( 9, 62 );
			this.textReplaceSearchExtensions.Name = "textReplaceSearchExtensions";
			this.textReplaceSearchExtensions.Size = new System.Drawing.Size( 417, 21 );
			this.textReplaceSearchExtensions.TabIndex = 3;
			toolTip.SetToolTip( this.textReplaceSearchExtensions, "File types to search for - eg: *.rb, *.rhtml, *.js" );
			// 
			// checkReplaceMatchCase
			// 
			this.checkReplaceMatchCase.AutoSize = true;
			this.checkReplaceMatchCase.Location = new System.Drawing.Point( 6, 20 );
			this.checkReplaceMatchCase.Name = "checkReplaceMatchCase";
			this.checkReplaceMatchCase.Size = new System.Drawing.Size( 82, 17 );
			this.checkReplaceMatchCase.TabIndex = 0;
			this.checkReplaceMatchCase.Text = "Match &case";
			toolTip.SetToolTip( this.checkReplaceMatchCase, "If ticked, the search will be case-sensitive" );
			this.checkReplaceMatchCase.UseVisualStyleBackColor = true;
			// 
			// checkReplaceUseRegex
			// 
			this.checkReplaceUseRegex.AutoSize = true;
			this.checkReplaceUseRegex.Location = new System.Drawing.Point( 94, 20 );
			this.checkReplaceUseRegex.Name = "checkReplaceUseRegex";
			this.checkReplaceUseRegex.Size = new System.Drawing.Size( 144, 17 );
			this.checkReplaceUseRegex.TabIndex = 1;
			this.checkReplaceUseRegex.Text = "Use Regular &Expressions";
			toolTip.SetToolTip( this.checkReplaceUseRegex, "If ticked, .NET regular expressions will be used to match the search pattern" );
			this.checkReplaceUseRegex.UseVisualStyleBackColor = true;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add( this.findTab );
			this.tabControl.Controls.Add( this.replaceTab );
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point( 0, 0 );
			this.tabControl.Margin = new System.Windows.Forms.Padding( 0 );
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size( 450, 331 );
			this.tabControl.TabIndex = 0;
			this.tabControl.SelectedIndexChanged += new System.EventHandler( this.OnTabChanged );
			// 
			// findTab
			// 
			this.findTab.BackColor = System.Drawing.SystemColors.Control;
			this.findTab.Controls.Add( this.buttonUseCurrentWord );
			this.findTab.Controls.Add( this.buttonUseCurrentDirectory );
			this.findTab.Controls.Add( this.buttonUseProjectDir );
			this.findTab.Controls.Add( this.comboSearchPattern );
			this.findTab.Controls.Add( this.comboSearchPath );
			this.findTab.Controls.Add( this.buttonBrowse );
			this.findTab.Controls.Add( this.textProgress );
			this.findTab.Controls.Add( this.groupFindOptions );
			this.findTab.Controls.Add( this.labelLookIn );
			this.findTab.Controls.Add( this.labelFindWhat );
			this.findTab.Controls.Add( this.buttonFind );
			this.findTab.Location = new System.Drawing.Point( 4, 22 );
			this.findTab.Name = "findTab";
			this.findTab.Padding = new System.Windows.Forms.Padding( 3 );
			this.findTab.Size = new System.Drawing.Size( 442, 305 );
			this.findTab.TabIndex = 0;
			this.findTab.Text = "Find";
			// 
			// textProgress
			// 
			this.textProgress.AutoEllipsis = true;
			this.textProgress.Location = new System.Drawing.Point( 7, 256 );
			this.textProgress.Name = "textProgress";
			this.textProgress.Size = new System.Drawing.Size( 229, 23 );
			this.textProgress.TabIndex = 19;
			// 
			// groupFindOptions
			// 
			this.groupFindOptions.Controls.Add( this.comboExcludeDirectories );
			this.groupFindOptions.Controls.Add( this.comboSearchExtensions );
			this.groupFindOptions.Controls.Add( this.labelExcludeDirectories );
			this.groupFindOptions.Controls.Add( this.labelFileTypes );
			this.groupFindOptions.Controls.Add( this.checkMatchCase );
			this.groupFindOptions.Controls.Add( this.checkUseRegex );
			this.groupFindOptions.Location = new System.Drawing.Point( 7, 95 );
			this.groupFindOptions.Name = "groupFindOptions";
			this.groupFindOptions.Size = new System.Drawing.Size( 432, 155 );
			this.groupFindOptions.TabIndex = 17;
			this.groupFindOptions.TabStop = false;
			this.groupFindOptions.Text = "Options";
			// 
			// labelExcludeDirectories
			// 
			this.labelExcludeDirectories.AutoSize = true;
			this.labelExcludeDirectories.Location = new System.Drawing.Point( 6, 110 );
			this.labelExcludeDirectories.Name = "labelExcludeDirectories";
			this.labelExcludeDirectories.Size = new System.Drawing.Size( 128, 13 );
			this.labelExcludeDirectories.TabIndex = 4;
			this.labelExcludeDirectories.Text = "E&xclude these directories:";
			// 
			// labelFileTypes
			// 
			this.labelFileTypes.AutoSize = true;
			this.labelFileTypes.Location = new System.Drawing.Point( 6, 65 );
			this.labelFileTypes.Name = "labelFileTypes";
			this.labelFileTypes.Size = new System.Drawing.Size( 119, 13 );
			this.labelFileTypes.TabIndex = 2;
			this.labelFileTypes.Text = "Look at these file &types:";
			// 
			// labelLookIn
			// 
			this.labelLookIn.AutoSize = true;
			this.labelLookIn.Location = new System.Drawing.Point( 4, 52 );
			this.labelLookIn.Name = "labelLookIn";
			this.labelLookIn.Size = new System.Drawing.Size( 45, 13 );
			this.labelLookIn.TabIndex = 15;
			this.labelLookIn.Text = "&Look in:";
			// 
			// labelFindWhat
			// 
			this.labelFindWhat.AutoSize = true;
			this.labelFindWhat.Location = new System.Drawing.Point( 4, 5 );
			this.labelFindWhat.Name = "labelFindWhat";
			this.labelFindWhat.Size = new System.Drawing.Size( 56, 13 );
			this.labelFindWhat.TabIndex = 13;
			this.labelFindWhat.Text = "Fi&nd what:";
			// 
			// buttonFind
			// 
			this.buttonFind.Location = new System.Drawing.Point( 249, 256 );
			this.buttonFind.Name = "buttonFind";
			this.buttonFind.Size = new System.Drawing.Size( 190, 23 );
			this.buttonFind.TabIndex = 21;
			this.buttonFind.Text = "Find All";
			this.buttonFind.UseVisualStyleBackColor = true;
			this.buttonFind.Click += new System.EventHandler( this.OnButtonFind_Click );
			// 
			// replaceTab
			// 
			this.replaceTab.BackColor = System.Drawing.SystemColors.Control;
			this.replaceTab.Controls.Add( this.labelReplaceWith );
			this.replaceTab.Controls.Add( this.comboReplaceWith );
			this.replaceTab.Controls.Add( this.buttonReplaceUseCurrentWord );
			this.replaceTab.Controls.Add( this.buttonReplaceUseCurrentDirectory );
			this.replaceTab.Controls.Add( this.buttonReplaceUseProjectDir );
			this.replaceTab.Controls.Add( this.buttonReplaceBrowse );
			this.replaceTab.Controls.Add( this.textReplaceProgress );
			this.replaceTab.Controls.Add( this.groupReplaceOptions );
			this.replaceTab.Controls.Add( this.labelReplaceLookIn );
			this.replaceTab.Controls.Add( this.labelReplaceFindWhat );
			this.replaceTab.Controls.Add( this.buttonReplace );
			this.replaceTab.Controls.Add( this.textReplaceSearchPattern );
			this.replaceTab.Controls.Add( this.textReplaceSearchPath );
			this.replaceTab.Location = new System.Drawing.Point( 4, 22 );
			this.replaceTab.Name = "replaceTab";
			this.replaceTab.Padding = new System.Windows.Forms.Padding( 3 );
			this.replaceTab.Size = new System.Drawing.Size( 442, 305 );
			this.replaceTab.TabIndex = 1;
			this.replaceTab.Text = "Replace";
			// 
			// labelReplaceWith
			// 
			this.labelReplaceWith.AutoSize = true;
			this.labelReplaceWith.Location = new System.Drawing.Point( 4, 46 );
			this.labelReplaceWith.Name = "labelReplaceWith";
			this.labelReplaceWith.Size = new System.Drawing.Size( 72, 13 );
			this.labelReplaceWith.TabIndex = 2;
			this.labelReplaceWith.Text = "&Replace with:";
			// 
			// textReplaceProgress
			// 
			this.textReplaceProgress.AutoEllipsis = true;
			this.textReplaceProgress.Location = new System.Drawing.Point( 6, 276 );
			this.textReplaceProgress.Name = "textReplaceProgress";
			this.textReplaceProgress.Size = new System.Drawing.Size( 229, 23 );
			this.textReplaceProgress.TabIndex = 30;
			// 
			// groupReplaceOptions
			// 
			this.groupReplaceOptions.Controls.Add( this.textReplaceDirectoryExcludes );
			this.groupReplaceOptions.Controls.Add( this.textReplaceSearchExtensions );
			this.groupReplaceOptions.Controls.Add( this.labelReplaceExcludeDirectories );
			this.groupReplaceOptions.Controls.Add( this.labelReplaceFileTypes );
			this.groupReplaceOptions.Controls.Add( this.checkReplaceMatchCase );
			this.groupReplaceOptions.Controls.Add( this.checkReplaceUseRegex );
			this.groupReplaceOptions.Location = new System.Drawing.Point( 7, 132 );
			this.groupReplaceOptions.Name = "groupReplaceOptions";
			this.groupReplaceOptions.Size = new System.Drawing.Size( 432, 141 );
			this.groupReplaceOptions.TabIndex = 28;
			this.groupReplaceOptions.TabStop = false;
			this.groupReplaceOptions.Text = "Options";
			// 
			// labelReplaceExcludeDirectories
			// 
			this.labelReplaceExcludeDirectories.AutoSize = true;
			this.labelReplaceExcludeDirectories.Location = new System.Drawing.Point( 6, 90 );
			this.labelReplaceExcludeDirectories.Name = "labelReplaceExcludeDirectories";
			this.labelReplaceExcludeDirectories.Size = new System.Drawing.Size( 128, 13 );
			this.labelReplaceExcludeDirectories.TabIndex = 4;
			this.labelReplaceExcludeDirectories.Text = "E&xclude these directories:";
			// 
			// labelReplaceFileTypes
			// 
			this.labelReplaceFileTypes.AutoSize = true;
			this.labelReplaceFileTypes.Location = new System.Drawing.Point( 6, 45 );
			this.labelReplaceFileTypes.Name = "labelReplaceFileTypes";
			this.labelReplaceFileTypes.Size = new System.Drawing.Size( 119, 13 );
			this.labelReplaceFileTypes.TabIndex = 2;
			this.labelReplaceFileTypes.Text = "Look at these file &types:";
			// 
			// labelReplaceLookIn
			// 
			this.labelReplaceLookIn.AutoSize = true;
			this.labelReplaceLookIn.Location = new System.Drawing.Point( 4, 91 );
			this.labelReplaceLookIn.Name = "labelReplaceLookIn";
			this.labelReplaceLookIn.Size = new System.Drawing.Size( 45, 13 );
			this.labelReplaceLookIn.TabIndex = 4;
			this.labelReplaceLookIn.Text = "&Look in:";
			// 
			// labelReplaceFindWhat
			// 
			this.labelReplaceFindWhat.AutoSize = true;
			this.labelReplaceFindWhat.Location = new System.Drawing.Point( 4, 5 );
			this.labelReplaceFindWhat.Name = "labelReplaceFindWhat";
			this.labelReplaceFindWhat.Size = new System.Drawing.Size( 56, 13 );
			this.labelReplaceFindWhat.TabIndex = 0;
			this.labelReplaceFindWhat.Text = "Fi&nd what:";
			// 
			// buttonReplace
			// 
			this.buttonReplace.Location = new System.Drawing.Point( 249, 276 );
			this.buttonReplace.Name = "buttonReplace";
			this.buttonReplace.Size = new System.Drawing.Size( 190, 23 );
			this.buttonReplace.TabIndex = 10;
			this.buttonReplace.Text = "Replace &All";
			this.buttonReplace.UseVisualStyleBackColor = true;
			this.buttonReplace.Click += new System.EventHandler( this.OnButtonReplace_Click );
			// 
			// FindForm
			// 
			this.AcceptButton = this.buttonFind;
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 450, 331 );
			this.Controls.Add( this.tabControl );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Find/Replace in Files...";
			this.Load += new System.EventHandler( this.OnThis_Load );
			this.Shown += new System.EventHandler( this.OnThis_Shown );
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.OnThis_Closing );
			this.tabControl.ResumeLayout( false );
			this.findTab.ResumeLayout( false );
			this.findTab.PerformLayout();
			this.groupFindOptions.ResumeLayout( false );
			this.groupFindOptions.PerformLayout();
			this.replaceTab.ResumeLayout( false );
			this.replaceTab.PerformLayout();
			this.groupReplaceOptions.ResumeLayout( false );
			this.groupReplaceOptions.PerformLayout();
			this.ResumeLayout( false );

        }

        #endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage findTab;
		private System.Windows.Forms.Button buttonUseCurrentWord;
		private System.Windows.Forms.Button buttonUseCurrentDirectory;
		private System.Windows.Forms.Button buttonUseProjectDir;
		private Controls.LinkedComboBox comboSearchPattern;
		private Controls.LinkedComboBox comboSearchPath;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.Label textProgress;
		private System.Windows.Forms.GroupBox groupFindOptions;
		private Controls.LinkedComboBox comboExcludeDirectories;
		private Controls.LinkedComboBox comboSearchExtensions;
		private System.Windows.Forms.Label labelExcludeDirectories;
		private System.Windows.Forms.Label labelFileTypes;
		private Controls.LinkedCheckBox checkMatchCase;
		private Controls.LinkedCheckBox checkUseRegex;
		private System.Windows.Forms.Label labelLookIn;
		private System.Windows.Forms.Label labelFindWhat;
		private System.Windows.Forms.Button buttonFind;
		private System.Windows.Forms.TabPage replaceTab;
		private System.Windows.Forms.Button buttonReplaceUseCurrentWord;
		private System.Windows.Forms.Button buttonReplaceUseCurrentDirectory;
		private System.Windows.Forms.Button buttonReplaceUseProjectDir;
		private Controls.LinkedComboBox textReplaceSearchPattern;
		private Controls.LinkedComboBox textReplaceSearchPath;
		private System.Windows.Forms.Button buttonReplaceBrowse;
		private System.Windows.Forms.Label textReplaceProgress;
		private System.Windows.Forms.GroupBox groupReplaceOptions;
		private Controls.LinkedComboBox textReplaceDirectoryExcludes;
		private Controls.LinkedComboBox textReplaceSearchExtensions;
		private System.Windows.Forms.Label labelReplaceExcludeDirectories;
		private System.Windows.Forms.Label labelReplaceFileTypes;
		private Controls.LinkedCheckBox checkReplaceMatchCase;
		private Controls.LinkedCheckBox checkReplaceUseRegex;
		private System.Windows.Forms.Label labelReplaceLookIn;
		private System.Windows.Forms.Label labelReplaceFindWhat;
		private System.Windows.Forms.Button buttonReplace;
		private System.Windows.Forms.ComboBox comboReplaceWith; // NOT LINKED
		private System.Windows.Forms.Label labelReplaceWith;

	}
}

