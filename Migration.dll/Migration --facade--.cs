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
        /// <summary>
        /// Хранит имя сервера баз данных.
        /// </summary>
        public static string ServerName
        {
            get
            {
                return Configuration.serverName;
            }
        }
        /// <summary>
        /// Хранит имя базы данных.
        /// </summary>
        public static string DatabaseName
        {
            get
            {
                return Configuration.databaseName;
            }
        }
        /// <summary>
        /// Хранит путь каталога хранения ривизий.
        /// </summary>
        public static string VersionDirectory
        {
            get
            {
                return Configuration.versionDirectory;
            }
        }
        /// <summary>
        /// Хранит имя пользователя системы.
        /// </summary>
        public static string NickName
        {
            get
            {
                return Configuration.nickName;
            }
        }
        /// <summary>
        /// Реализует сохранение основных настроек программы (без изменения имени базы данных).
        /// </summary>
        /// <param name="_Server">Новое имя сервера</param>
        /// <param name="_VersionDirectory">Новый путь к каталогу хранения ревизий</param>
        /// <param name="_NickName">Новое имя пользователя</param>
        public static void WriteConfiguration(string _Server, string _VersionDirectory, string _NickName)
        {
            Configuration.Write(_Server, _VersionDirectory, _NickName);
        }
        /// <summary>
        /// Реализует сохранение основных настроек программы (с изменением имени базы данных).
        /// </summary>
        /// <param name="_Server">Новое имя сервера</param>
        /// <param name="_Database">Новое имя базы данных</param>
        /// <param name="_VersionDirectory">Новый путь к каталогу хранения ревизий</param>
        /// <param name="_NickName">Новое имя пользователя</param>
        public static void WriteConfiguration(string _Server, string _Database, string _VersionDirectory, string _NickName)
        {
            Configuration.Write(_Server, _Database, _VersionDirectory, _NickName);
        }
        /// <summary>
        /// Получить список ревизий в прямом порядке.
        /// </summary>
        /// <returns>Список ревизий</returns>
        public static List<RevisionInfo> GetRevisionList()
        {
            return RevisionList.GetRevisionList();
        }
        /// <summary>
        /// Получить список ревизий в обратном порядке.
        /// </summary>
        /// <returns>Список ревизий</returns>
        public static List<RevisionInfo> GetReverseRevisionList()
        {
            return RevisionList.GetReverseRevisionList();
        }
        /// <summary>
        /// Получить номер ревизии базы данных.
        /// </summary>
        /// <returns>Номер ревизии из списка</returns>
        public static int GetCurrentRevision()
        {
            return RevisionList.GetCurrentRevision();
        }
        /// <summary>
        /// Флаг успешной загрузки файла конфигурации.
        /// </summary>
        public static bool IsLoad
        {
            get
            {
                return Configuration.isLoad;
            }
        }
    }
}
