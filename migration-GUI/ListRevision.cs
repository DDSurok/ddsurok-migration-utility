using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace migration
{
    public partial class ListRevision : Form, IActiveComment
    {
        public string ActiveComment { get; set; }

        List<RevisionInfo> revisionList;

        public ListRevision()
        {
            InitializeComponent();
            try
            {
                ConfigFile.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Продолжение повлечет за собой удаление всех\nревизий в указанном каталоге и создание новой ревизии.\nПродолжить?",
                "Инициализировать",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                EnterComment form = new EnterComment(this);
                form.ShowDialog();
                Init.Run(this.ActiveComment);
                this.ReloadListOfRevisions();
            }
        }

        private void ReloadListOfRevisions()
        {
            this.revisionList = RevisionList.GetRevisionList();
            this.Revisions.BeginUpdate();
            this.Revisions.Items.Clear();
            foreach (RevisionInfo info in this.revisionList)
            {
                this.Revisions.Items.Add(info.Id.ToString("0000  ") +
                    "Author: " + info.Author + "\n\t" +
                    info.GenerateDateTime.ToString("dd MMMM yyyy, hh:mm\n\t") +
                    "Comment: " + info.Comment);
            }
            this.Revisions.EndUpdate();
        }

        private void ListRevision_Shown(object sender, EventArgs e)
        {
            ReloadListOfRevisions();
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            using (ShowDiff form = new ShowDiff())
            {
                form.ShowDialog();
                ReloadListOfRevisions();
            }
        }
    }
}
