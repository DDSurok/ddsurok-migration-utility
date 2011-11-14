﻿using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace migration
{
    static public class Init
    {
        static private XmlWriter output;
        static private Database database;
        static private Server server;
        static private RevisionInfo currentRevision;
        /// <summary>
        /// Основной метод класса
        /// </summary>
        static public void Run(string Comment)
        {
            // FileName - имя файла, куда будет сохранен XML-документ
            // settings - настройки форматирования (и не только) вывода
            // (рассмотрен выше)
            Config.DeleteVersionDirectory();
            Init.currentRevision = RevisionInfo.GenerateRevisionInfo(Comment);
            using (Init.output = XmlWriter.Create(Config.GetFileName(Init.currentRevision), Config.XmlSettings()))
            {
                // Создание объектов для работы с БД
                Init.server = new Server(ConfigFile.serverName);
                Init.database = server.Databases[ConfigFile.databaseName];
                    
                // Пишем информацию в XML о типе записи
                Init.WriteXMLHeader();

                // Инициализируем базу данных на работу с нашей программой
                InitDatabase.Initial();
                InitDatabase.UpdateVersionDatabase(Init.currentRevision);

                // Сохранение информации о таблицах
                Init.GenerateTableScriptsWithDependence();
                //// Собственно схемы таблиц
                //Init.GenerateTableScripts();
                // Триггеры к таблицам
                Init.GenerateTriggerScripts();
                //// Индексы к таблицам
                //Init.GenerateIndexScripts();
                //// Проверки к таблицам
                //Init.GenerateCheckScripts();
                //// Внешние ключи
                //Init.GenerateForeignKeyScripts();
                // Сохранение информации о правилах
                Init.GenerateRuleScripts();
                // Сохранение информации о ролях
                Init.GenerateRoleScripts();
                // Сохранение информации о хранимых процедурах
                Init.GenerateStoredProcScripts();

                // Пишем подвал XML-документа
                Init.WriteXMLSuffix();
            }
        }

        private static void GenerateTableScriptsWithDependence()
        {
            Init.output.WriteStartElement("Tables");
            Scripter script = new Scripter(server);
            script.Options.ScriptDrops = false;
            script.Options.WithDependencies = true;
            script.Options.Indexes = true;   // To include indexes
            script.Options.DriAllConstraints = true;   // to include referential constraints in the script

            // Iterate through the tables in database and script each one. Display the script.   
            foreach (Table tb in database.Tables)
            {
                // check if the table is not a system table
                if (tb.IsSystemObject == false)
                {
                    Init.output.WriteElementString("Header", "[" + tb.Schema + "].[" + tb.Name + "]");
                    StringCollection sc = script.Script(new Urn[] { tb.Urn });
                    Init.output.WriteStartElement("script");
                    Init.output.WriteString("\n");
                    foreach (string st in sc)
                    {
                        Init.output.WriteString(st.Trim() + "\n");
                    }
                    Init.output.WriteEndElement();
                }
            }
            Init.output.WriteEndElement();
        }
        /// <summary>
        /// Записываем заголовок файла версий
        /// </summary>
        private static void WriteXMLHeader()
        {
            Init.output.WriteStartDocument();
            Init.output.WriteStartElement("Revision");
            Init.output.WriteAttributeString("Database", ConfigFile.databaseName);
            Init.output.WriteAttributeString("Create_date", Init.currentRevision.GenerateDateTime.ToShortDateString());
            Init.output.WriteAttributeString("Create_time", Init.currentRevision.GenerateDateTime.ToShortTimeString());
            Init.output.WriteAttributeString("Id", Init.currentRevision.HashCode);
            Init.output.WriteStartElement("Comment");
            Init.output.WriteString(Init.currentRevision.Comment);
            Init.output.WriteEndElement();
            Init.output.WriteStartElement("UpScripts");
            Init.output.WriteStartElement("IfExistsDatabase");
            Init.output.WriteString("\n");
            Init.output.WriteString("IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'" + ConfigFile.databaseName + "')\n");
            Init.output.WriteString("DROP DATABASE " + ConfigFile.databaseName + "\n");
            Init.output.WriteString("GO\n");
            Init.output.WriteEndElement(); // "IfExistsDatabase"
            Init.output.WriteStartElement("CreateDatabase");
            Init.output.WriteString("\n");
            Init.output.WriteString("CREATE DATABASE " + ConfigFile.databaseName + "\n");
            Init.output.WriteString("GO\n");
            Init.output.WriteEndElement(); // "CreateDatabase"
        }
        /// <summary>
        /// Записываем подвал файла версий
        /// </summary>
        private static void WriteXMLSuffix()
        {
            Init.output.WriteEndElement(); // "UpScripts"
            Init.output.WriteStartElement("DownScripts");
            Init.output.WriteEndElement(); // "DownScripts"
            Init.output.WriteEndElement(); // "Revision"
            Init.output.WriteEndDocument();
        }
        /// <summary>
        /// Генерирует скрипты создания хранимых процедур и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateStoredProcScripts()
        {
            // Создали открывающийся тег
            Init.output.WriteStartElement("Stored-Procedures");
            foreach (StoredProcedure proc in Init.database.StoredProcedures)
            {
                if ((proc.Schema != "sys") && ((proc.Schema != "dbo") || (proc.Name.Substring(0, 2) != "sp")))
                {
                    Init.output.WriteElementString("Header", proc.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = proc;
                    Scripter scriptor = new Scripter(Init.server);
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.Indexes = true;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.IncludeIfNotExists = true;
                    scriptor.Options.WithDependencies = true;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    Init.output.WriteStartElement("script");
                    Init.output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        Init.output.WriteString(s.Trim() + "\n");
                    }
                    Init.output.WriteEndElement();
                }
            }
            Init.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания ролей и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateRoleScripts()
        {
            // Создали открывающийся тег
            Init.output.WriteStartElement("Roles");
            foreach (DatabaseRole role in Init.database.Roles)
            {
                if (role.Name == "public")
                    continue;
                Init.output.WriteElementString("Header", role.Name);
                StringCollection strCollection = new StringCollection();
                SqlSmoObject[] smoObj = new SqlSmoObject[1];
                smoObj[0] = role;
                Scripter scriptor = new Scripter(Init.server);
                scriptor.Options.AllowSystemObjects = false;
                scriptor.Options.IncludeIfNotExists = true;
                strCollection = scriptor.Script(smoObj);
                Init.output.WriteStartElement("script");
                Init.output.WriteString("\n");
                foreach (string s in strCollection)
                {
                    Init.output.WriteString(s.Trim() + "\n");
                }
                Init.output.WriteEndElement();
            }
            Init.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания правил и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateRuleScripts()
        {
            // Создали открывающийся тег
            Init.output.WriteStartElement("Rules");
            foreach (Rule rule in Init.database.Rules)
            {
                Init.output.WriteElementString("Header", rule.Name);
                StringCollection strCollection = new StringCollection();
                SqlSmoObject[] smoObj = new SqlSmoObject[1];
                smoObj[0] = rule;
                Scripter scriptor = new Scripter(Init.server);
                scriptor.Options.AllowSystemObjects = false;
                scriptor.Options.Indexes = true;
                scriptor.Options.DriAll = true;
                scriptor.Options.Default = true;
                scriptor.Options.IncludeIfNotExists = true;
                scriptor.Options.WithDependencies = true;
                scriptor.Options.ScriptSchema = true;
                strCollection = scriptor.Script(smoObj);
                Init.output.WriteStartElement("script");
                Init.output.WriteString("\n");
                foreach (string s in strCollection)
                {
                    Init.output.WriteString(s.Trim() + "\n");
                }
                Init.output.WriteEndElement();
            }
            output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания таблиц и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateTableScripts()
        {
            // Создали открывающийся тег
            Init.output.WriteStartElement("Tables");
            foreach (Table table in Init.database.Tables)
            {
                Init.output.WriteElementString("Header", "[" + table.Schema + "].[" + table.Name + "]");
                StringCollection strCollection = new StringCollection();
                SqlSmoObject[] smoObj = new SqlSmoObject[1];
                smoObj[0] = table;
                Scripter scriptor = new Scripter(Init.server);
                scriptor.Options.AllowSystemObjects = false;
                //scriptor.Options.Indexes = true;
                //scriptor.Options.DriAll = true;
                scriptor.Options.IncludeIfNotExists = true;
                scriptor.Options.Default = true;
                scriptor.Options.WithDependencies = true;
                scriptor.Options.ScriptSchema = true;
                strCollection = scriptor.Script(smoObj);
                Init.output.WriteStartElement("script");
                Init.output.WriteString("\n");
                foreach (string s in strCollection)
                {
                    Init.output.WriteString(s.Trim() + "\n");
                }
                Init.output.WriteEndElement();
            }
            Init.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания триггеров и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateTriggerScripts()
        {
            // Создали открывающийся тег
            Init.output.WriteStartElement("DML-Triggers");
            foreach (Table table in Init.database.Tables)
            {
                foreach (Trigger trigger in table.Triggers)
                {
                    Init.output.WriteElementString("Header", trigger.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = trigger;
                    Scripter scriptor = new Scripter(Init.server);
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.IncludeIfNotExists = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    Init.output.WriteStartElement("script");
                    Init.output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        Init.output.WriteString(s.Trim() + "\n");
                    }
                    Init.output.WriteEndElement();
                }
            }
            Init.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания проверок и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateCheckScripts()
        {
            // Создали открывающийся тег
            Init.output.WriteStartElement("Checks");
            foreach (Table table in Init.database.Tables)
            {
                foreach (Check check in table.Checks)
                {
                    Init.output.WriteElementString("Header", check.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = check;
                    Scripter scriptor = new Scripter(Init.server);
                    scriptor.Options.IncludeIfNotExists = true;
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    Init.output.WriteStartElement("script");
                    Init.output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        Init.output.WriteString(s.Trim() + "\n");
                    }
                    Init.output.WriteEndElement();
                }
            }
            Init.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания внешних ключей и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateForeignKeyScripts()
        {
            // Создали открывающийся тег
            Init.output.WriteStartElement("Foreign-Key");
            foreach (Table table in Init.database.Tables)
            {
                foreach (ForeignKey fk in table.ForeignKeys)
                {
                    Init.output.WriteElementString("Header", fk.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = fk;
                    Scripter scriptor = new Scripter(Init.server);
                    scriptor.Options.IncludeIfNotExists = true;
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    Init.output.WriteStartElement("script");
                    Init.output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        Init.output.WriteString(s.Trim() + "\n");
                    }
                    Init.output.WriteEndElement();
                }
            }
            Init.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания индексов и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateIndexScripts()
        {
            // Создали открывающийся тег
            Init.output.WriteStartElement("Indexes");
            foreach (Table table in Init.database.Tables)
            {
                foreach (Index index in table.Indexes)
                {
                    Init.output.WriteElementString("Header", index.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = index;
                    Scripter scriptor = new Scripter(Init.server);
                    scriptor.Options.IncludeIfNotExists = true;
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    Init.output.WriteStartElement("script");
                    Init.output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        Init.output.WriteString(s.Trim() + "\n");
                    }
                    Init.output.WriteEndElement();
                }
            }
            Init.output.WriteEndElement();
        }
    }
}
