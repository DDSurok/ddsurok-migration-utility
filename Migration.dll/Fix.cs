using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace migration
{
    public static class Fix
    {
        public static void Run(string Comment)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=" + ConfigFile.serverName + ";Integrated Security=True"))
            {
                // Проверяем базу на наличие изменений
                connection.Open();
                connection.ChangeDatabase(ConfigFile.databaseName);
                if (GetCountChanges(connection) > 0)    // Если изменения есть, то начинаем
                {
                    FileStream fs = new FileStream(Config.GetFileName(Comment), FileMode.Append);
                    XmlWriter output = XmlWriter.Create(fs, Config.XmlSettings());
                    // Дописываем в файл заголовок следующего коммита
                    WriteXMLHeader(output);
                    // Пишем скрипты повышения
                    WriteScriptsUp(connection, output);
                    // Пишем скрипты понижения
                    WriteScriptsDown(connection, output);
                    // Пишем подвал коммита
                    WriteXMLSuffix(output);
                    output.Close();
                    fs.Close();         // Закрываем поток
                }
                connection.Close();
            }
        }
        /// <summary>
        /// Запись заголовка записи в XML
        /// </summary>
        /// <param name="output">XML, куда пишется запись</param>
        private static void WriteXMLHeader(XmlWriter output)
        {
            output.WriteStartElement("Revision");
            output.WriteAttributeString("Database", ConfigFile.databaseName);
            output.WriteAttributeString("Create_date", DateTime.Today.ToShortDateString());
            output.WriteAttributeString("Create_time", DateTime.Now.ToShortTimeString());
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] byteHash = sha.ComputeHash(Encoding.Unicode.GetBytes(DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()));
            string hash = "";
            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);
            output.WriteAttributeString("Id", hash);
        }
        /// <summary>
        /// Запись подвала записи в XML
        /// </summary>
        /// <param name="output"></param>
        private static void WriteXMLSuffix(XmlWriter output)
        {
            output.WriteEndElement(); // "Revision"
            output.WriteEndDocument();
        }
        /// <summary>
        /// Записываем в XML скрипты повышения
        /// </summary>
        /// <param name="connection">Соединение с базой, где храняться скрипты повышения (база должна быть выбрана)</param>
        /// <param name="output">XML, куда записываются скрипты</param>
        private static void WriteScriptsUp(SqlConnection connection, XmlWriter output)
        {
            output.WriteStartElement("UpScripts");
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT [dds].[up].[id] AS [id], [dds].[up].[script] AS [script] FROM [dds].[up]";
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
        /// <param name="connection">Соединение с базой, где храняться скрипты понижения (база должна быть выбрана)</param>
        /// <param name="output">XML, куда записываются скрипты</param>
        private static void WriteScriptsDown(SqlConnection connection, XmlWriter output)
        {
            output.WriteStartElement("DownScripts");
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT [dds].[down].[id] AS [id], [dds].[down].[script] AS [script] FROM [dds].[down]";
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
        /// <param name="connection">Соединение с сервером (база должна быть выбрана)</param>
        /// <returns>Количество изменений</returns>
        private static int GetCountChanges(SqlConnection connection)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) AS [COUNT] FROM [dds].[up]";
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int returnValue = int.Parse(reader["COUNT"].ToString());
            reader.Close();
            return returnValue;
        }
    }
}
