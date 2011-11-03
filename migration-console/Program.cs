﻿using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using migration;

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
                                Console.Write("If continue, full rewrite list of revisions");
                                char ch = Console.ReadKey(false).KeyChar;
                                if ( ch == 'y')
                                    Init.Run(args[1]);
                            }
                            else
                                throw new System.Exception("Укажите комментарий");
                            break;
                        case "--fix":
                            Fix.Run(args[1]);
                            break;
                        case "--list":
                            RevisionList.Run();
                            break;
                        case "--migrate-to":
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