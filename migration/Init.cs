﻿using System;
using System.Text;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;
using System.Diagnostics;
using System.Security.Cryptography;

namespace migration
{
    static class Init
    {
        /// <summary>
        /// Основной метод класса
        /// </summary>
        internal static void Run()
        {
            try
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
                using (XmlWriter output = XmlWriter.Create("versions.xml", settings))
                {
                    Server server = new Server(Config.serverName);
                    Database database = server.Databases[Config.databaseName];
                    // Пишем информацию в XML о типе записи
                    Init.WriteXMLHeader(output);

                    // Инициализируем базу данных на работу с нашей программой
                    Init.InitialDatabase();

                    // Сохранение информации о таблицах
                    // Собственно схемы таблиц
                    Init.GenerateTableScripts(output, database);
                    // Триггеры к таблицам
                    Init.GenerateTriggerScripts(output, database);
                    // Индексы к таблицам
                    Init.GenerateIndexScripts(output, database);
                    // Проверки к таблицам
                    Init.GenerateCheckScripts(output, database);
                    // Внешние ключи
                    Init.GenerateForeignKeyScripts(output, database);

                    // Сохранение информации о правилах
                    Init.GenerateRuleScripts(output, database);

                    // Сохранение информации о ролях
                    Init.GenerateRoleScripts(output, server, database);

                    // Сохранение информации о хранимых процедурах
                    Init.GenerateStoredProcScripts(output, server, database);

                    output.WriteEndElement();
                    output.WriteEndDocument();

                    Process hg = Process.Start("hg", "init");
                    hg.WaitForExit();

                    hg = Process.Start("hg", "add versions.xml migration.conf");
                    hg.WaitForExit();

                    hg = Process.Start("hg", "commit");
                    hg.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey(true);
            }
        }
        /// <summary>
        /// Заполняем базу таблицами необходимыми для работы таблицами и триггерами
        /// </summary>
        private static void InitialDatabase()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=" + Config.serverName + ";Integrated Security=True"))
            {
                connection.Open();
                connection.ChangeDatabase(Config.databaseName);
                SqlCommand command = connection.CreateCommand();
                command.CommandText = Init.LoadFileToStringCollection("migration/IfExists.sql");
                command.ExecuteNonQuery();
                command.CommandText = Init.LoadFileToStringCollection("migration/CreateSchema.sql");
                command.ExecuteNonQuery();
                command.CommandText = Init.LoadFileToStringCollection("migration/CreateTables.sql");
                command.ExecuteNonQuery();
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(".");
                command.CommandText = "CREATE ASSEMBLY CLRFunctions FROM '" + di.FullName + "\\SqlCLR.dll'";
                command.ExecuteNonQuery();
                command.CommandText = Init.LoadFileToStringCollection("migration/CreateCLRFunction.sql");
                command.ExecuteNonQuery();
                command.CommandText = Init.LoadFileToStringCollection("migration/CreateDDLTriggers.sql");
                command.ExecuteNonQuery();
                command.CommandText = "sp_configure 'clr enabled', 1";
                command.ExecuteNonQuery();
                command.CommandText = "reconfigure";
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        /// <summary>
        /// Загружает содержимое файла в одну строку
        /// </summary>
        /// <param name="fileName">Имя файла, содержимое которого надо загрузить</param>
        /// <returns>Содержимое файла, преобразованное в одну строку через пробел</returns>
        private static string LoadFileToStringCollection(string fileName)
        {
            string retStr = "";

            using (TextReader reader = File.OpenText(fileName))
            {
                string s = "";
                do
                {
                    s = reader.ReadLine();
                    if (s != null) if (s.Trim() != "") retStr += s.Trim() + " ";
                } while (s != null);
            }
            return retStr;
        }
        /// <summary>
        /// Записываем заголовок файла конфигурации
        /// </summary>
        /// <param name="output">Дескриптор файла, в который нужно записать заголовок</param>
        private static void WriteXMLHeader(XmlWriter output)
        {
            output.WriteStartDocument();
            output.WriteStartElement("Revision");
            output.WriteAttributeString("Database", Config.databaseName);
            output.WriteAttributeString("Create_date", DateTime.Today.ToShortDateString());
            output.WriteAttributeString("Create_time", DateTime.Now.ToShortTimeString());
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] byteHash = sha.ComputeHash(Encoding.Unicode.GetBytes(DateTime.Today.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()));
            string hash = "";
            foreach (byte b in byteHash)  
                hash += string.Format("{0:x2}", b);
            output.WriteAttributeString("Id", hash);
        }
        /// <summary>
        /// Генерирует скрипты создания хранимых процедур и пишет их в <code>output</code>
        /// </summary>
        /// <param name="output">Дескриптор файла, в который записываются хранимые процедуры</param>
        /// <param name="server">Сервер, на котором хранится база данных</param>
        /// <param name="database">Контролируемая база данных</param>
        private static void GenerateStoredProcScripts(XmlWriter output, Server server, Database database)
        {
            // Создали открывающийся тег
            output.WriteStartElement("Stored-Procedures");
            foreach (StoredProcedure proc in database.StoredProcedures)
            {
                if ((proc.Schema != "sys") && ((proc.Schema != "dbo") || (proc.Name.Substring(0, 2) != "sp")))
                {
                    output.WriteElementString("Header", proc.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = proc;
                    Scripter scriptor = new Scripter(server);
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.Indexes = true;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = true;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    output.WriteStartElement("script");
                    output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        output.WriteString(s.Trim() + "\n");
                    }
                    output.WriteEndElement();
                }
            }
            output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания ролей и пишет их в <code>output</code>
        /// </summary>
        /// <param name="output">Дескриптор файла, в который записываются хранимые процедуры</param>
        /// <param name="server">Сервер, на котором хранится база данных</param>
        /// <param name="database">Контролируемая база данных</param>
        private static void GenerateRoleScripts(XmlWriter output, Server server, Database database)
        {
            // Создали открывающийся тег
            output.WriteStartElement("Roles");
            foreach (DatabaseRole role in database.Roles)
            {
                if (role.Name == "public")
                    continue;
                output.WriteElementString("Header", role.Name);
                StringCollection strCollection = new StringCollection();
                SqlSmoObject[] smoObj = new SqlSmoObject[1];
                smoObj[0] = role;
                Scripter scriptor = new Scripter(server);
                scriptor.Options.AllowSystemObjects = false;
                strCollection = scriptor.Script(smoObj);
                output.WriteStartElement("script");
                output.WriteString("\n");
                foreach (string s in strCollection)
                {
                    output.WriteString(s.Trim() + "\n");
                }
                output.WriteEndElement();
            }
            output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания правил и пишет их в <code>output</code>
        /// </summary>
        /// <param name="output">Дескриптор файла, в который записываются хранимые процедуры</param>
        /// <param name="server">Сервер, на котором хранится база данных</param>
        /// <param name="database">Контролируемая база данных</param>
        private static void GenerateRuleScripts(XmlWriter output, Database database)
        {
            // Создали открывающийся тег
            output.WriteStartElement("Rules");
            foreach (Rule rule in database.Rules)
            {
                output.WriteElementString("Header", rule.Name);
                StringCollection strCollection = new StringCollection();
                SqlSmoObject[] smoObj = new SqlSmoObject[1];
                smoObj[0] = rule;
                Scripter scriptor = new Scripter(database.Parent);
                scriptor.Options.AllowSystemObjects = false;
                scriptor.Options.Indexes = true;
                scriptor.Options.DriAll = true;
                scriptor.Options.Default = true;
                scriptor.Options.WithDependencies = true;
                scriptor.Options.ScriptSchema = true;
                strCollection = scriptor.Script(smoObj);
                output.WriteStartElement("script");
                output.WriteString("\n");
                foreach (string s in strCollection)
                {
                    output.WriteString(s.Trim() + "\n");
                }
                output.WriteEndElement();
            }
            output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания таблиц и пишет их в <code>output</code>
        /// </summary>
        /// <param name="output">Дескриптор файла, в который записываются хранимые процедуры</param>
        /// <param name="server">Сервер, на котором хранится база данных</param>
        /// <param name="database">Контролируемая база данных</param>
        private static void GenerateTableScripts(XmlWriter output, Database database)
        {
            // Создали открывающийся тег
            output.WriteStartElement("Tables");
            foreach (Table table in database.Tables)
            {
                output.WriteElementString("Header", "[" + table.Schema + "].[" + table.Name + "]");
                StringCollection strCollection = new StringCollection();
                SqlSmoObject[] smoObj = new SqlSmoObject[1];
                smoObj[0] = table;
                Scripter scriptor = new Scripter(database.Parent);
                scriptor.Options.AllowSystemObjects = false;
                //scriptor.Options.Indexes = true;
                //scriptor.Options.DriAll = true;
                scriptor.Options.Default = true;
                //scriptor.Options.WithDependencies = true;
                scriptor.Options.ScriptSchema = true;
                strCollection = scriptor.Script(smoObj);
                output.WriteStartElement("script");
                output.WriteString("\n");
                foreach (string s in strCollection)
                {
                    output.WriteString(s.Trim() + "\n");
                }
                output.WriteEndElement();
            }
            output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания триггеров и пишет их в <code>output</code>
        /// </summary>
        /// <param name="output">Дескриптор файла, в который записываются хранимые процедуры</param>
        /// <param name="server">Сервер, на котором хранится база данных</param>
        /// <param name="database">Контролируемая база данных</param>
        private static void GenerateTriggerScripts(XmlWriter output, Database database)
        {
            // Создали открывающийся тег
            output.WriteStartElement("DML-Triggers");
            foreach (Table table in database.Tables)
            {
                foreach (Trigger trigger in table.Triggers)
                {
                    output.WriteElementString("Header", trigger.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = trigger;
                    Scripter scriptor = new Scripter(database.Parent);
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    output.WriteStartElement("script");
                    output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        output.WriteString(s.Trim() + "\n");
                    }
                    output.WriteEndElement();
                }
            }
            output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания проверок и пишет их в <code>output</code>
        /// </summary>
        /// <param name="output">Дескриптор файла, в который записываются хранимые процедуры</param>
        /// <param name="server">Сервер, на котором хранится база данных</param>
        /// <param name="database">Контролируемая база данных</param>
        private static void GenerateCheckScripts(XmlWriter output, Database database)
        {
            // Создали открывающийся тег
            output.WriteStartElement("Checks");
            foreach (Table table in database.Tables)
            {
                foreach (Check check in table.Checks)
                {
                    output.WriteElementString("Header", check.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = check;
                    Scripter scriptor = new Scripter(database.Parent);
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    output.WriteStartElement("script");
                    output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        output.WriteString(s.Trim() + "\n");
                    }
                    output.WriteEndElement();
                }
            }
            output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания внешних ключей и пишет их в <code>output</code>
        /// </summary>
        /// <param name="output">Дескриптор файла, в который записываются хранимые процедуры</param>
        /// <param name="server">Сервер, на котором хранится база данных</param>
        /// <param name="database">Контролируемая база данных</param>
        private static void GenerateForeignKeyScripts(XmlWriter output, Database database)
        {
            // Создали открывающийся тег
            output.WriteStartElement("Foreign-Key");
            foreach (Table table in database.Tables)
            {
                foreach (ForeignKey fk in table.ForeignKeys)
                {
                    output.WriteElementString("Header", fk.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = fk;
                    Scripter scriptor = new Scripter(database.Parent);
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    output.WriteStartElement("script");
                    output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        output.WriteString(s.Trim() + "\n");
                    }
                    output.WriteEndElement();
                }
            }
            output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания индексов и пишет их в <code>output</code>
        /// </summary>
        /// <param name="output">Дескриптор файла, в который записываются хранимые процедуры</param>
        /// <param name="server">Сервер, на котором хранится база данных</param>
        /// <param name="database">Контролируемая база данных</param>
        private static void GenerateIndexScripts(XmlWriter output, Database database)
        {
            // Создали открывающийся тег
            output.WriteStartElement("Indexes");
            foreach (Table table in database.Tables)
            {
                foreach (Index index in table.Indexes)
                {
                    output.WriteElementString("Header", index.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = index;
                    Scripter scriptor = new Scripter(database.Parent);
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    output.WriteStartElement("script");
                    output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        output.WriteString(s.Trim() + "\n");
                    }
                    output.WriteEndElement();
                }
            }
            output.WriteEndElement();
        }
    }
}
