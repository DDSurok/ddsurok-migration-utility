using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace migration.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Program.PrintWrongMessage();
                }
                else
                {
                    switch (args[0])
                    {
                        case "--init":
                            if (args.Length > 1)
                            {
                                System.Console.Write("If continue, full rewrite list of revisions\n");
                                System.Console.Write("Press 'y' from continue: ");
                                char ch = System.Console.ReadKey(false).KeyChar;
                                if (ch.ToString().ToUpper() == "Y")
                                {
                                    System.Console.WriteLine("\n\n");
                                    Migration.Init(args[1]);
                                }
                            }
                            else
                                throw new System.Exception("Укажите комментарий");
                            break;
                        case "--fix":
                            Migration.Fix(args[1]);
                            break;
                        case "--forgot":
                            Migration.Forgot();
                            break;
                        case "--list":
                            System.Console.Write(Migration.GetCurrentRevision().ToString("0000+ ") + "Current revision\n\n");
                            foreach (Library.RevisionInfo info in Migration.GetReverseRevisionList())
                            {
                                foreach (string s in info.ToStrings())
                                {
                                    System.Console.WriteLine(s + "\n");
                                }
                                System.Console.WriteLine("\n\n");
                            }
                            System.Console.ReadKey(true);
                            break;
                        case "--migrate-to":
                            Migration.Migrate(int.Parse(args[1]));
                            break;
                        case "--help":
                            migration.Console.Program.PrintHelp();
                            break;
                        default:
                            PrintWrongMessage();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.ReadKey(false);
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        private static void PrintWrongMessage()
        {
            System.Console.WriteLine("Please use the - HELP for help with the work program");
            System.Console.ReadKey(true);
        }
        /// <summary>
        /// 
        /// </summary>
        private static void PrintHelp()    // TODO
        {
            System.Console.WriteLine("Migration utility v. " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            System.Console.WriteLine("Use the utility as follows:\tMIGRATION.EXE [OPTIONS]");
            System.Console.WriteLine("\n\tOPTIONS:");
            System.Console.WriteLine("  --init [COMMENT]                 Creating first revision of database with COMMENT");
            System.Console.WriteLine("  --fix [COMMENT]                  Creating next revision of database with COMMENT");
            System.Console.WriteLine("  --migrate-to [VERSION NUMBER]    Migrate to VERSION NUMBER version of database");
            System.Console.WriteLine("  --list                           Print list of revisions of database");
            System.Console.ReadKey(true);
        }
    }
}
