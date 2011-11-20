using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace migration
{
    internal static class Database
    {
        public static bool IsExists { get; set; }
        private static SqlConnection connection { get; set; }
        /// <summary>
        /// Стартовая инициализация программы на сервер и БД
        /// </summary>
        public static void Initial()
        {
            Database.connection = new SqlConnection("Data Source=" + ConfigFile.serverName + ";Integrated Security=True");
            Database.connection.Open();
            try
            {
                Database.connection.ChangeDatabase(ConfigFile.databaseName);
                Database.IsExists = true;
            }
            catch (Exception)
            {
                Database.IsExists = false;
            }
        }
        /// <summary>
        /// Заполняем базу таблицами необходимыми для работы таблицами и триггерами
        /// </summary>
        public static void InitialDatabase()
        {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "ALTER DATABASE [" + ConfigFile.databaseName + "] SET TRUSTWORTHY ON";
                command.ExecuteNonQuery();
                command.CommandText = Database.LoadFileToStringCollection("SQL/IfExists.sql");
                command.ExecuteNonQuery();
                command.CommandText = Database.LoadFileToStringCollection("SQL/CreateSchema.sql");
                command.ExecuteNonQuery();
                command.CommandText = Database.LoadFileToStringCollection("SQL/CreateTables.sql");
                command.ExecuteNonQuery();
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(".");
                command.CommandText = @"CREATE ASSEMBLY CLRFunctions FROM '" + di.FullName + @"\SqlCLR\SqlCLR.dll' WITH PERMISSION_SET = UNSAFE";
                command.ExecuteNonQuery();
                //command.CommandText = @"CREATE ASYMMETRIC KEY [DDSurok] FROM EXECUTABLE FILE = '" + di.FullName + @"\SqlCLR\SqlCLR.dll'";
                //command.ExecuteNonQuery();
                command.CommandText = Database.LoadFileToStringCollection("SQL/CreateCLRFunction.sql");
                command.ExecuteNonQuery();
                command.CommandText = Database.LoadFileToStringCollection("SQL/CreateDDLTriggers.sql");
                command.ExecuteNonQuery();
                command.CommandText = @"sp_configure 'clr enabled', 1";
                command.ExecuteNonQuery();
                command.CommandText = @"reconfigure";
                command.ExecuteNonQuery();
        }
        /// <summary>
        /// Записываем в XML скрипты повышения
        /// </summary>
        /// <param name="output">XML, куда записываются скрипты</param>
        public static void WriteScriptsUp(XmlWriter output)
        {
            output.WriteStartElement("UpScripts");
            SqlCommand command = Database.connection.CreateCommand();
            command.CommandText = "SELECT [dds].[up].[id] AS [id], [dds].[up].[script] AS [script] FROM [dds].[up] ORDER BY [dds].[up].[id] ASC";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.WriteElementString(int.Parse(reader["id"].ToString()).ToString("up00000000"), reader["script"].ToString());
            }
            output.WriteEndElement();
            reader.Close();
        }
        /// <summary>
        /// Записываем с XML скрипты понижения
        /// </summary>
        /// <param name="output">XML, куда записываются скрипты</param>
        public static void WriteScriptsDown(XmlWriter output)
        {
            output.WriteStartElement("DownScripts");
            SqlCommand command = Database.connection.CreateCommand();
            command.CommandText = "SELECT [dds].[down].[id] AS [id], [dds].[down].[script] AS [script] FROM [dds].[down] ORDER BY [dds].[down].[id] DESC";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.WriteElementString(int.Parse(reader["id"].ToString()).ToString("down00000000"), reader["script"].ToString());
            }
            output.WriteEndElement();
            reader.Close();
        }
        /// <summary>
        /// Очистка таблиц dds.up, dds.down и dds.version от данных
        /// </summary>
        public static void ClearUpDownScripts()
        {
            SqlCommand command = Database.connection.CreateCommand();
            command.CommandText = "TRUNCATE TABLE [dds].[up]";
            command.ExecuteNonQuery();
            command.CommandText = "TRUNCATE TABLE [dds].[down];";
            command.ExecuteNonQuery();
            command.CommandText = "TRUNCATE TABLE [dds].[version];";
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Получить количество изменений в базе
        /// </summary>
        /// <returns>Количество изменений</returns>
        public static int GetCountChanges()
        {
            SqlCommand command = Database.connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) AS [COUNT] FROM [dds].[up]";
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int returnValue = int.Parse(reader["COUNT"].ToString());
            reader.Close();
            return returnValue;
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
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "insert into [dds].[version] (hashCode, generateDateTime, nickName, comment) VALUES ('"
                                    + revision.HashCode + "','"
                                    + revision.GenerateDateTime.ToString("dd.MM.yyyy:hh.mm") + "','"
                                    + revision.Author + "','"
                                    + revision.Comment + "')";
            command.ExecuteNonQuery();
        }
    }
}
