using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace migration
{
    public static class Fix
    {
        private static RevisionInfo currentRevision;
        private static SqlConnection connection;
        private static XmlWriter output;
        public static void Run(string Comment)
        {
            using (connection = new SqlConnection("Data Source=" + ConfigFile.serverName + ";Integrated Security=True"))
            {
                // Проверяем базу на наличие изменений
                connection.Open();
                connection.ChangeDatabase(ConfigFile.databaseName);
                if (GetCountChanges() > 0)    // Если изменения есть, то начинаем
                {
                    Fix.currentRevision = RevisionInfo.GenerateRevisionInfo(Comment);
                    FileStream fs = new FileStream(Config.GetFileName(Fix.currentRevision), FileMode.Append);
                    output = XmlWriter.Create(fs, Config.XmlSettings());
                    // Дописываем в файл заголовок следующего коммита
                    WriteXMLHeader();
                    // Пишем скрипты повышения
                    WriteScriptsUp();
                    // Пишем скрипты понижения
                    WriteScriptsDown();
                    // Чистим список изменений и обновляем версию СУБД
                    ClearUpDownScripts();
                    InitDatabase.UpdateVersionDatabase(Fix.currentRevision);
                    // Пишем подвал коммита
                    WriteXMLSuffix();
                    output.Close();
                    fs.Close();         // Закрываем поток
                }
                connection.Close();
            }
        }

        private static void ClearUpDownScripts()
        {
            SqlCommand command = Fix.connection.CreateCommand();
            command.CommandText = "TRUNCATE TABLE [dds].[up]";
            command.ExecuteNonQuery();
            command.CommandText = "TRUNCATE TABLE [dds].[down];";
            command.ExecuteNonQuery();
            command.CommandText = "TRUNCATE TABLE [dds].[version];";
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Запись заголовка записи в XML
        /// </summary>
        /// <param name="output">XML, куда пишется запись</param>
        private static void WriteXMLHeader()
        {
            Fix.output.WriteStartElement("Revision");
            Fix.output.WriteAttributeString("Database", ConfigFile.databaseName);
            Fix.output.WriteAttributeString("Create_date", Fix.currentRevision.GenerateDateTime.ToShortDateString());
            Fix.output.WriteAttributeString("Create_time", Fix.currentRevision.GenerateDateTime.ToShortTimeString());
            Fix.output.WriteAttributeString("Id", Fix.currentRevision.HashCode);
            Fix.output.WriteStartElement("Comment");
            Fix.output.WriteString(Fix.currentRevision.Comment);
            Fix.output.WriteEndElement();
        }
        /// <summary>
        /// Запись подвала записи в XML
        /// </summary>
        /// <param name="output"></param>
        private static void WriteXMLSuffix()
        {
            Fix.output.WriteEndElement(); // "Revision"
            Fix.output.WriteEndDocument();
        }
        /// <summary>
        /// Записываем в XML скрипты повышения
        /// </summary>
        /// <param name="connection">Соединение с базой, где храняться скрипты повышения (база должна быть выбрана)</param>
        /// <param name="output">XML, куда записываются скрипты</param>
        private static void WriteScriptsUp()
        {
            Fix.output.WriteStartElement("UpScripts");
            SqlCommand command = Fix.connection.CreateCommand();
            command.CommandText = "SELECT [dds].[up].[id] AS [id], [dds].[up].[script] AS [script] FROM [dds].[up] ORDER BY [dds].[up].[id] ASC";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Fix.output.WriteElementString(int.Parse(reader["id"].ToString()).ToString("up00000000"), reader["script"].ToString());
            }
            Fix.output.WriteEndElement();
            reader.Close();
        }
        /// <summary>
        /// Записываем с XML скрипты понижения
        /// </summary>
        /// <param name="connection">Соединение с базой, где храняться скрипты понижения (база должна быть выбрана)</param>
        /// <param name="output">XML, куда записываются скрипты</param>
        private static void WriteScriptsDown()
        {
            Fix.output.WriteStartElement("DownScripts");
            SqlCommand command = Fix.connection.CreateCommand();
            command.CommandText = "SELECT [dds].[down].[id] AS [id], [dds].[down].[script] AS [script] FROM [dds].[down] ORDER BY [dds].[down].[id] DESC";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Fix.output.WriteElementString(int.Parse(reader["id"].ToString()).ToString("down00000000"), reader["script"].ToString());
            }
            Fix.output.WriteEndElement();
            reader.Close();
        }
        /// <summary>
        /// Получить количество изменений в базе
        /// </summary>
        /// <param name="connection">Соединение с сервером (база должна быть выбрана)</param>
        /// <returns>Количество изменений</returns>
        private static int GetCountChanges()
        {
            SqlCommand command = Fix.connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) AS [COUNT] FROM [dds].[up]";
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int returnValue = int.Parse(reader["COUNT"].ToString());
            reader.Close();
            return returnValue;
        }
    }
}
