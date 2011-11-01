using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Xml;

namespace migration
{
    internal static class Commit
    {
        public static void Run()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=" + Config.serverName + ";Integrated Security=True"))
            {
                // Проверяем базу на наличие изменений
                connection.Open();
                connection.ChangeDatabase(Config.databaseName);
                if (GetCountChanges(connection) > 0)    // Если изменения есть, то начинаем
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.
                    XmlWriter output = XmlTextWriter.Create(Program.fileName, 
                    output.Load(Program.fileName);
                    // Дописываем в файл заголовок следующего коммита
                    doc.
                    // Пишем скрипты повышения

                    // Пишем скрипты понижения

                    // Пишем подвал коммита

                }
                connection.Close();
            }
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
            return int.Parse(reader["COUNT"].ToString());
        }
    }
}
