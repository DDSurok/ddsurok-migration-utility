using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.IO;

namespace migration
{
    public static class Forgot
    {
        
        public static void Run()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=" + ConfigFile.serverName + ";Integrated Security=True"))
            {
                connection.Open();
                connection.ChangeDatabase(ConfigFile.databaseName);
                SqlCommand command = connection.CreateCommand();
                command.CommandText = Forgot.LoadFileToStringCollection("SQL/IfExists.sql");
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
