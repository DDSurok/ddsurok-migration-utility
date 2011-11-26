using System;
using System.Collections.Generic;

namespace migration
{
    /// <summary>
    /// Фасад сборки.
    /// </summary>
    public static class Migration
    {
        /// <summary>
        /// Статический контруктор -- загрузка данных из файла.
        /// </summary>
        static Migration()
        {
            Configuration.Load();
            DatabaseAdapter.Initial();
        }
        /// <summary>
        /// Очистить базу данных от служебных структур.
        /// </summary>
        public static void Forgot()
        {
            DatabaseAdapter.RemoveServiceDataInDatabase();
        }
        /// <summary>
        /// Создание служебных структур в базе данных и создание Baseline.
        /// </summary>
        /// <param name="Comment">Текст комментария к Baseline</param>
        public static void Init(string Comment)
        {
            cInit._Main(Comment);
        }
        /// <summary>
        /// Создание ревизии базы данных.
        /// </summary>
        /// <param name="Comment">Текст комментария к ревизии</param>
        public static void Fix(string Comment)
        {
            cFix._Main(Comment);
        }
        /// <summary>
        /// Выполнение миграции к версии с номером <code>Version</code>.
        /// </summary>
        /// <param name="Version">Номер ревизии - направления миграции</param>
        public static void Migrate(int Version)
        {
            UpDown._Main(Version);
        }

        public static string ServerName
        {
            get
            {
                return Configuration.serverName;
            }
        }

        public static string DatabaseName
        {
            get
            {
                return Configuration.databaseName;
            }
        }

        public static string VersionDirectory
        {
            get
            {
                return Configuration.versionDirectory;
            }
        }

        public static string NickName
        {
            get
            {
                return Configuration.nickName;
            }
        }

        public static void WriteConfiguration(string _Server, string _VersionDirectory, string _NickName)
        {
            Configuration.Write(_Server, _VersionDirectory, _NickName);
        }

        public static void WriteConfiguration(string _Server, string _Database, string _VersionDirectory, string _NickName)
        {
            Configuration.Write(_Server, _Database, _VersionDirectory, _NickName);
        }

        public static List<RevisionInfo> GetRevisionList()
        {
            return RevisionList.GetRevisionList();
        }

        public static List<RevisionInfo> GetReverseRevisionList()
        {
            return RevisionList.GetReverseRevisionList();
        }

        public static int GetCurrentRevision()
        {
            return RevisionList.GetCurrentRevision();
        }

        public static bool IsLoad
        {
            get
            {
                return Configuration.isLoad;
            }
        }
    }
}
