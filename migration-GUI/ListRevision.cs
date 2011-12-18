using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace migration.GUI
{
    public partial class ListRevision : Form, IActiveComment
    {
        public string ActiveComment { get; set; }
        List<Library.RevisionInfo> _revisionList;
        List<Library.RevisionInfo> revisionList
        {
            get
            {
                return this._revisionList;
            }
            // при заполнении списка ревизий, он автоматически отобразится в окне
            set
            {
                this._revisionList = value;
                this.Revisions.BeginUpdate();
                this.Revisions.Items.Clear();
                foreach (Library.RevisionInfo info in this._revisionList)
                {
                    this.Revisions.Items.Add(info.ToString());
                }
                this.Revisions.EndUpdate();
            }
        }
        int _currentRevision;
        int currentRevision
        {
            get
            {
                return _currentRevision;
            }
            // при заполнении текущей ревизии, информация автоматически отобразится в окне
            set
            {
                this._currentRevision = value;
                if (this._currentRevision == -1)
                {
                    this.CurrentRevision.Text = @"База данных не инициализирована или ее версия отсутствует в хранилище";
                    this.btnFix.Enabled = false;
                    this.btnForgot.Enabled = false;
                    this.btnBaseline.Enabled = false;
                    this.btnMigrate.Enabled = false;
                }
                else
                {
                    this.CurrentRevision.Text = this.revisionList[this.revisionList.Count - 1 - this._currentRevision].ToString();
                    this.btnFix.Enabled = true;
                    this.btnForgot.Enabled = true;
                    this.btnBaseline.Enabled = true;
                    this.btnMigrate.Enabled = true;
                }
            }
        }
        /// <summary>
        /// Перезагрузить список ревизий и информацию о текущей версии базы данных
        /// </summary>
        private void ReloadListOfRevisions()
        {
            this.revisionList = Migration.GetReverseRevisionList();
            this.currentRevision = Migration.GetCurrentRevision();
        }
        
        public ListRevision()
        {
            InitializeComponent();
        }

        private void ListRevision_Shown(object sender, EventArgs e)
        {
            this.ReloadListOfRevisions();
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
                Migration.Init(this.ActiveComment);
                this.ReloadListOfRevisions();
            }
        }
        
        private void btnFix_Click(object sender, EventArgs e)
        {
            using (ShowDiff form = new ShowDiff())
            {
                form.ShowDialog();
                this.ReloadListOfRevisions();
            }
        }

        private void btnMigrate_Click(object sender, EventArgs e)
        {
            if (this.Revisions.SelectedIndex != -1)
            {
                Migration.Migrate(this.revisionList[this.Revisions.SelectedIndex].Id);
                this.ReloadListOfRevisions();
            }
        }
    }
}
