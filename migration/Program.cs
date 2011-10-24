using System;

namespace migration
{
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
                            Init.Run();
                            break;
                        case "--state-fix":
                            Program.StateFix();
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
    }
}
