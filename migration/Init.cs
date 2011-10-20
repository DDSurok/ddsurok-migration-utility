using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Collections.Specialized;

namespace migration
{
    static class Init
    {
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
                using (XmlWriter output = XmlWriter.Create("init.xml", settings))
                {
                    Server server = new Server(Config.serverName);
                    Database database = server.Databases[Config.databaseName];
                    // Пишем информацию в XML о типе записи
                    Init.WriteXMLHeader(output);

                    // Инициализируем сервер на работу с нашей программой
                    Init.InitialServer(server);

                    // Инициализируем базу данных на работу с нашей программой
                    Init.InitialDatabase(database);

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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey(true);
            }
        }

        private static void InitialServer(Server server)
        {
            
        }

        private static void InitialDatabase(Database database)
        {
            StringCollection strCol = new StringCollection();
            //strCol.Add("create login ddsurok");
            //strCol.Add("with password = 'ddsurok';");
            strCol.Add("use proba;");
            strCol.Add("create user ddsurok without login;");
            strCol.Add("go");
            strCol.Add("execute as user = 'ddsurok'");
            strCol.Add("go");
            //strCol.Add("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ddsurok].[Customer]') AND type in (N'U'))");
            //strCol.Add("DROP TABLE [dbo].[Customer]");
            //strCol.Add("");
            //strCol.Add("");
            //strCol.Add("");
            //strCol.Add("");
            //strCol.Add("");
            //strCol.Add("");
            strCol.Add("CREATE SCHEMA [ddsurok] AUTHORIZATION [ddsurok]");
            strCol.Add("GO");
            strCol.Add("CREATE TABLE [ddsurok].[up]");
            strCol.Add("(");
            strCol.Add("id int NOT NULL IDENTITY(1, 1) PRIMARY KEY,");
            strCol.Add("script text NOT NULL");
            strCol.Add(") ON");
            strCol.Add("GO");
            strCol.Add("CREATE TABLE [ddsurok].[down]");
            strCol.Add("(");
            strCol.Add("id int NOT NULL IDENTITY(1, 1) PRIMARY KEY,");
            strCol.Add("script text NOT NULL");
            strCol.Add(") ON");
            strCol.Add("GO");
            //strCol.Add("CREATE TRIGGER ddl_all_changes_log");
            //strCol.Add("AS DATABASE");
            //strCol.Add("");
            //strCol.Add("");
            
            database.ExecuteNonQuery(strCol);
        }

        private static void WriteXMLHeader(XmlWriter output)
        {
            output.WriteStartDocument();
            output.WriteStartElement("Configuration");
            output.WriteAttributeString("Database", Config.databaseName);
            output.WriteAttributeString("Create_date", DateTime.Today.ToShortDateString());
            output.WriteAttributeString("Create_time", DateTime.Now.ToShortTimeString());
            output.WriteAttributeString("Version", 1.ToString("0000"));
        }

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
