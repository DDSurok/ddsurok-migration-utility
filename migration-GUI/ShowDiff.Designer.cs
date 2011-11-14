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
            this.diffDown.Location = new System.Drawing.Point(448, 13);
            this.diffDown.Name = "diffDown";
            this.diffDown.Size = new System.Drawing.Size(444, 482);
            this.diffDown.TabIndex = 1;
            // 
            // ShowDiff
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 555);
            this.Controls.Add(this.diffDown);
            this.Controls.Add(this.diffUp);
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
    }
}