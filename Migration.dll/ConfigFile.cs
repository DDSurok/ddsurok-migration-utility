using System.IO;
using System.Xml;

namespace migration
{
    static public class ConfigFile
    {
        private static readonly string configFileName = @"migration.conf";
        /// <summary>
        /// Загрузка настроек из файла
        /// </summary>
        static public void Load()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(ConfigFile.configFileName))
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
                                        ConfigFile.serverName = reader.Value;
                                        break;
                                    case "databaseName":
                                        reader.Read();
                                        ConfigFile.databaseName = reader.Value;
                                        break;
                                    case "versionDirectory":
                                        reader.Read();
                                        ConfigFile.versionDirectory = reader.Value;
                                        break;
                                    case "nickName":
                                        reader.Read();
                                        ConfigFile.nickName = reader.Value;
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
                if ((ConfigFile.serverName.Trim() == "") || (ConfigFile.databaseName.Trim() == ""))
                {
                    File.Delete(ConfigFile.configFileName);
                    throw new System.ArgumentException("Ошибочный файл настроек программы.\nПерезапустите генератор файла конфигурации.");
                }
                ConfigFile.isLoad = true;
            }
            catch (FileNotFoundException)
            {
                ConfigFile.isLoad = false;
            }
        }
        /// <summary>
        /// Изменение настроек (без смены имени базы данных)
        /// </summary>
        /// <param name="_Server">Имя сервера</param>
        /// <param name="_VersionDirectory">Путь к каталогу версий</param>
        /// <param name="_NickName">Имя пользователя</param>
        static public void Write(string _Server, string _VersionDirectory, string _NickName)
        {
            ConfigFile.serverName = _Server;
            ConfigFile.versionDirectory = _VersionDirectory;
            ConfigFile.nickName = _NickName;
            ConfigFile.WriteData();
        }
        /// <summary>
        /// Изменение настроек (включая смену имени базы данных)
        /// </summary>
        /// <param name="_Server">Имя сервера</param>
        /// <param name="_Database"></param>
        /// <param name="_VersionDirectory">Путь к каталогу версий</param>
        /// <param name="_NickName">Имя пользователя</param>
        static public void Rewrite(string _Server, string _Database, string _VersionDirectory, string _NickName)
        {
            ConfigFile.Write(_Server, _VersionDirectory, _NickName);
            ConfigFile.databaseName = _Database;
            ConfigFile.WriteData();
        }
        /// <summary>
        /// Выгрузка данных в файл
        /// </summary>
        static private void WriteData()
        {
            using (XmlWriter output = XmlWriter.Create(ConfigFile.configFileName, functions.XmlSettings()))
            {
                // Создали открывающийся тег
                output.WriteStartElement("MigrationConfigure");

                // Создаем элемент connectionString
                output.WriteElementString("serverName", ConfigFile.serverName);

                // Создаем элемент databaseName
                output.WriteElementString("databaseName", ConfigFile.databaseName);

                // Создаем элемент versionDirectory
                output.WriteElementString("versionDirectory", ConfigFile.versionDirectory);

                // Создаем элемент nickName
                output.WriteElementString("nickName", ConfigFile.nickName);

                // Сбрасываем буфферизированные данные
                output.Flush();

                // Закрываем фаил, с которым связан output
                output.Close();
            }
        }
        static public bool isLoad { get; private set; }
        static public string serverName { get; private set; }
        static public string databaseName { get; private set; }
        static public string versionDirectory { get; private set; }
        static public string nickName { get; private set; }
    }
}
