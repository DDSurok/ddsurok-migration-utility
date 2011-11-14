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
        /// <summary>
        /// 
        /// </summary>
        public string ActiveComment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        List<RevisionInfo> revisionList;

        RevisionInfo _CurrentRevision;
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 
        /// </summary>
        private void ReloadListOfRevisions()
        {
            this.revisionList = RevisionList.GetRevisionList();
            this.Revisions.BeginUpdate();
            this.Revisions.Items.Clear();
            string Comment;
            foreach (RevisionInfo info in this.revisionList)
            {
                Comment = info.Comment.Replace("\n", " \t");
                this.Revisions.Items.Add(info.Id.ToString("0000  ") +
                    "Author: " + info.Author + "\n\t" +
                    info.GenerateDateTime.ToString("dd MMMM yyyy, hh:mm\n\t") +
                    "Comment: " + Comment);
            }
            this.Revisions.EndUpdate();
            this._CurrentRevision = this.revisionList[this.revisionList.Count-1-RevisionList.GetCurrentRevision()];
            Comment = this._CurrentRevision.Comment.Replace("\n", " \t");
            this.CurrentRevision.Text = this._CurrentRevision.Id.ToString("0000  ") +
                "Author: " + this._CurrentRevision.Author + "\n\t" +
                this._CurrentRevision.GenerateDateTime.ToString("dd MMMM yyyy, hh:mm\n\t") +
                "Comment: " + Comment;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListRevision_Shown(object sender, EventArgs e)
        {
            ReloadListOfRevisions();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
