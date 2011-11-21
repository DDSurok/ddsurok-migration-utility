using System.Data.SqlClient;
using System.Xml;

namespace migration
{
    internal static partial class DatabaseAdapter
    {
        /// <summary>
        /// Записываем в XML скрипты повышения
        /// </summary>
        /// <param name="output">XML, куда записываются скрипты</param>
        public static void WriteScriptsUp(XmlWriter output)
        {
            output.WriteStartElement("UpScripts");
            SqlCommand command = DatabaseAdapter.connection.CreateCommand();
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
            SqlCommand command = DatabaseAdapter.connection.CreateCommand();
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
        /// Получить количество изменений в базе
        /// </summary>
        /// <returns>Количество изменений</returns>
        public static int GetCountChanges()
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