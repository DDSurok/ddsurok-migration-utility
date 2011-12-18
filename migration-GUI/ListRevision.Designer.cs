namespace migration.GUI
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
            this.btnMigrate = new System.Windows.Forms.Button();
            this.btnBaseline = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnForgot = new System.Windows.Forms.Button();
            this.CurrentRevision = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Revisions
            // 
            this.Revisions.FormattingEnabled = true;
            this.Revisions.Location = new System.Drawing.Point(12, 64);
            this.Revisions.Name = "Revisions";
            this.Revisions.Size = new System.Drawing.Size(759, 329);
            this.Revisions.TabIndex = 1;
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(12, 399);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(113, 23);
            this.btnInit.TabIndex = 2;
            this.btnInit.Text = "Инициализировать";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnFix
            // 
            this.btnFix.Location = new System.Drawing.Point(131, 399);
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(98, 23);
            this.btnFix.TabIndex = 3;
            this.btnFix.Text = "Зафиксировать";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // btnMigrate
            // 
            this.btnMigrate.Location = new System.Drawing.Point(235, 399);
            this.btnMigrate.Name = "btnMigrate";
            this.btnMigrate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnMigrate.Size = new System.Drawing.Size(112, 23);
            this.btnMigrate.TabIndex = 4;
            this.btnMigrate.Text = "Мигрировать до ...";
            this.btnMigrate.UseVisualStyleBackColor = true;
            this.btnMigrate.Click += new System.EventHandler(this.btnMigrate_Click);
            // 
            // btnBaseline
            // 
            this.btnBaseline.Location = new System.Drawing.Point(353, 399);
            this.btnBaseline.Name = "btnBaseline";
            this.btnBaseline.Size = new System.Drawing.Size(276, 23);
            this.btnBaseline.TabIndex = 5;
            this.btnBaseline.Text = "Сгенерировать скрипт первоначального создания";
            this.btnBaseline.UseVisualStyleBackColor = true;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "Baseline.sql";
            // 
            // btnForgot
            // 
            this.btnForgot.Location = new System.Drawing.Point(635, 399);
            this.btnForgot.Name = "btnForgot";
            this.btnForgot.Size = new System.Drawing.Size(136, 23);
            this.btnForgot.TabIndex = 6;
            this.btnForgot.Text = "Очистить базу данных";
            this.btnForgot.UseVisualStyleBackColor = true;
            // 
            // CurrentRevision
            // 
            this.CurrentRevision.Enabled = false;
            this.CurrentRevision.Location = new System.Drawing.Point(12, 25);
            this.CurrentRevision.Name = "CurrentRevision";
            this.CurrentRevision.Size = new System.Drawing.Size(759, 20);
            this.CurrentRevision.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Все ревизии";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Текущая ревизия";
            // 
            // ListRevision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 434);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CurrentRevision);
            this.Controls.Add(this.btnForgot);
            this.Controls.Add(this.btnBaseline);
            this.Controls.Add(this.btnMigrate);
            this.Controls.Add(this.btnFix);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.Revisions);
            this.Name = "ListRevision";
            this.Text = "Список ревизий базы данных";
            this.Shown += new System.EventHandler(this.ListRevision_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox Revisions;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Button btnFix;
        private System.Windows.Forms.Button btnMigrate;
        private System.Windows.Forms.Button btnBaseline;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnForgot;
        private System.Windows.Forms.TextBox CurrentRevision;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}