using System;
using System.Data.SqlClient;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

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
                Server server = new Server(Config.serverName);
                Database database = server.Databases[Config.databaseName];
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
