using System;

namespace migration
{
    /// <summary>
    /// Фасад сборки
    /// </summary>
    public static class Migration
    {
        /// <summary>
        /// Статический контруктор -- загрузка данных из файла
        /// </summary>
        static Migration()
        {
            ConfigFile.Load();
            DatabaseAdapter.Initial();
        }
        /// <summary>
        /// Очистить базу данных от служебных структур
        /// </summary>
        public static void Forgot()
        {
            DatabaseAdapter.RemoveServiceDataInDatabase();
        }
        /// <summary>
        /// Создание служебных структур в базе данных и создание Baseline
        /// </summary>
        /// <param name="Comment">Текст комментария к Baseline</param>
        public static void Init(string Comment)
        {
            cInit._Main(Comment);
        }
        /// <summary>
        /// Создание ревизии базы данных
        /// </summary>
        /// <param name="Comment">Текст комментария к ревизии</param>
        public static void Fix(string Comment)
        {
            cFix._Main(Comment);
        }

        public static void Migrate(int Version)
        {
            UpDown._Main(Version);
        }
    }
}
