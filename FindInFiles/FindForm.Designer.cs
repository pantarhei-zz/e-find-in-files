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
            this.buttonFind = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textSearchPattern = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textSearchPath = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textDirectoryExcludes = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textSearchExtensions = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkMatchCase = new System.Windows.Forms.CheckBox();
            this.checkUseRegex = new System.Windows.Forms.CheckBox();
            this.textProgress = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonFind
            // 
            this.buttonFind.Location = new System.Drawing.Point(257, 260);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(154, 23);
            this.buttonFind.TabIndex = 0;
            this.buttonFind.Text = "Find All";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.OnButtonFind_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Fi&nd what:";
            // 
            // textSearchPattern
            // 
            this.textSearchPattern.Location = new System.Drawing.Point(15, 25);
            this.textSearchPattern.Name = "textSearchPattern";
            this.textSearchPattern.Size = new System.Drawing.Size(396, 20);
            this.textSearchPattern.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "&Look in:";
            // 
            // textSearchPath
            // 
            this.textSearchPath.Location = new System.Drawing.Point(15, 72);
            this.textSearchPath.Name = "textSearchPath";
            this.textSearchPath.Size = new System.Drawing.Size(396, 20);
            this.textSearchPath.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textDirectoryExcludes);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textSearchExtensions);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.checkMatchCase);
            this.groupBox1.Controls.Add(this.checkUseRegex);
            this.groupBox1.Location = new System.Drawing.Point(15, 99);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(396, 155);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Find Options";
            // 
            // textDirectoryExcludes
            // 
            this.textDirectoryExcludes.Location = new System.Drawing.Point(9, 126);
            this.textDirectoryExcludes.Name = "textDirectoryExcludes";
            this.textDirectoryExcludes.Size = new System.Drawing.Size(381, 20);
            this.textDirectoryExcludes.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "E&xclude these directories:";
            // 
            // textSearchExtensions
            // 
            this.textSearchExtensions.Location = new System.Drawing.Point(9, 81);
            this.textSearchExtensions.Name = "textSearchExtensions";
            this.textSearchExtensions.Size = new System.Drawing.Size(381, 20);
            this.textSearchExtensions.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Look at these file &types:";
            // 
            // checkMatchCase
            // 
            this.checkMatchCase.AutoSize = true;
            this.checkMatchCase.Location = new System.Drawing.Point(9, 19);
            this.checkMatchCase.Name = "checkMatchCase";
            this.checkMatchCase.Size = new System.Drawing.Size(82, 17);
            this.checkMatchCase.TabIndex = 1;
            this.checkMatchCase.Text = "Match &case";
            this.checkMatchCase.UseVisualStyleBackColor = true;
            // 
            // checkUseRegex
            // 
            this.checkUseRegex.AutoSize = true;
            this.checkUseRegex.Location = new System.Drawing.Point(9, 41);
            this.checkUseRegex.Name = "checkUseRegex";
            this.checkUseRegex.Size = new System.Drawing.Size(144, 17);
            this.checkUseRegex.TabIndex = 0;
            this.checkUseRegex.Text = "Use Regular &Expressions";
            this.checkUseRegex.UseVisualStyleBackColor = true;
            // 
            // textProgress
            // 
            this.textProgress.AutoEllipsis = true;
            this.textProgress.Location = new System.Drawing.Point(15, 260);
            this.textProgress.Name = "textProgress";
            this.textProgress.Size = new System.Drawing.Size(229, 23);
            this.textProgress.TabIndex = 6;
            // 
            // FindForm
            // 
            this.AcceptButton = this.buttonFind;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 292);
            this.Controls.Add(this.textProgress);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textSearchPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textSearchPattern);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find in Files...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnThis_Closing);
            this.Load += new System.EventHandler(this.OnThis_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textSearchPattern;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textSearchPath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkUseRegex;
        private System.Windows.Forms.CheckBox checkMatchCase;
        private System.Windows.Forms.TextBox textSearchExtensions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textDirectoryExcludes;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label textProgress;
    }
}

