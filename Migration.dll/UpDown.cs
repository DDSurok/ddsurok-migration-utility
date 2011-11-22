using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace migration
{
    public static class UpDown
    {
        private static int CurrentVersion;
        private static int DestinationVersion;
        /// <summary>
        /// Предоставляет доступ к основному механизму класса
        /// </summary>
        /// <param name="Version">Номер версии-результата миграции</param>
        internal static void _Main(int Version)
        {
            UpDown.DestinationVersion = Version;
            UpDown.CurrentVersion = RevisionList.GetCurrentRevision();

            if (UpDown.CurrentVersion > UpDown.DestinationVersion)       // Понижение версии
            {
                List<RevisionInfo> list = RevisionList.GetRevisionList().GetRange(UpDown.DestinationVersion + 1, UpDown.CurrentVersion - UpDown.DestinationVersion);
                list.Reverse();
                foreach (RevisionInfo info in list)
                {
                    UpDown.ApplyScripts(info.GetDownScripts());
                }
            }
            else                                // Повышение версии
            {
                foreach (RevisionInfo info in RevisionList.GetRevisionList().GetRange(UpDown.CurrentVersion + 1, UpDown.DestinationVersion - UpDown.CurrentVersion))
                {
                    UpDown.ApplyScripts(info.GetUpScripts());
                }
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
                command.Transaction = tran;
                try
                {
                    foreach (string query in list)
                    {
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                    tran.Commit();
                    
                    DatabaseAdapter.UpdateVersionDatabase(RevisionList.GetRevisionList()[UpDown.DestinationVersion]);
                }
                catch (Exception)
                {
                    tran.Rollback();
                    
                    DatabaseAdapter.UpdateVersionDatabase(RevisionList.GetRevisionList()[UpDown.CurrentVersion]);
                }

            }
        }
    }
}
