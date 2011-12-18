using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace migration.Library
{
    /// <summary>
    /// Механизмы получения списка версий.
    /// </summary>
    internal static class RevisionList
    {
        /// <summary>
        /// Получить список всех ревизий.
        /// </summary>
        /// <returns>Список ревизий</returns>
        internal static List<RevisionInfo> GetRevisionList()
        {
            List<RevisionInfo> returnList = new List<RevisionInfo>();
            int i = 0;
            if (Directory.Exists(Configuration.versionDirectory))
            {
                foreach(string File in Directory.GetFiles(Configuration.versionDirectory))
                {
                    RevisionInfo tempInfo = RevisionInfo.GetInfoFromFile(File);
                    tempInfo.Id = i;

                    // Добавляем информацию в список
                    returnList.Add(tempInfo);

                    // Переключаем счетчик файлов
                    i++;
                }
            }
            return returnList;
        }
        /// <summary>
        /// Получить список всех ревизий в обратном порядке.
        /// </summary>
        /// <returns>Список ревизий</returns>
        internal static List<RevisionInfo> GetReverseRevisionList()
        {
            List<RevisionInfo> returnList = new List<RevisionInfo>();
            int i = 0;
            if (Directory.Exists(Configuration.versionDirectory))
            {
                foreach (string File in Directory.GetFiles(Configuration.versionDirectory))
                {
                    RevisionInfo tempInfo = RevisionInfo.GetInfoFromFile(File);
                    tempInfo.Id = i;

                    // Добавляем информацию в список
                    returnList.Insert(0, tempInfo);

                    // Переключаем счетчик файлов
                    i++;
                }
            }
            return returnList;
        }
        /// <summary>
        /// Получить номер ревизии, текущей для базы данных
        /// </summary>
        /// <returns>Номер из списка ревизий. -1 если в базе нет версии или версия базы отсутствует в системе</returns>
        internal static int GetCurrentRevision()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=" + Configuration.serverName + ";Integrated Security=True"))
                {
                    connection.Open();
                    connection.ChangeDatabase(Configuration.databaseName);
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT [dds].[version].[hashCode] AS hashCode FROM [dds].[version]";
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    string hash = reader["hashCode"].ToString();
                    foreach (RevisionInfo info in RevisionList.GetRevisionList())
                    {
                        if (info.HashCode == hash)
                            return info.Id;
                    }
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
