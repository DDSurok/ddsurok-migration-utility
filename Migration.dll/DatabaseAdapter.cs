using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace migration
{
    internal static class DatabaseAdapter
    {
        /// <summary>
        /// Соединение с сервером и, в случае <code>IsExists</code> = <value>true</value>,
        /// то база данных уже выбрана, иначе - не выбрана.
        /// </summary>
        private static SqlConnection connection { get; set; }
        /// <summary>
        /// Хранит флаг наличия базы данных.
        /// </summary>
        internal static bool IsExists { get; private set; }
        /// <summary>
        /// Инициализация компонентов программы для работы с сервером и БД.
        /// </summary>
        internal static void Initial()
        {
            DatabaseAdapter.connection = new SqlConnection("Data Source=" + Configuration.serverName + ";Integrated Security=True");
            DatabaseAdapter.connection.Open();
            try
            {
                DatabaseAdapter.connection.ChangeDatabase(Configuration.databaseName);
                DatabaseAdapter.IsExists = true;
            }
            catch (Exception)
            {
                DatabaseAdapter.IsExists = false;
            }
        }
        /// <summary>
        /// Обновить информацию о версии ревизии базы данных.
        /// </summary>
        /// <param name="revision">Данные о ревизии</param>
        internal static void UpdateVersionDatabase(RevisionInfo revision)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "TRUNCATE TABLE [dds].[version]";
            command.ExecuteNonQuery();
            command.CommandText = "insert into [dds].[version] (hashCode, generateDateTime, nickName, comment) VALUES ('"
                                    + revision.HashCode + "','"
                                    + revision.GenerateDateTime.ToString("dd.MM.yyyy:hh.mm") + "','"
                                    + revision.Author + "','"
                                    + revision.Comment + "')";
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Очистка таблиц <code>dds.up</code>, <code>dds.down</code> от данных.
        /// </summary>
        internal static void ClearUpDownScripts()
        {
            SqlCommand command = DatabaseAdapter.connection.CreateCommand();
            command.CommandText = "TRUNCATE TABLE [dds].[up]";
            command.ExecuteNonQuery();
            command.CommandText = "TRUNCATE TABLE [dds].[down];";
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Применить список скриптов.
        /// </summary>
        /// <param name="list">Список скриптов</param>
        internal static void ApplyScripts(List<string> list, int DestinationVersion)
        {
            SqlTransaction tran = connection.BeginTransaction();
            SqlCommand command = connection.CreateCommand();
            command.Transaction = tran;
            try
            {
                foreach (string query in list)
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                tran.Commit();
                    
                DatabaseAdapter.UpdateVersionDatabase(RevisionList.GetRevisionList()[DestinationVersion]);
            }
            catch (Exception)
            {
                tran.Rollback();
            }
        }
        /// <summary>
        /// Заполняем базу таблицами необходимыми для работы таблицами и триггерами.
        /// </summary>
        internal static void IntegrateServiceDataInDatabase()
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "ALTER DATABASE [" + Configuration.databaseName + "] SET TRUSTWORTHY ON";
            command.ExecuteNonQuery();
            DatabaseAdapter.RemoveServiceDataInDatabase();
            command.CommandText = functions.LoadFileToStringCollection("SQL/CreateSchema.sql");
            command.ExecuteNonQuery();
            command.CommandText = functions.LoadFileToStringCollection("SQL/CreateTables.sql");
            command.ExecuteNonQuery();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(".");
            command.CommandText = @"CREATE ASSEMBLY CLRFunctions FROM '" + di.FullName + @"\SqlCLR\SqlCLR.dll' WITH PERMISSION_SET = UNSAFE";
            command.ExecuteNonQuery();
            //command.CommandText = @"CREATE ASYMMETRIC KEY [DDSurok] FROM EXECUTABLE FILE = '" + di.FullName + @"\SqlCLR\SqlCLR.dll'";
            //command.ExecuteNonQuery();
            command.CommandText = functions.LoadFileToStringCollection("SQL/CreateCLRFunction.sql");
            command.ExecuteNonQuery();
            command.CommandText = functions.LoadFileToStringCollection("SQL/CreateDDLTriggers.sql");
            command.ExecuteNonQuery();
            command.CommandText = @"sp_configure 'clr enabled', 1";
            command.ExecuteNonQuery();
            command.CommandText = @"reconfigure";
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Очистка базы данных от созданных при инициализации служебных структур.
        /// </summary>
        internal static void RemoveServiceDataInDatabase()
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = functions.LoadFileToStringCollection("SQL/IfExists.sql");
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Получить из базы данных скрипты повышения
        /// </summary>
        /// <returns>Список скриптов повышения</returns>
        internal static List<string> GetUpScripts()
        {
            SqlCommand command = DatabaseAdapter.connection.CreateCommand();
            command.CommandText = "SELECT [dds].[up].[script] AS [script] FROM [dds].[up] ORDER BY [dds].[up].[id] ASC";
            SqlDataReader reader = command.ExecuteReader();
            List<string> ret = new List<string>();
            while (reader.Read())
            {
                ret.Add(reader["script"].ToString());
            }
            reader.Close();
            return ret;
        }
        /// <summary>
        /// Получить из базы данных скрипты понижения
        /// </summary>
        /// <returns>Список скриптов понижения</returns>
        internal static List<string> GetDownScripts()
        {
            SqlCommand command = DatabaseAdapter.connection.CreateCommand();
            command.CommandText = "SELECT [dds].[down].[script] AS [script] FROM [dds].[down] ORDER BY [dds].[down].[id] DESC";
            SqlDataReader reader = command.ExecuteReader();
            List<string> ret = new List<string>();
            while (reader.Read())
            {
                ret.Add(reader["script"].ToString());
            }
            reader.Close();
            return ret;
        }
        /// <summary>
        /// Получить количество изменений в базе
        /// </summary>
        /// <returns>Количество изменений</returns>
        internal static int GetCountChanges()
        {
            SqlCommand command = DatabaseAdapter.connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) AS [COUNT] FROM [dds].[up]";
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int returnValue = int.Parse(reader["COUNT"].ToString());
            reader.Close();
            return returnValue;
        }
    }
}