﻿using System.Data.SqlClient;
using System.IO;

namespace migration
{
    internal static class InitDatabase
    {
        /// <summary>
        /// Заполняем базу таблицами необходимыми для работы таблицами и триггерами
        /// </summary>
        public static void Initial()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=" + ConfigFile.serverName + ";Integrated Security=True"))
            {
                connection.Open();
                connection.ChangeDatabase(ConfigFile.databaseName);
                SqlCommand command = connection.CreateCommand();
                command.CommandText = InitDatabase.LoadFileToStringCollection("SQL/IfExists.sql");
                command.ExecuteNonQuery();
                command.CommandText = InitDatabase.LoadFileToStringCollection("SQL/CreateSchema.sql");
                command.ExecuteNonQuery();
                command.CommandText = InitDatabase.LoadFileToStringCollection("SQL/CreateTables.sql");
                command.ExecuteNonQuery();
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(".");
                command.CommandText = "CREATE ASSEMBLY CLRFunctions FROM '" + di.FullName + @"\SqlCLR.dll'";
                command.ExecuteNonQuery();
                command.CommandText = InitDatabase.LoadFileToStringCollection("SQL/CreateCLRFunction.sql");
                command.ExecuteNonQuery();
                command.CommandText = InitDatabase.LoadFileToStringCollection("SQL/CreateDDLTriggers.sql");
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
    }
}