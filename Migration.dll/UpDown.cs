using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace migration.Library
{
    internal static class UpDown
    {
        /// <summary>
        /// Хранит номер текущей ревизии базы данных.
        /// </summary>
        private static int CurrentVersion;
        /// <summary>
        /// Хранит номер ревизии-назначения.
        /// </summary>
        private static int DestinationVersion;
        /// <summary>
        /// Предоставляет доступ к основному механизму класса.
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
                    UpDown.CurrentVersion--;
                    DatabaseAdapter.ApplyScripts(info.GetDownScripts(), UpDown.CurrentVersion);
                }
            }
            else                                // Повышение версии
            {
                foreach (RevisionInfo info in RevisionList.GetRevisionList().GetRange(UpDown.CurrentVersion + 1, UpDown.DestinationVersion - UpDown.CurrentVersion))
                {
                    UpDown.CurrentVersion++;
                    DatabaseAdapter.ApplyScripts(info.GetUpScripts(), UpDown.CurrentVersion);
                }
            }
        }
    }
}
