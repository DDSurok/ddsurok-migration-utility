using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace migration
{
    internal static partial class DatabaseAdapter
    {
        internal static bool IsExists { get; private set; }
        private static SqlConnection connection { get; set; }
        /// <summary>
        /// Инициализация компонентов программы для работы с сервером и БД
        /// </summary>
        internal static void Initial()
        {
            DatabaseAdapter.connection = new SqlConnection("Data Source=" + ConfigFile.serverName + ";Integrated Security=True");
            DatabaseAdapter.connection.Open();
            try
            {
                DatabaseAdapter.connection.ChangeDatabase(ConfigFile.databaseName);
                DatabaseAdapter.IsExists = true;
            }
            catch (Exception)
            {
                DatabaseAdapter.IsExists = false;
            }
        }
        /// <summary>
        /// Обновить информацию о версии ревизии базы данных
        /// </summary>
        /// <param name="revision">Данные о ревизии</param>
        internal static void UpdateVersionDatabase(RevisionInfo revision)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "insert into [dds].[version] (hashCode, generateDateTime, nickName, comment) VALUES ('"
                                    + revision.HashCode + "','"
                                    + revision.GenerateDateTime.ToString("dd.MM.yyyy:hh.mm") + "','"
                                    + revision.Author + "','"
                                    + revision.Comment + "')";
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Очистка таблиц dds.up, dds.down и dds.version от данных
        /// </summary>
        internal static void ClearUpDownScripts()
        {
            SqlCommand command = DatabaseAdapter.connection.CreateCommand();
            command.CommandText = "TRUNCATE TABLE [dds].[up]";
            command.ExecuteNonQuery();
            command.CommandText = "TRUNCATE TABLE [dds].[down];";
            command.ExecuteNonQuery();
            command.CommandText = "TRUNCATE TABLE [dds].[version];";
            command.ExecuteNonQuery();
        }
    }
}