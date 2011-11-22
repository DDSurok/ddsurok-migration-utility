using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace migration
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConfigFile.Load();
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
                                Console.Write("If continue, full rewrite list of revisions\n");
                                Console.Write("Press 'y' from continue: ");
                                char ch = Console.ReadKey(false).KeyChar;
                                if (ch.ToString().ToUpper() == "Y")
                                {
                                    Console.WriteLine("\n\n");
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
                            Console.Write(RevisionList.GetCurrentRevision().ToString("0000+ ") + "Current revision\n\n");
                            foreach (RevisionInfo info in RevisionList.GetReverseRevisionList())
                            {
                                foreach (string s in info.ToStrings())
                                {
                                    Console.WriteLine(s + "\n");
                                }
                                Console.WriteLine("\n\n");
                            }
                            Console.ReadKey(true);
                            break;
                        case "--migrate-to":
                            Migration.Migrate(int.Parse(args[1]));
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
            Console.WriteLine("  --init [COMMENT]                 Creating first revision of database with COMMENT");
            Console.WriteLine("  --fix [COMMENT]                  Creating next revision of database with COMMENT");
            Console.WriteLine("  --migrate-to [VERSION NUMBER]    Migrate to VERSION NUMBER version of database");
            Console.WriteLine("  --list                           Print list of revisions of database");
            Console.ReadKey(true);
        }
    }
}
