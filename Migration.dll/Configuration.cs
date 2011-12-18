using System.IO;
using System.Xml;

namespace migration.Library
{
    internal static class Configuration
    {
        /// <summary>
        /// Имя файла конфигураций.
        /// </summary>
        private static readonly string configFileName = @"migration.conf";
        /// <summary>
        /// Выгрузка данных в файл.
        /// </summary>
        private static void SaveData()
        {
            using (XmlWriter output = XmlWriter.Create(Configuration.configFileName, functions.XmlSettings()))
            {
                // Создали открывающийся тег
                output.WriteStartElement("MigrationConfigure");
                // Создаем элемент connectionString
                output.WriteElementString("serverName", Configuration.serverName);
                // Создаем элемент databaseName
                output.WriteElementString("databaseName", Configuration.databaseName);
                // Создаем элемент versionDirectory
                output.WriteElementString("versionDirectory", Configuration.versionDirectory);
                // Создаем элемент nickName
                output.WriteElementString("nickName", Configuration.nickName);
                // Сбрасываем буфферизированные данные
                output.Flush();
                // Закрываем фаил, с которым связан output
                output.Close();
            }
        }
        /// <summary>
        /// Загрузка настроек из файла.
        /// </summary>
        internal static void Load()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(Configuration.configFileName))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "serverName":
                                        reader.Read();
                                        Configuration.serverName = reader.Value;
                                        break;
                                    case "databaseName":
                                        reader.Read();
                                        Configuration.databaseName = reader.Value;
                                        break;
                                    case "versionDirectory":
                                        reader.Read();
                                        Configuration.versionDirectory = reader.Value;
                                        break;
                                    case "nickName":
                                        reader.Read();
                                        Configuration.nickName = reader.Value;
                                        break;
                                }
                                break;
                            case XmlNodeType.Text:
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                            case XmlNodeType.Comment:
                            case XmlNodeType.EndElement:
                                break;
                        }
                    }
                }
                if ((Configuration.serverName.Trim() == "") || (Configuration.databaseName.Trim() == ""))
                {
                    File.Delete(Configuration.configFileName);
                    throw new System.ArgumentException("Ошибочный файл настроек программы.\nПерезапустите генератор файла конфигурации.");
                }
                Configuration.isLoad = true;
            }
            catch (FileNotFoundException)
            {
                Configuration.isLoad = false;
            }
        }
        /// <summary>
        /// Изменение настроек (без смены имени базы данных).
        /// </summary>
        /// <param name="_Server">Имя сервера</param>
        /// <param name="_VersionDirectory">Путь к каталогу версий</param>
        /// <param name="_NickName">Имя пользователя</param>
        internal static void Write(string _Server, string _VersionDirectory, string _NickName)
        {
            Configuration.serverName = _Server;
            Configuration.versionDirectory = _VersionDirectory;
            Configuration.nickName = _NickName;
            Configuration.SaveData();
        }
        /// <summary>
        /// Изменение настроек (включая смену имени базы данных).
        /// </summary>
        /// <param name="_Server">Имя сервера</param>
        /// <param name="_Database">Имя базы данных</param>
        /// <param name="_VersionDirectory">Путь к каталогу версий</param>
        /// <param name="_NickName">Имя пользователя</param>
        internal static void Write(string _Server, string _Database, string _VersionDirectory, string _NickName)
        {
            Configuration.Write(_Server, _VersionDirectory, _NickName);
            Configuration.databaseName = _Database;
            Configuration.SaveData();
        }
        /// <summary>
        /// Хранит флаг успешной загрузки настроек из файла.
        /// </summary>
        internal static bool isLoad { get; private set; }
        /// <summary>
        /// Хранит имя сервера, загруженное из файла.
        /// С этим сервером производится работа программы.
        /// </summary>
        internal static string serverName { get; private set; }
        /// <summary>
        /// Хранит имя базы данных, загруженное из файла.
        /// С этой базой данных производится работа программы.
        /// </summary>
        internal static string databaseName { get; private set; }
        /// <summary>
        /// Хранит путь к каталогу хранения ревизий, загруженный из файла.
        /// В этом каталоге хранятся все ревизии для текущей базы данных.
        /// </summary>
        internal static string versionDirectory { get; private set; }
        /// <summary>
        /// Хранит имя пользователя программы, загруженное из файла.
        /// Имя пользователя идентифицирует его в команде.
        /// </summary>
        internal static string nickName { get; private set; }
    }
}
