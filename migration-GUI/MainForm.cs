using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Smo;

namespace migration
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
            ConfigFile.Load();
            if (ConfigFile.isLoad)
            {
                // Запись информации о базе данных
                // Имя базы данных неизменно для проекта
                this.DatabaseComboBox.Text = ConfigFile.databaseName;
                this.DatabaseComboBox.Enabled = false;
                this.btnUpdateDatabaseList.Enabled = false;

                // Заполнение информации о имени сервера и репозитории
                this.ServerComboBox.SelectedIndex = this.ServerComboBox.FindString(ConfigFile.serverName);
                this.Directory.Text = ConfigFile.versionDirectory;
                this.NickName.Text = ConfigFile.nickName;
            }
            else
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
        /// <summary>
        /// 
        /// </summary>
        private void ExportConfigure()
        {
            if (!this.btnUpdateDatabaseList.Enabled)
            {
                // Файл конфигурации существовал
                ConfigFile.Write(this.ServerComboBox.SelectedItem.ToString(), this.Directory.Text, this.NickName.Text);
            }
            else
            {
                // Файл конфигурации не существовал
                ConfigFile.Rewrite(this.ServerComboBox.SelectedItem.ToString(), this.DatabaseComboBox.SelectedItem.ToString(), this.Directory.Text, this.NickName.Text);
            }
        }
        
        private void btnUpdateServerList_Click(object sender, System.EventArgs e)
        {
            this.ReloadServerList();
        }

        private void btnUpdateDatabaseList_Click(object sender, System.EventArgs e)
        {
            this.ReloadDatabaseList();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (this.btnUpdateDatabaseList.Enabled)
            {
                if ((this.ServerComboBox.SelectedIndex > -1) && (this.DatabaseComboBox.SelectedIndex > -1))
                {
                    try
                    {
                        this.ExportConfigure();
                        MessageBox.Show("Файл конфигурации успешно создан");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            else
            {
                if (this.ServerComboBox.SelectedIndex > -1)
                {
                    try
                    {
                        this.ExportConfigure();
                        MessageBox.Show("Файл конфигурации успешно создан");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.DatabaseComboBox.Text = "";
            this.DatabaseComboBox.Enabled = true;
            this.btnUpdateDatabaseList.Enabled = true;
            this.ReloadDatabaseList();
        }
    }
}
