namespace migration
{
    partial class ShowDiff
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
            this.diffUp = new System.Windows.Forms.DataGridView();
            this.diffDown = new System.Windows.Forms.DataGridView();
            this.btnFix = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.diffUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.diffDown)).BeginInit();
            this.SuspendLayout();
            // 
            // diffUp
            // 
            this.diffUp.AllowUserToAddRows = false;
            this.diffUp.AllowUserToDeleteRows = false;
            this.diffUp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.diffUp.Location = new System.Drawing.Point(13, 13);
            this.diffUp.Name = "diffUp";
            this.diffUp.Size = new System.Drawing.Size(428, 482);
            this.diffUp.TabIndex = 0;
            // 
            // diffDown
            // 
            this.diffDown.AllowUserToAddRows = false;
            this.diffDown.AllowUserToDeleteRows = false;
            this.diffDown.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.diffDown.Location = new System.Drawing.Point(451, 13);
            this.diffDown.Name = "diffDown";
            this.diffDown.Size = new System.Drawing.Size(428, 482);
            this.diffDown.TabIndex = 1;
            // 
            // btnFix
            // 
            this.btnFix.Location = new System.Drawing.Point(713, 501);
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(97, 23);
            this.btnFix.TabIndex = 2;
            this.btnFix.Text = "Зафиксировать";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(816, 501);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(63, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ShowDiff
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 536);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFix);
            this.Controls.Add(this.diffDown);
            this.Controls.Add(this.diffUp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ShowDiff";
            this.Text = "Текущие изменения";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShowDiff_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.diffUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.diffDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView diffUp;
        private System.Windows.Forms.DataGridView diffDown;
        private System.Windows.Forms.Button btnFix;
        private System.Windows.Forms.Button btnCancel;
    }
}