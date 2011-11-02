using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace migration
{
    class Program
    {
        static public string fileName = @"conf\versions.xml";
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
                            Init.Run();
                            break;
                        case "--migration-first":
                            ToFirst.Run();
                            break;
                        case "--commit":
                            Commit.Run();
                            break;
                        case "--pop":
                            migration.Program.Pop();
                            break;
                        case "--push":
                            migration.Program.Push();
                            break;
                        case "--list":
                            migration.Program.PrintListChanges();
                            break;
                        case "--migration-to":
                            UpDown.Run(args[1]);
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
                Console.ReadKey(false);
            }
            
        }
        /// <summary>
        /// Протолкнуть все изменения в удаленный репозиторий
        /// </summary>
        private static void Push()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.WorkingDirectory = Directory.GetCurrentDirectory() + @"\conf";
            info.FileName = "hg";

            info.Arguments = "push " + Config.remoteRepository;    // Отправим в удаленный репозиторий

            Process hg = Process.Start(info);
            hg.WaitForExit();
        }
        /// <summary>
        /// Получить все изменения из удаленного репозитория
        /// </summary>
        private static void Pop()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.WorkingDirectory = Directory.GetCurrentDirectory() + @"\conf";
            info.FileName = "hg";

            info.Arguments = "pop " + Config.remoteRepository;    // Получим из удаленного репозитория

            Process hg = Process.Start(info);
            hg.WaitForExit();
        }
        /// <summary>
        /// 
        /// </summary>
        private static void PrintListChanges()
        {
            using (XmlReader input = XmlReader.Create(Program.fileName))
            {
                //input.
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private static void PrintWrongMessage()
        {
            Console.WriteLine("Please use the - HELP for help with the work program");
            Console.ReadKey(true);
        }
        /// <summary>
        /// 
        /// </summary>
        private static void PrintHelp()    // TODO
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
    }
}
