namespace migration
{
    partial class ListRevision
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
            this.Revisions = new System.Windows.Forms.ListBox();
            this.btnInit = new System.Windows.Forms.Button();
            this.btnFix = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Revisions
            // 
            this.Revisions.FormattingEnabled = true;
            this.Revisions.Location = new System.Drawing.Point(12, 12);
            this.Revisions.Name = "Revisions";
            this.Revisions.Size = new System.Drawing.Size(773, 381);
            this.Revisions.TabIndex = 0;
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(12, 399);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(130, 23);
            this.btnInit.TabIndex = 1;
            this.btnInit.Text = "Инициализировать";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnFix
            // 
            this.btnFix.Location = new System.Drawing.Point(148, 399);
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(113, 23);
            this.btnFix.TabIndex = 2;
            this.btnFix.Text = "Зафиксировать";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // ListRevision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 434);
            this.Controls.Add(this.btnFix);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.Revisions);
            this.Name = "ListRevision";
            this.Text = "Список ревизий базы данных";
            this.Shown += new System.EventHandler(this.ListRevision_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox Revisions;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Button btnFix;
    }
}