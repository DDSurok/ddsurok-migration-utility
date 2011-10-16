using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Smo;

namespace ConfigureMigrationTool
{
    public partial class MainForm : Form
    {
        private List<string> _serversList = new List<string>();
        private List<string> _databaseList = new List<string>();
        private Server _selectedServer;
        public MainForm()
        {
            InitializeComponent();
            this.ReloadServerList();
            this.ReloadDatabaseList();
        }
        /// <summary>
        /// Обновление списка доступных серверов MS SQL Server, запущенных на локальном компьютере
        /// </summary>
        private void ReloadServerList()
        {
            this._serversList.Clear();
            foreach (DataRow row in SmoApplication.EnumAvailableSqlServers(false).Rows)
            {
                string sqlServerName = row["Server"].ToString();
                if (row["Instance"] != null && row["Instance"].ToString().Length > 0)
                    sqlServerName += @"\" + row["Instance"].ToString();
                this._serversList.Add(sqlServerName);
            }
            this.ServerComboBox.BeginUpdate();
            this.ServerComboBox.DataSource = this._serversList;
            this.ServerComboBox.EndUpdate();
        }
        /// <summary>
        /// Обновление списка баз данных выбранного сервера
        /// </summary>
        private void ReloadDatabaseList()
        {
            this._databaseList.Clear();
            if (this.ServerComboBox.SelectedIndex > -1)
            {
                this._selectedServer = new Server(this.ServerComboBox.SelectedItem.ToString());
                foreach (Database dbName in this._selectedServer.Databases)
                    this._databaseList.Add(dbName.Name);
            }
            this.DatabaseComboBox.BeginUpdate();
            this.DatabaseComboBox.DataSource = this._databaseList;
            this.DatabaseComboBox.EndUpdate();
        }
        
        private void btnUpdateServerList_Click(object sender, System.EventArgs e)
        {
            this.ReloadServerList();
        }

        private void btnUpdateDatabaseList_Click(object sender, System.EventArgs e)
        {
            this.ReloadDatabaseList();
        }
    }
}
