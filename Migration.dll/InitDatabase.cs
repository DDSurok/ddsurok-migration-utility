using System.Data.SqlClient;
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
                command.CommandText = "ALTER DATABASE [" + ConfigFile.databaseName + "] SET TRUSTWORTHY ON";
                command.ExecuteNonQuery();
                command.CommandText = InitDatabase.LoadFileToStringCollection("SQL/IfExists.sql");
                command.ExecuteNonQuery();
                command.CommandText = InitDatabase.LoadFileToStringCollection("SQL/CreateSchema.sql");
                command.ExecuteNonQuery();
                command.CommandText = InitDatabase.LoadFileToStringCollection("SQL/CreateTables.sql");
                command.ExecuteNonQuery();
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(".");
                command.CommandText = @"CREATE ASSEMBLY CLRFunctions FROM '" + di.FullName + @"\SqlCLR\SqlCLR.dll' WITH PERMISSION_SET = UNSAFE";
                command.ExecuteNonQuery();
                //command.CommandText = @"CREATE ASYMMETRIC KEY [DDSurok] FROM EXECUTABLE FILE = '" + di.FullName + @"\SqlCLR\SqlCLR.dll'";
                //command.ExecuteNonQuery();
                command.CommandText = InitDatabase.LoadFileToStringCollection("SQL/CreateCLRFunction.sql");
                command.ExecuteNonQuery();
                command.CommandText = InitDatabase.LoadFileToStringCollection("SQL/CreateDDLTriggers.sql");
                command.ExecuteNonQuery();
                command.CommandText = @"sp_configure 'clr enabled', 1";
                command.ExecuteNonQuery();
                command.CommandText = @"reconfigure";
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
        /// <summary>
        /// Обновить информацию о версии ревизии базы данных
        /// </summary>
        /// <param name="revision">Данные о ревизии</param>
        public static void UpdateVersionDatabase(RevisionInfo revision)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=" + ConfigFile.serverName + ";Integrated Security=True"))
            {
                connection.Open();
                connection.ChangeDatabase(ConfigFile.databaseName);
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "insert into [dds].[version] (hashCode, generateDateTime, nickName, comment) VALUES ('"
                                      + revision.HashCode + "','"
                                      + revision.GenerateDateTime.ToString("dd.MM.yyyy:hh.mm") + "','"
                                      + revision.Author + "','"
                                      + revision.Comment + "')";
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
