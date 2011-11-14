namespace migration
{
    partial class MainForm
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
            this.ServerComboBox = new System.Windows.Forms.ComboBox();
            this.btnUpdateServerList = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnUpdateDatabaseList = new System.Windows.Forms.Button();
            this.DatabaseComboBox = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.Directory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.NickName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnSelectDir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ServerComboBox
            // 
            this.ServerComboBox.AllowDrop = true;
            this.ServerComboBox.FormattingEnabled = true;
            this.ServerComboBox.Location = new System.Drawing.Point(108, 14);
            this.ServerComboBox.Name = "ServerComboBox";
            this.ServerComboBox.Size = new System.Drawing.Size(400, 21);
            this.ServerComboBox.TabIndex = 0;
            // 
            // btnUpdateServerList
            // 
            this.btnUpdateServerList.Location = new System.Drawing.Point(514, 12);
            this.btnUpdateServerList.Name = "btnUpdateServerList";
            this.btnUpdateServerList.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateServerList.TabIndex = 1;
            this.btnUpdateServerList.Text = "Update";
            this.btnUpdateServerList.UseVisualStyleBackColor = true;
            this.btnUpdateServerList.Click += new System.EventHandler(this.btnUpdateServerList_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "SQL Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Database";
            // 
            // btnUpdateDatabaseList
            // 
            this.btnUpdateDatabaseList.Location = new System.Drawing.Point(514, 41);
            this.btnUpdateDatabaseList.Name = "btnUpdateDatabaseList";
            this.btnUpdateDatabaseList.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateDatabaseList.TabIndex = 4;
            this.btnUpdateDatabaseList.Text = "Update";
            this.btnUpdateDatabaseList.UseVisualStyleBackColor = true;
            this.btnUpdateDatabaseList.Click += new System.EventHandler(this.btnUpdateDatabaseList_Click);
            // 
            // DatabaseComboBox
            // 
            this.DatabaseComboBox.AllowDrop = true;
            this.DatabaseComboBox.FormattingEnabled = true;
            this.DatabaseComboBox.Location = new System.Drawing.Point(108, 43);
            this.DatabaseComboBox.Name = "DatabaseComboBox";
            this.DatabaseComboBox.Size = new System.Drawing.Size(400, 21);
            this.DatabaseComboBox.TabIndex = 3;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(514, 96);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = @"Export & Exit";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // Directory
            // 
            this.Directory.Location = new System.Drawing.Point(108, 70);
            this.Directory.Name = "Directory";
            this.Directory.Size = new System.Drawing.Size(400, 20);
            this.Directory.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Revision directory";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(405, 96);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(103, 23);
            this.btnReset.TabIndex = 9;
            this.btnReset.Text = "Reset configure";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // NickName
            // 
            this.NickName.Location = new System.Drawing.Point(108, 99);
            this.NickName.Name = "NickName";
            this.NickName.Size = new System.Drawing.Size(291, 20);
            this.NickName.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "NickName";
            // 
            // btnSelectDir
            // 
            this.btnSelectDir.Location = new System.Drawing.Point(514, 68);
            this.btnSelectDir.Name = "btnSelectDir";
            this.btnSelectDir.Size = new System.Drawing.Size(75, 23);
            this.btnSelectDir.TabIndex = 12;
            this.btnSelectDir.Text = ". . .";
            this.btnSelectDir.UseVisualStyleBackColor = true;
            this.btnSelectDir.Click += new System.EventHandler(this.btnSelectDir_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 131);
            this.Controls.Add(this.btnSelectDir);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NickName);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Directory);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnUpdateDatabaseList);
            this.Controls.Add(this.DatabaseComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnUpdateServerList);
            this.Controls.Add(this.ServerComboBox);
            this.Name = "MainForm";
            this.Text = "Configure migration utility";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ServerComboBox;
        private System.Windows.Forms.Button btnUpdateServerList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnUpdateDatabaseList;
        private System.Windows.Forms.ComboBox DatabaseComboBox;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.TextBox Directory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox NickName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button btnSelectDir;
    }
}

