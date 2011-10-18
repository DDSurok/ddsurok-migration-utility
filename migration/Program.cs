using System;
using System.Data.SqlClient;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Collections.Specialized;

namespace migration
{
    public class Config // OK
    {
        static internal void Load()
        {
            using (XmlReader reader = XmlReader.Create("migration.conf"))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "serverName":
                                    reader.Read();
                                    Config.serverName = reader.Value;
                                    break;
                                case "databaseName":
                                    reader.Read();
                                    Config.databaseName = reader.Value;
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.Comment:
                        case XmlNodeType.EndElement:
                            break;
                    }
                }
            }
            if ((Config.serverName.Trim() == "") || (Config.databaseName.Trim() == ""))
                throw new System.ArgumentException("Ошибочный файл настроек программы.\nПерезапустите генератор файла конфигурации.");
        }
        static public string serverName { get; set; }
        static public string databaseName { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Config.Load();
                if (args.Length == 0)
                {
                    migration.Program.PrintWrongMessage();
                }
                else
                {
                    switch (args[0])
                    {
                        case "--init":
                            migration.Program.Init();
                            break;
                        case "--state-fix":
                            migration.Program.StateFix();
                            break;
                        case "--migration-up":
                            migration.Program.MigrationUp();
                            break;
                        case "--migration-down":
                            migration.Program.MigrationDown();
                            break;
                        case "--help":
                            migration.Program.PrintHelp();
                            break;
                        default:
                            PrintWrongMessage();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        private static void PrintWrongMessage()
        {
            Console.WriteLine("Please use the - HELP for help with the work program");
            Console.ReadKey(true);
        }
        private static void PrintHelp()
        {
            Console.WriteLine("Migration utility v. " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("Use the utility as follows:\tMIGRATION.EXE [OPTIONS]");
            Console.WriteLine("\n\tOPTIONS:");
            Console.WriteLine("  --init\t\tCreating first version of database");
            Console.WriteLine("  --state-fix\t\tCreating next version of database");
            Console.WriteLine("  --migration-up\tMigrate to next version of database");
            Console.WriteLine("  --migration-down\tMigrate to previous version of database");
            Console.ReadKey(true);
        }
        private static void MigrationDown()
        {
            throw new NotImplementedException();
        }
        private static void MigrationUp()
        {
            throw new NotImplementedException();
        }
        private static void StateFix()
        {
            throw new NotImplementedException();
        }
        private static void Init()
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
                    output.WriteStartElement("Configuration");
                    output.WriteAttributeString("Database", Config.databaseName);
                    output.WriteAttributeString("Create_date", DateTime.Today.ToShortDateString());
                    output.WriteAttributeString("Create_time", DateTime.Now.ToShortTimeString());
                    output.WriteAttributeString("Version", 1.ToString("0000"));
                    // Сохранение информации о таблицах
                    
                    // Создали открывающийся тег
                    output.WriteStartElement("Tables");
                    foreach (Table table in database.Tables)
                    {
                        output.WriteElementString("Header", "Create table [" + table.Schema + "].[" + table.Name + "]");

                        StringCollection strCollection = new StringCollection();
                        SqlSmoObject[] smoObj = new SqlSmoObject[1];
                        smoObj[0] = table;
                        Scripter scriptor = new Scripter(server);
                        scriptor.Options.ScriptDrops = false;
                        scriptor.Options.WithDependencies = true;
                        strCollection = scriptor.Script(smoObj);
                        output.WriteStartElement("script");
                        output.WriteString("");
                        foreach (string s in strCollection)
                        {
                            output.WriteString(s);
                        }
                        output.WriteEndElement();
                    }
                    output.WriteEndElement();

                    // Сохранение информации о правилах

                    // Создали открывающийся тег
                    output.WriteStartElement("Rules");
                    foreach (Rule rule in database.Rules)
                    {
                        output.WriteElementString("Header", "Create rule " + rule.Name);

                        StringCollection strCollection = new StringCollection();
                        SqlSmoObject[] smoObj = new SqlSmoObject[1];
                        smoObj[0] = rule;
                        Scripter scriptor = new Scripter(server);
                        strCollection = scriptor.Script(smoObj);
                        output.WriteStartElement("script");
                        output.WriteString("");
                        foreach (string s in strCollection)
                        {
                            output.WriteString(s);
                        }
                        output.WriteEndElement();
                    }
                    output.WriteEndElement();

                    // Сохранение информации о ролях

                    // Создали открывающийся тег
                    output.WriteStartElement("Roles");
                    foreach (DatabaseRole role in database.Roles)
                    {
                        output.WriteElementString("Header", "Create role " + role.Name);

                        StringCollection strCollection = new StringCollection();
                        SqlSmoObject[] smoObj = new SqlSmoObject[1];
                        smoObj[0] = role;
                        Scripter scriptor = new Scripter(server);
                        strCollection = scriptor.Script(smoObj);
                        output.WriteStartElement("script");
                        output.WriteString("");
                        foreach (string s in strCollection)
                        {
                            output.WriteString(s);
                        }
                        output.WriteEndElement();
                    }
                    output.WriteEndElement();

                    // Сохранение информации о хранимых процедурах

                    // Создали открывающийся тег
                    output.WriteStartElement("storedProcedures");
                    foreach (StoredProcedure proc in database.StoredProcedures)
                    {
                        if ((proc.Schema != "sys") && ((proc.Schema != "dbo") || (proc.Name.Substring(0, 2) != "sp")))
                        {
                            output.WriteElementString("Header", "Create stored procedure " + proc.Name);

                            StringCollection strCollection = new StringCollection();
                            SqlSmoObject[] smoObj = new SqlSmoObject[1];
                            smoObj[0] = proc;
                            Scripter scriptor = new Scripter(server);
                            strCollection = scriptor.Script(smoObj);
                            output.WriteStartElement("script");
                            output.WriteString("");
                            foreach (string s in strCollection)
                            {
                                output.WriteString(s);
                            }
                            output.WriteEndElement();
                        }
                    }
                    output.WriteEndElement();

                    output.WriteEndElement();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey(true);
            }
        }
    }
}
