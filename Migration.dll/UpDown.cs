using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace migration
{
    public static class UpDown
    {
        /// <summary>
        /// Предоставляет доступ к основному механизму класса
        /// </summary>
        /// <param name="Version"></param>
        public static void Run(int Version)
        {
            int CurrentVersion = RevisionList.GetCurrentRevision();
            if (CurrentVersion > Version)       // Понижение версии
            {
                List<RevisionInfo> list = RevisionList.GetRevisionList().GetRange(Version + 1, CurrentVersion - Version);
                list.Reverse();
                foreach (RevisionInfo info in list)
                {
                    UpDown.ApplyScripts(info.GetDownScripts());
                }
            }
            else if (CurrentVersion < Version)  // Повышение версии
            {
                foreach (RevisionInfo info in RevisionList.GetRevisionList().GetRange(CurrentVersion + 1, Version - CurrentVersion))
                {
                    UpDown.ApplyScripts(info.GetUpScripts());
                }
            }
            else                                // Отбрасывание изменений
            {
                // TODO
            }
        }
        /// <summary>
        /// Применить список скриптов
        /// </summary>
        /// <param name="list">Список скриптов</param>
        private static void ApplyScripts(List<string> list)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=" + ConfigFile.serverName + ";Integrated Security=True"))
            {
                connection.Open();
                connection.ChangeDatabase(ConfigFile.databaseName);
                SqlTransaction tran = connection.BeginTransaction();
                SqlCommand command = connection.CreateCommand();
                try
                {
                    foreach (string query in list)
                    {
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }

            }
        }
    }
}
