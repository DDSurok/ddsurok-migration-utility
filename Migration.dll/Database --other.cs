using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace migration
{
    internal static partial class DatabaseAdapter
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
    }
}