using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Smo;
using System.Xml;
using System;

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
        /// <summary>
        /// 
        /// </summary>
        private void ExportConfigure()
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            // включаем отступ для элементов XML документа
            // (позволяет наглядно изобразить иерархию XML документа)
            settings.Indent = true;
            settings.IndentChars = "  "; // задаем отступ, здесь у меня 2 пробела

            // задаем переход на новую строку
            settings.NewLineChars = "\n";

            // Нужно ли опустить строку декларации формата XML документа
            // речь идет о строке вида "<?xml version="1.0" encoding="utf-8"?>"
            settings.OmitXmlDeclaration = false;
            
            // FileName - имя файла, куда будет сохранен XML-документ
            // settings - настройки форматирования (и не только) вывода
            // (рассмотрен выше)
            using (XmlWriter output = XmlWriter.Create("migration.conf", settings))
            {
                // Создали открывающийся тег
                output.WriteStartElement("MigrationConfigure");
                
                // Создаем элемент connectionString
                output.WriteElementString("serverName", this.ServerComboBox.SelectedItem.ToString());

                // Создаем элемент databaseName
                output.WriteElementString("databaseName", this.DatabaseComboBox.SelectedItem.ToString());
 
                // Сбрасываем буфферизированные данные
                output.Flush();
 
                // Закрываем фаил, с которым связан output
                output.Close();
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
    }
}
