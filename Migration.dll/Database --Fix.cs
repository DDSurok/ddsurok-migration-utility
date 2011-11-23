using System.Data.SqlClient;
using System.Xml;
using System.Collections.Generic;

namespace migration
{
    internal static partial class DatabaseAdapter
    {
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